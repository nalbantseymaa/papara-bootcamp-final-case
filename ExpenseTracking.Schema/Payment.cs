namespace ExpenseTracking.Schema;

public class PaymentRequest
{
    public long ExpenseId { get; set; }
    public long EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string IBAN { get; set; }
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public string ReferenceNumber { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Message { get; set; }
}