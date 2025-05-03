using ExpenseFileTracking.Api.Filter;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/managers")]
[Authorize(Roles = "Manager")]
[ServiceFilter(typeof(LogResourceFilter))]
public class ManagersController : ControllerBase
{
    private readonly IMediator mediator;
    public ManagersController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResponse<List<ManagerResponse>>> GetAll()
    {
        var operation = new GetAllManagersQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<ManagerResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetManagerByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<CreateManagerResponse>> Post([FromBody] CreateManagerRequest request)
    {
        var operation = new CreateManagerCommand(request.User, request.Manager);
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