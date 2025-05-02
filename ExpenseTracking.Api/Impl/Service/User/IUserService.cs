using ExpenseTracking.Api.Domain;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Impl.Service;

public interface IUserService
{
    Task<(User userEntity, string plainPassword)> CreateUserAsync(UserRequest request, string role);

}

