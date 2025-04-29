namespace ExpenseTracking.Api.Schema;

public class AuthorizationRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class AuthorizationResponse
{
    public string Token { get; set; }
    public string UserName { get; set; }
    public DateTime Expiration { get; set; }
}