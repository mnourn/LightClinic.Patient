using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Address;

namespace lightClinic.Patient.Application.Validators.Address;

public class UpdateAddressValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressValidator()
    {
        Include(new CreateAddressValidator());

        RuleFor(x => x.Id).GreaterThan(0);
    }
}