using GradeFlow.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GradeFlow.Api.Services;

public sealed class DatabaseHealthCheck(GradeFlowDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
        => await dbContext.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Database connection failed.");
}
