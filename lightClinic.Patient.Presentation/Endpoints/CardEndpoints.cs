using lightClinic.Patient.Contracts.DTOs.Card;
using lightClinic.Patient.Contracts.Interfaces.Card;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class CardEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/card")
            .WithTags("Card")
            .WithOpenApi();

        // Authenticated endpoints
        var authenticatedGroup = group.MapGroup("")
            .RequireAuthorization()
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName);

        // Get Card by ID
        authenticatedGroup.MapGet("Get/{id:int}", GetAsync)
            .WithName("GetCard")
            .WithSummary("Get card by ID")
            .WithDescription("Get card information by ID");

        // Get All Cards
        authenticatedGroup.MapGet("GetAll", GetAllAsync)
            .WithName("GetAllCards")
            .WithSummary("Get all cards")
            .WithDescription("Get all cards for the current user");

        // Create Card
        authenticatedGroup.MapPost("/Create", CreateAsync)
            .WithName("CreateCard")
            .WithSummary("Create card")
            .WithDescription("Create a new payment card");

        // Update Card
        authenticatedGroup.MapPut("/Update", UpdateAsync)
            .WithName("UpdateCard")
            .WithSummary("Update card")
            .WithDescription("Update existing card information");

        // Delete Card
        authenticatedGroup.MapDelete("Delete/{id:int}", DeleteAsync)
            .WithName("DeleteCard")
            .WithSummary("Delete card")
            .WithDescription("Delete card by ID");

        // Check Card Exists
        authenticatedGroup.MapGet("/Exists/{cardId:int}", ExistsAsync)
            .WithName("CardExists")
            .WithSummary("Check card exists")
            .WithDescription("Check if a card exists by card ID");
    }

    private static async Task<IResult> GetAsync(
        int id,
        ICardService cardService,
        CancellationToken cancellationToken)
    {
        var result = await cardService.GetAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetAllAsync(
        ICardService cardService,
        CancellationToken cancellationToken)
    {
        var result = await cardService.GetAllAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> CreateAsync(
        CreateCardDto cardDto,
        ICardService cardService,
        CancellationToken cancellationToken)
    {
        var result = await cardService.CreateAsync(cardDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> UpdateAsync(
        UpdateCardDto cardDto,
        ICardService cardService,
        CancellationToken cancellationToken)
    {
        var result = await cardService.UpdateAsync(cardDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> DeleteAsync(
        int id,
        ICardService cardService,
        CancellationToken cancellationToken)
    {
        var result = await cardService.DeleteAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> ExistsAsync(
        int cardId,
        ICardService cardService,
        CancellationToken cancellationToken)
    {
        var result = await cardService.ExistsAsync(cardId, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
