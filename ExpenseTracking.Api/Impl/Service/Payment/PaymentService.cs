using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Service.Bank;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Impl.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext dbContext;
        private readonly IBankClient bankClient;
        private readonly IMapper mapper;

        public PaymentService(AppDbContext context, IBankClient bankClient, IMapper mapper)
        {
            this.dbContext = context;
            this.bankClient = bankClient;
            this.mapper = mapper;
        }

        public async Task<Payment> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken)
        {
            var bankResponse = await bankClient.SendPaymentAsync(request.IBAN, request.Description, request.Amount);

            var payment = mapper.Map<Payment>(bankResponse);
            payment.IBAN = request.IBAN;
            payment.Description = request.Description;
            payment.Amount = request.Amount;
            payment.EmployeeId = request.EmployeeId;
            payment.ExpenseId = request.ExpenseId;

            var entity = await dbContext.AddAsync(payment, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return payment;
        }
    }
}
