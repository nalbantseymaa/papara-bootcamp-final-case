using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

//ROLE=ADMIN 
[ApiController]
[Route("api/[controller]")]

public class PhonesController : ControllerBase
{
    private readonly IMediator mediator;
    public PhonesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<PhoneResponse>>> GetAll()
    {
        var operation = new GetAllPhonesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<PhoneResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetPhonesByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("employee/{employeeId}")]
    public async Task<ApiResponse<PhoneResponse>> PostForEmployee(int EmployeeId, [FromBody] PhoneRequest Phone)
    {
        var operation = new CreatePhoneForEmployeeCommand(EmployeeId, Phone);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("department/{departmentId}")]
    public async Task<ApiResponse<PhoneResponse>> PostForDepartment(int DepartmentId, [FromBody] PhoneRequest Phones)
    {
        var operation = new CreatePhoneForDepartmentCommand(DepartmentId, Phones);
        var result = await mediator.Send(operation);
        return result;
    }
    [HttpPost("manager/{managerId}")]
    public async Task<ApiResponse<PhoneResponse>> PostForManager(int ManagerId, [FromBody] PhoneRequest Phones)
    {
        var operation = new CreatePhoneForManagerCommand(ManagerId, Phones);
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