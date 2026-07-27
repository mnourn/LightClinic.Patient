using lightClinic.Patient.Contracts.DTOs.Address;
using lightClinic.Patient.Contracts.Interfaces.Address;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class AddressEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/address")
            .WithTags("Address")
            .WithOpenApi();

        // Authenticated endpoints
        var authenticatedGroup = group.MapGroup("")
            .RequireAuthorization()
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName);

        // Get Address by ID
        authenticatedGroup.MapGet("Get/{id:int}", GetAsync)
            .WithName("GetAddress")
            .WithSummary("Get address by ID")
            .WithDescription("Get address information by ID");

        // Get All Addresses
        authenticatedGroup.MapGet("/GetAll", GetAllAsync)
            .WithName("GetAllAddresses")
            .WithSummary("Get all addresses")
            .WithDescription("Get all addresses for the current user");

        // Create Address
        authenticatedGroup.MapPost("/Create", CreateAsync)
            .WithName("CreateAddress")
            .WithSummary("Create address")
            .WithDescription("Create a new address");

        // Update Address
        authenticatedGroup.MapPut("/Update", UpdateAsync)
            .WithName("UpdateAddress")
            .WithSummary("Update address")
            .WithDescription("Update existing address information");

        // Delete Address
        authenticatedGroup.MapDelete("Delete/{id:int}", DeleteAsync)
            .WithName("DeleteAddress")
            .WithSummary("Delete address")
            .WithDescription("Delete address by ID");
    }

    private static async Task<IResult> GetAsync(
        int id,
        IAddressService addressService,
        CancellationToken cancellationToken)
    {
        var result = await addressService.GetAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetAllAsync(
        IAddressService addressService,
        CancellationToken cancellationToken)
    {
        var result = await addressService.GetAllAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> CreateAsync(
        CreateAddressDto addressDto,
        IAddressService addressService,
        CancellationToken cancellationToken)
    {
        var result = await addressService.CreateAsync(addressDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> UpdateAsync(
        UpdateAddressDto addressDto,
        IAddressService addressService,
        CancellationToken cancellationToken)
    {
        var result = await addressService.UpdateAsync(addressDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> DeleteAsync(
        int id,
        IAddressService addressService,
        CancellationToken cancellationToken)
    {
        var result = await addressService.DeleteAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
