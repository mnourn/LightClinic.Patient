using FluentValidation;
using lightClinic.Patient.Application.Extensions;
using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Contracts.DTOs.Notification;
using lightClinic.Patient.Contracts.DTOs.OTP;
using lightClinic.Patient.Contracts.Interfaces.Auth;
using lightClinic.Patient.Contracts.Interfaces.Common;
using lightClinic.Patient.Contracts.Interfaces.Notification;
using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared;
using lightClinic.Patient.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace lightClinic.Patient.Application.Services.Auth;

public class UserService : IUserService, IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly INotificationApi _notificationApi;
    private readonly ILogger<UserService> _logger;
    private readonly IValidator<RegisterUserDto> _registerUserValidator;

    public UserService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        INotificationApi notificationApi,
        ILogger<UserService> logger,
        IValidator<RegisterUserDto> registerUserValidator)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
        _notificationApi = notificationApi;
        _logger = logger;
        _registerUserValidator = registerUserValidator;
    }

    public async Task<IResult<LoginResponseDto>> LoginAsync(LoginDto login, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(login.Email) && string.IsNullOrWhiteSpace(login.PhoneNumber))
        {
            return await Result<LoginResponseDto>.FailureAsync("Email or PhoneNumber must not be empty!", System.Net.HttpStatusCode.BadRequest);
        }

        var user = await _userManager.Users
            .WhereIf(!string.IsNullOrWhiteSpace(login.Email), x => x.Email.Trim().ToLower() == login.Email.Trim().ToLower())
            .WhereIf(!string.IsNullOrWhiteSpace(login.PhoneNumber), x => x.PhoneNumber.Trim().ToLower() == login.PhoneNumber.Trim().ToLower())
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return await Result<LoginResponseDto>.FailureAsync("Invalid Username and/or Password", System.Net.HttpStatusCode.BadRequest);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return await Result<LoginResponseDto>.FailureAsync($"Account is locked due to multiple failed attempts. Please try again later.", System.Net.HttpStatusCode.Forbidden);
        }

        if (!await _userManager.CheckPasswordAsync(user, login.Password))
        {
            var remainingAttempts = ApplicationConstants.LockoutMaxFailedAccessAttempts - user.AccessFailedCount - 1;
            var message = remainingAttempts > 0
                ? $"Invalid Username and/or Password. {remainingAttempts} attempt(s) remaining before account lockout."
                : $"Invalid Username and/or Password. Account has been locked for {ApplicationConstants.LockoutDurationMinutes} minutes due to too many failed attempts.";
            await _userManager.AccessFailedAsync(user);
            return await Result<LoginResponseDto>.FailureAsync(message, System.Net.HttpStatusCode.BadRequest);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var response = new LoginResponseDto()
        {
            UserId = user.Id,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            Language = user.Language,
            NotificationEnabled = user.NotificationEnabled,
        };
        if (user.EmailConfirmed || user.PhoneNumberConfirmed)
        {
            response.Token = GenerateJwtToken(user);
            user.RefreshToken = response.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(ApplicationConstants.RefreshTokenExpirationDays);

            await _userManager.UpdateAsync(user);
        }

        return await Result<LoginResponseDto>.SuccessAsync(response, !user.EmailConfirmed && !user.PhoneNumberConfirmed ? "Account is not verified!" : "Login successful");
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<IResult<RegisterOtpDto>> RegisterAsync(RegisterUserDto user, CancellationToken cancellationToken)
    {
        var result = _registerUserValidator.Validate(user);
        if (!result.IsValid)
        {
            return await Result<RegisterOtpDto>.ValidationFailureAsync(result.Errors, System.Net.HttpStatusCode.UnprocessableEntity);
        }
        if (!string.IsNullOrWhiteSpace(user.Email) && await _userManager.Users
            .AnyAsync(x => x.Email.Trim().ToLower() == user.Email.Trim().ToLower(), cancellationToken))
        {
            return await Result<RegisterOtpDto>.FailureAsync("Email is already existed!", System.Net.HttpStatusCode.BadRequest);
        }
        if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && await _userManager.Users
            .AnyAsync(x => x.PhoneNumber.Trim().ToLower() == user.PhoneNumber.Trim().ToLower(), cancellationToken))
        {
            return await Result<RegisterOtpDto>.FailureAsync("PhoneNumber is already existed!", System.Net.HttpStatusCode.BadRequest);
        }

        var profile = new Domain.Entities.Profile()
        {
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
        };
        var newUser = new ApplicationUser
        {
            UserName = user.Email,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email,
            CreatedDate = DateTime.UtcNow,
            Profiles = [profile],
        };

        var res = await _userManager.CreateAsync(newUser, user.Password);
        if (!res.Succeeded)
        {
            return await Result<RegisterOtpDto>.FailureAsync(res.Errors.Select(x => x.Code + ":" + x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
        }

        var otpId = await GenerateOtpAsync(new OtpRequestDto()
        {
            UserId = newUser.Id,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
        }, cancellationToken);

        return await Result<RegisterOtpDto>.SuccessAsync(new RegisterOtpDto()
        {
            UserId = newUser.Id,
            OtpId = otpId.Data,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber
        }, otpId.Succeeded ? "User registered successfully" : "User registered with warning: " + otpId.Message);
    }

    public async Task<IResult<string>> VerifyAccountAsync(VerifyAccountRequestDto request, CancellationToken cancellationToken)
    {
        if (!await _userManager.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken))
        {
            return await Result<string>.FailureAsync("User not found!", System.Net.HttpStatusCode.BadRequest);
        }

        var otp = await VerifyOtpAsync(new VerifyOtpRequestDto()
        {
            UserId = request.UserId,
            OtpId = request.OtpId,
            Otp = request.Otp,
        }, cancellationToken);

        if (otp is null || !otp.Succeeded)
        {
            return await Result<string>.FailureAsync(otp is null ? "Verification failed" : otp.Data.Message, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        return await Result<string>.SuccessAsync("Verified successfully.");
    }

    public async Task<IResult<OtpDto>> ForgetPasswordAsync(ForgetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return await Result<OtpDto>.FailureAsync("Email or Phone must not be empty!", System.Net.HttpStatusCode.BadRequest);
        }

        var user = await _userManager.Users
            .WhereIf(!string.IsNullOrWhiteSpace(request.Email), x => x.Email.Trim().ToLower() == request.Email.Trim().ToLower())
            .WhereIf(!string.IsNullOrWhiteSpace(request.PhoneNumber), x => x.PhoneNumber.Trim().ToLower() == request.PhoneNumber.Trim().ToLower())
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return await Result<OtpDto>.FailureAsync("User not found!", System.Net.HttpStatusCode.BadRequest);
        }

        var otpId = await GenerateOtpAsync(new OtpRequestDto()
        {
            UserId = user.Id,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
        }, cancellationToken);

        return await Result<OtpDto>.SuccessAsync(new OtpDto()
        {
            OtpId = otpId.Data,
            UserId = user.Id,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        }, otpId.Succeeded ? null : otpId.Message);
    }

    public async Task<IResult<VerifyResetPasswordOtpDto>> VerifyResetPasswordAsync(VerifyResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null)
        {
            return await Result<VerifyResetPasswordOtpDto>.FailureAsync("User not found!", System.Net.HttpStatusCode.UnprocessableEntity);
        }

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var otp = await VerifyOtpAsync(new VerifyOtpRequestDto()
        {
            UserId = user.Id,
            OtpId = request.OtpId,
            Otp = request.Otp,
        }, cancellationToken);

        if (otp is null || !otp.Succeeded)
        {
            return await Result<VerifyResetPasswordOtpDto>.FailureAsync(otp is null ? "Verification failed" : otp.Data.Message, System.Net.HttpStatusCode.UnprocessableEntity);
        }

        return await Result<VerifyResetPasswordOtpDto>.SuccessAsync(new VerifyResetPasswordOtpDto
        {
            UserId = user.Id,
            Verified = true,
            ResetPasswordToken = resetPasswordToken
        }, "Verified successfully.");
    }

    public async Task<IResult<string>> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return await Result<string>.FailureAsync("User not found!", System.Net.HttpStatusCode.UnprocessableEntity);
        }
        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (resetPasswordResult.Errors.Any())
        {
            return await Result<string>.ValidationFailureAsync(resetPasswordResult.Errors.ToList(), System.Net.HttpStatusCode.UnprocessableEntity);
        }

        if (resetPasswordResult.Succeeded)
        {
            return await Result<string>.SuccessAsync("The password updated successfully.");
        }

        return await Result<string>.FailureAsync("Invalid token: Unable to complete the verification process for this account", System.Net.HttpStatusCode.BadRequest);
    }

    public async Task<IResult<long>> GenerateOtpAsync(OtpRequestDto request, CancellationToken cancellationToken)
    {
        if (!await _userManager.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken))
        {
            return await Result<long>.FailureAsync("User not found!", System.Net.HttpStatusCode.BadRequest);
        }
        var otpRepository = _unitOfWork.Repository<AuthOtp>();
        int otpNumber = 100000;
        var useStaticOtp = _configuration["UseStaticOtp"];
        if (useStaticOtp == null || !bool.TryParse(useStaticOtp, out bool staticOtp) || !staticOtp)
        {
            int start = (int)Math.Pow(10, ApplicationConstants.OTPLength - 1);
            int end = (int)Math.Pow(10, ApplicationConstants.OTPLength) - 1;
            otpNumber = new Random().Next(start, end);
        }

        var otp = new AuthOtp()
        {
            UserId = request.UserId,
            Otp = otpNumber,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ExpiryTime = DateTime.UtcNow.AddSeconds(ApplicationConstants.OTPExpirationSeconds),
        };

        var existingOtp = await otpRepository.Entities
            .Where(x => x.UserId == request.UserId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Email), x => x.Email.Trim().ToLower() == request.Email.Trim().ToLower())
            .WhereIf(!string.IsNullOrWhiteSpace(request.PhoneNumber), x => x.PhoneNumber.Trim().ToLower() == request.PhoneNumber.Trim().ToLower())
            .FirstOrDefaultAsync(cancellationToken);

        if (existingOtp is not null)
        {
            await otpRepository.DeleteAsync(existingOtp);
        }

        await otpRepository.AddAsync(otp);

        var notification = await SendOtpAsync(request.Email, request.PhoneNumber, otpNumber, cancellationToken);
        var res = await _unitOfWork.SaveAsync(cancellationToken) == 0;

        if (res)
        {
            return await Result<long>.FailureAsync("Connection failed!", System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<long>.SuccessAsync(otp.Id, string.IsNullOrWhiteSpace(notification) ? "OTP was sent to your email." : "OTP was created with warning: " + notification);
    }

    public async Task<IResult<CheckOtpDto>> VerifyOtpAsync(VerifyOtpRequestDto request, CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.Repository<AuthOtp>().Entities
            .FirstOrDefaultAsync(x => x.Id == request.OtpId, cancellationToken);

        CheckOtpDto resultDto = new() { Message = "OTP is not correct." };
        if (ValidateOtp(entity, request))
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (!string.IsNullOrWhiteSpace(entity.PhoneNumber))
            {
                user.PhoneNumberConfirmed = true;
            }
            else
            {
                user.EmailConfirmed = true;
            }

            resultDto.Success = true;
            resultDto.Message = "OTP is correct.";

            await _unitOfWork.Repository<AuthOtp>().DeleteAsync(entity);
            await _unitOfWork.SaveAsync(cancellationToken);

            return await Result<CheckOtpDto>.SuccessAsync(resultDto);
        }

        return await Result<CheckOtpDto>.FailureAsync(resultDto, System.Net.HttpStatusCode.BadRequest);
    }

    public AuthenticationProperties GoogleLogin()
    {
        var provider = "Google";
        var redirectUrl = "/api/Auth/CallBack";

        return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    }

    public async Task<IResult<UserDto>> GoogleLoginCallbackAsync(string remoteError, CancellationToken cancellationToken)
    {
        if (remoteError != null)
        {
            return await Result<UserDto>.FailureAsync(remoteError, System.Net.HttpStatusCode.BadRequest);
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return await Result<UserDto>.FailureAsync("Google authentication failed", System.Net.HttpStatusCode.BadRequest);
        }
        var newUser = new ApplicationUser
        {
            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
            PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedDate = DateTime.UtcNow,
        };

        var res = await _userManager.CreateAsync(newUser);
        if (!res.Succeeded)
        {
            return await Result<UserDto>.FailureAsync(res.Errors.Select(x => x.Code + ":" + x.Description).ToList(), System.Net.HttpStatusCode.BadRequest);
        }

        return await Result<UserDto>.SuccessAsync(new UserDto()
        {
            UserId = newUser.Id,
            UserName = newUser.Email,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
        });
    }

    public async Task<IResult<TokenModel>> RefreshTokenAsync(TokenModel tokenModel, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tokenModel.AccessToken) || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
        {
            return await Result<TokenModel>.FailureAsync("Empty token!", System.Net.HttpStatusCode.BadRequest);
        }
        var userId = ReadUserId(tokenModel.AccessToken);
        if (userId.Id is null)
        {
            return await Result<TokenModel>.FailureAsync(userId.Message, System.Net.HttpStatusCode.BadRequest);
        }
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId.Id && x.RefreshToken == tokenModel.RefreshToken && x.RefreshTokenExpiryTime > DateTime.UtcNow, cancellationToken);

        if (user is null)
        {
            return await Result<TokenModel>.FailureAsync("Refresh token not found!", System.Net.HttpStatusCode.BadRequest);
        }

        var accessToken = GenerateJwtToken(user);

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(ApplicationConstants.RefreshTokenExpirationDays);

        await _userManager.UpdateAsync(user);

        return await Result<TokenModel>.SuccessAsync(new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        });
    }

    private (Guid? Id, string Message) ReadUserId(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JsonWebTokenKeys:IssuerSigningKey"]);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, null, out Guid id))
        {
            return (null, "Invalid access token!");
        }

        return (id, null);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JsonWebTokenKeys:IssuerSigningKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, ApplicationConstants.PatientRoleName),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JsonWebTokenKeys:ValidIssuer"],
            audience: _configuration["JsonWebTokenKeys:ValidAudience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JsonWebTokenKeys:ValidityMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[ApplicationConstants.RefreshTokenLength * 3 / 4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> SendOtpAsync(string email, string phoneNumber, int otp, CancellationToken cancellationToken)
    {
        string res = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailRequest = new EmailRequestDto()
                {
                    To = email,
                    Subject = "Account Activation",
                    Body = @"Dear,<br />There was an action performed from LightClinic. So, please use the below OTP to complete your request. <br />OTP: " + otp,
                };
                res = await _notificationApi.CreateEmailAsync(emailRequest, cancellationToken);
            }
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                var smsRequest = new SmsRequestDto()
                {
                    To = phoneNumber,
                    Message = $"Dear, please use OTP to proceed: {otp}",
                };
                res = await _notificationApi.CreateSmsAsync(smsRequest, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MethodBase.GetCurrentMethod().Name);
            return "Connection failed!";
        }

        if (!string.IsNullOrWhiteSpace(res))
        {
            var json = JsonConvert.DeserializeObject<Result<string>>(res);

            return json.Message;
        }

        return null;
    }

    private static bool ValidateOtp(AuthOtp otp, VerifyOtpRequestDto request)
    {
        return otp is not null &&
            otp.Otp == request.Otp &&
            otp.UserId == request.UserId &&
            otp.ExpiryTime > DateTime.UtcNow;
    }
}
