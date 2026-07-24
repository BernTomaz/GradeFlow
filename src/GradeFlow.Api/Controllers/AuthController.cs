using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;
using GradeFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    ITokenService tokenService,
    IMemoryCache memoryCache) : ControllerBase
{
    private const int MaxLoginAttempts = 5;
    private static readonly TimeSpan LoginAttemptWindow = TimeSpan.FromMinutes(1);

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.RegisterAsync(request, cancellationToken));
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var key = LoginAttemptKey(request.Email);
        var attempts = memoryCache.Get<int>(key);
        if (attempts >= MaxLoginAttempts)
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new { error = "Muitas tentativas invalidas. Tente novamente em 1 minuto." });
        }

        if (await authService.LoginAsync(request, cancellationToken) is { } response)
        {
            memoryCache.Remove(key);
            return Ok(response);
        }

        attempts++;
        memoryCache.Set(key, attempts, LoginAttemptWindow);
        var remaining = Math.Max(0, MaxLoginAttempts - attempts);
        return Unauthorized(new { error = $"Email ou senha invalidos. Restam {remaining} tentativa(s)." });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            return Unauthorized(new { error = "Token invalido." });

        try
        {
            return await authService.ChangePasswordAsync(id, request, cancellationToken)
                ? NoContent()
                : BadRequest(new { error = "Senha atual invalida." });
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public ActionResult<AuthResponse> RefreshToken()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            || !Enum.TryParse<UserRole>(User.FindFirstValue(ClaimTypes.Role), out var role))
        {
            return Unauthorized(new { error = "Token invalido." });
        }

        return Ok(tokenService.Create(new User
        {
            Id = id,
            Name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Role = role
        }));
    }

    private string LoginAttemptKey(string email)
        => $"login:{HttpContext.Connection.RemoteIpAddress}:{email.Trim().ToLowerInvariant()}";
}
