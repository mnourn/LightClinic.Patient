using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.FamilyMember;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.FamilyMember;

public class CreateFamilyMemberValidator : AbstractValidator<CreateFamilyMemberDto>
{
    public CreateFamilyMemberValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ApplicationConstants.PhoneNumberRegex)
            .WithMessage("Enter a valid Egyptian phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaxNameLength);

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth must be in the past");

        RuleFor(x => x.Address).MaximumLength(ApplicationConstants.MaxAddressLength);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.FamilyMembership).IsInEnum();
        RuleFor(x => x.Weight).InclusiveBetween(ApplicationConstants.MinWeight, ApplicationConstants.MaxWeight);
        RuleFor(x => x.Height).InclusiveBetween(ApplicationConstants.MinHeight, ApplicationConstants.MaxHeight);
        RuleFor(x => x.BloodGroup).IsInEnum();
        RuleFor(x => x.ProfileImage).MaximumLength(ApplicationConstants.MaxUrlLength);
    }
}