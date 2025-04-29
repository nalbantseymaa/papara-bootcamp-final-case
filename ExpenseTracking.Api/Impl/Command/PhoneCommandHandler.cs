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

public class PhoneCommandHandler :
IRequestHandler<CreatePhoneForEmployeeCommand, ApiResponse>,
IRequestHandler<CreatePhoneForDepartmentCommand, ApiResponse>,
IRequestHandler<CreatePhoneForManagerCommand, ApiResponse>,
IRequestHandler<UpdatePhoneCommand, ApiResponse>,
IRequestHandler<DeletePhoneCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public PhoneCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    private async Task<ApiResponse> CreatePhone<T>(T request,
      CancellationToken cancellationToken) where T : ICreatePhoneCommand
    {
        var entity = mapper.Map<Phone>(request.Phone);
        request.ApplyOwner(entity);

        if (entity.IsDefault)
        {
            bool hasDefault = await dbContext.Phones.AnyAsync(p =>
                p.IsActive && p.IsDefault && p.Id != entity.Id &&
                (
                    (entity.UserId != null && p.UserId == entity.UserId) ||
                    (entity.DepartmentId != null && p.DepartmentId == entity.DepartmentId)
                ),
                cancellationToken);

            if (hasDefault)
                return new ApiResponse(false, $"A default phone already exists for this owner {(entity.UserId != null ? "employee" : "department")}.");
        }

        var entry = await dbContext.Phones.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var phone = mapper.Map<PhoneResponse>(entry.Entity);
        return new ApiResponse(true, "Phone successfully created");
    }

    public async Task<ApiResponse> Handle(CreatePhoneForEmployeeCommand request, CancellationToken cancellationToken)
       => await CreatePhone(request, cancellationToken);

    public async Task<ApiResponse> Handle(CreatePhoneForDepartmentCommand request, CancellationToken cancellationToken)
        => await CreatePhone(request, cancellationToken);

    public async Task<ApiResponse> Handle(CreatePhoneForManagerCommand request, CancellationToken cancellationToken)
        => await CreatePhone(request, cancellationToken);

    public async Task<ApiResponse> Handle(UpdatePhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Phones.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null || !entity.IsActive)
            return new ApiResponse("Phone not found or inactive");

        long? userId = entity.UserId;
        long? departmentId = entity.DepartmentId;

        if (request.Phone.IsDefault)
        {
            bool isDefaultExists = await dbContext.Phones
                .AnyAsync(p => p.IsDefault && (p.UserId == userId || p.DepartmentId == departmentId), cancellationToken);

            if (isDefaultExists)
                return new ApiResponse($"The default phone number already exists for {(userId != null ? "user" : "department")}.");
        }

        entity.CountryCode = request.Phone.CountryCode ?? entity.CountryCode;
        entity.PhoneNumber = request.Phone.PhoneNumber ?? entity.PhoneNumber;
        entity.IsDefault = request.Phone.IsDefault;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Phone successfully updated");
    }

    public async Task<ApiResponse> Handle(DeletePhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Phones.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null || !entity.IsActive)
            return new ApiResponse("Phone not found or inactive");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse(true, "Phone successfully deleted");
    }
}