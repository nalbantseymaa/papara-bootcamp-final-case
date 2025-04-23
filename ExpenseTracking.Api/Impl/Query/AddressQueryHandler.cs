using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class AddressQueryHandler :
IRequestHandler<GetAllAdressesQuery, ApiResponse<List<AddressResponse>>>,
IRequestHandler<GetAdressesByIdQuery, ApiResponse<AddressResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;


    public AddressQueryHandler(AppDbContext dbcontext, IMapper mapper)
    {
        this.dbcontext = dbcontext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAllAdressesQuery request, CancellationToken cancellationToken)
    {

        var Addresss = await dbcontext.Addresses
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<AddressResponse>>(Addresss);

        return new ApiResponse<List<AddressResponse>>(mapped);
    }

    public async Task<ApiResponse<AddressResponse>> Handle(GetAdressesByIdQuery request, CancellationToken cancellationToken)
    {
        var Address = await dbcontext.Addresses
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (Address == null)
        {
            return new ApiResponse<AddressResponse>("Address not found");
        }

        var mapped = mapper.Map<AddressResponse>(Address);
        return new ApiResponse<AddressResponse>(mapped);
    }
}
