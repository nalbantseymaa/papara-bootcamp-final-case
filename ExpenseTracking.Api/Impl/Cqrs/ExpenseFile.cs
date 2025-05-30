using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllExpenseFilesQuery : IRequest<ApiResponse<List<ExpenseFileResponse>>>;
public record GetExpenseFileByIdQuery(long Id) : IRequest<ApiResponse<ExpenseFileResponse>>;
public record CreateExpenseFileCommand(ExpenseFileRequest ExpenseFile) : IRequest<ApiResponse>;
public record UpdateExpenseFileCommand(long Id, UpdateExpenseFileRequest ExpenseFile) : IRequest<ApiResponse>;
public record DeleteExpenseFileCommand(long Id) : IRequest<ApiResponse>;