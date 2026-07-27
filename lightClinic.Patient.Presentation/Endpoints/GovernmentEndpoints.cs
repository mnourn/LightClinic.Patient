using lightClinic.Patient.Contracts.Interfaces.Government;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class GovernmentEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/government")
            .WithTags("Government")
            .WithOpenApi();

        // Anonymous endpoint with rate limiting
        var anonymousGroup = group.MapGroup("")
            .AllowAnonymous()
            .RequireRateLimiting(ApplicationConstants.AnonymousPolicyName);

        // Get All Governments
        anonymousGroup.MapGet("GetAll", GetAllAsync)
            .WithName("GetAllGovernments")
            .WithSummary("Get all governments")
            .WithDescription("Get all available governments with their cities");
    }

    private static async Task<IResult> GetAllAsync(
        IGovernmentService governmentService,
        CancellationToken cancellationToken)
    {
        var result = await governmentService.GetAllAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
