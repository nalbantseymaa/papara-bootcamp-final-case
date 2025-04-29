using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Interfaces;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
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

    public AddressCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    private async Task<ApiResponse> CreateAddress<T>(T request,
        CancellationToken cancellationToken) where T : ICreateAddressCommand
    {
        var entity = mapper.Map<Address>(request.Address);
        request.ApplyOwner(entity);

        if (entity.IsDefault)
        {
            bool hasDefault = await dbContext.Addresses.AnyAsync(a =>
                a.IsActive && a.IsDefault && a.Id != entity.Id &&
                (
                    (entity.EmployeeId != null && a.EmployeeId == entity.EmployeeId) ||
                    (entity.DepartmentId != null && a.DepartmentId == entity.DepartmentId)
                ),
                cancellationToken);

            if (hasDefault)
                return new ApiResponse(false, $"A default address already exists for this owner {(entity.EmployeeId != null ? "employee" : "department")}.");
        }

        var entry = await dbContext.Addresses.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var address = mapper.Map<AddressResponse>(entry.Entity);
        return new ApiResponse(true, "Address successfully created");
    }

    public async Task<ApiResponse> Handle(CreateAddressForEmployeeCommand request, CancellationToken cancellationToken)
       => await CreateAddress(request, cancellationToken);

    public async Task<ApiResponse> Handle(CreateAddressForDepartmentCommand request, CancellationToken cancellationToken)
        => await CreateAddress(request, cancellationToken);


    public async Task<ApiResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.IsActive, cancellationToken);

        if (entity == null)
            return new ApiResponse(false, "Address not found or inactive");

        if (request.Address.IsDefault)
        {
            var defaultExists = await dbContext.Addresses.AnyAsync(a =>
                a.IsActive && a.IsDefault && a.Id != entity.Id &&
                (
                    (entity.EmployeeId != null && a.EmployeeId == entity.EmployeeId) ||
                    (entity.DepartmentId != null && a.DepartmentId == entity.DepartmentId)
                ),
                cancellationToken);

            if (defaultExists)
                return new ApiResponse(false, $"A default address already exists for this owner {(entity.EmployeeId != null ? "employee" : "department")}.");
        }
        mapper.Map(request.Address, entity);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Address successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Addresses
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken);

        if (entity == null)
            return new ApiResponse("Address not found or not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Address successfully deleted");
    }
}