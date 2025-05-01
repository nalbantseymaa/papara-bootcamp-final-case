using System.Text.Json.Serialization;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;

namespace ExpenseTracking.Schema;

public class ExpenseRequest
{
    public long CategoryId { get; set; }
    public long PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public DateTime ExpenseDate { get; set; }
}

public class ExpenseResponse : BaseResponse
{
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public long CategoryId { get; set; }
    public long PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public DateTime ExpenseDate { get; set; }
    public ExpenseStatus Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RejectionReason { get; set; }

}

public class RejectExpenseRequest
{
    public string RejectionReason { get; set; }
}


