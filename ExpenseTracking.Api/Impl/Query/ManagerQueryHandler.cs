using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class ManagerQueryHandler :
IRequestHandler<GetAllManagersQuery, ApiResponse<List<ManagerResponse>>>,
IRequestHandler<GetManagerByIdQuery, ApiResponse<ManagerResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;


    public ManagerQueryHandler(AppDbContext dbcontext, IMapper mapper)
    {
        this.dbcontext = dbcontext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<ManagerResponse>>> Handle(GetAllManagersQuery request, CancellationToken cancellationToken)
    {

        var Managers = await dbcontext.Managers
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<ManagerResponse>>(Managers);

        return new ApiResponse<List<ManagerResponse>>(mapped);
    }

    public async Task<ApiResponse<ManagerResponse>> Handle(GetManagerByIdQuery request, CancellationToken cancellationToken)
    {
        var Manager = await dbcontext.Managers
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (Manager == null)
        {
            return new ApiResponse<ManagerResponse>("Manager not found");
        }

        var mapped = mapper.Map<ManagerResponse>(Manager);
        return new ApiResponse<ManagerResponse>(mapped);
    }
}
