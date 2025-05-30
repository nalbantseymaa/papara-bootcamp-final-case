using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllEmployeesQuery : IRequest<ApiResponse<List<EmployeeResponse>>>;
public record GetEmployeeByIdQuery(long Id) : IRequest<ApiResponse<EmployeeDetailResponse>>;
public record GetEmployeesByParametersQuery(long? DepartmentId, int? MinSalary, int? MaxSalary) : IRequest<ApiResponse<List<EmployeeResponse>>>;

public record CreateEmployeeCommand(
    UserRequest User,
    EmployeeRequest Employee
) : IRequest<ApiResponse<CreateEmployeeResponse>>;

public record UpdateEmployeeCommand(long Id, UpdateEmployeeRequest Employee) : IRequest<ApiResponse>;
public record DeleteEmployeeCommand(long Id) : IRequest<ApiResponse>;