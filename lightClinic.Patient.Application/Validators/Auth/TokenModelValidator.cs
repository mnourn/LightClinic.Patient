using FluentValidation;
using lightClinic.Patient.Contracts.DTOs.Auth;

namespace lightClinic.Patient.Application.Validators.Auth;

public class TokenModelValidator : AbstractValidator<TokenModel>
{
    public TokenModelValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage("AccessToken is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("RefreshToken is required.");
    }
}

