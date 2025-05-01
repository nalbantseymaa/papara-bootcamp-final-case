using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseFileTracking.Api.Impl.Command;

public class ExpenseFileCommandHandler :
     IRequestHandler<CreateExpenseFileCommand, ApiResponse>,
     IRequestHandler<UpdateExpenseFileCommand, ApiResponse>,
     IRequestHandler<DeleteExpenseFileCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAppSession appSession;

    private static class Message
    {
        public const string NotFound = "not found";
        public const string Inactive = "is inactive";
        public const string BadFormat = "Unsupported file format";
        public const string UploadSuccess = "File uploaded successfully";
        public const string UpdateSuccess = "File updated successfully";
        public const string DeleteSuccess = "File deleted successfully";
        public const string CannotUpdate = "Only files belonging to pending expenses can be updated";
        public const string ExtensionChangeError = "File extension cannot be changed";
        public const string CannotDeleteAdmin = "Admin cannot delete files belonging to pending expenses";
        public const string CannotDeleteUser = "Employee can only delete files belonging to pending expenses";
    }

    public ExpenseFileCommandHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.appSession = appSession;
    }

    public async Task<ApiResponse> Handle(CreateExpenseFileCommand request, CancellationToken cancellationToken)
    {
        var (isValid, expense, errorMessage) = await ValidateExpenseAsync(request.ExpenseFile.ExpenseId, cancellationToken);
        if (!isValid) return new ApiResponse(false, errorMessage!);

        var fileExtension = Path.GetExtension(request.ExpenseFile.File.FileName).ToLowerInvariant();
        var fileType = GetFileType(fileExtension);

        if (!fileType.HasValue)
            return new ApiResponse(false, Message.BadFormat);

        var fileData = await ReadAllBytesAsync(request.ExpenseFile.File, cancellationToken);

        using var memoryStream = new MemoryStream();
        await request.ExpenseFile.File.CopyToAsync(memoryStream, cancellationToken);

        var mapped = mapper.Map<ExpenseFile>(request.ExpenseFile);
        mapped.FileData = fileData;
        mapped.FileType = fileType.Value;

        var entity = await dbContext.ExpenseFiles.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<ExpenseFileResponse>(entity.Entity);
        return new ApiResponse(true, Message.UploadSuccess);
    }

    public async Task<ApiResponse> Handle(UpdateExpenseFileCommand request, CancellationToken cancellationToken)
    {
        var (isValid, expenseFile, errorMessage) = await ValidateExpenseFileAsync(request.Id, cancellationToken);
        if (!isValid) return new ApiResponse(false, errorMessage!);

        var expenseStatus = await GetExpenseStatusAsync(expenseFile.ExpenseId, cancellationToken);
        if (expenseStatus != ExpenseStatus.Pending) return new ApiResponse(Message.CannotUpdate);

        var newFile = request.ExpenseFile.File;

        if (GetFileType(Path.GetExtension(expenseFile.FileName))
         != GetFileType(Path.GetExtension(newFile.FileName)))
            return new ApiResponse(Message.ExtensionChangeError);

        var fileData = await ReadAllBytesAsync(newFile, cancellationToken);

        expenseFile.FileName = newFile.FileName;
        expenseFile.FileData = fileData;
        expenseFile.FileSize = fileData.Length;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, Message.UpdateSuccess);
    }

    public async Task<ApiResponse> Handle(DeleteExpenseFileCommand request, CancellationToken cancellationToken)
    {
        var (isValid, expenseFile, errorMessage) = await ValidateExpenseFileAsync(request.Id, cancellationToken);
        if (!isValid)
            return new ApiResponse(false, errorMessage!);

        var isManager = appSession.UserRole == UserRole.Manager.ToString();

        if (isManager && expenseFile.Expense?.Status == ExpenseStatus.Pending)
            return new ApiResponse(Message.CannotDeleteAdmin);

        if (!isManager && expenseFile.Expense?.Status != ExpenseStatus.Pending)
            return new ApiResponse(Message.CannotDeleteUser);

        expenseFile.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, Message.DeleteSuccess);
    }

    private FileType? GetFileType(string extension)
    {
        return extension switch
        {
            ".pdf" => FileType.PDF,
            ".jpg" or ".jpeg" => FileType.JPG,
            ".png" => FileType.PNG,
            _ => null
        };
    }

    private static async Task<byte[]> ReadAllBytesAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();

    }

    private async Task<ExpenseStatus?> GetExpenseStatusAsync(long expenseId, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .Where(x => x.Id == expenseId && x.IsActive)
            .Select(x => x.Status)
            .FirstOrDefaultAsync(cancellationToken);

        return expense;
    }

    private async Task<(bool IsValid, Expense? Expense, string? ErrorMessage)> ValidateExpenseAsync(long expenseId, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .Where(x => x.Id == expenseId)
            .FirstOrDefaultAsync(cancellationToken);

        if (expense == null)
            return (false, null, $"{nameof(Expense)} {Message.NotFound}");

        if (!expense.IsActive)
            return (false, null, $"{nameof(Expense)} {Message.Inactive}");

        return (true, expense, null);
    }

    private async Task<(bool IsValid, ExpenseFile? ExpenseFile, string? ErrorMessage)> ValidateExpenseFileAsync(long expenseFileId, CancellationToken cancellationToken)
    {
        var expenseFile = await dbContext.ExpenseFiles
            .FirstOrDefaultAsync(d => d.Id == expenseFileId, cancellationToken);

        if (expenseFile == null)
            return (false, null, $"{nameof(ExpenseFile)} {Message.NotFound}");

        if (!expenseFile.IsActive)
            return (false, null, $"{nameof(ExpenseFile)} {Message.Inactive}");

        return (true, expenseFile, null);
    }
}