using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class ManagerRequest
{
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class ManagerResponse : BaseResponse
{
    public long UserId { get; set; }
    public string ManagerName { get; set; }
    public string Email { get; set; }
    public ICollection<PhoneResponse> Phones { get; set; }
}