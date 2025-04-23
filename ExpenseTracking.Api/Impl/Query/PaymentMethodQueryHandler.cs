using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs.PaymentMethod;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class PaymentMethodQueryHandler :
IRequestHandler<GetAllPaymentMethodsQuery, ApiResponse<List<PaymentMethodResponse>>>,
IRequestHandler<GetPaymentMethodByIdQuery, ApiResponse<PaymentMethodResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;

    public PaymentMethodQueryHandler(AppDbContext context, IMapper mapper)
    {
        this.dbcontext = context;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<PaymentMethodResponse>>> Handle(GetAllPaymentMethodsQuery request, CancellationToken cancellationToken)
    {

        var PaymentMethods = await dbcontext.PaymentMethods
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<PaymentMethodResponse>>(PaymentMethods);

        return new ApiResponse<List<PaymentMethodResponse>>(mapped);
    }

    public async Task<ApiResponse<PaymentMethodResponse>> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
    {
        var PaymentMethod = await dbcontext.PaymentMethods
            .Include(x => x.Expenses)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (PaymentMethod == null)
        {
            return new ApiResponse<PaymentMethodResponse>("PaymentMethod not found");
        }

        var mapped = mapper.Map<PaymentMethodResponse>(PaymentMethod);
        return new ApiResponse<PaymentMethodResponse>(mapped);
    }
}
