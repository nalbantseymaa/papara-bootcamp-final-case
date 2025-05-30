using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.GenericValidator;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Api.Impl.UnitOfWork;
using ExpenseTracking.Base;
using ExpenseTracking.Base.Enum;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Command;

public class ManagerCommandHandler :
    IRequestHandler<CreateManagerCommand, ApiResponse<CreateManagerResponse>>,
    IRequestHandler<UpdateManagerCommand, ApiResponse>,
    IRequestHandler<DeleteManagerCommand, ApiResponse>
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUserService userService;
    private readonly IGenericEntityValidator genericEntityValidator;
    private readonly IUnitOfWork unitOfWork;

    public ManagerCommandHandler(AppDbContext dbContext, IMapper mapper, IUserService userService, IGenericEntityValidator genericEntityValidator, IUnitOfWork unitOfWork)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userService = userService;
        this.genericEntityValidator = genericEntityValidator;
        this.unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CreateManagerResponse>> Handle(CreateManagerCommand request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<UserRequest>(request.User);

        var userResponse = await userService.CreateUserAsync(user, UserRole.Manager.ToString());
        if (userResponse.userEntity == null)
            return new ApiResponse<CreateManagerResponse>("User creation failed");

        var manager = mapper.Map<Manager>(request.Manager);
        mapper.Map(userResponse.userEntity, manager);

        await unitOfWork.Repository<Manager>().AddAsync(manager);
        await unitOfWork.CommitAsync();

        return new ApiResponse<CreateManagerResponse>(new CreateManagerResponse
        {
            User = mapper.Map<UserResponse>(manager),
            Manager = mapper.Map<ManagerResponse>(manager),
            PlainPassword = userResponse.plainPassword

        });
    }

    public async Task<ApiResponse> Handle(UpdateManagerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Managers, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var entity = validationResult.Entity!;
        entity.FirstName = request.Manager.FirstName;
        entity.LastName = request.Manager.LastName;
        entity.MiddleName = request.Manager.MiddleName ?? entity.MiddleName;

        unitOfWork.Repository<Manager>().Update(entity);
        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Manager successfully updated");
    }

    public async Task<ApiResponse> Handle(DeleteManagerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await genericEntityValidator.ValidateActiveAndExistsAsync(dbContext.Managers, request.Id, cancellationToken);
        if (!validationResult.IsValid)
            return new ApiResponse(validationResult.ErrorMessage!);

        var entity = validationResult.Entity!;

        if (await dbContext.Departments.AnyAsync(x => x.ManagerId == request.Id && x.IsActive, cancellationToken))
            return new ApiResponse("Cannot delete manager with active departments");

        entity.IsActive = false;
        unitOfWork.Repository<Manager>().Remove(entity);

        await unitOfWork.CommitAsync();
        return new ApiResponse(true, "Manager successfully deleted");
    }

}
