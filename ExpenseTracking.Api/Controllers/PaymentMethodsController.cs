using ExpenseTracking.Api.Impl.Cqrs.PaymentMethod;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracking.Api.Controllers;

[ApiController]
[Route("api/payment-methods")]
[Authorize(Roles = "Manager")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IMediator mediator;
    public PaymentMethodsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResponse<List<PaymentMethodResponse>>> GetAll()
    {
        var operation = new GetAllPaymentMethodsQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<PaymentMethodResponse>> GetByIdAsync([FromRoute] int id)
    {
        var operation = new GetPaymentMethodByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    public async Task<ApiResponse<PaymentMethodResponse>> Post([FromBody] PaymentMethodRequest PaymentMethod)
    {
        var operation = new CreatePaymentMethodCommand(PaymentMethod);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] UpdatePaymentMethodRequest PaymentMethod)
    {
        var operation = new UpdatePaymentMethodCommand(id, PaymentMethod);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete([FromRoute] int id)
    {
        var operation = new DeletePaymentMethodCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}