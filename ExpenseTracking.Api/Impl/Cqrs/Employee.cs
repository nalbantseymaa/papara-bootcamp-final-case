using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace Namespace.ExpenseTracking.Api.Impl.Cqrs.Employee;

public record GetAllEmployeesQuery : IRequest<ApiResponse<List<EmployeeResponse>>>;
public record GetEmployeeByIdQuery(int Id) : IRequest<ApiResponse<EmployeeResponse>>;
public record CreateEmployeeCommand(EmployeeRequest Employee) : IRequest<ApiResponse<EmployeeResponse>>;
public record UpdateEmployeeCommand(int Id, UpdateEmployeeRequest Employee) : IRequest<ApiResponse>;
public record DeleteEmployeeCommand(int Id) : IRequest<ApiResponse>;