using ExpenseTracking.Api.Schema;
using ExpenseTracking.Base;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record CreateAuthorizationTokenCommand(AuthorizationRequest Request) : IRequest<ApiResponse<AuthorizationResponse>>;