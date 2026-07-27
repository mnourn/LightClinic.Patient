using lightClinic.Patient.WebApi.Configuration;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace lightClinic.Patient.WebApi.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddAdvancedRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitConfig = configuration.GetSection("RateLimitSettings").Get<RateLimitConfig>() ?? new RateLimitConfig();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };

            // Global rate limiter
            options.AddFixedWindowLimiter("Global", limiterOptions =>
            {
                limiterOptions.PermitLimit = rateLimitConfig.Global.PermitLimit;
                limiterOptions.Window = TimeSpan.FromSeconds(rateLimitConfig.Global.WindowSeconds);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = rateLimitConfig.Global.QueueLimit;
            });

            // Authenticated users get higher limits
            options.AddFixedWindowLimiter("Authenticated", limiterOptions =>
            {
                limiterOptions.PermitLimit = rateLimitConfig.Authenticated.PermitLimit;
                limiterOptions.Window = TimeSpan.FromSeconds(rateLimitConfig.Authenticated.WindowSeconds);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = rateLimitConfig.Authenticated.QueueLimit;
            });

            // Anonymous users get lower limits
            options.AddFixedWindowLimiter("Anonymous", limiterOptions =>
            {
                limiterOptions.PermitLimit = rateLimitConfig.Anonymous.PermitLimit;
                limiterOptions.Window = TimeSpan.FromSeconds(rateLimitConfig.Anonymous.WindowSeconds);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = rateLimitConfig.Anonymous.QueueLimit;
            });

            // Auth-specific rate limiter
            options.AddFixedWindowLimiter("Auth", limiterOptions =>
            {
                limiterOptions.PermitLimit = rateLimitConfig.Auth.PermitLimit;
                limiterOptions.Window = TimeSpan.FromSeconds(rateLimitConfig.Auth.WindowSeconds);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = rateLimitConfig.Auth.QueueLimit;
            });

            // Token bucket limiter for burst handling
            options.AddTokenBucketLimiter("TokenBucket", limiterOptions =>
            {
                limiterOptions.TokenLimit = 100;
                limiterOptions.TokensPerPeriod = 10;
                limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
            });

            // Sliding window for better distribution
            options.AddSlidingWindowLimiter("SlidingWindow", limiterOptions =>
            {
                limiterOptions.PermitLimit = 50;
                limiterOptions.Window = TimeSpan.FromSeconds(10);
                limiterOptions.SegmentsPerWindow = 2;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });
        });

        return services;
    }
}
