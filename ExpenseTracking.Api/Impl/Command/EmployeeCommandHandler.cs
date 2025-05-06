using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Api.Impl.Service.Helper;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class EmployeeCommandHandler :
IRequestHandler<CreateEmployeeCommand, ApiResponse<CreateEmployeeResponse>>,
IRequestHandler<UpdateEmployeeCommand, ApiResponse>,
IRequestHandler<DeleteEmployeeCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserService userService;

    public EmployeeCommandHandler(AppDbContext dbContext, IMapper mapper, IUserService userService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userService = userService;
    }

    public async Task<ApiResponse<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var (isValid, department, errorMessage) = await ValidateDepartmentAsync(request.Employee.DepartmentId, cancellationToken);
        if (!isValid) return new ApiResponse<CreateEmployeeResponse>(errorMessage!);

        var user = mapper.Map<UserRequest>(request.User);

        var userResponse = await userService.CreateUserAsync(user, UserRole.Employee.ToString());
        if (userResponse.userEntity == null)
            return new ApiResponse<CreateEmployeeResponse>("User creation failed");

        var employee = mapper.Map<Employee>(request.Employee);
        mapper.Map(userResponse.userEntity, employee);

        InitializeNewEmployee(employee);

        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse<CreateEmployeeResponse>(new CreateEmployeeResponse
        {
            User = mapper.Map<UserResponse>(employee),
            Employee = mapper.Map<EmployeeResponse>(employee),
            PlainPassword = userResponse.plainPassword
        });
    }

    public async Task<ApiResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var (isValid, entity, errorMessage) = await ValidateEmployeeAsync(request.Id, cancellationToken);
        if (!isValid)
            return new ApiResponse(false, errorMessage!);

        var (isValidDepartment, department, errorMessageDepartment) = await ValidateDepartmentAsync(request.Employee.DepartmentId, cancellationToken);
        if (!isValidDepartment)
            return new ApiResponse(false, errorMessageDepartment!);

        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        employee.DepartmentId = (int)request.Employee.DepartmentId;
        employee.Salary = request.Employee.Salary;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Employee updated successfully");
    }

    public async Task<ApiResponse> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .Include(e => e.Phones)
            .Include(e => e.Addresses)
            .Include(e => e.ManagedDepartments)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        var (isValid, entity, errorMessage) = await ValidateEmployeeAsync(request.Id, cancellationToken);
        if (!isValid)
            return new ApiResponse(false, errorMessage!);

        employee.IsActive = false;
        employee.Phones?.ToList().ForEach(p => p.IsActive = false);
        employee.Addresses?.ToList().ForEach(a => a.IsActive = false);
        employee.ManagedDepartments?.ToList().ForEach(d => d.ManagerId = null);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Employee successfully deleted");
    }

    private async Task<(bool IsValid, Employee? Employee, string? ErrorMessage)> ValidateEmployeeAsync(long employeeId, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(d => d.Id == employeeId, cancellationToken);

        if (employee == null)
            return (false, null, "Employee not found");

        if (!employee.IsActive)
            return (false, null, "Employee is inactive");

        return (true, employee, null);
    }

    private void InitializeNewEmployee(Employee employee)
    {
        employee.HireDate = DateTime.UtcNow;
        employee.OpenDate = DateTime.UtcNow;
        employee.IBAN = IbanGenerator.GenerateIban();
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

