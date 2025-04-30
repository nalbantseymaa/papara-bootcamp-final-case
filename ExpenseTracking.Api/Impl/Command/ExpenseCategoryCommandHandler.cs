using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs.Category;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class ExpenseCategoryCommandHandler :
    IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryResponse>>,
    IRequestHandler<UpdateCategoryCommand, ApiResponse>,
    IRequestHandler<DeleteCategoryCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public ExpenseCategoryCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        bool exists = await dbContext.ExpenseCategories.AnyAsync(x => x.Name == request.Category.Name, cancellationToken);
        if (exists)
            return new ApiResponse<CategoryResponse>("Category already exists");

        var mapped = mapper.Map<ExpenseCategory>(request.Category);

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<CategoryResponse>(entity.Entity);
        return new ApiResponse<CategoryResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var validation = await ValidateCategoryAsync(request.Id, cancellationToken);
        if (!validation.Success)
            return validation;

        var entity = await dbContext.ExpenseCategories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        entity.Name = string.IsNullOrWhiteSpace(request.Category.Name) ? entity.Name : request.Category.Name;
        entity.Description = string.IsNullOrWhiteSpace(request.Category.Description) ? entity.Description : request.Category.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Category successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var validation = await ValidateCategoryAsync(request.Id, cancellationToken);
        if (!validation.Success)
            return validation;

        var entity = await dbContext.ExpenseCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        bool hasExpenses = await dbContext.Expenses
            .AnyAsync(e => e.CategoryId == request.Id, cancellationToken);
        if (hasExpenses)
            return new ApiResponse("Category cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ApiResponse(true, "Category successfully deleted");
    }

    private async Task<ApiResponse> ValidateCategoryAsync(long? categoryId,
      CancellationToken cancellationToken)
    {
        if (!categoryId.HasValue)
            return new ApiResponse(true, string.Empty);

        var category = await dbContext.ExpenseCategories
        .FirstOrDefaultAsync(e => e.Id == categoryId.Value, cancellationToken);

        if (category == null)
            return new ApiResponse(false, "Category not found");

        if (!category.IsActive)
            return new ApiResponse(false, "Category is inactive");

        return new ApiResponse(true, string.Empty);
    }
}