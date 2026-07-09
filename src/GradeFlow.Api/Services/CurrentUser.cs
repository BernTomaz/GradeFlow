using System.Security.Claims;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Enums;

namespace GradeFlow.Api.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? Id
        => Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public UserRole? Role
        => Enum.TryParse<UserRole>(User?.FindFirstValue(ClaimTypes.Role), out var role) ? role : null;
}
