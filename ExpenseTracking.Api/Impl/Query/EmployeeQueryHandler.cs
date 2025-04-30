using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class EmployeeQueryHandler :
IRequestHandler<GetAllEmployeesQuery, ApiResponse<List<EmployeeResponse>>>,
IRequestHandler<GetEmployeeByIdQuery, ApiResponse<EmployeeDetailResponse>>,
IRequestHandler<GetEmployeesByParametersQuery, ApiResponse<List<EmployeeResponse>>>
{
    private readonly AppDbContext dbcontext;
    private readonly IMapper mapper;

    public EmployeeQueryHandler(AppDbContext dbcontext, IMapper mapper)
    {
        this.dbcontext = dbcontext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<EmployeeResponse>>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {

        var Employees = await dbcontext.Employees.Include(x => x.ManagedDepartments)
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<EmployeeResponse>>(Employees);

        return new ApiResponse<List<EmployeeResponse>>(mapped);
    }

    public async Task<ApiResponse<EmployeeDetailResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await dbcontext.Employees
            .Include(x => x.Addresses)
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (employee == null)
        {
            return new ApiResponse<EmployeeDetailResponse>("Employee not found");
        }

        var mapped = mapper.Map<EmployeeDetailResponse>(employee);
        return new ApiResponse<EmployeeDetailResponse>(mapped);
    }

    public async Task<ApiResponse<List<EmployeeResponse>>> Handle(GetEmployeesByParametersQuery request, CancellationToken cancellationToken)
    {
        var query = dbcontext.Employees.AsQueryable();

        if (request.DepartmentId.HasValue)
            query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);

        if (request.MinSalary.HasValue)
            query = query.Where(e => e.Salary >= request.MinSalary.Value);

        if (request.MaxSalary.HasValue)
            query = query.Where(e => e.Salary <= request.MaxSalary.Value);

        var data = await query
              .Where(x => x.IsActive)
              .ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<EmployeeResponse>>(data);
        return new ApiResponse<List<EmployeeResponse>>(mapped);
    }
}
