using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllDepartmentsQuery : IRequest<ApiResponse<List<DepartmentResponse>>>;
public record GetDepartmentByIdQuery(int Id) : IRequest<ApiResponse<DepartmentResponse>>;
public record CreateDepartmentCommand(DepartmentRequest Department) : IRequest<ApiResponse<DepartmentResponse>>;
public record UpdateDepartmentCommand(int Id, DepartmentRequest Department) : IRequest<ApiResponse>;
public record DeleteDepartmentCommand(int Id) : IRequest<ApiResponse>;