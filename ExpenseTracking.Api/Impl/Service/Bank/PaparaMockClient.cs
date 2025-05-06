using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Impl.Service.Bank;

public class PaparaMockClient : IBankClient
{
    public async Task<PaymentResponse> SendPaymentAsync(string iban, string description, decimal amount)
    {
        var response = new PaymentResponse
        {
            Success = true,
            Message = "Payment transaction completed successfully",
            ReferenceNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10),
            Timestamp = DateTime.UtcNow
        };
        return response;
    }
}
