using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class DepartmentCommandHandler :
    IRequestHandler<CreateDepartmentCommand, ApiResponse<DepartmentResponse>>,
    IRequestHandler<UpdateDepartmentCommand, ApiResponse>,
    IRequestHandler<DeleteDepartmentCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public DepartmentCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<DepartmentResponse>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Department>(request.Department);

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<DepartmentResponse>(entity.Entity);
        return new ApiResponse<DepartmentResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Departments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Department not found");

        if (!entity.IsActive)
            return new ApiResponse("Department is not active");

        if (string.IsNullOrWhiteSpace(request.Department.Name))
            return new ApiResponse("Department name is required");

        if (request.Department.ManagerId <= 0)
            return new ApiResponse("Valid Manager ID is required");

        var managerExists = await dbContext.Employees
            .AnyAsync(x => x.Id == request.Department.ManagerId && x.IsActive, cancellationToken);

        if (!managerExists)
            return new ApiResponse("Manager not found or not active");

        entity.Name = request.Department.Name;
        entity.ManagerId = request.Department.ManagerId;

        if (!string.IsNullOrWhiteSpace(request.Department.Description))
        {
            entity.Description = request.Department.Description;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse("Department successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        // Get department with related entities
        var entity = await dbContext.Departments
            .Include(x => x.Employees)
            .Include(x => x.Addresses)
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Department not found");

        if (!entity.IsActive)
            return new ApiResponse("Department is already inactive");

        if (entity.Employees.Any(e => e.IsActive))
            return new ApiResponse("Cannot delete department with active employees");

        if (entity.Addresses.Any())
            return new ApiResponse("Cannot delete department with associated addresses");

        if (entity.Phones.Any())
            return new ApiResponse("Cannot delete department with associated phone numbers");

        entity.IsActive = false;
        entity.UpdatedDate = DateTime.UtcNow;
        entity.UpdatedUser = "System";

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse("Department successfully deleted");
    }
}