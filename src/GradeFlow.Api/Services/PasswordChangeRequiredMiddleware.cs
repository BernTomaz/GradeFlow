using GradeFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace GradeFlow.Api.Services;

public sealed class PasswordChangeRequiredMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var changingPassword = HttpMethods.IsPost(context.Request.Method)
            && string.Equals(context.Request.Path.Value?.TrimEnd('/'), "/api/auth/change-password", StringComparison.OrdinalIgnoreCase);
        if (context.Items[typeof(User)] is User { MustChangePassword: true }
            && endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is null
            && endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>().Count > 0
            && !changingPassword)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Altere sua senha antes de continuar." });
            return;
        }

        await next(context);
    }
}
