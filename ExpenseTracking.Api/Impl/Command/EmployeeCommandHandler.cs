using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
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

    public EmployeeCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    public async Task<ApiResponse<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var (isValidUser, user, errorMessageUser) = await ValidateUserAsync(request.User.UserName, cancellationToken);
        if (!isValidUser) return new ApiResponse<CreateEmployeeResponse>(errorMessageUser!);

        var (isValid, department, errorMessage) = await ValidateDepartmentAsync(request.Employee.DepartmentId, cancellationToken);
        if (!isValid) return new ApiResponse<CreateEmployeeResponse>(errorMessage!);

        var employee = mapper.Map<Employee>(request.User);
        mapper.Map(request.Employee, employee);

        InitializeNewEmployee(employee);
        var pwd = PasswordGenerator.GeneratePassword(6);
        employee.Password = PasswordGenerator.CreateSHA256(pwd, employee.Secret);

        await dbContext.Employees.AddAsync(employee, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CreateEmployeeResponse
        {
            User = mapper.Map<UserResponse>(employee),
            Employee = mapper.Map<EmployeeResponse>(employee),
            PlainPassword = pwd
        };
        return new ApiResponse<CreateEmployeeResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        var (isValid, entity, errorMessage) = await ValidateEmployeeAsync(request.Id, cancellationToken);
        if (!isValid)
            return new ApiResponse(false, errorMessage!);

        if (await dbContext.Employees
            .AnyAsync(x => x.Email == request.Employee.Email && x.Id != request.Id, cancellationToken))
            return new ApiResponse("Email already exists for another employee");

        employee.Email = request.Employee.Email;

        var (isValidDepartment, department, errorMessageDepartment) = await ValidateDepartmentAsync(request.Employee.DepartmentId, cancellationToken);
        if (!isValidDepartment)
            return new ApiResponse(false, errorMessageDepartment!);

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

    private async Task<(bool IsValid, User? User, string? ErrorMessage)> ValidateUserAsync(string userName, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(d => d.UserName == userName, cancellationToken);

        if (user != null)
            return (false, null, "User name already exists");

        return (true, user, null);
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
        employee.EmployeeNumber = new Random().Next(1000000, 999999999);
        employee.HireDate = DateTime.UtcNow;
        employee.Role = UserRole.Employee.ToString();
        employee.OpenDate = DateTime.UtcNow;
        employee.Secret = PasswordGenerator.GeneratePassword(30);
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

