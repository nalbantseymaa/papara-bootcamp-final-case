using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllManagersQuery : IRequest<ApiResponse<List<ManagerResponse>>>;
public record GetManagerByIdQuery(int Id) : IRequest<ApiResponse<ManagerResponse>>;
public record CreateManagerCommand(UserRequest User, ManagerRequest Manager) : IRequest<ApiResponse<CreateManagerResponse>>;
public record UpdateManagerCommand(int Id, ManagerRequest Manager) : IRequest<ApiResponse>;
public record DeleteManagerCommand(int Id) : IRequest<ApiResponse>;