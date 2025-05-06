using ExpenseTracking.Api.Domain;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Impl.Service;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken);
}