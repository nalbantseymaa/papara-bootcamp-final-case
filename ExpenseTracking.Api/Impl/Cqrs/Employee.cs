using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllEmployeesQuery : IRequest<ApiResponse<List<EmployeeResponse>>>;
public record GetEmployeeByIdQuery(int Id) : IRequest<ApiResponse<EmployeeResponse>>;
public record CreateEmployeeCommand(EmployeeRequest Employee) : IRequest<ApiResponse<EmployeeResponse>>;
public record UpdateEmployeeCommand(int Id, UpdateEmployeeRequest Employee) : IRequest<ApiResponse>;
public record DeleteEmployeeCommand(int Id) : IRequest<ApiResponse>;