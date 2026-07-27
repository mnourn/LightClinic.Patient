using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Auth;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Profile;

public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ApplicationConstants.PhoneNumberRegex)
            .WithMessage("Enter a valid Egyptian phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Password).NotEmpty().MinimumLength(ApplicationConstants.MinPasswordLength).MaximumLength(ApplicationConstants.MaxPasswordLength);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(ApplicationConstants.MaxNameLength);
        RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Gender).IsInEnum();
    }
}
