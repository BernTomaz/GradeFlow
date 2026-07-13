using System.Security.Claims;
using System.Text;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GradeFlow.Api.Services;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions options = options.Value;

    public AuthResponse Create(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(options.ExpirationMinutes);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        return new AuthResponse(
            new JsonWebTokenHandler().CreateToken(descriptor),
            expiresAt,
            new UserResponse(user.Id, user.Name, user.Email, user.Role));
    }
}
