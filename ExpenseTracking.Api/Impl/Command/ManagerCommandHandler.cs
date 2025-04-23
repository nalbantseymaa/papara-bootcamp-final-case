using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class ManagerCommandHandler :
    IRequestHandler<CreateManagerCommand, ApiResponse<ManagerResponse>>,
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

    public async Task<ApiResponse<ManagerResponse>> Handle(CreateManagerCommand request, CancellationToken cancellationToken)
    {
        var userExists = await dbContext.Managers
            .AnyAsync(x => x.Id == request.Manager.UserId && x.IsActive, cancellationToken);

        if (!userExists)
            return new ApiResponse<ManagerResponse>("Manager not found or not active");

        var emailExists = await dbContext.Managers
            .AnyAsync(x => x.Email == request.Manager.Email && x.IsActive, cancellationToken);

        if (emailExists)
            return new ApiResponse<ManagerResponse>("Email already exists");

        var mapped = mapper.Map<Manager>(request.Manager);
        mapped.IsActive = true;
        mapped.InsertedDate = DateTime.UtcNow;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<ManagerResponse>(entity.Entity);
        return new ApiResponse<ManagerResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateManagerCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Managers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Manager not found");

        if (!entity.IsActive)
            return new ApiResponse("Manager is not active");

        if (string.IsNullOrWhiteSpace(request.Manager.FirstName))
            return new ApiResponse("First name is required");

        if (string.IsNullOrWhiteSpace(request.Manager.LastName))
            return new ApiResponse("Last name is required");

        if (string.IsNullOrWhiteSpace(request.Manager.Email))
            return new ApiResponse("Email is required");

        if (entity.Email != request.Manager.Email)
        {
            var emailExists = await dbContext.Managers
                .AnyAsync(x => x.Email == request.Manager.Email && x.Id != request.Id && x.IsActive, cancellationToken);

            if (emailExists)
                return new ApiResponse("Email already exists");
        }

        entity.FirstName = string.IsNullOrWhiteSpace(request.Manager.FirstName)
        ? entity.FirstName
        : request.Manager.FirstName;

        entity.LastName = string.IsNullOrWhiteSpace(request.Manager.LastName)
            ? entity.LastName
            : request.Manager.LastName;

        entity.MiddleName = request.Manager.MiddleName;

        if (!string.IsNullOrWhiteSpace(request.Manager.Email) && request.Manager.Email != entity.Email)
        {
            var emailExists = await dbContext.Managers
                .AnyAsync(x => x.Email == request.Manager.Email && x.Id != request.Id && x.IsActive, cancellationToken);

            if (emailExists)
                return new ApiResponse("Email already exists");

            entity.Email = request.Manager.Email;
        }

        entity.UpdatedDate = DateTime.UtcNow;


        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse("Manager successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteManagerCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Managers
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Manager not found");

        if (!entity.IsActive)
            return new ApiResponse("Manager is already inactive");

        var hasActiveDepartments = await dbContext.Departments
            .AnyAsync(x => x.ManagerId == request.Id && x.IsActive, cancellationToken);

        if (hasActiveDepartments)
            return new ApiResponse("Cannot delete manager with active departments");

        if (entity.Phones.Any())
            return new ApiResponse("Cannot delete manager with associated phone numbers");

        entity.IsActive = false;
        entity.UpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse("Manager successfully deleted");
    }
}