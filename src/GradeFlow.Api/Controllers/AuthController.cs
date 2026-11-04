using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Services;
using GradeFlow.Application.Repositories;
using GradeFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthService authService,
    ITokenService tokenService,
    IUserRepository userRepository,
    IMemoryCache memoryCache) : ControllerBase
{
    private const int MaxLoginAttempts = 5;
    private static readonly TimeSpan LoginAttemptWindow = TimeSpan.FromMinutes(1);

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> ListUsers(CancellationToken cancellationToken)
        => Ok(await authService.ListUsersAsync(cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await authService.ResetPasswordAsync(id, request, cancellationToken)
                ? NoContent()
                : NotFound(new { error = "Usuario nao encontrado." });
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
            if (!await authService.ChangePasswordAsync(id, request, cancellationToken))
                return BadRequest(new { error = "Senha atual invalida." });

            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            return user is null ? Unauthorized() : Ok(tokenService.Create(user));
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [Authorize]
    [HttpPost("change-name")]
    public async Task<ActionResult<AuthResponse>> ChangeName(ChangeNameRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            return Unauthorized(new { error = "Token invalido." });

        try
        {
            return await authService.ChangeNameAsync(id, request, cancellationToken) is { } response
                ? Ok(response)
                : Unauthorized(new { error = "Token invalido." });
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
        if (HttpContext.Items[typeof(User)] is not User user)
        {
            return Unauthorized(new { error = "Token invalido." });
        }

        return Ok(tokenService.Create(user));
    }

    private string LoginAttemptKey(string email)
        => $"login:{HttpContext.Connection.RemoteIpAddress}:{email.Trim().ToLowerInvariant()}";
}
