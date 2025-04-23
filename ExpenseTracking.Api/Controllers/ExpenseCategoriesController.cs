using ExpenseTracking.Api.Impl.Cqrs.Category;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

//ROLE=ADMIN 
[ApiController]
[Route("api/[controller]")]

public class ExpenseCategoriesController : ControllerBase
{
    private readonly IMediator mediator;
    public ExpenseCategoriesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<CategoryResponse>>> GetAll()
    {
        var operation = new GetAllCategoriesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<CategoryResponse>> GetByIdAsync([FromRoute] int id)
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
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] UpdateCategoryRequest Category)
    {
        var operation = new UpdateCategoryCommand(id, Category);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteCategoryCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}