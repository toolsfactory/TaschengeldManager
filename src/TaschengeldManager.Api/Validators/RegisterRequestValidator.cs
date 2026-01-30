using FluentValidation;
using TaschengeldManager.Core.DTOs.Auth;

namespace TaschengeldManager.Api.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-Mail ist erforderlich")
            .EmailAddress().WithMessage("Ungültiges E-Mail-Format")
            .MaximumLength(256).WithMessage("E-Mail darf maximal 256 Zeichen lang sein");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Passwort ist erforderlich")
            .MinimumLength(12).WithMessage("Passwort muss mindestens 12 Zeichen lang sein")
            .MaximumLength(128).WithMessage("Passwort darf maximal 128 Zeichen lang sein")
            .Matches("[A-Z]").WithMessage("Passwort muss mindestens einen Großbuchstaben enthalten")
            .Matches("[a-z]").WithMessage("Passwort muss mindestens einen Kleinbuchstaben enthalten")
            .Matches("[0-9]").WithMessage("Passwort muss mindestens eine Ziffer enthalten")
            .Matches("[^a-zA-Z0-9]").WithMessage("Passwort muss mindestens ein Sonderzeichen enthalten");

        RuleFor(x => x.Nickname)
            .NotEmpty().WithMessage("Nickname ist erforderlich")
            .MinimumLength(2).WithMessage("Nickname muss mindestens 2 Zeichen lang sein")
            .MaximumLength(50).WithMessage("Nickname darf maximal 50 Zeichen lang sein");
    }
}
