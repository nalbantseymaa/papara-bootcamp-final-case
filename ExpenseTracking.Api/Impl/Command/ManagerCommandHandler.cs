using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Enum;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
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

    public ManagerCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<CreateManagerResponse>> Handle(CreateManagerCommand request, CancellationToken cancellationToken)
    {
        if (await dbContext.Managers.AnyAsync(x => x.Email == request.Manager.Email && x.IsActive, cancellationToken))
            return new ApiResponse<CreateManagerResponse>("Manager already exists");

        var manager = mapper.Map<Manager>(request.User);
        mapper.Map(request.Manager, manager);

        manager.Role = UserRole.Manager.ToString();
        manager.OpenDate = DateTime.UtcNow;
        manager.IsActive = true;
        manager.Secret = PasswordGenerator.GeneratePassword(30);
        var pwd = PasswordGenerator.GeneratePassword(6);
        manager.Password = PasswordGenerator.CreateSHA256(pwd, manager.Secret);
        manager.InsertedUser = "System";
        manager.InsertedDate = DateTime.UtcNow;

        await dbContext.Managers.AddAsync(manager, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse<CreateManagerResponse>(new CreateManagerResponse
        {
            User = mapper.Map<UserResponse>(manager),
            Manager = mapper.Map<ManagerResponse>(manager),
            PlainPassword = pwd
        });
    }

    public async Task<ApiResponse> Handle(UpdateManagerCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Managers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null || !entity.IsActive) return new ApiResponse("Manager not found or inactive");

        var emailResult = await UpdateEmailIfChanged(request.Manager.Email, entity.Id, cancellationToken);
        if (emailResult != null) return emailResult;

        entity.FirstName = UpdateIfNotEmpty(request.Manager.FirstName, entity.FirstName);
        entity.LastName = UpdateIfNotEmpty(request.Manager.LastName, entity.LastName);
        entity.MiddleName = request.Manager.MiddleName;
        entity.Email = request.Manager.Email;
        entity.UpdatedDate = DateTime.UtcNow;
        entity.UpdatedUser = "System";

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Manager successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteManagerCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Managers.Include(x => x.Phones).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null) return new ApiResponse("Manager not found");
        if (!entity.IsActive) return new ApiResponse("Manager is already inactive");

        if (await dbContext.Departments.AnyAsync(x => x.ManagerId == request.Id && x.IsActive, cancellationToken))
            return new ApiResponse("Cannot delete manager with active departments");

        if (entity.Phones.Any()) return new ApiResponse("Cannot delete manager with associated phone numbers");

        entity.IsActive = false;
        entity.UpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Manager successfully deleted");
    }

    private async Task<ApiResponse?> UpdateEmailIfChanged(string email, long managerId, CancellationToken cancellationToken)
    {
        var existingEmail = (await dbContext.Managers.FirstOrDefaultAsync(x => x.Id == managerId, cancellationToken))?.Email;

        if (string.IsNullOrWhiteSpace(email) || email == existingEmail)
            return null;

        var emailExists = await dbContext.Managers
            .AnyAsync(x => x.Email == email && x.Id != managerId && x.IsActive, cancellationToken);

        return emailExists ? new ApiResponse("Email already exists") : null;
    }

    private string UpdateIfNotEmpty(string newValue, string currentValue) => string.IsNullOrWhiteSpace(newValue) ? currentValue : newValue;
}
