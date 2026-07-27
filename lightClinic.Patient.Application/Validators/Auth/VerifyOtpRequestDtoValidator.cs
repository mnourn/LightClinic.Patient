using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Auth;

namespace lightClinic.Patient.Application.Validators.Auth;

public class VerifyOtpRequestDtoValidator : AbstractValidator<VerifyOtpRequestDto>
{
    public VerifyOtpRequestDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId is required.");

        RuleFor(x => x.OtpId)
            .GreaterThan(0)
            .WithMessage("OtpId must be greater than 0.");

        RuleFor(x => x.Otp)
            .GreaterThan(0)
            .WithMessage("Otp must be greater than 0.");
    }
}

