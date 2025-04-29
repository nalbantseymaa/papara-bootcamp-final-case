using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/phones")]
[Authorize(Roles = "Manager")]
public class PhonesController : ControllerBase
{
    private readonly IMediator mediator;
    public PhonesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResponse<List<PhoneResponse>>> GetAll()
    {
        var operation = new GetAllPhonesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<PhoneResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetPhonesByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("~/api/employees/{employeeId}/phones")]
    public async Task<ApiResponse> PostForEmployee(long employeeId, [FromBody] PhoneRequest Phone)
    {
        var operation = new CreatePhoneForEmployeeCommand(employeeId, Phone);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("~/api/departments/{departmentId}/phones")]
    public async Task<ApiResponse> PostForDepartment(long departmentId, [FromBody] PhoneRequest Phones)
    {
        var operation = new CreatePhoneForDepartmentCommand(departmentId, Phones);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("~/api/managers/{managerId}/phones")]
    public async Task<ApiResponse> PostForManager(long managerId, [FromBody] PhoneRequest Phones)
    {
        var operation = new CreatePhoneForManagerCommand(managerId, Phones);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] PhoneRequest Phones)
    {
        var operation = new UpdatePhoneCommand(id, Phones);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeletePhoneCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}