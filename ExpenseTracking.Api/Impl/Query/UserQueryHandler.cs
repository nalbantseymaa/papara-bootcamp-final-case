using AutoMapper;
using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Base;
using ExpenseTracking.Schema;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class UserQueryHandler :
IRequestHandler<GetAllUsersQuery, ApiResponse<List<UserResponse>>>,
IRequestHandler<GetUserByIdQuery, ApiResponse<UserResponse>>
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;
    public UserQueryHandler(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var Users = await context.Users.ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<UserResponse>>(Users);
        return new ApiResponse<List<UserResponse>>(mapped);
    }

    public async Task<ApiResponse<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var User = await context.Users
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (User == null)
            return new ApiResponse<UserResponse>("User not found");

        var mapped = mapper.Map<UserResponse>(User);
        return new ApiResponse<UserResponse>(mapped);
    }

}