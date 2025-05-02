using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class UserCommandHandler :
IRequestHandler<CreateUserCommand, ApiResponse<UserResponse>>,
IRequestHandler<UpdateUserCommand, ApiResponse>,
IRequestHandler<DeleteUserCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAppSession appSession;

    public UserCommandHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession)
    {
        this.appSession = appSession;
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return new ApiResponse<UserResponse>(
        "User cannot be created directly. Please use '/api/employees' or '/api/managers' endpoints to create a user.");
    }

    public async Task<ApiResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await GetActiveUserMethodAsync(request.Id, cancellationToken);
        if (!validation.Success)
            return new ApiResponse(validation.Message);

        var entity = validation.Entity!;

        if (await dbContext.Users.AnyAsync(u => u.UserName == request.User.UserName && u.Id != request.Id))
            return new ApiResponse("User name already exists");

        if (await dbContext.Users.AnyAsync(u => u.Email == request.User.Email && u.Id != request.Id))
            return new ApiResponse("Email already exists");

        entity.UserName = request.User.UserName;
        entity.Email = request.User.Email;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "User updated successfully");
    }

    public async Task<ApiResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return new ApiResponse("User cannot be deleted directly. Please use '/api/employees' or '/api/managers' endpoints to delete a user.");
    }

    private async Task<(bool Success, string? Message, User? Entity)> GetActiveUserMethodAsync(
    long? userId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue)
            return (false, "User ID is required", null);

        var user = await dbContext.Users
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        if (user == null)
            return (false, "User not found", null);

        if (!user.IsActive)
            return (false, "User is inactive", null);

        return (true, null, user);
    }
}