using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Address;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Address;

public class CreateAddressValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressValidator()
    {
        RuleFor(x => x.Location).MaximumLength(ApplicationConstants.MaxNameLength);
        RuleFor(x => x.CityId).GreaterThan(0).When(x => x.CityId != null);
        RuleFor(x => x.LocationType).IsInEnum();
        RuleFor(x => x.AptNo).GreaterThan((byte)0);
        RuleFor(x => x.Floor).GreaterThan((byte)0);
        RuleFor(x => x.AdditionalDirections).MaximumLength(ApplicationConstants.MaxNameLength);

        RuleFor(x => x.BuildingName)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaxNameLength);
        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaxNameLength);


        // Both Latitude and Longitude must be either both null or both not null
        RuleFor(x => x)
            .Must(x => (x.Latitude == null && x.Longitude == null) || (x.Latitude != null && x.Longitude != null))
            .WithMessage("Both Latitude and Longitude must be provided together or both must be null.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(ApplicationConstants.PhoneNumberRegex)
            .WithMessage("Enter a valid Egyptian phone number.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}