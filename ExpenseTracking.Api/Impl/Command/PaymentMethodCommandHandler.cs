using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs.PaymentMethod;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class PaymentMethodCommandHandler :
    IRequestHandler<CreatePaymentMethodCommand, ApiResponse<PaymentMethodResponse>>,
    IRequestHandler<UpdatePaymentMethodCommand, ApiResponse>,
    IRequestHandler<DeletePaymentMethodCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public PaymentMethodCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<PaymentMethodResponse>> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<PaymentMethod>(request.PaymentMethod);
        mapped.IsActive = true;
        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<PaymentMethodResponse>(entity.Entity);
        return new ApiResponse<PaymentMethodResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var validation = await GetActivePaymentMethod(request.Id, cancellationToken);
        if (!validation.Success)
            return new ApiResponse(validation.Message);
        var entity = validation.Entity!;

        entity.Name = string.IsNullOrWhiteSpace(request.PaymentMethod.Name) ? entity.Name : request.PaymentMethod.Name;
        entity.Description = string.IsNullOrWhiteSpace(request.PaymentMethod.Description) ? entity.Description : request.PaymentMethod.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "PaymentMethod successfully updated");
    }

    public async Task<ApiResponse> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var validation = await GetActivePaymentMethod(request.Id, cancellationToken);
        if (!validation.Success)
            return new ApiResponse(validation.Message);

        var entity = validation.Entity!;

        bool hasExpenses = await dbContext.Expenses
           .AnyAsync(e => e.PaymentMethodId == request.Id, cancellationToken);
        if (hasExpenses)
            return new ApiResponse("PaymentMethod cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "PaymentMethod successfully deleted");
    }

    private async Task<(bool Success, string? Message, PaymentMethod? Entity)> GetActivePaymentMethod(
    long? paymentMethodId, CancellationToken cancellationToken)
    {
        if (!paymentMethodId.HasValue)
            return (false, "PaymentMethod ID is required", null);

        var paymentMethod = await dbContext.PaymentMethods
            .FirstOrDefaultAsync(e => e.Id == paymentMethodId, cancellationToken);

        if (paymentMethod == null)
            return (false, "PaymentMethod not found", null);

        if (!paymentMethod.IsActive)
            return (false, "PaymentMethod is inactive", null);

        return (true, null, paymentMethod);
    }
}