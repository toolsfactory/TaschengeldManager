using FluentValidation;
using TaschengeldManager.Core.DTOs.Family;

namespace TaschengeldManager.Api.Validators;

/// <summary>
/// Validator for ChangeChildPinRequest.
/// </summary>
public class ChangeChildPinRequestValidator : AbstractValidator<ChangeChildPinRequest>
{
    public ChangeChildPinRequestValidator()
    {
        RuleFor(x => x.NewPin)
            .NotEmpty().WithMessage("PIN ist erforderlich")
            .Length(4, 6).WithMessage("PIN muss zwischen 4 und 6 Zeichen lang sein")
            .Matches(@"^\d+$").WithMessage("PIN darf nur Zahlen enthalten");

        RuleFor(x => x.ParentPassword)
            .NotEmpty().WithMessage("Eltern-Passwort ist erforderlich");
    }
}
