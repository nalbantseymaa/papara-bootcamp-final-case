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

    public async Task<ApiResponse<DepartmentResponse>> Handle(CreateDepartmentCommand request,
       CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Department.Name))
            return new ApiResponse<DepartmentResponse>("Department name is required");

        bool exists = await dbContext.Departments
            .AnyAsync(d => d.Name.Trim() == request.Department.Name.Trim(), cancellationToken);

        if (exists) return new ApiResponse<DepartmentResponse>("Department already exists");

        var manager = await ValidateManagerAsync(request.Department.ManagerId, cancellationToken);
        if (!manager.Success)
            return new ApiResponse<DepartmentResponse>(manager.Message);

        var department = mapper.Map<Department>(request.Department);

        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<DepartmentResponse>(department);
        return new ApiResponse<DepartmentResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
    {
        var (isValid, entity, errorMessage) = await ValidateDepartmentAsync(request.Id, cancellationToken);
        if (!isValid)
            return new ApiResponse(false, errorMessage!);
        var department = request.Department;

        if (entity.ManagerId.HasValue && department.ManagerId.HasValue
            && entity.ManagerId.Value != department.ManagerId.Value)
            return new ApiResponse(false, "This department already has a manager");

        var manager = await ValidateManagerAsync(request.Department.ManagerId, cancellationToken);

        if (!manager.Success) return manager;
        entity.Name = department.Name.Trim();

        if (!string.IsNullOrWhiteSpace(department.Description))
            entity.Description = department.Description.Trim();

        if (department.ManagerId.HasValue)
            entity.ManagerId = department.ManagerId.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Department successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var (isValid, department, errorMessage) = await ValidateDepartmentAsync(request.Id, cancellationToken);
        if (!isValid)
            return new ApiResponse(false, errorMessage!);

        var activeEmployees = await dbContext.Employees
            .Where(e => e.DepartmentId == department.Id && e.IsActive)
            .CountAsync(cancellationToken);

        if (activeEmployees > 0)
            return new ApiResponse("Cannot delete department with active employees");

        department.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Department successfully deleted");
    }

    private async Task<ApiResponse> ValidateManagerAsync(long? managerId,
      CancellationToken cancellationToken)
    {
        if (!managerId.HasValue)
            return new ApiResponse(true, string.Empty);

        var manager = await dbContext.Employees
       .FirstOrDefaultAsync(e => e.Id == managerId.Value, cancellationToken);

        if (manager == null)
            return new ApiResponse(false, "Manager not found");

        if (!manager.IsActive)
            return new ApiResponse(false, "Manager is inactive");

        return new ApiResponse(true, string.Empty);
    }

    private async Task<(bool IsValid, Department? Department, string? ErrorMessage)> ValidateDepartmentAsync(long departmentId, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

        if (department == null)
            return (false, null, "Department not found");

        if (!department.IsActive)
            return (false, null, "Department is inactive");

        return (true, department, null);
    }
}