using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllDepartmentsQuery : IRequest<ApiResponse<List<DepartmentResponse>>>;
public record GetDepartmentByIdQuery(long Id) : IRequest<ApiResponse<DepartmentDetailResponse>>;
public record CreateDepartmentCommand(DepartmentRequest Department) : IRequest<ApiResponse<DepartmentResponse>>;
public record UpdateDepartmentCommand(long Id, DepartmentRequest Department) : IRequest<ApiResponse>;
public record DeleteDepartmentCommand(long Id) : IRequest<ApiResponse>;