using FluentValidation;
using TaschengeldManager.Core.DTOs.Family;

namespace TaschengeldManager.Api.Validators;

public class AddChildRequestValidator : AbstractValidator<AddChildRequest>
{
    public AddChildRequestValidator()
    {
        RuleFor(x => x.Nickname)
            .NotEmpty().WithMessage("Nickname ist erforderlich")
            .MinimumLength(2).WithMessage("Nickname muss mindestens 2 Zeichen lang sein")
            .MaximumLength(50).WithMessage("Nickname darf maximal 50 Zeichen lang sein");

        RuleFor(x => x.Pin)
            .NotEmpty().WithMessage("PIN ist erforderlich")
            .MinimumLength(4).WithMessage("PIN muss mindestens 4 Zeichen lang sein")
            .MaximumLength(6).WithMessage("PIN darf maximal 6 Zeichen lang sein")
            .Matches(@"^\d+$").WithMessage("PIN darf nur Ziffern enthalten");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Startguthaben darf nicht negativ sein")
            .LessThanOrEqualTo(10000).WithMessage("Startguthaben darf maximal 10.000 € betragen");
    }
}
