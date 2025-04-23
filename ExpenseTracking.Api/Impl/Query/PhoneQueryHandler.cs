using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class PhoneQueryHandler :
    IRequestHandler<GetAllPhonesQuery, ApiResponse<List<PhoneResponse>>>,
    IRequestHandler<GetPhonesByIdQuery, ApiResponse<PhoneResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;


    public PhoneQueryHandler(AppDbContext dbcontext, IMapper mapper)
    {
        this.dbcontext = dbcontext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<PhoneResponse>>> Handle(GetAllPhonesQuery request, CancellationToken cancellationToken)
    {

        var Phones = await dbcontext.Phones
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<PhoneResponse>>(Phones);

        return new ApiResponse<List<PhoneResponse>>(mapped);
    }

    public async Task<ApiResponse<PhoneResponse>> Handle(GetPhonesByIdQuery request, CancellationToken cancellationToken)
    {
        var Phone = await dbcontext.Phones
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (Phone == null)
        {
            return new ApiResponse<PhoneResponse>("Phone not found");
        }

        var mapped = mapper.Map<PhoneResponse>(Phone);
        return new ApiResponse<PhoneResponse>(mapped);
    }
}
