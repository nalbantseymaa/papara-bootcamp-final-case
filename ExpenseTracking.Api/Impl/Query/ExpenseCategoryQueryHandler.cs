using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs.Category;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class ExpenseCategoryQueryHandler :
IRequestHandler<GetAllCategoriesQuery, ApiResponse<List<CategoryResponse>>>,
IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;

    public ExpenseCategoryQueryHandler(AppDbContext context, IMapper mapper)
    {
        this.dbcontext = context;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<CategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {

        var ExpenseCategorys = await dbcontext.ExpenseCategories
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<CategoryResponse>>(ExpenseCategorys);

        return new ApiResponse<List<CategoryResponse>>(mapped);
    }

    public async Task<ApiResponse<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var ExpenseCategory = await dbcontext.ExpenseCategories
            .Include(x => x.Expenses)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (ExpenseCategory == null)
        {
            return new ApiResponse<CategoryResponse>("ExpenseCategory not found");
        }

        var mapped = mapper.Map<CategoryResponse>(ExpenseCategory);
        return new ApiResponse<CategoryResponse>(mapped);
    }
}
