using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class DepartmentQueryHandler :
IRequestHandler<GetAllDepartmentsQuery, ApiResponse<List<DepartmentResponse>>>,
IRequestHandler<GetDepartmentByIdQuery, ApiResponse<DepartmentDetailResponse>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;

    public DepartmentQueryHandler(AppDbContext dbcontext, IMapper mapper)
    {
        this.dbcontext = dbcontext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<DepartmentResponse>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {

        var Departments = await dbcontext.Departments.Include(x => x.Manager)
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<DepartmentResponse>>(Departments);

        return new ApiResponse<List<DepartmentResponse>>(mapped);
    }

    public async Task<ApiResponse<DepartmentDetailResponse>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var Department = await dbcontext.Departments
            .Include(x => x.Employees)
            .Include(x => x.Manager)
            .Include(x => x.Addresses)
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (Department == null)
        {
            return new ApiResponse<DepartmentDetailResponse>("Department not found");
        }

        var mapped = mapper.Map<DepartmentDetailResponse>(Department);
        return new ApiResponse<DepartmentDetailResponse>(mapped);
    }
}
