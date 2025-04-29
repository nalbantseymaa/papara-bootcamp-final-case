using ExpenseTracking.Api.Context;
using ExpenseTracking.Api.Impl.Cqrs;
using ExpenseTracking.Api.Impl.Service;
using ExpenseTracking.Api.Schema;
using ExpenseTracking.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Api.Impl.Query;

public class AuthorizationCommandHandler :
IRequestHandler<CreateAuthorizationTokenCommand, ApiResponse<AuthorizationResponse>>
{
    private readonly AppDbContext dbContext;
    private readonly ITokenService tokenService;
    private readonly JwtConfig jwtConfig;

    public AuthorizationCommandHandler(AppDbContext dbContext, ITokenService tokenService, JwtConfig jwtConfig)
    {
        this.jwtConfig = jwtConfig;
        this.dbContext = dbContext;
        this.tokenService = tokenService;
    }
    public async Task<ApiResponse<AuthorizationResponse>> Handle(CreateAuthorizationTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.Request.UserName, cancellationToken);

        if (user == null)
            return new ApiResponse<AuthorizationResponse>("User not found");

        var hashedPassword = PasswordGenerator.CreateSHA256(request.Request.Password, user.Secret);
        if (hashedPassword != user.Password)
            return new ApiResponse<AuthorizationResponse>("Invalid password");

        var token = tokenService.GenerateToken(user);
        var entity = new AuthorizationResponse
        {
            UserName = user.UserName,
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(jwtConfig.AccessTokenExpiration)
        };

        return new ApiResponse<AuthorizationResponse>(entity);
    }
}
