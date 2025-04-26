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
        if (request?.Department == null)
            return new ApiResponse<DepartmentResponse>("Department data is required");

        if (await dbContext.Departments
            .AnyAsync(d => d.Name == request.Department.Name && d.IsActive, cancellationToken))
            return new ApiResponse<DepartmentResponse>("Department already exists");

        if (request.Department.ManagerId.HasValue)
        {
            var manager = await dbContext.Managers
                .FirstOrDefaultAsync(m => m.Id == request.Department.ManagerId.Value, cancellationToken);

            if (manager == null)
                return new ApiResponse<DepartmentResponse>("Manager not found");

            if (!manager.IsActive)
                return new ApiResponse<DepartmentResponse>("Manager is inactive");
        }

        var department = mapper.Map<Department>(request.Department);
        department.IsActive = true;
        department.InsertedDate = DateTime.UtcNow;
        department.InsertedUser = "system";

        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<DepartmentResponse>(department);
        return new ApiResponse<DepartmentResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Id && d.IsActive, cancellationToken);
        if (department == null || request.Department == null)
            return new ApiResponse("Department not found or data is required");

        if (request.Department.Name != department.Name) department.Name = request.Department.Name;
        if (!string.IsNullOrWhiteSpace(request.Department.Description) && request.Department.Description != department.Description)
            department.Description = request.Department.Description;

        if (request.Department.ManagerId.HasValue)
        {
            var managerEmployee = await dbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Department.ManagerId.Value && e.IsActive, cancellationToken);
            if (managerEmployee == null) return new ApiResponse("Manager not found or inactive");
            department.ManagerId = managerEmployee.Id;
        }
        else
            department.ManagerId = null;
        department.UpdatedDate = DateTime.UtcNow;
        department.UpdatedUser = "system";
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Department successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
            return new ApiResponse("Department not found");

        if (!department.IsActive)
            return new ApiResponse("Department is already inactive");

        var activeEmployees = await dbContext.Employees
            .Where(e => e.DepartmentId == department.Id && e.IsActive)
            .CountAsync(cancellationToken);

        if (activeEmployees > 0)
            return new ApiResponse("Cannot delete department with active employees");

        department.IsActive = false;
        department.UpdatedDate = DateTime.UtcNow;
        department.UpdatedUser = "system";

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Department successfully deleted");
    }

}