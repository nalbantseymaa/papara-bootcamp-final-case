using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Api.Impl.Service;

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

    public ExpenseCommandHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession, IPaymentService paymentService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.appSession = appSession;
        this.paymentService = paymentService;
    }

    public async Task<ApiResponse<ExpenseResponse>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var categoryCheck = await ValidateEntity(dbContext.ExpenseCategories, request.Expense.CategoryId, "Category", cancellationToken);
        if (!categoryCheck.isValid) return new ApiResponse<ExpenseResponse>(categoryCheck.errorMessage!);

        var paymentCheck = await ValidateEntity(dbContext.PaymentMethods, request.Expense.PaymentMethodId, "Payment Method", cancellationToken);
        if (!paymentCheck.isValid) return new ApiResponse<ExpenseResponse>(paymentCheck.errorMessage!);

        var expenseEntity = mapper.Map<Expense>(request.Expense);
        var duplicateCheckResult = await CheckIfExpenseDuplicate(expenseEntity, cancellationToken);
        if (!duplicateCheckResult.Success)
            return new ApiResponse<ExpenseResponse>(duplicateCheckResult.Message);

        var mapped = mapper.Map<Expense>(request.Expense);
        mapped.EmployeeId = Convert.ToInt64(appSession.UserId);

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var createdExpense = await dbContext.Expenses.Include(e => e.Employee)
           .FirstOrDefaultAsync(e => e.Id == entity.Entity.Id, cancellationToken);

        var response = mapper.Map<ExpenseResponse>(createdExpense);
        return new ApiResponse<ExpenseResponse>(response);
    }

    public async Task<ApiResponse> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses.Include(e => e.Employee)
    .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (expense == null) return new ApiResponse(false, "Expense not found.");

        var expenseCheck = await ValidateEntity(dbContext.Expenses, expense.Id, "Expense", cancellationToken);
        if (!expenseCheck.isValid) return new ApiResponse(expenseCheck.errorMessage!);

        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Cannot approve expense in {expense.Status} status. Only pending expenses can be approved.");

        var paymentRequest = mapper.Map<PaymentRequest>(expense);
        var payment = await paymentService.ProcessPaymentAsync(paymentRequest, cancellationToken);

        expense.Status = ExpenseStatus.Paid;
        expense.ApprovedDate = DateTime.UtcNow;
        expense.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(payment.Success, payment.Message);
    }

    public async Task<ApiResponse> Handle(RejectExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseCheck = await ValidateEntity(dbContext.Expenses, request.Id, "Expense", cancellationToken);
        if (!expenseCheck.isValid) return new ApiResponse(expenseCheck.errorMessage!);

        var expense = expenseCheck.entity!;

        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Cannot reject expense in {expense.Status} status. Only pending expenses can be rejected.");

        if (string.IsNullOrWhiteSpace(request.RejectExpense.RejectionReason))
            return new ApiResponse(false, "Rejection reason is required");

        expense.Status = ExpenseStatus.Rejected;
        expense.RejectionReason = request.RejectExpense.RejectionReason;
        expense.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Expense rejected successfully");
    }

    public async Task<ApiResponse> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseCheck = await ValidateEntity(dbContext.Expenses, request.Id, "Expense", cancellationToken);
        if (!expenseCheck.isValid) return new ApiResponse(expenseCheck.errorMessage!);

        var expense = expenseCheck.entity!;
        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Expense is already processed and cannot be updated. Current status: {expense.Status}");

        var categoryCheck = await ValidateEntity(dbContext.ExpenseCategories, request.Expense.CategoryId, "Category", cancellationToken);
        if (!categoryCheck.isValid) return new ApiResponse(categoryCheck.errorMessage!);

        var paymentCheck = await ValidateEntity(dbContext.PaymentMethods, request.Expense.PaymentMethodId, "Payment Method", cancellationToken);
        if (!paymentCheck.isValid) return new ApiResponse(paymentCheck.errorMessage!);

        var e = request.Expense;
        expense.CategoryId = e.CategoryId > 0 ? e.CategoryId : expense.CategoryId;
        expense.PaymentMethodId = e.PaymentMethodId > 0 ? e.PaymentMethodId : expense.PaymentMethodId;
        expense.Amount = e.Amount > 0 ? e.Amount : expense.Amount;
        expense.Description = e.Description ?? expense.Description;
        expense.Location = !string.IsNullOrWhiteSpace(e.Location) ? e.Location : expense.Location;
        expense.ExpenseDate = (e.ExpenseDate != default && e.ExpenseDate <= DateTime.Today) ? e.ExpenseDate : expense.ExpenseDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Expense updated successfully");
    }

    public async Task<ApiResponse> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseCheck = await ValidateEntity(dbContext.Expenses, request.Id, "Expense", cancellationToken);
        if (!expenseCheck.isValid) return new ApiResponse(expenseCheck.errorMessage!);

        var expense = expenseCheck.entity!;
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

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Expense deleted successfully");
    }

    /// <summary>
    /// Validates the existence and active status of an entity in the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity being validated.</typeparam>
    /// <param name="dbSet">The DbSet representing the collection of entities.</param>
    /// <param name="id">The unique identifier of the entity to validate.</param>
    /// <param name="entityName">The name of the entity type, used in error messages.</param>
    /// <param name="token">The cancellation token to observe while awaiting the task.</param>
    /// <returns>
    /// A tuple containing a boolean indicating validity, an optional error message, 
    /// and the entity if found and active.
    /// </returns>
    private async Task<(bool isValid, string? errorMessage, T? entity)> ValidateEntity<T>(
    DbSet<T> dbSet, long id, string entityName, CancellationToken token) where T : class
    {
        var entity = await dbSet.FindAsync(new object[] { id }, token);
        if (entity == null)
            return (false, $"{entityName} not found", null);

        if (!((BaseEntity)(object)entity).IsActive)
            return (false, $"{entityName} is inactive", null);

        return (true, null, entity);
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