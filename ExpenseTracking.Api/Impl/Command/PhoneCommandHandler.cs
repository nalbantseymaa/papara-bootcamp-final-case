using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracking.Api.Impl.Command;

public class PhoneCommandHandler :
IRequestHandler<CreatePhoneForEmployeeCommand, ApiResponse<PhoneResponse>>,
IRequestHandler<CreatePhoneForDepartmentCommand, ApiResponse<PhoneResponse>>,
IRequestHandler<CreatePhoneForManagerCommand, ApiResponse<PhoneResponse>>,
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
    public async Task<ApiResponse<PhoneResponse>> Handle(CreatePhoneForEmployeeCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Phone>(request.Phone);
        mapped.EmployeeId = request.EmployeeId;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<PhoneResponse>(entity.Entity);
        return new ApiResponse<PhoneResponse>(response);
    }

    public async Task<ApiResponse<PhoneResponse>> Handle(CreatePhoneForDepartmentCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Phone>(request.Phone);
        mapped.DepartmentId = request.DepartmentId;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<PhoneResponse>(entity.Entity);
        return new ApiResponse<PhoneResponse>(response);
    }
    public async Task<ApiResponse<PhoneResponse>> Handle(CreatePhoneForManagerCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Phone>(request.Phone);
        mapped.ManagerId = request.ManagerId;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<PhoneResponse>(entity.Entity);
        return new ApiResponse<PhoneResponse>(response);
    }
    public async Task<ApiResponse> Handle(UpdatePhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Phones.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Phone not found");

        if (!entity.IsActive)
            return new ApiResponse("Phone is not active");

        var r = request.Phone;

        entity.CountryCode = !string.IsNullOrWhiteSpace(r.CountryCode)
                               ? r.CountryCode!
                               : entity.CountryCode;
        entity.PhoneNumber = !string.IsNullOrWhiteSpace(r.PhoneNumber)
                       ? r.PhoneNumber!
                       : entity.PhoneNumber;


        entity.IsDefault = r.IsDefault;

        entity.UpdatedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeletePhoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Phones.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Phone not found");

        if (!entity.IsActive)
            return new ApiResponse("Phone is not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

}