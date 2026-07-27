using lightClinic.Patient.Application.Extensions;
using lightClinic.Patient.Infrastructure.Extensions;
using lightClinic.Patient.Persistence.Contexts;
using lightClinic.Patient.Persistence.Extensions;
using lightClinic.Patient.Persistence.Seeders;
using lightClinic.Patient.Presentation.Extensions;
using lightClinic.Patient.WebApi;
using lightClinic.Patient.WebApi.Extensions;
using lightClinic.Patient.WebApi.HealthChecks;
using lightClinic.Patient.WebApi.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var origin = "patientorigin";
var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddInMemoryCollection(GetSensitiveConfigFromEnvironment());

// Validate required sensitive configuration (from env vars) in non-Development environments
ValidateSensitiveConfiguration(builder.Configuration);

var corsOrigins = builder.Configuration.GetSection("App:CorsOrigins").Get<string>()?.Split(';') ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(origin, builder =>
    {
        builder.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add health checks: liveness (app running), readiness (Database + RateLimiter)
builder.Services.AddHealthChecks()
    .AddCheck("self", _ => HealthCheckResult.Healthy("Application is running"), tags: ["live"])
    .AddDbContextCheck<PatientAuthDbContext>("Database", tags: ["ready"])
    .AddCheck<RateLimitHealthCheck>("RateLimiter", tags: ["ready"]);

// Map Minimal API endpoints
builder.Services.AddMinimalEndpoints();
builder.Services.AddAdvancedRateLimiting(builder.Configuration);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddBackendServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Use relative path for API Gateway compatibility
    c.SwaggerEndpoint("v1/swagger.json", "LightClinic Patient Auth API V1");
});

using (var scope = app.Services.CreateScope())
using (var dbContext = scope.ServiceProvider.GetRequiredService<PatientAuthDbContext>())
{
    if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
    {
        await dbContext.Database.MigrateAsync();
    }
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(origin);
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseSerilogRequestLogging();
// Add rate limiting policy middleware before rate limiter
app.UseRateLimitPolicy();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.RegisterMinimalEndpoints();
app.UseExceptionHandler();

// Shared: set status code (200 vs 503) and content type for health responses (for load balancers / App Runner)
static void SetHealthResponseStatus(HttpContext context, HealthReport report)
{
    context.Response.StatusCode = report.Status == HealthStatus.Healthy
        ? StatusCodes.Status200OK
        : StatusCodes.Status503ServiceUnavailable;
    context.Response.ContentType = "application/json";
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        SetHealthResponseStatus(context, report);
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

// Liveness: is the process up? (no dependencies). Use for orchestrator/container restart.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = reg => reg.Tags.Contains("live"),
    ResponseWriter = async (context, report) =>
    {
        SetHealthResponseStatus(context, report);
        await context.Response.WriteAsJsonAsync(new { status = report.Status.ToString() });
    }
});

// Readiness: can the app accept traffic? (Database + RateLimiter). Use for load balancer routing.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = reg => reg.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        SetHealthResponseStatus(context, report);
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

await app.RunAsync();

/// <summary>
/// Maps sensitive configuration from environment variables into the configuration key names used by the app.
/// Set LIGHTCLINIC_CONNECTION_STRING, LIGHTCLINIC_JWT_ISSUER_SIGNING_KEY, LIGHTCLINIC_AWS_ACCESS_KEY,
/// LIGHTCLINIC_AWS_SECRET_KEY, LIGHTCLINIC_GOOGLE_CLIENT_ID, LIGHTCLINIC_GOOGLE_CLIENT_SECRET.
/// </summary>
static Dictionary<string, string?> GetSensitiveConfigFromEnvironment()
{
    var overrides = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    var connectionString = Environment.GetEnvironmentVariable("LIGHTCLINIC_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(connectionString))
        overrides["ConnectionStrings:DefaultConnection"] = connectionString;
    var jwtKey = Environment.GetEnvironmentVariable("LIGHTCLINIC_JWT_ISSUER_SIGNING_KEY");
    if (!string.IsNullOrEmpty(jwtKey))
        overrides["JsonWebTokenKeys:IssuerSigningKey"] = jwtKey;
    var awsAccessKey = Environment.GetEnvironmentVariable("LIGHTCLINIC_AWS_ACCESS_KEY");
    if (!string.IsNullOrEmpty(awsAccessKey))
        overrides["AWS:AccessKey"] = awsAccessKey;
    var awsSecretKey = Environment.GetEnvironmentVariable("LIGHTCLINIC_AWS_SECRET_KEY");
    if (!string.IsNullOrEmpty(awsSecretKey))
        overrides["AWS:SecretKey"] = awsSecretKey;
    var googleClientId = Environment.GetEnvironmentVariable("LIGHTCLINIC_GOOGLE_CLIENT_ID");
    if (!string.IsNullOrEmpty(googleClientId))
        overrides["Authentication:Google:ClientId"] = googleClientId;
    var googleClientSecret = Environment.GetEnvironmentVariable("LIGHTCLINIC_GOOGLE_CLIENT_SECRET");
    if (!string.IsNullOrEmpty(googleClientSecret))
        overrides["Authentication:Google:ClientSecret"] = googleClientSecret;
    return overrides;
}

static void ValidateSensitiveConfiguration(IConfiguration configuration)
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if (string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
        return;
    var errors = new List<string>();
    if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
        errors.Add("Connection string is required. Set LIGHTCLINIC_CONNECTION_STRING or ConnectionStrings__DefaultConnection.");
    if (string.IsNullOrWhiteSpace(configuration["JsonWebTokenKeys:IssuerSigningKey"]))
        errors.Add("JWT signing key is required. Set LIGHTCLINIC_JWT_ISSUER_SIGNING_KEY or JsonWebTokenKeys__IssuerSigningKey.");
    if (errors.Count > 0)
        throw new InvalidOperationException("Missing required configuration: " + string.Join(" ", errors));
}