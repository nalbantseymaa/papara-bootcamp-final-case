using ExpenseFileTracking.Api.Filter;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Roles = "Manager")]
[ServiceFilter(typeof(LogResourceFilter))]
public class EmployeesController : ControllerBase
{
    private readonly IMediator mediator;
    public EmployeesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResponse<List<EmployeeResponse>>> GetAll()
    {
        var operation = new GetAllEmployeesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<EmployeeDetailResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetEmployeeByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("ByParameters")]
    public async Task<ApiResponse<List<EmployeeResponse>>> GetByParameters([FromQuery] long? departmentId, [FromQuery] int? minSalary, [FromQuery] int? MaxSalary)
    {
        var operation = new GetEmployeesByParametersQuery(departmentId, minSalary, MaxSalary);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<CreateEmployeeResponse>> Post([FromBody] CreateEmployeeRequest request)
    {
        var operation = new CreateEmployeeCommand(request.User, request.Employee);

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