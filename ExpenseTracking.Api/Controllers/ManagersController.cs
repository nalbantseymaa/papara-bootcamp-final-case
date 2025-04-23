using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

//ROLE=ADMIN 
[ApiController]
[Route("api/[controller]")]

public class ManagersController : ControllerBase
{
    private readonly IMediator mediator;
    public ManagersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<ManagerResponse>>> GetAll()
    {
        var operation = new GetAllManagersQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<ManagerResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetManagerByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<ManagerResponse>> Post([FromBody] ManagerRequest Manager)
    {
        var operation = new CreateManagerCommand(Manager);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] ManagerRequest Manager)
    {
        var operation = new UpdateManagerCommand(id, Manager);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteManagerCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}