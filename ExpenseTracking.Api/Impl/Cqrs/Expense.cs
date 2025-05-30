using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllExpensesQuery : IRequest<ApiResponse<List<ExpenseResponse>>>;
public record GetExpenseByIdQuery(int Id) : IRequest<ApiResponse<ExpenseResponse>>;
public record GetExpensesByParametersQuery(long? CategoryId, long? PaymentMethodId, decimal? MinAmount, decimal? MaxAmount, ExpenseStatus? Status, string? Location) : IRequest<ApiResponse<List<ExpenseResponse>>>;
public record CreateExpenseCommand(ExpenseRequest Expense)
  : IRequest<ApiResponse<ExpenseResponse>>;
public record UpdateExpenseCommand(int Id, ExpenseRequest Expense) : IRequest<ApiResponse>;
public record ApproveExpenseCommand(int Id) : IRequest<ApiResponse>;
public record RejectExpenseCommand(int Id, RejectExpenseRequest RejectExpense) : IRequest<ApiResponse>;
public record DeleteExpenseCommand(int Id) : IRequest<ApiResponse>;