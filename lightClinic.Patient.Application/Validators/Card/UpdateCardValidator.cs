using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Card;

namespace lightClinic.Patient.Application.Validators.Card;

public class UpdateCardValidator : AbstractValidator<UpdateCardDto>
{
    public UpdateCardValidator()
    {
        Include(new CreateCardValidator());

        RuleFor(x => x.Id).GreaterThan(0);
    }
}