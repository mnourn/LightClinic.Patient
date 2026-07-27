using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Contracts.DTOs.OTP;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace lightClinic.Patient.Contracts.Interfaces.Auth;

public interface IUserService
{
    Task<IResult<LoginResponseDto>> LoginAsync(LoginDto login, CancellationToken cancellationToken);
    Task LogoutAsync();
    Task<IResult<RegisterOtpDto>> RegisterAsync(RegisterUserDto user, CancellationToken cancellationToken);
    Task<IResult<string>> VerifyAccountAsync(VerifyAccountRequestDto request, CancellationToken cancellationToken);
    Task<IResult<OtpDto>> ForgetPasswordAsync(ForgetPasswordRequestDto request, CancellationToken cancellationToken);
    Task<IResult<VerifyResetPasswordOtpDto>> VerifyResetPasswordAsync(VerifyResetPasswordRequestDto request, CancellationToken cancellationToken);
    Task<IResult<string>> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken);
    Task<IResult<long>> GenerateOtpAsync(OtpRequestDto request, CancellationToken cancellationToken);
    Task<IResult<CheckOtpDto>> VerifyOtpAsync(VerifyOtpRequestDto request, CancellationToken cancellationToken);
    AuthenticationProperties GoogleLogin();
    Task<IResult<UserDto>> GoogleLoginCallbackAsync(string remoteError, CancellationToken cancellationToken);
    Task<IResult<TokenModel>> RefreshTokenAsync(TokenModel tokenModel, CancellationToken cancellationToken);
}
