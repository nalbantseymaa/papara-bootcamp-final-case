using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class ManagerCommandHandler :
    IRequestHandler<CreateManagerCommand, ApiResponse<CreateManagerResponse>>,
    IRequestHandler<UpdateManagerCommand, ApiResponse>,
    IRequestHandler<DeleteManagerCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserService userService;

    public ManagerCommandHandler(AppDbContext dbContext, IMapper mapper, IUserService userService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userService = userService;
    }

    public async Task<ApiResponse<CreateManagerResponse>> Handle(CreateManagerCommand request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<UserRequest>(request.User);

        var userResponse = await userService.CreateUserAsync(user, UserRole.Manager.ToString());
        if (userResponse.userEntity == null)
            return new ApiResponse<CreateManagerResponse>("User creation failed");

        var manager = mapper.Map<Manager>(request.Manager);
        mapper.Map(userResponse.userEntity, manager);

        await dbContext.Managers.AddAsync(manager, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse<CreateManagerResponse>(new CreateManagerResponse
        {
            User = mapper.Map<UserResponse>(manager),
            Manager = mapper.Map<ManagerResponse>(manager),
            PlainPassword = userResponse.plainPassword

        });
    }

    public async Task<ApiResponse> Handle(UpdateManagerCommand request, CancellationToken cancellationToken)
    {
        var validation = await GetActiveManager(request.Id, cancellationToken);
        if (!validation.Success)
            return new ApiResponse(validation.Message);
        var entity = validation.Entity!;

        entity.FirstName = request.Manager.FirstName;
        entity.LastName = request.Manager.LastName;
        entity.MiddleName = request.Manager.MiddleName ?? entity.MiddleName;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Manager successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteManagerCommand request, CancellationToken cancellationToken)
    {
        var validation = await GetActiveManager(request.Id, cancellationToken);
        if (!validation.Success)
            return new ApiResponse(validation.Message);

        var entity = validation.Entity!;

        if (await dbContext.Departments.AnyAsync(x => x.ManagerId == request.Id && x.IsActive, cancellationToken))
            return new ApiResponse("Cannot delete manager with active departments");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Manager successfully deleted");
    }

    private async Task<(bool Success, string? Message, Manager? Entity)> GetActiveManager(
      long? managerId, CancellationToken cancellationToken)
    {
        if (!managerId.HasValue)
            return (false, "Manager ID is required", null);

        var manager = await dbContext.Managers
            .FirstOrDefaultAsync(e => e.Id == managerId, cancellationToken);

        if (manager == null)
            return (false, "Manager not found", null);

        if (!manager.IsActive)
            return (false, "Manager is inactive", null);

        return (true, null, manager);
    }
}
