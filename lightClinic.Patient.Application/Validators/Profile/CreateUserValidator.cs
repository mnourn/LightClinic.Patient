using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Profile;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ApplicationConstants.PhoneNumberRegex)
            .WithMessage("Enter a valid Egyptian phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.FullName).NotEmpty().MaximumLength(ApplicationConstants.MaxNameLength);
        RuleFor(x => x.Gender).IsInEnum();
    }
}
