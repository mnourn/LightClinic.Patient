using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Contracts.Interfaces.Profile;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class ProfileEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/profile")
            .WithTags("Profile")
            .WithOpenApi();

        // Authenticated endpoints
        var authenticatedGroup = group.MapGroup("")
            .RequireAuthorization()
            .RequireRateLimiting(ApplicationConstants.AuthenticatedPolicyName);

        // Doctor role endpoints
        var doctorGroup = authenticatedGroup.MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationConstants.DoctorRoleName));

        // Create Profile
        doctorGroup.MapPost("/Create", CreateAsync)
            .WithName("CreateProfile")
            .WithSummary("Create profile")
            .WithDescription("Create a new patient profile");

        // Get Profiles by IDs
        doctorGroup.MapPost("/GetProfiles", GetProfilesAsync)
            .WithName("GetProfiles")
            .WithSummary("Get profiles by IDs")
            .WithDescription("Get multiple profiles by their IDs");

        // Search Profiles
        doctorGroup.MapGet("/Search", SearchAsync)
            .WithName("SearchProfiles")
            .WithSummary("Search profiles")
            .WithDescription("Search profiles by search term");

        // Get Profile by Profile ID
        doctorGroup.MapGet("GetByProfile/{profileId:guid}", GetByProfileAsync)
            .WithName("GetByProfile")
            .WithSummary("Get profile by profile ID")
            .WithDescription("Get profile information by profile ID");

        // Patient role endpoints
        var patientGroup = authenticatedGroup.MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(ApplicationConstants.PatientRoleName));

        // Get Profile
        patientGroup.MapGet("/Get", GetAsync)
            .WithName("GetProfile")
            .WithSummary("Get profile")
            .WithDescription("Get current user profile information");

        // Update Password
        patientGroup.MapPost("/UpdatePassword", UpdatePasswordAsync)
            .WithName("UpdatePassword")
            .WithSummary("Update password")
            .WithDescription("Update current user password");

        // Update Profile
        patientGroup.MapPatch("/Update", UpdateAsync)
            .WithName("UpdateProfile")
            .WithSummary("Update profile")
            .WithDescription("Update current user profile information");

        // Get Family Member
        patientGroup.MapGet("/FamilyMember/{id:guid}", GetFamilyMemberAsync)
            .WithName("GetFamilyMember")
            .WithSummary("Get family member")
            .WithDescription("Get family member by ID");

        // Get All Family Members
        patientGroup.MapGet("/GetAllFamilyMembers", GetAllFamilyMembersAsync)
            .WithName("GetAllFamilyMembers")
            .WithSummary("Get all family members")
            .WithDescription("Get all family members for current user");

        // Create Family Member
        patientGroup.MapPost("CreateFamilyMember", CreateFamilyMemberAsync)
            .WithName("CreateFamilyMember")
            .WithSummary("Create family member")
            .WithDescription("Create a new family member");

        // Update Family Member
        patientGroup.MapPut("UpdateFamilyMember", UpdateFamilyMemberAsync)
            .WithName("UpdateFamilyMember")
            .WithSummary("Update family member")
            .WithDescription("Update family member information");

        // Delete Family Member
        patientGroup.MapDelete("DeleteFamilyMember/{id:guid}", DeleteFamilyMemberAsync)
            .WithName("DeleteFamilyMember")
            .WithSummary("Delete family member")
            .WithDescription("Delete family member by ID");

        // Check Profile Exists
        patientGroup.MapGet("/Exists/{id:guid}", ExistsAsync)
            .WithName("ProfileExists")
            .WithSummary("Check profile exists")
            .WithDescription("Check if profile exists by ID");

        // Get Profiles by IDs (no role restriction)
        authenticatedGroup.MapPost("/GetByIds", GetByIdsAsync)
            .WithName("GetProfilesByIds")
            .WithSummary("Get profiles by IDs")
            .WithDescription("Get multiple profiles by their IDs");
    }

    private static async Task<IResult> CreateAsync(
        CreateUserDto user,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.CreateAsync(user, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetAsync(
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.GetAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> UpdatePasswordAsync(
        UpdatePasswordDto updatePasswordDto,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.UpdatePasswordAsync(updatePasswordDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetProfilesAsync(
        Guid[] ids,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.GetProfilesAsync(ids, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> UpdateAsync(
        UpdateProfileDto profile,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.UpdateAsync(profile, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetFamilyMemberAsync(
        Guid id,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.GetFamilyMemberAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetAllFamilyMembersAsync(
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.GetAllFamilyMembersAsync(cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> CreateFamilyMemberAsync(
        CreateFamilyMemberDto memberDto,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.CreateFamilyMemberAsync(memberDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> UpdateFamilyMemberAsync(
        UpdateFamilyMemberDto memberDto,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.UpdateFamilyMemberAsync(memberDto, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> DeleteFamilyMemberAsync(
        Guid id,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.DeleteFamilyMemberAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> ExistsAsync(
        Guid id,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.ExistsAsync(id, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> SearchAsync(
        string searchTerm,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.SearchAsync(searchTerm, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetByProfileAsync(
        Guid profileId,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.GetByProfileAsync(profileId, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }

    private static async Task<IResult> GetByIdsAsync(
        Guid[] ids,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var result = await profileService.GetByIdsAsync(ids, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code);
    }
}
