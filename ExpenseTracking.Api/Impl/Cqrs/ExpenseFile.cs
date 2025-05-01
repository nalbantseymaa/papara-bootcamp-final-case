using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs;

public record GetAllExpenseFilesQuery : IRequest<ApiResponse<List<ExpenseFileResponse>>>;
public record GetExpenseFileByIdQuery(int Id) : IRequest<ApiResponse<ExpenseFileResponse>>;
public record CreateExpenseFileCommand(ExpenseFileRequest ExpenseFile) : IRequest<ApiResponse>;
public record UpdateExpenseFileCommand(int Id, UpdateExpenseFileRequest ExpenseFile) : IRequest<ApiResponse>;
public record DeleteExpenseFileCommand(int Id) : IRequest<ApiResponse>;