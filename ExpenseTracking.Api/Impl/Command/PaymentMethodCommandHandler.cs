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

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<PaymentMethodResponse>(entity.Entity);
        return new ApiResponse<PaymentMethodResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PaymentMethods.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("PaymentMethod not found");

        if (!entity.IsActive)
            return new ApiResponse("PaymentMethod is not active");

        entity.Name = string.IsNullOrWhiteSpace(request.PaymentMethod.Name) ? entity.Name : request.PaymentMethod.Name;
        entity.Description = string.IsNullOrWhiteSpace(request.PaymentMethod.Description) ? entity.Description : request.PaymentMethod.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.PaymentMethods.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse(" PaymentMethod not found");

        if (!entity.IsActive)
            return new ApiResponse("PaymentMethod is not active");

        if (entity.Expenses.Any())
            return new ApiResponse(" PaymentMethod cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }



}