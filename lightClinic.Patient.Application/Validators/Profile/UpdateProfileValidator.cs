using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Profile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ApplicationConstants.PhoneNumberRegex)
            .WithMessage("Enter a valid Egyptian phone number.")
            .When(x => x.PhoneNumber is not null);

        RuleFor(x => x.FullName).MaximumLength(ApplicationConstants.MaxNameLength);
        RuleFor(x => x.Weight).InclusiveBetween(ApplicationConstants.MinWeight, ApplicationConstants.MaxWeight);
        RuleFor(x => x.Height).InclusiveBetween(ApplicationConstants.MinHeight, ApplicationConstants.MaxHeight);
        RuleFor(x => x.BloodGroup).IsInEnum();
        RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Address).MaximumLength(ApplicationConstants.MaxAddressLength);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.ProfileImage).MaximumLength(ApplicationConstants.MaxUrlLength);
    }
}