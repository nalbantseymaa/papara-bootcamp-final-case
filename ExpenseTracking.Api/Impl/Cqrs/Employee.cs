using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllEmployeesQuery : IRequest<ApiResponse<List<EmployeeResponse>>>;
public record GetEmployeeByIdQuery(int Id) : IRequest<ApiResponse<EmployeeDetailResponse>>;
public record GetEmployeesByParametersQuery(long? DepartmentId, int? MinSalary, int? MaxSalary) : IRequest<ApiResponse<List<EmployeeResponse>>>;

public record CreateEmployeeCommand(
    UserRequest User,
    EmployeeRequest Employee
) : IRequest<ApiResponse<CreateEmployeeResponse>>;

public record UpdateEmployeeCommand(int Id, UpdateEmployeeRequest Employee) : IRequest<ApiResponse>;
public record DeleteEmployeeCommand(int Id) : IRequest<ApiResponse>;