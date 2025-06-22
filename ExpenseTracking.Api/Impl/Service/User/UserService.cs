using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Service;

public class UserService : IUserService
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public UserService(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<(User userEntity, string plainPassword)> CreateUserAsync(UserRequest request, string role)
    {
        if (await dbContext.Users.AnyAsync(u => u.UserName == request.UserName))
            throw new InvalidOperationException("Registration failed. Please check your input.");

        if (await dbContext.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Registration failed. Please check your input.");

        var user = mapper.Map<User>(request);
        user.OpenDate = DateTime.UtcNow;
        user.Role = role;
        user.Secret = PasswordGenerator.GeneratePassword(30);
        var pwd = PasswordGenerator.GeneratePassword(6);
        user.Password = PasswordGenerator.CreateSHA256(pwd, user.Secret);

        return (user, pwd);
    }
}