using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.UnitOfWork;

namespace ExpenseTracking.Api.Impl.Command;

public class ExpenseCommandHandler :
    IRequestHandler<CreateExpenseCommand, ApiResponse<ExpenseResponse>>,
    IRequestHandler<UpdateExpenseCommand, ApiResponse>,
    IRequestHandler<DeleteExpenseCommand, ApiResponse>,
    IRequestHandler<ApproveExpenseCommand, ApiResponse>,
    IRequestHandler<RejectExpenseCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAppSession appSession;
    private readonly IPaymentService paymentService;
    private readonly IGenericEntityValidator genericEntityValidator;
    private readonly IUnitOfWork unitOfWork;

    public ExpenseCommandHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession, IPaymentService paymentService, IGenericEntityValidator genericEntityValidator, IUnitOfWork unitOfWork)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.appSession = appSession;
        this.paymentService = paymentService;
        this.genericEntityValidator = genericEntityValidator;
        this.unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ExpenseResponse>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var validateResultCategory = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.ExpenseCategories, request.Expense.CategoryId, cancellationToken);
        if (!validateResultCategory.IsValid)
            return new ApiResponse<ExpenseResponse>(validateResultCategory.ErrorMessage!);
        var validateResultPaymentMethod = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.PaymentMethods, request.Expense.PaymentMethodId, cancellationToken);
        if (!validateResultPaymentMethod.IsValid)
            return new ApiResponse<ExpenseResponse>(validateResultPaymentMethod.ErrorMessage!);

        var expenseEntity = mapper.Map<Expense>(request.Expense);
        var duplicateCheckResult = await CheckIfExpenseDuplicate(expenseEntity, cancellationToken);
        if (!duplicateCheckResult.Success)
            return new ApiResponse<ExpenseResponse>(duplicateCheckResult.Message);

        var mapped = mapper.Map<Expense>(request.Expense);
        mapped.EmployeeId = Convert.ToInt64(appSession.UserId);

        await unitOfWork.Repository<Expense>().AddAsync(mapped);
        await unitOfWork.CommitAsync();
        var createdExpense = await unitOfWork.Repository<Expense>()
            .GetByIdAsync(mapped.Id);
        var response = mapper.Map<ExpenseResponse>(createdExpense);
        return new ApiResponse<ExpenseResponse>(response);
    }

    public async Task<ApiResponse> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses.Include(e => e.Employee)
        .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null) return new ApiResponse(false, "Expense not found.");

        var validateResultExpense = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Expenses, request.Id, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(validateResultExpense.ErrorMessage!);
        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Cannot approve expense in {expense.Status} status. Only pending expenses can be approved.");

        var paymentRequest = mapper.Map<PaymentRequest>(expense);
        var payment = await paymentService.ProcessPaymentAsync(paymentRequest, cancellationToken);

        expense.Status = ExpenseStatus.Paid;
        expense.ApprovedDate = DateTime.UtcNow;
        expense.IsActive = false;

        unitOfWork.Repository<Expense>().Update(expense);
        await unitOfWork.CommitAsync();
        return new ApiResponse(payment.Success, payment.Message);
    }

    public async Task<ApiResponse> Handle(RejectExpenseCommand request, CancellationToken cancellationToken)
    {
        var validateResultExpense = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Expenses, request.Id, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(validateResultExpense.ErrorMessage!);

        var expense = validateResultExpense.Entity!;

        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Cannot reject expense in {expense.Status} status. Only pending expenses can be rejected.");

        if (string.IsNullOrWhiteSpace(request.RejectExpense.RejectionReason))
            return new ApiResponse(false, "Rejection reason is required");

        expense.Status = ExpenseStatus.Rejected;
        expense.RejectionReason = request.RejectExpense.RejectionReason;
        expense.IsActive = false;

        unitOfWork.Repository<Expense>().Update(expense);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Expense rejected successfully");
    }

    public async Task<ApiResponse> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var validateResultExpense = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Expenses, request.Id, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(validateResultExpense.ErrorMessage!);

        var expense = validateResultExpense.Entity!;
        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Expense is already processed and cannot be updated. Current status: {expense.Status}");

        var validateResultCategory = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.ExpenseCategories, request.Id, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(validateResultExpense.ErrorMessage!);

        var validateResultPaymentMethod = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.PaymentMethods, request.Id, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(validateResultExpense.ErrorMessage!);
        var e = request.Expense;
        expense.CategoryId = e.CategoryId > 0 ? e.CategoryId : expense.CategoryId;
        expense.PaymentMethodId = e.PaymentMethodId > 0 ? e.PaymentMethodId : expense.PaymentMethodId;
        expense.Amount = e.Amount > 0 ? e.Amount : expense.Amount;
        expense.Description = e.Description ?? expense.Description;
        expense.Location = !string.IsNullOrWhiteSpace(e.Location) ? e.Location : expense.Location;
        expense.ExpenseDate = (e.ExpenseDate != default && e.ExpenseDate <= DateTime.Today) ? e.ExpenseDate : expense.ExpenseDate;

        unitOfWork.Repository<Expense>().Update(expense);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Expense updated successfully");
    }

    public async Task<ApiResponse> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var validateResultExpense = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Expenses, request.Id, cancellationToken);
        if (!validateResultExpense.IsValid)
            return new ApiResponse(validateResultExpense.ErrorMessage!);
        var expense = validateResultExpense.Entity!;
        if (expense.Status == ExpenseStatus.Pending)
            return new ApiResponse(false, $"Cannot delete expense in {expense.Status} status. Only approved and rejected expenses can be deleted.");

        expense.IsActive = false;

        var expenseFiles = await dbContext.ExpenseFiles
        .Where(f => f.ExpenseId == expense.Id && f.IsActive)
        .ToListAsync(cancellationToken);

        foreach (var file in expenseFiles)
        {
            file.IsActive = false;
        }

        unitOfWork.Repository<Expense>().Remove(expense);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Expense deleted successfully");
    }

    public async Task<ApiResponse> CheckIfExpenseDuplicate(Expense expense, CancellationToken cancellationToken)
    {
        bool isDuplicate = await dbContext.Expenses
            .AnyAsync(e =>
                e.EmployeeId == Convert.ToInt64(appSession.UserId) &&
                e.CategoryId == expense.CategoryId &&
                e.PaymentMethodId == expense.PaymentMethodId &&
                e.Location == expense.Location &&
                e.Amount == expense.Amount &&
                e.ExpenseDate.Date == expense.ExpenseDate.Date &&
                e.Description == expense.Description,
                cancellationToken);

        if (isDuplicate)
        {
            return new ApiResponse(false, "A similar expense already exists. Duplicate entries are not allowed.");
        }
        return new ApiResponse();
    }
}