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

public class AddressCommandHandler :
    IRequestHandler<CreateAddressForEmployeeCommand, ApiResponse>,
    IRequestHandler<CreateAddressForDepartmentCommand, ApiResponse>,
    IRequestHandler<UpdateAddressCommand, ApiResponse>,
    IRequestHandler<DeleteAddressCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;
    private readonly IGenericEntityValidator genericEntityValidator;

    public AddressCommandHandler(AppDbContext dbContext, IMapper mapper, IUnitOfWork unitOfWork, IGenericEntityValidator genericEntityValidator)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.genericEntityValidator = genericEntityValidator;
    }

    private async Task<ApiResponse> CreateAddress<T>(T request,
        CancellationToken cancellationToken) where T : ICreateAddressCommand
    {
        var entity = mapper.Map<Address>(request.Address);
        request.ApplyOwner(entity);

        if (entity.IsDefault && await CheckDefaultAddressExists(entity, cancellationToken))
        {
            return new ApiResponse(false, $"A default address already exists for this owner {(entity.EmployeeId != null ? "employee" : "department")}.");
        }

        await unitOfWork.Repository<Address>().AddAsync(entity);
        await unitOfWork.CommitAsync();

        return new ApiResponse(true, "Address successfully created");
    }

    public async Task<ApiResponse> Handle(CreateAddressForEmployeeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Employees, request.EmployeeId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        return await CreateAddress(request, cancellationToken);
    }

    public async Task<ApiResponse> Handle(CreateAddressForDepartmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Departments, request.DepartmentId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        return await CreateAddress(request, cancellationToken);
    }

    public async Task<ApiResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Addresses, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var entity = validationResult.Entity;

        if (request.Address.IsDefault && await CheckDefaultAddressExists(entity, cancellationToken))
        {
            return new ApiResponse(false, $"A default address already exists for this owner {(entity.EmployeeId != null ? "employee" : "department")}.");
        }

        entity.CountryCode = request.Address.CountryCode;
        entity.City = request.Address.City;
        entity.District = request.Address.District;
        entity.Street = request.Address.Street;
        entity.ZipCode = request.Address.ZipCode;
        entity.IsDefault = request.Address.IsDefault;

        unitOfWork.Repository<Address>().Update(entity);
        await unitOfWork.CommitAsync();

        return new ApiResponse(true, "Address successfully updated");
    }
    public async Task<ApiResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Addresses, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        validationResult.Entity.IsActive = false;

        unitOfWork.Repository<Address>().Remove(validationResult.Entity);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Address successfully deleted");
    }

    private async Task<bool> CheckDefaultAddressExists(Address entity, CancellationToken cancellationToken)
    {
        return await dbContext.Addresses.AnyAsync(a =>
            a.IsActive && a.IsDefault && a.Id != entity.Id &&
            (
                (entity.EmployeeId != null && a.EmployeeId == entity.EmployeeId) ||
                (entity.DepartmentId != null && a.DepartmentId == entity.DepartmentId)
            ),
            cancellationToken);
    }
}