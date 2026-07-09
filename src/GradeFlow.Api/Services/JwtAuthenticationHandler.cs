using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GradeFlow.Api.Services;

public sealed class JwtAuthenticationOptions : AuthenticationSchemeOptions;

public sealed class JwtAuthenticationHandler(
    IOptionsMonitor<JwtAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration) : AuthenticationHandler<JwtAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        var payload = JwtToken.ReadValid(header["Bearer ".Length..].Trim(), configuration["Jwt:Key"]!);
        if (payload is null
            || payload["iss"].GetString() != configuration["Jwt:Issuer"]
            || payload["aud"].GetString() != configuration["Jwt:Audience"]
            || DateTimeOffset.FromUnixTimeSeconds(payload["exp"].GetInt64()) <= DateTimeOffset.UtcNow)
            return Task.FromResult(AuthenticateResult.Fail("Token invalido."));

        var claims = payload
            .Where(x => x.Value.ValueKind == System.Text.Json.JsonValueKind.String)
            .Select(x => new Claim(x.Key, x.Value.GetString()!));
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}
