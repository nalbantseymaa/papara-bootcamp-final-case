using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Api.Impl.Service.Helper;
using ExpenseTracking.Api.Impl.UnitOfWork;
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
    private readonly IUnitOfWork unitOfWork;
    private readonly IGenericEntityValidator genericEntityValidator;

    public EmployeeCommandHandler(AppDbContext dbContext, IMapper mapper, IUserService userService, IUnitOfWork unitOfWork, IGenericEntityValidator genericEntityValidator)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userService = userService;
        this.unitOfWork = unitOfWork;
        this.genericEntityValidator = genericEntityValidator;
    }

    public async Task<ApiResponse<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Departments, request.Employee.DepartmentId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse<CreateEmployeeResponse>(validationResult.ErrorMessage!);

        var user = mapper.Map<UserRequest>(request.User);

        var userResponse = await userService.CreateUserAsync(user, UserRole.Employee.ToString());
        if (userResponse.userEntity == null)
            return new ApiResponse<CreateEmployeeResponse>("User creation failed");

        var employee = mapper.Map<Employee>(request.Employee);
        mapper.Map(userResponse.userEntity, employee);

        InitializeNewEmployee(employee);

        await unitOfWork.Repository<Employee>().AddAsync(employee);
        await unitOfWork.CommitAsync();

        return new ApiResponse<CreateEmployeeResponse>(new CreateEmployeeResponse
        {
            User = mapper.Map<UserResponse>(employee),
            Employee = mapper.Map<EmployeeResponse>(employee),
            PlainPassword = userResponse.plainPassword
        });
    }

    public async Task<ApiResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var validationResultEmployee = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Employees, request.Id, cancellationToken);
        if (!validationResultEmployee.IsValid)
            return new ApiResponse(false, validationResultEmployee.ErrorMessage!);

        var validationResultDepartment = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Departments, request.Employee.DepartmentId, cancellationToken);
        if (!validationResultDepartment.IsValid)
            return new ApiResponse(false, validationResultDepartment.ErrorMessage!);

        var employee = validationResultEmployee.Entity;

        employee.DepartmentId = (int)request.Employee.DepartmentId;
        employee.Salary = request.Employee.Salary;

        unitOfWork.Repository<Employee>().Update(employee);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Employee updated successfully");
    }

    public async Task<ApiResponse> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Employees, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(false, validationResult.ErrorMessage!);

        var employee = await dbContext.Employees
            .Include(e => e.Phones)
            .Include(e => e.Addresses)
            .Include(e => e.ManagedDepartments)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        employee.IsActive = false;
        employee.Phones?.ToList().ForEach(p => p.IsActive = false);
        employee.Addresses?.ToList().ForEach(a => a.IsActive = false);
        employee.ManagedDepartments?.ToList().ForEach(d => d.ManagerId = null);

        unitOfWork.Repository<Employee>().Remove(employee);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Employee successfully deleted");
    }

    private void InitializeNewEmployee(Employee employee)
    {
        employee.HireDate = DateTime.UtcNow;
        employee.OpenDate = DateTime.UtcNow;
        employee.IBAN = IbanGenerator.GenerateIban();
    }

}

