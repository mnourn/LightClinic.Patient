using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class FcmTokenEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/fcmtoken")
            .WithTags("FcmToken")
            .WithOpenApi()
            .RequireAuthorization(policy => policy.RequireRole(ApplicationConstants.PatientRoleName))
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName);

        // Update Fcm Token
        group.MapPut("", UpdateAsync)
            .WithName("UpdateFcmToken")
            .WithSummary("Update Fcm Token")
            .WithDescription("Update Fcm Token");
    }

    private static async Task<IResult> UpdateAsync(
        IFcmService fcmService,
        string fcmToken,
        string deviceId,
        CancellationToken cancellationToken)
    {
        var result = await fcmService.UpdateAsync(fcmToken, deviceId, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
