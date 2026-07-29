using System.ComponentModel.DataAnnotations;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradeFlow.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/setup")]
public sealed class SetupController(IAuthService authService) : ControllerBase
{
    [HttpGet("status")]
    public async Task<ActionResult<SetupStatusResponse>> Status(CancellationToken cancellationToken)
        => Ok(new SetupStatusResponse(await authService.IsSetupAvailableAsync(cancellationToken)));

    [HttpPost("admin")]
    public async Task<ActionResult<AuthResponse>> CreateAdmin(SetupAdminRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await authService.CreateSetupAdminAsync(request, cancellationToken));
        }
        catch (ValidationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
