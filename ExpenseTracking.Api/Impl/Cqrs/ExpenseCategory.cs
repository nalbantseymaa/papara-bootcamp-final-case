using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;

namespace ExpenseTracking.Api.Impl.Cqrs.Category;

public record GetAllCategoriesQuery : IRequest<ApiResponse<List<CategoryResponse>>>;
public record GetCategoryByIdQuery(int Id) : IRequest<ApiResponse<CategoryResponse>>;
public record CreateCategoryCommand(CategoryRequest Category) : IRequest<ApiResponse<CategoryResponse>>;
public record UpdateCategoryCommand(int Id, UpdateCategoryRequest Category) : IRequest<ApiResponse>;
public record DeleteCategoryCommand(int Id) : IRequest<ApiResponse>;