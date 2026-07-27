using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace lightClinic.Patient.Presentation.Endpoints;

public class AuthEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api/auth")
            .WithTags("Auth")
            .WithOpenApi();

        // Anonymous endpoints with rate limiting
        var anonymousGroup = group.MapGroup("")
            .AllowAnonymous()
            .RequireRateLimiting(ApplicationConstants.AnonymousPolicyName);

        // Login
        anonymousGroup.MapPost("/Login", LoginAsync)
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticate user with email/phone and password");

        // Logout
        anonymousGroup.MapPost("/Logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("User logout")
            .WithDescription("Logout current user");

        // Refresh Token
        anonymousGroup.MapPost("/RefreshToken", RefreshTokenAsync)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Generate new access token using refresh token");

        // Register
        anonymousGroup.MapPost("/Register", RegisterAsync)
            .WithName("Register")
            .WithSummary("User registration")
            .WithDescription("Register a new patient user");

        // Verify Account
        anonymousGroup.MapPost("/VerifyAccount", VerifyAccountAsync)
            .WithName("VerifyAccount")
            .WithSummary("Verify account")
            .WithDescription("Verify user account with OTP");

        // Forget Password
        anonymousGroup.MapPost("/ForgetPassword", ForgetPasswordAsync)
            .WithName("ForgetPassword")
            .WithSummary("Request password reset")
            .WithDescription("Send OTP for password reset");

        // Reset Password
        anonymousGroup.MapPost("/ResetPassword", ResetPasswordAsync)
            .WithName("ResetPassword")
            .WithSummary("Reset password")
            .WithDescription("Reset password using reset token");

        // Verify Reset Password
        anonymousGroup.MapPost("/VerifyResetPassword", VerifyResetPasswordAsync)
            .WithName("VerifyResetPassword")
            .WithSummary("Verify reset password OTP")
            .WithDescription("Verify OTP for password reset");

        // Generate OTP
        anonymousGroup.MapPost("/GenerateOtp", GenerateOtpAsync)
            .WithName("GenerateOtp")
            .WithSummary("Generate OTP")
            .WithDescription("Generate OTP for verification");

        // Verify OTP
        anonymousGroup.MapPost("/VerifyOtp", VerifyOtpAsync)
            .WithName("VerifyOtp")
            .WithSummary("Verify OTP")
            .WithDescription("Verify OTP code");

        // Google Login
        anonymousGroup.MapGet("/GoogleLogin", GoogleLogin)
            .WithName("GoogleLogin")
            .WithSummary("Google login")
            .WithDescription("Initiate Google OAuth login");

        // Google Callback
        anonymousGroup.MapGet("/Callback", CallBackAsync)
            .WithName("GoogleCallback")
            .WithSummary("Google login callback")
            .WithDescription("Handle Google OAuth callback");
    }

    // Private delegate methods
    private static async Task<IResult> LoginAsync(
        LoginDto login,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.LoginAsync(login, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> LogoutAsync(IUserService userService)
    {
        await userService.LogoutAsync();
        return Results.Ok();
    }

    private static async Task<IResult> RefreshTokenAsync(
        TokenModel tokenModel,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.RefreshTokenAsync(tokenModel, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> RegisterAsync(
        RegisterUserDto user,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.RegisterAsync(user, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> VerifyAccountAsync(
        VerifyAccountRequestDto request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.VerifyAccountAsync(request, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> ForgetPasswordAsync(
        ForgetPasswordRequestDto request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.ForgetPasswordAsync(request, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> ResetPasswordAsync(
        ResetPasswordRequestDto request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.ResetPasswordAsync(request, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> VerifyResetPasswordAsync(
        VerifyResetPasswordRequestDto request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.VerifyResetPasswordAsync(request, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> GenerateOtpAsync(
        OtpRequestDto request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.GenerateOtpAsync(request, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static async Task<IResult> VerifyOtpAsync(
        VerifyOtpRequestDto request,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.VerifyOtpAsync(request, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }

    private static IResult GoogleLogin(IUserService userService)
    {
        var provider = "Google";
        var properties = userService.GoogleLogin();
        return Results.Challenge(properties, [provider]);
    }

    private static async Task<IResult> CallBackAsync(
        string remoteError,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var result = await userService.GoogleLoginCallbackAsync(remoteError, cancellationToken);
        return Results.Json(result, statusCode: (int)result.Code!);
    }
}
