using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs.Category;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.Service.Cache;
using ExpenseTracking.Api.Impl.UnitOfWork;
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
    private readonly IUnitOfWork unitOfWork;
    private readonly IGenericEntityValidator genericEntityValidator;
    private readonly ICacheService<ExpenseCategory> cacheService;


    public ExpenseCategoryCommandHandler(AppDbContext dbContext, IMapper mapper, IUnitOfWork unitOfWork, IGenericEntityValidator genericEntityValidator, ICacheService<ExpenseCategory> cacheService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.genericEntityValidator = genericEntityValidator;
        this.cacheService = cacheService;
    }

    public async Task<ApiResponse<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        bool exists = await dbContext.ExpenseCategories.AnyAsync(x => x.Name == request.Category.Name, cancellationToken);
        if (exists)
            return new ApiResponse<CategoryResponse>("Category already exists");

        var mapped = mapper.Map<ExpenseCategory>(request.Category);

        await unitOfWork.Repository<ExpenseCategory>().AddAsync(mapped);
        await unitOfWork.CommitAsync();

        var allCategories = await unitOfWork.Repository<ExpenseCategory>().GetAllAsync();
        await cacheService.SetAsync<ExpenseCategory, CategoryResponse>("categories", allCategories.ToList());

        var response = mapper.Map<CategoryResponse>(mapped);
        return new ApiResponse<CategoryResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.ExpenseCategories, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var entity = await unitOfWork.Repository<ExpenseCategory>()
            .GetByIdAsync(request.Id);

        entity.Name = string.IsNullOrWhiteSpace(request.Category.Name) ? entity.Name : request.Category.Name;
        entity.Description = string.IsNullOrWhiteSpace(request.Category.Description) ? entity.Description : request.Category.Description;

        unitOfWork.Repository<ExpenseCategory>().Update(entity);
        await unitOfWork.CommitAsync();
        var allCategories = await unitOfWork.Repository<ExpenseCategory>().GetAllAsync();
        await cacheService.SetAsync<ExpenseCategory, CategoryResponse>("categories", allCategories.ToList());
        return new ApiResponse(true, "Category successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.ExpenseCategories, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var entity = await unitOfWork.Repository<ExpenseCategory>()
            .GetByIdAsync(request.Id);
        bool hasExpenses = await dbContext.Expenses
            .AnyAsync(e => e.CategoryId == request.Id, cancellationToken);
        if (hasExpenses)
            return new ApiResponse("Category cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;
        unitOfWork.Repository<ExpenseCategory>().Remove(entity);
        await unitOfWork.CommitAsync();

        var allCategories = await unitOfWork.Repository<ExpenseCategory>().GetAllAsync();
        await cacheService.SetAsync<ExpenseCategory, CategoryResponse>("categories", allCategories.ToList());

        return new ApiResponse(true, "Category successfully deleted");
    }

}