using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize(Roles = "Manager")]
public class AdressesController : ControllerBase
{
    private readonly IMediator mediator;
    public AdressesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResponse<List<AddressResponse>>> GetAll()
    {
        var operation = new GetAllAdressesQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<AddressResponse>> GetByIdAsync([FromRoute] long id)
    {
        var operation = new GetAdressesByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("ByParameters")]
    public async Task<ApiResponse<List<AddressResponse>>> GetByParameters([FromQuery] string? city, [FromQuery] string? zipCode, [FromQuery] bool? ısDefault)
    {
        var operation = new GetAdressesByParametersQuery(city, zipCode, ısDefault);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("~/api/employees/{employeeId}/addresses")]
    public async Task<ApiResponse> PostForEmployee([FromRoute] long employeeId, [FromBody] AddressRequest Address)
    {
        var operation = new CreateAddressForEmployeeCommand(employeeId, Address);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost("~/api/departments/{departmentId}/addresses")]
    public async Task<ApiResponse> PostForDepartment([FromRoute] long departmentId, [FromBody] AddressRequest Adresses)
    {
        var operation = new CreateAddressForDepartmentCommand(departmentId, Adresses);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] long id, [FromBody] AddressRequest Adresses)
    {
        var operation = new UpdateAddressCommand(id, Adresses);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] long id)
    {
        var operation = new DeleteAddressCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}
