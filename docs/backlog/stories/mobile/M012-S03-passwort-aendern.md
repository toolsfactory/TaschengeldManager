# Story M012-S03: Passwort aendern

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **mein Passwort aendern koennen**, damit **ich mein Konto sicher halten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er sein Passwort aendern will, dann muss er sein aktuelles Passwort eingeben
- [ ] Gegeben ein korrektes aktuelles Passwort, wenn der Benutzer ein neues Passwort eingibt, dann wird es validiert
- [ ] Gegeben ein neues Passwort, wenn es weniger als 8 Zeichen hat, dann wird ein Fehler angezeigt
- [ ] Gegeben zwei Passwort-Eingaben, wenn sie nicht uebereinstimmen, dann wird ein Fehler angezeigt
- [ ] Gegeben ein erfolgreiches Passwort-Update, wenn es gespeichert wird, dann wird der Benutzer informiert

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck    Passwort aendern    |
+------------------------------------+
|                                    |
|  Aktuelles Passwort                |
|  +--------------------------------+|
|  | ************             [Eye] ||
|  +--------------------------------+|
|                                    |
|  Neues Passwort                    |
|  +--------------------------------+|
|  | ************             [Eye] ||
|  +--------------------------------+|
|  Min. 8 Zeichen                    |
|                                    |
|  Passwort bestaetigen              |
|  +--------------------------------+|
|  | ************             [Eye] ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |     [Save] Passwort aendern    ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Passwort-Staerke Indikator
```
+------------------------------------+
|  Neues Passwort                    |
|  +--------------------------------+|
|  | MyP@ssw0rd!              [Eye] ||
|  +--------------------------------+|
|  [============================]    |
|  Stark                             |
|                                    |
|  [x] Min. 8 Zeichen                |
|  [x] Grossbuchstabe                |
|  [x] Kleinbuchstabe                |
|  [x] Zahl                          |
|  [x] Sonderzeichen                 |
+------------------------------------+
```

## API-Endpunkt

```
PUT /api/users/me/password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "altes-passwort",
  "newPassword": "neues-passwort",
  "confirmPassword": "neues-passwort"
}

Response 200:
{
  "success": true,
  "message": "Passwort erfolgreich geaendert"
}

Response 400:
{
  "errors": {
    "currentPassword": ["Aktuelles Passwort ist falsch"],
    "newPassword": ["Passwort muss mindestens 8 Zeichen haben"]
  }
}

Response 401:
{
  "error": "Unauthorized"
}
```

## Technische Notizen

- ViewModel: `ChangePasswordViewModel`
- Passwort-Validierung: Min. 8 Zeichen (empfohlen: Gross/Klein/Zahl/Sonderzeichen)
- Show/Hide Toggle fuer alle Passwort-Felder
- Nach Erfolg: Optional alle Sessions invalidieren

## Implementierungshinweise

```csharp
public partial class ChangePasswordViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
    private string _currentPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
    [NotifyPropertyChangedFor(nameof(PasswordStrength))]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChangePasswordCommand))]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _showCurrentPassword;

    [ObservableProperty]
    private bool _showNewPassword;

    [ObservableProperty]
    private bool _showConfirmPassword;

    public PasswordStrengthResult PasswordStrength => CalculatePasswordStrength(NewPassword);

    private bool CanChangePassword =>
        !string.IsNullOrEmpty(CurrentPassword) &&
        NewPassword.Length >= 8 &&
        NewPassword == ConfirmPassword;

    [RelayCommand(CanExecute = nameof(CanChangePassword))]
    private async Task ChangePasswordAsync()
    {
        IsBusy = true;

        try
        {
            await _authService.ChangePasswordAsync(
                CurrentPassword,
                NewPassword,
                ConfirmPassword);

            await _toastService.ShowSuccessAsync("Passwort erfolgreich geaendert");
            await Shell.Current.GoToAsync("..");
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
        }
        catch (AuthenticationException)
        {
            await _toastService.ShowErrorAsync("Aktuelles Passwort ist falsch");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private PasswordStrengthResult CalculatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
            return new PasswordStrengthResult { Level = StrengthLevel.None };

        var result = new PasswordStrengthResult
        {
            HasMinLength = password.Length >= 8,
            HasUppercase = password.Any(char.IsUpper),
            HasLowercase = password.Any(char.IsLower),
            HasDigit = password.Any(char.IsDigit),
            HasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c))
        };

        var score = new[] {
            result.HasMinLength,
            result.HasUppercase,
            result.HasLowercase,
            result.HasDigit,
            result.HasSpecialChar
        }.Count(b => b);

        result.Level = score switch
        {
            0 or 1 => StrengthLevel.Weak,
            2 or 3 => StrengthLevel.Medium,
            4 => StrengthLevel.Strong,
            5 => StrengthLevel.VeryStrong,
            _ => StrengthLevel.None
        };

        return result;
    }
}

public class PasswordStrengthResult
{
    public StrengthLevel Level { get; set; }
    public bool HasMinLength { get; set; }
    public bool HasUppercase { get; set; }
    public bool HasLowercase { get; set; }
    public bool HasDigit { get; set; }
    public bool HasSpecialChar { get; set; }

    public string LevelText => Level switch
    {
        StrengthLevel.Weak => "Schwach",
        StrengthLevel.Medium => "Mittel",
        StrengthLevel.Strong => "Stark",
        StrengthLevel.VeryStrong => "Sehr stark",
        _ => string.Empty
    };

    public Color LevelColor => Level switch
    {
        StrengthLevel.Weak => Colors.Red,
        StrengthLevel.Medium => Colors.Orange,
        StrengthLevel.Strong => Colors.Green,
        StrengthLevel.VeryStrong => Colors.DarkGreen,
        _ => Colors.Gray
    };
}

public enum StrengthLevel
{
    None,
    Weak,
    Medium,
    Strong,
    VeryStrong
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Korrektes aktuelles Passwort | Validierung erfolgreich |
| TC-002 | Falsches aktuelles Passwort | Fehlermeldung |
| TC-003 | Neues Passwort < 8 Zeichen | Validierungsfehler |
| TC-004 | Passwoerter stimmen nicht ueberein | Validierungsfehler |
| TC-005 | Starkes Passwort | Staerke-Indikator "Stark" |
| TC-006 | Schwaches Passwort | Staerke-Indikator "Schwach" |
| TC-007 | Erfolgreich geaendert | Toast + Navigation zurueck |

## Story Points

2

## Prioritaet

Hoch
