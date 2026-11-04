using System.Text;
using System.Security.Claims;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GradeFlow.Api.Services;

public sealed class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var jwt = jwtOptions.Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;
                if (!Guid.TryParse(principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                {
                    context.Fail("Invalid session.");
                    return;
                }

                var repository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var user = await repository.GetByIdAsync(id, context.HttpContext.RequestAborted);
                if (user is null
                    || principal?.FindFirstValue("password_stamp") != JwtTokenService.PasswordStamp(user)
                    || principal.FindFirstValue(ClaimTypes.Role) != user.Role.ToString())
                {
                    context.Fail("Invalid or revoked session.");
                    return;
                }

                context.HttpContext.Items[typeof(User)] = user;
            }
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);
}
