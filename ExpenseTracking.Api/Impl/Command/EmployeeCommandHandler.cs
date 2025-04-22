using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Namespace.ExpenseTracking.Api.Impl.Cqrs.Employee;


namespace Net.Api.Impl.Query;

public class EmployeeCommandHandler :
IRequestHandler<CreateEmployeeCommand, ApiResponse<EmployeeResponse>>,
IRequestHandler<UpdateEmployeeCommand, ApiResponse>,
IRequestHandler<DeleteEmployeeCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public EmployeeCommandHandler(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Employee not found");

        if (!entity.IsActive)
            return new ApiResponse("Employee is not active");

        entity.IsActive = false;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }


    public async Task<ApiResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return new ApiResponse("Employee not found");

        if (!entity.IsActive)
            return new ApiResponse("Employee is not active");

        entity.Email = string.IsNullOrWhiteSpace(request.Employee.Email) ? entity.Email : request.Employee.Email;
        entity.DepartmentId = request.Employee.DepartmentId ?? entity.DepartmentId;
        entity.Salary = request.Employee.Salary ?? entity.Salary;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse<EmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var mapped = mapper.Map<Employee>(request.Employee);
        mapped.EmployeeNumber = new Random().Next(1000000, 999999999);
        mapped.HireDate = DateTime.UtcNow;
        mapped.IsActive = true;

        var entity = await dbContext.AddAsync(mapped, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = mapper.Map<EmployeeResponse>(entity.Entity);
        return new ApiResponse<EmployeeResponse>(response);
    }
}