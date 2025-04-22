using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namespace.ExpenseTracking.Api.Impl.Cqrs.Employee;


namespace Net.Api.Controllers;

//ROLE=ADMIN 
[ApiController]
[Route("api/[controller]")]

public class EmployeesController : ControllerBase
{
    private readonly IMediator mediator;
    public EmployeesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<EmployeeResponse>>> GetAll()
    {
        var operation = new GetAllEmployeesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<EmployeeResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetEmployeeByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<EmployeeResponse>> Post([FromBody] EmployeeRequest Employee)
    {
        var operation = new CreateEmployeeCommand(Employee);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] UpdateEmployeeRequest Employee)
    {
        var operation = new UpdateEmployeeCommand(id, Employee);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteEmployeeCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}