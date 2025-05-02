using ExpenseTracking.Base;

namespace ExpenseTracking.Schema;

public class UserRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class UserResponse : BaseResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
