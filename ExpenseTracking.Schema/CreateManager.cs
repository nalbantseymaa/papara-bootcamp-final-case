using ExpenseTracking.Schema;

public class CreateManagerRequest
{
    public UserRequest User { get; set; }
    public ManagerRequest Manager { get; set; }
}

public class CreateManagerResponse
{
    public UserResponse User { get; set; }
    public ManagerResponse Manager { get; set; }
    public string PlainPassword { get; set; }
    public string PlainPasswordNote { get; set; } = "This password is displayed only for the first login. Please save it.";
}


