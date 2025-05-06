using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class ExpenseQueryHandler :
IRequestHandler<GetAllExpensesQuery, ApiResponse<List<ExpenseResponse>>>,
IRequestHandler<GetExpenseByIdQuery, ApiResponse<ExpenseResponse>>,
IRequestHandler<GetExpensesByParametersQuery, ApiResponse<List<ExpenseResponse>>>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAppSession appSession;


    public ExpenseQueryHandler(AppDbContext dbContext, IMapper mapper, IAppSession appSession)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.appSession = appSession;
    }

    public async Task<ApiResponse<List<ExpenseResponse>>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Expenses.Include(x => x.Employee).AsQueryable();

        query = FilterExpensesByRole(query);

        var expenses = await query.Where(x => x.IsActive).ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<ExpenseResponse>>(expenses);

        return new ApiResponse<List<ExpenseResponse>>(mapped);
    }

    public async Task<ApiResponse<ExpenseResponse>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Expenses.Include(x => x.Employee).AsQueryable();

        query = FilterExpensesByRole(query);

        var expense = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (expense == null)
        {
            return new ApiResponse<ExpenseResponse>("Expense not found.");
        }

        var mapped = mapper.Map<ExpenseResponse>(expense);
        return new ApiResponse<ExpenseResponse>(mapped);
    }

    public async Task<ApiResponse<List<ExpenseResponse>>> Handle(GetExpensesByParametersQuery request, CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<Expense>(e => e.IsActive);

        if (appSession.UserRole != UserRole.Manager.ToString())
            predicate = predicate.And(e => e.EmployeeId == long.Parse(appSession.UserId));

        if (request.CategoryId.HasValue) predicate = predicate.And(e => e.CategoryId == request.CategoryId);
        if (request.PaymentMethodId.HasValue) predicate = predicate.And(e => e.PaymentMethodId == request.PaymentMethodId);
        if (request.MinAmount.HasValue) predicate = predicate.And(e => e.Amount >= request.MinAmount);
        if (request.MaxAmount.HasValue) predicate = predicate.And(e => e.Amount <= request.MaxAmount);
        if (request.Status.HasValue) predicate = predicate.And(e => e.Status == request.Status);
        if (!string.IsNullOrWhiteSpace(request.Location)) predicate = predicate.And(e => e.Location.Contains(request.Location));

        var expenses = await dbContext.Expenses.Include(e => e.Employee)
            .Where(predicate)
            .ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<ExpenseResponse>>(expenses);
        return new ApiResponse<List<ExpenseResponse>>(mapped);
    }

    private IQueryable<Expense> FilterExpensesByRole(IQueryable<Expense> query)
    {
        if (appSession.UserRole != UserRole.Manager.ToString())
        {
            var userId = long.Parse(appSession.UserId);
            query = query.Where(x => x.EmployeeId == userId);
        }
        return query;
    }

}
