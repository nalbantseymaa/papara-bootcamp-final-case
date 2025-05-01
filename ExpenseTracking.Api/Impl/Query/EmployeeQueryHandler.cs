using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using LinqKit;
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
        var predicate = PredicateBuilder.New<Employee>(e => e.IsActive);

        if (request.DepartmentId.HasValue)
            predicate = predicate.And(e => e.DepartmentId == request.DepartmentId);

        if (request.MinSalary.HasValue)
            predicate = predicate.And(e => e.Salary >= request.MinSalary);

        if (request.MaxSalary.HasValue)
            predicate = predicate.And(e => e.Salary <= request.MaxSalary);

        var employees = await dbcontext.Employees.Where(predicate)
            .ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<EmployeeResponse>>(employees);
        return new ApiResponse<List<EmployeeResponse>>(mapped);
    }

}
