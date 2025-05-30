using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.Service.Cache;
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
    private readonly ICacheService<Department> cacheService;

    public DepartmentQueryHandler(AppDbContext dbcontext, IMapper mapper, ICacheService<Department> cacheService)
    {
        this.dbcontext = dbcontext;
        this.mapper = mapper;
        this.cacheService = cacheService;
    }

    public async Task<ApiResponse<List<DepartmentResponse>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await cacheService.GetAllAsync<DepartmentResponse>("departments");
        return new ApiResponse<List<DepartmentResponse>>(departments);
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
