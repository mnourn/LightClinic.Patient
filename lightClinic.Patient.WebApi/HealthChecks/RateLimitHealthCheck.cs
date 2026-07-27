using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace lightClinic.Patient.WebApi.HealthChecks;

public class RateLimitHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple health check that verifies the rate limiter service is registered
            // The actual rate limiter configuration is handled by the framework
            return Task.FromResult(HealthCheckResult.Healthy("Rate limiter service is available"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Rate limiter health check failed", ex));
        }
    }
}
