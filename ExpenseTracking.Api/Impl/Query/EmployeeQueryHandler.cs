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
IRequestHandler<GetEmployeeByIdQuery, ApiResponse<EmployeeResponse>>
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

        var Employees = await dbcontext.Employees
        .ToListAsync(cancellationToken);
        var mapped = mapper.Map<List<EmployeeResponse>>(Employees);

        return new ApiResponse<List<EmployeeResponse>>(mapped);
    }

    public async Task<ApiResponse<EmployeeResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await dbcontext.Employees
            .Include(x => x.Addresses)
            .Include(x => x.Phones)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (employee == null)
        {
            return new ApiResponse<EmployeeResponse>("Employee not found");
        }

        var mapped = mapper.Map<EmployeeResponse>(employee);
        return new ApiResponse<EmployeeResponse>(mapped);
    }
}
