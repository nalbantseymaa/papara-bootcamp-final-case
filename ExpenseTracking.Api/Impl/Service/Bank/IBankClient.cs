using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Impl.Service.Bank;

public interface IBankClient
{
    Task<PaymentResponse> SendPaymentAsync(string iban, string description, decimal amount);
}
