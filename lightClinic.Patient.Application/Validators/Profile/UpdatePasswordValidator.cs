using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Profile;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Profile;

public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordDto>
{
    public UpdatePasswordValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().MaximumLength(ApplicationConstants.MaxPasswordLength);
        RuleFor(x => x.NewPassword).NotEmpty().MaximumLength(ApplicationConstants.MaxPasswordLength);
    }
}
