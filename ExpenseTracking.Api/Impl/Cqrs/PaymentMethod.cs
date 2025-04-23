using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs.PaymentMethod;

public record GetAllPaymentMethodsQuery : IRequest<ApiResponse<List<PaymentMethodResponse>>>;
public record GetPaymentMethodByIdQuery(int Id) : IRequest<ApiResponse<PaymentMethodResponse>>;
public record CreatePaymentMethodCommand(PaymentMethodRequest PaymentMethod) : IRequest<ApiResponse<PaymentMethodResponse>>;
public record UpdatePaymentMethodCommand(int Id, UpdatePaymentMethodRequest PaymentMethod) : IRequest<ApiResponse>;
public record DeletePaymentMethodCommand(int Id) : IRequest<ApiResponse>;