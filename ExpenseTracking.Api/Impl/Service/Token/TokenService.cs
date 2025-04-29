using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracking.Api.Domain;
using ExpenseTracking.Base;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracking.Api.Impl.Service;

public class TokenService : ITokenService
{
    private readonly JwtConfig jwtConfig;

    public TokenService(JwtConfig jwtConfig)
    {
        this.jwtConfig = jwtConfig;
    }
    public string GenerateTokenAsync(User user)
    {
        string token = GenerateToken(user);
        return token;

    }

    //HEADER.PAYLOAD.SIGNATURE
    //header = {alg: HS256, typ: JWT}
    //payload = {iss: issuer, aud: audience, exp: expiration, iat: issued at, sub: subject}
    //signature = HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
    public string GenerateToken(User user)
    {
        var claims = GetClaims(user);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtConfig.AccessTokenExpiration),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private Claim[] GetClaims(User user)
    {
        var claims = new List<Claim>
         {
             new Claim(ClaimTypes.Role, user.Role.ToString()),
             new Claim("UserName", user.UserName),
             new Claim("UserId", user.Id.ToString()),
             new Claim("Secret", user.Secret),
         };

        if (user is Manager manager)
        {
            claims.Add(new Claim("name", $"{manager.FirstName} {manager.LastName}"));
        }
        else if (user is Employee employee)
        {
            claims.Add(new Claim("name", $"{employee.FirstName} {employee.LastName}"));
        }

        return claims.ToArray();
    }
}