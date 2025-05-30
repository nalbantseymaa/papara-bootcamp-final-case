using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs.Category;
using ExpenseTracking.Api.Impl.Service.Cache;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class ExpenseCategoryQueryHandler :
IRequestHandler<GetAllCategoriesQuery, ApiResponse<List<CategoryResponse>>>,
IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDetailResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;
    private readonly ICacheService<ExpenseCategory> cacheService;

    public ExpenseCategoryQueryHandler(AppDbContext context, IMapper mapper, ICacheService<ExpenseCategory> cacheService)
    {
        this.dbcontext = context;
        this.mapper = mapper;
        this.cacheService = cacheService;
    }

    public async Task<ApiResponse<List<CategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var expenseCategories = await cacheService.GetAllAsync<CategoryResponse>("categories");

        return new ApiResponse<List<CategoryResponse>>(expenseCategories);
    }

    public async Task<ApiResponse<CategoryDetailResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var ExpenseCategory = await dbcontext.ExpenseCategories
            .Include(x => x.Expenses)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (ExpenseCategory == null)
        {
            return new ApiResponse<CategoryDetailResponse>("ExpenseCategory not found");
        }

        var mapped = mapper.Map<CategoryDetailResponse>(ExpenseCategory);
        return new ApiResponse<CategoryDetailResponse>(mapped);
    }
}
