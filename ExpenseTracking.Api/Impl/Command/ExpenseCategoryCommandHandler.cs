using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
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

    public async Task<ApiResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ExpenseCategories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Category not found");

        if (!entity.IsActive)
            return new ApiResponse("Category is not active");

        if (entity.Expenses.Any())
            return new ApiResponse("Category cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }


    public async Task<ApiResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.ExpenseCategories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Category not found");

        if (!entity.IsActive)
            return new ApiResponse("Category is not active");

        entity.Name = string.IsNullOrWhiteSpace(request.Category.Name) ? entity.Name : request.Category.Name;
        entity.Description = string.IsNullOrWhiteSpace(request.Category.Description) ? entity.Description : request.Category.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<ExpenseCategory>(request.Category);

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<CategoryResponse>(entity.Entity);
        return new ApiResponse<CategoryResponse>(response);
    }
}