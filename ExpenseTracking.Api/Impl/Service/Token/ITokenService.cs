using ExpenseTracking.Api.Domain;

namespace ExpenseTracking.Api.Impl.Service;

public interface ITokenService
{
    public string GenerateToken(User user);
}