using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace lightClinic.Patient.WebApi;

public static class DependencyInjection
{
    /// <summary>
    /// Adds backend services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    public static void AddBackendServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerConfiguration();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationConfiguration();
        services.AddCookieAuthentication();
    }

    /// <summary>
    /// Configures Swagger/OpenAPI documentation
    /// </summary>
    private static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LightClinic Patient Auth API",
                Version = "v1",
                Description = "Authentication and authorization API for LightClinic Patient application"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });
    }

    /// <summary>
    /// Configures JWT authentication
    /// </summary>
    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JsonWebTokenKeys");
        var issuerSigningKey = jwtSettings["IssuerSigningKey"];

        if (string.IsNullOrEmpty(issuerSigningKey))
        {
            throw new InvalidOperationException("JWT IssuerSigningKey is not configured");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issuerSigningKey)),
                ValidateIssuer = jwtSettings.GetValue<bool>("ValidateIssuer"),
                ValidIssuer = jwtSettings["ValidIssuer"],
                ValidateAudience = jwtSettings.GetValue<bool>("ValidateAudience"),
                ValidAudience = jwtSettings["ValidAudience"],
            };
        }).AddGoogle(options =>
        {
            IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");

            options.ClientId = googleAuthNSection["ClientId"];
            options.ClientSecret = googleAuthNSection["ClientSecret"];
        });
    }

    /// <summary>
    /// Configures authorization policies
    /// </summary>
    private static void AddAuthorizationConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    /// <summary>
    /// Configures cookie authentication for non-production environments
    /// </summary>
    private static void AddCookieAuthentication(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }
                    return Task.CompletedTask;
                }
            };
        });
    }
}