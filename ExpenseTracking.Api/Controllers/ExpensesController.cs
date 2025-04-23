using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ExpenseController : ControllerBase
{
    private readonly IMediator mediator;
    public ExpenseController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    [Authorize(Roles = "Manager,Employee")]
    public async Task<ApiResponse<List<ExpenseResponse>>> GetAll()
    {
        var operation = new GetAllExpensesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "Manager,Employee")]
    public async Task<ApiResponse<ExpenseResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetExpenseByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<ApiResponse<ExpenseResponse>> Post([FromBody] ExpenseRequest Expense)
    {
        var operation = new CreateExpenseCommand(Expense);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] ExpenseRequest request)
    {
        var operation = new UpdateExpenseCommand(id, request);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("approve/{expenseId}")]
    [Authorize(Roles = "Manager")]
    public async Task<ApiResponse> Approve([FromRoute] int expenseId)
    {
        var operation = new ApproveExpenseCommand(expenseId);
        var result = await mediator.Send(operation);
        return result;
    }


    [HttpPut("reject/{expenseId}")]
    [Authorize(Roles = "Manager")]
    public async Task<ApiResponse> Reject([FromRoute] int expenseId, [FromBody] RejectExpenseRequest request)
    {
        var operation = new RejectExpenseCommand(expenseId, request);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteExpenseCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}