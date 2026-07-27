using lightClinic.Patient.Contracts.Interfaces.City;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class CityEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/city")
            .WithTags("City")
            .WithOpenApi();

        // Anonymous endpoint with rate limiting
        var anonymousGroup = group.MapGroup("")
            .AllowAnonymous()
            .RequireRateLimiting(ApplicationConstants.AnonymousPolicyName);

        // Get All Cities
        anonymousGroup.MapGet("GetAll", GetAllAsync)
            .WithName("GetAllCities")
            .WithSummary("Get all cities")
            .WithDescription("Get all available cities");
    }

    private static async Task<IResult> GetAllAsync(
        ICityService cityService,
        CancellationToken cancellationToken)
    {
        var result = await cityService.GetAllAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
