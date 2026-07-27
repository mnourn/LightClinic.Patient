using lightClinic.Patient.Contracts.DTOs.Setting;
using lightClinic.Patient.Contracts.Interfaces.Setting;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class SettingsEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/settings")
            .WithTags("Settings")
            .WithOpenApi();

        // Authenticated endpoints
        var authenticatedGroup = group.MapGroup("")
            .RequireAuthorization()
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName);

        // Patient role endpoints
        var patientGroup = authenticatedGroup.MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationConstants.PatientRoleName));

        // Get Settings
        patientGroup.MapGet("/Get", GetAsync)
            .WithName("GetSettings")
            .WithSummary("Get settings")
            .WithDescription("Get current user settings");

        // Update Settings
        patientGroup.MapPut("/Update", UpdateAsync)
            .WithName("UpdateSettings")
            .WithSummary("Update settings")
            .WithDescription("Update current user settings");
    }

    private static async Task<IResult> GetAsync(
        ISettingService settingService,
        CancellationToken cancellationToken)
    {
        var result = await settingService.GetAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> UpdateAsync(
        UpdateSettingDto setting,
        ISettingService settingService,
        CancellationToken cancellationToken)
    {
        var result = await settingService.UpdateAsync(setting, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
