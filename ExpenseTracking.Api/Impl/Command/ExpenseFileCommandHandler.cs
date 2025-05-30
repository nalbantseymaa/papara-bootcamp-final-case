using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.UnitOfWork;
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
    private readonly IUnitOfWork unitOfWork;
    private readonly IGenericEntityValidator genericEntityValidator;

    private static class Message
    {
        public const string BadFormat = "Unsupported file format";
        public const string UploadSuccess = "File uploaded successfully";
        public const string UpdateSuccess = "File updated successfully";
        public const string DeleteSuccess = "File deleted successfully";
        public const string CannotUpdate = "Only files belonging to pending expenses can be updated";
        public const string ExtensionChangeError = "File extension cannot be changed";
        public const string CannotDeleteAdmin = "Admin cannot delete files belonging to pending expenses";
        public const string CannotDeleteUser = "Employee can only delete files belonging to pending expenses";
    }

    public ExpenseFileCommandHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession,
        IUnitOfWork unitOfWork, IGenericEntityValidator genericEntityValidator)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.appSession = appSession;
        this.unitOfWork = unitOfWork;
        this.genericEntityValidator = genericEntityValidator;
    }

    public async Task<ApiResponse> Handle(CreateExpenseFileCommand request, CancellationToken cancellationToken)
    {
        var validateResultExpense = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Expenses, request.ExpenseFile.ExpenseId, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(false, validateResultExpense.ErrorMessage!);

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

        await unitOfWork.Repository<ExpenseFile>().AddAsync(mapped);
        await unitOfWork.CommitAsync();
        var response = mapper.Map<ExpenseFileResponse>(mapped);
        return new ApiResponse(true, Message.UploadSuccess);
    }

    public async Task<ApiResponse> Handle(UpdateExpenseFileCommand request, CancellationToken cancellationToken)
    {

        var validateResultExpenseFile = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.ExpenseFiles, request.Id, cancellationToken);
        if (!validateResultExpenseFile.IsValid)
            return new ApiResponse(false, validateResultExpenseFile.ErrorMessage!);

        var expenseFile = validateResultExpenseFile.Entity!;

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

        unitOfWork.Repository<ExpenseFile>().Update(expenseFile);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, Message.UpdateSuccess);
    }

    public async Task<ApiResponse> Handle(DeleteExpenseFileCommand request, CancellationToken cancellationToken)
    {
        var ValidateResultExpenseFile = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.ExpenseFiles, request.Id, cancellationToken);
        if (!ValidateResultExpenseFile.IsValid)
            return new ApiResponse(false, ValidateResultExpenseFile.ErrorMessage!);

        var expenseFile = ValidateResultExpenseFile.Entity!;

        var isManager = appSession.UserRole == UserRole.Manager.ToString();

        if (isManager && expenseFile.Expense?.Status == ExpenseStatus.Pending)
            return new ApiResponse(Message.CannotDeleteAdmin);

        if (!isManager && expenseFile.Expense?.Status != ExpenseStatus.Pending)
            return new ApiResponse(Message.CannotDeleteUser);

        expenseFile.IsActive = false;

        unitOfWork.Repository<ExpenseFile>().Remove(expenseFile);
        await unitOfWork.CommitAsync();
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
}