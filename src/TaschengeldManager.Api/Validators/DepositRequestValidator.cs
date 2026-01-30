using FluentValidation;
using TaschengeldManager.Core.DTOs.Account;

namespace TaschengeldManager.Api.Validators;

public class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    public DepositRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Betrag muss größer als 0 sein")
            .LessThanOrEqualTo(10000).WithMessage("Betrag darf maximal 10.000 € betragen");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Beschreibung darf maximal 200 Zeichen lang sein")
            .When(x => x.Description != null);
    }
}
