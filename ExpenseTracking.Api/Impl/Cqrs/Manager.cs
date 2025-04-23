using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllManagersQuery : IRequest<ApiResponse<List<ManagerResponse>>>;
public record GetManagerByIdQuery(int Id) : IRequest<ApiResponse<ManagerResponse>>;
public record CreateManagerCommand(ManagerRequest Manager) : IRequest<ApiResponse<ManagerResponse>>;
public record UpdateManagerCommand(int Id, ManagerRequest Manager) : IRequest<ApiResponse>;
public record DeleteManagerCommand(int Id) : IRequest<ApiResponse>;