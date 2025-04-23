using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

//ROLE=ADMIN 
[ApiController]
[Route("api/[controller]")]

public class DepartmentsController : ControllerBase
{
    private readonly IMediator mediator;
    public DepartmentsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<DepartmentResponse>>> GetAll()
    {
        var operation = new GetAllDepartmentsQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<DepartmentResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetDepartmentByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<DepartmentResponse>> Post([FromBody] DepartmentRequest Department)
    {
        var operation = new CreateDepartmentCommand(Department);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] DepartmentRequest Department)
    {
        var operation = new UpdateDepartmentCommand(id, Department);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteDepartmentCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}