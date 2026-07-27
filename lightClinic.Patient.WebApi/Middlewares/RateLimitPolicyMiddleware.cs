namespace lightClinic.Patient.WebApi.Middlewares;

public class RateLimitPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitPolicyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var policyName = DetermineRateLimitPolicy(context);

        var endpoint = context.GetEndpoint();
        if (endpoint is not null)
        {
            // Attach metadata to the endpoint if possible (reflection or custom logic may be needed)
            // Or, use HttpContext.Items as a workaround:
            context.Items["RateLimitPolicy"] = policyName;
        }

        await _next(context);
    }

    private static string DetermineRateLimitPolicy(HttpContext context)
    {
        // Check if user is authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Check endpoint-specific policies
            var path = context.Request.Path.Value?.ToLower();
            
            if (path?.Contains("/auth") == true)
            {
                return "Auth";
            }
            
            // Default for authenticated users
            return "Authenticated";
        }
        
        // Anonymous users
        return "Anonymous";
    }
}

public static class RateLimitPolicyMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimitPolicy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitPolicyMiddleware>();
    }
}
