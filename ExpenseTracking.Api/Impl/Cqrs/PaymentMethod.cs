using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs.PaymentMethod;

public record GetAllPaymentMethodsQuery : IRequest<ApiResponse<List<PaymentMethodResponse>>>;
public record GetPaymentMethodByIdQuery(long Id) : IRequest<ApiResponse<PaymentMethodResponse>>;
public record CreatePaymentMethodCommand(PaymentMethodRequest PaymentMethod) : IRequest<ApiResponse<PaymentMethodResponse>>;
public record UpdatePaymentMethodCommand(long Id, UpdatePaymentMethodRequest PaymentMethod) : IRequest<ApiResponse>;
public record DeletePaymentMethodCommand(long Id) : IRequest<ApiResponse>;