using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Setting;

namespace lightClinic.Patient.Application.Validators.Setting;

public class UpdateSettingValidator : AbstractValidator<UpdateSettingDto>
{
    public UpdateSettingValidator()
    {
        RuleFor(x => x.Language).IsInEnum();
    }
}