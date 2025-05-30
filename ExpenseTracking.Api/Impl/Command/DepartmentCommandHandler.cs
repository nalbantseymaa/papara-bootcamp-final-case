using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.UnitOfWork;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class DepartmentCommandHandler :
    IRequestHandler<CreateDepartmentCommand, ApiResponse<DepartmentResponse>>,
    IRequestHandler<UpdateDepartmentCommand, ApiResponse>,
    IRequestHandler<DeleteDepartmentCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;
    private readonly IGenericEntityValidator genericEntityValidator;

    public DepartmentCommandHandler(AppDbContext dbContext, IMapper mapper, IUnitOfWork unitOfWork, IGenericEntityValidator genericEntityValidator)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
        this.genericEntityValidator = genericEntityValidator;
    }

    public async Task<ApiResponse<DepartmentResponse>> Handle(CreateDepartmentCommand request,
       CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Department.Name))
            return new ApiResponse<DepartmentResponse>("Department name is required");

        bool exists = await dbContext.Departments
            .AnyAsync(d => d.Name.Trim() == request.Department.Name.Trim(), cancellationToken);

        if (exists) return new ApiResponse<DepartmentResponse>("Department already exists");

        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Employees, request.Department.ManagerId, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse<DepartmentResponse>(validationResult.ErrorMessage!);

        var department = mapper.Map<Department>(request.Department);

        await unitOfWork.Repository<Department>().AddAsync(department);
        await unitOfWork.CommitAsync();

        var response = mapper.Map<DepartmentResponse>(department);
        return new ApiResponse<DepartmentResponse>(response);
    }

    public async Task<ApiResponse> Handle(UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
    {
        var validationResultDepartment = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Departments, request.Id, cancellationToken);
        if (!validationResultDepartment.IsValid)
            return new ApiResponse(false, validationResultDepartment.ErrorMessage!);
        var entity = validationResultDepartment.Entity!;
        var department = request.Department;

        if (entity.ManagerId.HasValue && department.ManagerId.HasValue
            && entity.ManagerId.Value != department.ManagerId.Value)
            return new ApiResponse(false, "This department already has a manager");

        var validationResultManager = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Employees, department.ManagerId, cancellationToken);
        if (!validationResultManager.IsValid)
            return new ApiResponse(false, validationResultManager.ErrorMessage!);

        entity.Name = department.Name.Trim();

        if (!string.IsNullOrWhiteSpace(department.Description))
            entity.Description = department.Description.Trim();

        if (department.ManagerId.HasValue)
            entity.ManagerId = department.ManagerId.Value;

        unitOfWork.Repository<Department>().Update(entity);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Department successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Departments, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var activeEmployees = await dbContext.Employees
            .Where(e => e.DepartmentId == validationResult.Entity!.Id && e.IsActive)
            .CountAsync(cancellationToken);

        if (activeEmployees > 0)
            return new ApiResponse("Cannot delete department with active employees");

        validationResult.Entity!.IsActive = false;

        unitOfWork.Repository<Department>().Remove(validationResult.Entity);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Department successfully deleted");
    }

}