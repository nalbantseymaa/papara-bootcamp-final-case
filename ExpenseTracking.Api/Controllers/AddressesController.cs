using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

//ROLE=ADMIN 
[ApiController]
[Route("api/[controller]")]
public class AdressesController : ControllerBase
{
    private readonly IMediator mediator;
    public AdressesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("GetAll")]
    public async Task<ApiResponse<List<AddressResponse>>> GetAll()
    {
        var operation = new GetAllAdressesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("GetById/{id}")]
    public async Task<ApiResponse<AddressResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetAdressesByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("employee/{employeeId}")]
    public async Task<ApiResponse<AddressResponse>> PostForEmployee(int EmployeeId, [FromBody] AddressRequest Address)
    {
        var operation = new CreateAddressForEmployeeCommand(EmployeeId, Address);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("department/{departmentId}")]
    public async Task<ApiResponse<AddressResponse>> PostForDepartment(int DepartmentId, [FromBody] AddressRequest Adresses)
    {
        var operation = new CreateAddressForDepartmentCommand(DepartmentId, Adresses);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] AddressRequest Adresses)
    {
        var operation = new UpdateAddressCommand(id, Adresses);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeleteAddressCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}