using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Card;
using lightClinic.Patient.Shared;

namespace lightClinic.Patient.Application.Validators.Card;

public class CreateCardValidator : AbstractValidator<CreateCardDto>
{
    public CreateCardValidator()
    {
        RuleFor(x => x.CardNumber).GreaterThan(0);
        RuleFor(x => x.CardHolderName).NotEmpty().MaximumLength(ApplicationConstants.MaxNameLength);
        RuleFor(x => x.ExpiryDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Card must not be expired");
        RuleFor(x => x.CVVCode).GreaterThan(0).Must(x => x.ToString().Length == 3 || x.ToString().Length == 4).WithMessage("CVV must be 3 or 4 digits");
    }
}