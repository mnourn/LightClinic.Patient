using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.FamilyMember;

namespace lightClinic.Patient.Application.Validators.FamilyMember;

public class UpdateFamilyMemberValidator : AbstractValidator<UpdateFamilyMemberDto>
{
    public UpdateFamilyMemberValidator()
    {
        Include(new CreateFamilyMemberValidator());

        RuleFor(x => x.Id).NotEmpty();
    }
}