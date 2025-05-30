using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.UnitOfWork;
using ExpenseTracking.Api.Interfaces;
using ExpenseTracking.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class PhoneCommandHandler :
IRequestHandler<CreatePhoneForEmployeeCommand, ApiResponse>,
IRequestHandler<CreatePhoneForDepartmentCommand, ApiResponse>,
IRequestHandler<CreatePhoneForManagerCommand, ApiResponse>,
IRequestHandler<UpdatePhoneCommand, ApiResponse>,
IRequestHandler<DeletePhoneCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IGenericEntityValidator genericEntityValidator;
    private readonly IUnitOfWork unitOfWork;

    public PhoneCommandHandler(AppDbContext dbContext, IMapper mapper, IUnitOfWork unitOfWork, IGenericEntityValidator genericEntityValidator)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.genericEntityValidator = genericEntityValidator;
    }

    private async Task<ApiResponse> CreatePhone<T>(T request,
      CancellationToken cancellationToken) where T : ICreatePhoneCommand
    {
        var entity = mapper.Map<Phone>(request.Phone);
        request.ApplyOwner(entity);

        if (entity.IsDefault && await CheckDefaultPhoneExists(entity, cancellationToken))
        {
            return new ApiResponse(false, $"The default phone number already exists for {(entity.UserId != null ? "user" : "department")}.");
        }

        await unitOfWork.Repository<Phone>().AddAsync(entity);
        await unitOfWork.CommitAsync();

        return new ApiResponse(true, "Phone successfully created");
    }

    public async Task<ApiResponse> Handle(CreatePhoneForEmployeeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Phones, request.EmployeeId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        return await CreatePhone(request, cancellationToken);
    }

    public async Task<ApiResponse> Handle(CreatePhoneForDepartmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Departments, request.DepartmentId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        return await CreatePhone(request, cancellationToken);
    }

    public async Task<ApiResponse> Handle(CreatePhoneForManagerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Managers, request.ManagerId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        return await CreatePhone(request, cancellationToken);
    }

    public async Task<ApiResponse> Handle(UpdatePhoneCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Phones, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var entity = validationResult.Entity;

        if (request.Phone.IsDefault && await CheckDefaultPhoneExists(entity, cancellationToken))
        {
            return new ApiResponse(false, $"The default phone number already exists for {(entity.UserId != null ? "user" : "department")}.");
        }

        entity.CountryCode = request.Phone.CountryCode ?? entity.CountryCode;
        entity.PhoneNumber = request.Phone.PhoneNumber ?? entity.PhoneNumber;
        entity.IsDefault = request.Phone.IsDefault;

        unitOfWork.Repository<Phone>().Update(entity);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Phone successfully updated");
    }

    public async Task<ApiResponse> Handle(DeletePhoneCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Phones, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        validationResult.Entity.IsActive = false;

        unitOfWork.Repository<Phone>().Remove(validationResult.Entity);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Phone successfully deleted");
    }

    private async Task<bool> CheckDefaultPhoneExists(Phone entity, CancellationToken cancellationToken)
    {
        return await dbContext.Phones.AnyAsync(p =>
            p.IsActive && p.IsDefault && p.Id != entity.Id &&
            (
                (entity.UserId != null && p.UserId == entity.UserId) ||
                (entity.DepartmentId != null && p.DepartmentId == entity.DepartmentId)
            ),
            cancellationToken);
    }
}