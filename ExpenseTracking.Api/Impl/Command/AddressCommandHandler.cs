using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracking.Api.Impl.Command;

public class AddressCommandHandler :
IRequestHandler<CreateAddressForEmployeeCommand, ApiResponse<AddressResponse>>,
IRequestHandler<CreateAddressForDepartmentCommand, ApiResponse<AddressResponse>>,
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
    public async Task<ApiResponse<AddressResponse>> Handle(CreateAddressForEmployeeCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Address>(request.Address);
        mapped.EmployeeId = request.EmployeeId;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<AddressResponse>(entity.Entity);
        return new ApiResponse<AddressResponse>(response);
    }

    public async Task<ApiResponse<AddressResponse>> Handle(CreateAddressForDepartmentCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Address>(request.Address);
        mapped.DepartmentId = request.DepartmentId;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<AddressResponse>(entity.Entity);
        return new ApiResponse<AddressResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Address not found");

        if (!entity.IsActive)
            return new ApiResponse("Address is not active");

        var r = request.Address;

        entity.CountryCode = !string.IsNullOrWhiteSpace(r.CountryCode)
                      ? r.CountryCode!
                      : entity.CountryCode;

        entity.City = !string.IsNullOrWhiteSpace(r.City)
                             ? r.City!
                             : entity.City;

        entity.District = !string.IsNullOrWhiteSpace(r.District)
                             ? r.District!
                             : entity.District;

        entity.Street = !string.IsNullOrWhiteSpace(r.Street)
                             ? r.Street!
                             : entity.Street;

        entity.ZipCode = !string.IsNullOrWhiteSpace(r.ZipCode)
                             ? r.ZipCode!
                             : entity.ZipCode;

        entity.IsDefault = r.IsDefault;

        entity.UpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Addresses.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Address not found");

        if (!entity.IsActive)
            return new ApiResponse("Address is not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

}