using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Auth;

public class ForgetPasswordRequestDtoValidator : AbstractValidator<ForgetPasswordRequestDto>
{
    public ForgetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email address.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(ApplicationConstants.PhoneNumberRegex)
            .WithMessage("Enter a valid Egyptian phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Either Email or PhoneNumber must be provided.");
    }
}

