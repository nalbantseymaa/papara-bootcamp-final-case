using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs.PaymentMethod;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.Service.Cache;
using ExpenseTracking.Api.Impl.UnitOfWork;
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
    private readonly IGenericEntityValidator genericEntityValidator;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICacheService<PaymentMethod> cacheService;


    public PaymentMethodCommandHandler(AppDbContext dbContext, IMapper mapper, IGenericEntityValidator genericEntityValidator, IUnitOfWork unitOfWork, ICacheService<PaymentMethod> cacheService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.genericEntityValidator = genericEntityValidator;
        this.unitOfWork = unitOfWork;
        this.cacheService = cacheService;
    }

    public async Task<ApiResponse<PaymentMethodResponse>> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<PaymentMethod>(request.PaymentMethod);
        mapped.IsActive = true;
        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await unitOfWork.Repository<PaymentMethod>().AddAsync(mapped);
        await unitOfWork.CommitAsync();
        var allPaymentMethods = await unitOfWork.Repository<PaymentMethod>().GetAllAsync();
        await cacheService.SetAsync<PaymentMethod, PaymentMethodResponse>("paymentMethods", allPaymentMethods.ToList());

        var response = mapper.Map<PaymentMethodResponse>(entity.Entity);
        return new ApiResponse<PaymentMethodResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.PaymentMethods, request.Id, cancellationToken);
        var entity = validationResult.Entity!;

        entity.Name = string.IsNullOrWhiteSpace(request.PaymentMethod.Name) ? entity.Name : request.PaymentMethod.Name;
        entity.Description = string.IsNullOrWhiteSpace(request.PaymentMethod.Description) ? entity.Description : request.PaymentMethod.Description;

        unitOfWork.Repository<PaymentMethod>().Update(entity);
        await unitOfWork.CommitAsync();
        var allPaymentMethods = await unitOfWork.Repository<PaymentMethod>().GetAllAsync();
        await cacheService.SetAsync<PaymentMethod, PaymentMethodResponse>("paymentMethods", allPaymentMethods.ToList());

        return new ApiResponse(true, "PaymentMethod successfully updated");
    }

    public async Task<ApiResponse> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.PaymentMethods, request.Id, cancellationToken);
        var entity = validationResult.Entity!;

        bool hasExpenses = await dbContext.Expenses
           .AnyAsync(e => e.PaymentMethodId == request.Id, cancellationToken);
        if (hasExpenses)
            return new ApiResponse("PaymentMethod cannot be deleted because it has expenses associated with it");

        entity.IsActive = false;

        unitOfWork.Repository<PaymentMethod>().Remove(entity);
        await unitOfWork.CommitAsync();
        var allPaymentMethods = await unitOfWork.Repository<PaymentMethod>().GetAllAsync();
        await cacheService.SetAsync<PaymentMethod, PaymentMethodResponse>("paymentMethods", allPaymentMethods.ToList());
        return new ApiResponse(true, "PaymentMethod successfully deleted");
    }
}