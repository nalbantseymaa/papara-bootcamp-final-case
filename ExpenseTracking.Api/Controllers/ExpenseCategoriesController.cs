using ExpenseTracking.Api.Impl.Cqrs.Category;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize(Roles = "Manager")]
public class ExpenseCategoriesController : ControllerBase
{
    private readonly IMediator mediator;
    public ExpenseCategoriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResponse<List<CategoryResponse>>> GetAll()
    {
        var operation = new GetAllCategoriesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<CategoryDetailResponse>> GetByIdAsync([FromRoute] long id)
    {
        var operation = new GetCategoryByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<CategoryResponse>> Post([FromBody] CategoryRequest Category)
    {
        var operation = new CreateCategoryCommand(Category);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] long id, [FromBody] UpdateCategoryRequest Category)
    {
        var operation = new UpdateCategoryCommand(id, Category);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] long id)
    {
        var operation = new DeleteCategoryCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}