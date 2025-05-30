using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs.Category;

public record GetAllCategoriesQuery : IRequest<ApiResponse<List<CategoryResponse>>>;
public record GetCategoryByIdQuery(long Id) : IRequest<ApiResponse<CategoryDetailResponse>>;
public record CreateCategoryCommand(CategoryRequest Category) : IRequest<ApiResponse<CategoryResponse>>;
public record UpdateCategoryCommand(long Id, UpdateCategoryRequest Category) : IRequest<ApiResponse>;
public record DeleteCategoryCommand(long Id) : IRequest<ApiResponse>;