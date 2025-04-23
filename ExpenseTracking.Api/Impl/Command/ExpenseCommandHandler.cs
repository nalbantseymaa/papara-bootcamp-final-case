using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Api.Enum;


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

    public ExpenseCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    public async Task<ApiResponse<ExpenseResponse>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Expense>(request.Expense);

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<ExpenseResponse>(entity.Entity);
        return new ApiResponse<ExpenseResponse>(response);
    }

    public async Task<ApiResponse> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (expense == null)
            return new ApiResponse("Expense not found");

        if (!expense.IsActive)
            return new ApiResponse("Expense is not active");

        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse("Only pending expenses can be approved");

        expense.Status = ExpenseStatus.Approved;
        expense.UpdatedDate = DateTime.UtcNow;
        expense.ApprovedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse("Expense approved successfully");
    }

    public async Task<ApiResponse> Handle(RejectExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (expense == null)
            return new ApiResponse("Expense not found");

        if (!expense.IsActive)
            return new ApiResponse("Expense is not active");

        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse($"Cannot reject expense in {expense.Status} status. Only pending expenses can be rejected.");

        if (string.IsNullOrWhiteSpace(request.RejectExpense.RejectionReason))
            return new ApiResponse("Rejection reason is required");

        expense.Status = ExpenseStatus.Rejected;
        expense.RejectionReason = request.RejectExpense.RejectionReason;
        expense.UpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse("Expense rejected successfully");

    }

    public async Task<ApiResponse> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await dbContext.Expenses
            .Include(x => x.Category)
            .Include(x => x.PaymentMethod)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (expense == null)
            return new ApiResponse("Expense not found");

        if (!expense.IsActive)
            return new ApiResponse("Expense is not active");

        if (expense.Status != ExpenseStatus.Pending)
            return new ApiResponse("Only pending expenses can be updated");

        if (request.Expense.CategoryId > 0)
        {
            var categoryExists = await dbContext.ExpenseCategories
                .AnyAsync(x => x.Id == request.Expense.CategoryId && x.IsActive, cancellationToken);
            if (!categoryExists)
                return new ApiResponse("Invalid category");
            expense.CategoryId = request.Expense.CategoryId;
        }

        if (request.Expense.PaymentMethodId > 0)
        {
            var paymentMethodExists = await dbContext.PaymentMethods
                .AnyAsync(x => x.Id == request.Expense.PaymentMethodId && x.IsActive, cancellationToken);
            if (!paymentMethodExists)
                return new ApiResponse("Invalid payment method");
            expense.PaymentMethodId = request.Expense.PaymentMethodId;
        }

        var e = request.Expense;

        expense.Amount = e.Amount > 0
                              ? e.Amount
                              : expense.Amount;

        expense.Description = e.Description
                              ?? expense.Description;

        expense.Location = !string.IsNullOrWhiteSpace(e.Location)
                              ? e.Location
                              : expense.Location;

        expense.ExpenseDate = (e.ExpenseDate != default && e.ExpenseDate <= DateTime.Today)
                              ? e.ExpenseDate
                              : expense.ExpenseDate;


        expense.UpdatedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse("Expense updated successfully");

    }

    public async Task<ApiResponse> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ExpenseCategories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Expense not found");

        if (!entity.IsActive)
            return new ApiResponse("Expense is not active");

        if (entity.Expenses.Any())
            return new ApiResponse("Expense cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }


}