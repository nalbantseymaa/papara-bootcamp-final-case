using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class PaymentMethodRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
}

public class PaymentMethodResponse : BaseResponse
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ExpenseResponse> Expenses { get; set; }

}

public class UpdatePaymentMethodRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
