using ExpenseTracking.Base;
using ExpenseTracking.Schema;

public class CreateEmployeeRequest
{
    public UserRequest User { get; set; }
    public EmployeeRequest Employee { get; set; }
}

public class CreateEmployeeResponse
{
    public UserResponse User { get; set; }
    public EmployeeResponse Employee { get; set; }
    public string PlainPassword { get; set; }
    public string PlainPasswordNote { get; set; } = "This password is displayed only for the first login. Please save it.";
}


