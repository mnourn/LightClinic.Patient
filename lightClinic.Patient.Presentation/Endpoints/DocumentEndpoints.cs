using lightClinic.Patient.Contracts.Interfaces.Document;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class DocumentEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api")
            .WithTags("Document")
            .WithOpenApi();

        group.MapPost("/Profile/Upload", UploadDocument)
            .WithName("UploadDocument")
            .WithSummary("Upload a document for profile")
            .WithDescription("Uploads a document file and returns the filename")
            .Accepts<IFormFile>("multipart/form-data")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationConstants.PatientRoleName))
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName)
            .DisableAntiforgery();
    }

    private static async Task<IResult> UploadDocument(
        IFormFile file,
        IDocumentService documentService,
        CancellationToken cancellationToken)
    {
        var result = await documentService.UploadAsync(file, cancellationToken);

        return Results.Json(result, statusCode: (int)result.Code);
    }
}
