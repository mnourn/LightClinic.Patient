using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace lightClinic.Patient.Shared.Extensions;

public static class ListExtensions
{
    public static Dictionary<string, List<string>> GetDictionary(this List<ValidationFailure> validationFailures)
    {
        Dictionary<string, List<string>> errors = [];
        validationFailures.ForEach(a =>
        {
            if (errors.TryGetValue(a.PropertyName, out List<string> value))
            {
                value.Add(a.ErrorMessage);
            }
            else
            {
                errors.Add(a.PropertyName, [a.ErrorMessage]);
            }
        });

        return errors;
    }

    public static Dictionary<string, List<string>> GetDictionary(this List<IdentityError> validationFailures)
    {
        Dictionary<string, List<string>> errors = [];
        validationFailures.ForEach(a =>
        {
            if (errors.TryGetValue(a.Code, out List<string> value))
            {
                value.Add(a.Description);
            }
            else
            {
                errors.Add(a.Code, [a.Description]);
            }
        });

        return errors;
    }
}
