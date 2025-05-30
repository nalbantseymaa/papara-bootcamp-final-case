using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/expensefiles")]
public class ExpenseFilesController : ControllerBase
{
    private readonly IMediator mediator;
    public ExpenseFilesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Manager,Employee")]
    public async Task<ApiResponse<List<ExpenseFileResponse>>> GetAll()
    {
        var operation = new GetAllExpenseFilesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Employee")]
    public async Task<ApiResponse<ExpenseFileResponse>> GetByIdAsync([FromRoute] long id)
    {
        var operation = new GetExpenseFileByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Employee")]
    public async Task<ApiResponse> CreateExpenseFile([FromForm] ExpenseFileRequest request)

    {
        var operation = new CreateExpenseFileCommand(request);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Employee")]
    public async Task<ApiResponse> Put([FromRoute] long id, [FromForm] UpdateExpenseFileRequest ExpenseFile)
    {
        var operation = new UpdateExpenseFileCommand(id, ExpenseFile);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Employee")]
    public async Task<ApiResponse> Delete([FromRoute] long id)
    {
        var operation = new DeleteExpenseFileCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}