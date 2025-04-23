using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class ExpenseQueryHandler :
IRequestHandler<GetAllExpensesQuery, ApiResponse<List<ExpenseResponse>>>,
IRequestHandler<GetExpenseByIdQuery, ApiResponse<ExpenseResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;

    public ExpenseQueryHandler(AppDbContext context, IMapper mapper)
    {
        this.dbcontext = context;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<ExpenseResponse>>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
    {

        var Expenses = await dbcontext.Expenses
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<ExpenseResponse>>(Expenses);

        return new ApiResponse<List<ExpenseResponse>>(mapped);
    }

    public async Task<ApiResponse<ExpenseResponse>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var Expense = await dbcontext.Expenses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (Expense == null)
        {
            return new ApiResponse<ExpenseResponse>("Expense not found");
        }

        var mapped = mapper.Map<ExpenseResponse>(Expense);
        return new ApiResponse<ExpenseResponse>(mapped);
    }
}
