using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Auth;

public class ResetPasswordRequestDtoValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId is required.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("NewPassword is required.")
            .MinimumLength(ApplicationConstants.MinPasswordLength)
            .MaximumLength(ApplicationConstants.MaxPasswordLength)
            .WithMessage($"Password must be between {ApplicationConstants.MinPasswordLength} and {ApplicationConstants.MaxPasswordLength} characters.");
    }
}

