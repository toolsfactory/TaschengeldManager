# Story M012-S04: Email aendern

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **meine E-Mail-Adresse aendern koennen**, damit **ich erreichbar bleibe, wenn sich meine E-Mail aendert**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er seine E-Mail aendern will, dann muss er sein Passwort bestaetigen
- [ ] Gegeben eine neue E-Mail-Adresse, wenn sie gueltig ist, dann wird ein Verifizierungslink gesendet
- [ ] Gegeben ein Verifizierungslink, wenn der Benutzer darauf klickt, dann wird die neue E-Mail aktiviert
- [ ] Gegeben eine bereits registrierte E-Mail, wenn der Benutzer sie verwenden will, dann wird ein Fehler angezeigt
- [ ] Gegeben eine unbestaetigte neue E-Mail, wenn der Benutzer die App oeffnet, dann sieht er den Pending-Status

## UI-Entwurf

### Email aendern
```
+------------------------------------+
|  <- Zurueck       Email aendern    |
+------------------------------------+
|                                    |
|  Aktuelle Email                    |
|  max@example.com                   |
|                                    |
|  Neue Email                        |
|  +--------------------------------+|
|  | max.neu@example.com            ||
|  +--------------------------------+|
|                                    |
|  Passwort bestaetigen              |
|  +--------------------------------+|
|  | ************             [Eye] ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |   [Mail] Verifizierung senden  ||
|  +--------------------------------+|
|                                    |
|  Nach dem Senden erhaeltst du      |
|  eine Email mit einem Link zur     |
|  Bestaetigung.                     |
|                                    |
+------------------------------------+
```

### Pending-Status
```
+------------------------------------+
|  <- Zurueck       Email aendern    |
+------------------------------------+
|                                    |
|  Aktuelle Email                    |
|  max@example.com                   |
|                                    |
|  +--------------------------------+|
|  | [Clock] Ausstehende Aenderung  ||
|  |                                ||
|  | Neue Email: max.neu@example... ||
|  | Bitte bestaetigen Sie die neue ||
|  | Email-Adresse.                 ||
|  |                                ||
|  | [Link erneut senden]           ||
|  | [Aenderung abbrechen]          ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Email-Aenderung anfordern
```
POST /api/users/me/email/change
Authorization: Bearer {token}
Content-Type: application/json

{
  "newEmail": "max.neu@example.com",
  "password": "aktuelles-passwort"
}

Response 200:
{
  "success": true,
  "message": "Verifizierungslink wurde an die neue Email gesendet",
  "expiresAt": "2026-01-21T10:00:00Z"
}

Response 400:
{
  "errors": {
    "newEmail": ["Email wird bereits verwendet"],
    "password": ["Passwort ist falsch"]
  }
}
```

### Pending-Aenderung pruefen
```
GET /api/users/me/email/pending
Authorization: Bearer {token}

Response 200 (keine ausstehende Aenderung):
{
  "hasPending": false
}

Response 200 (ausstehend):
{
  "hasPending": true,
  "newEmail": "max.neu@example.com",
  "requestedAt": "2026-01-20T10:00:00Z",
  "expiresAt": "2026-01-21T10:00:00Z"
}
```

### Verifizierungslink erneut senden
```
POST /api/users/me/email/resend-verification
Authorization: Bearer {token}

Response 200:
{
  "success": true,
  "message": "Neuer Verifizierungslink wurde gesendet"
}
```

### Aenderung abbrechen
```
DELETE /api/users/me/email/pending
Authorization: Bearer {token}

Response 204 No Content
```

### Email bestaetigen (via Link)
```
POST /api/users/email/verify
Content-Type: application/json

{
  "token": "verification-token"
}

Response 200:
{
  "success": true,
  "email": "max.neu@example.com"
}
```

## Technische Notizen

- ViewModel: `ChangeEmailViewModel`
- Verifizierungstoken: 24 Stunden gueltig
- Deep-Link: `taschengeld://email-verify/{token}`
- Nach Verifizierung: User-Objekt aktualisieren

## Implementierungshinweise

```csharp
public partial class ChangeEmailViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private string _currentEmail = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RequestChangeCommand))]
    private string _newEmail = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RequestChangeCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _hasPendingChange;

    [ObservableProperty]
    private string _pendingEmail = string.Empty;

    [ObservableProperty]
    private DateTime? _pendingExpiresAt;

    private bool CanRequestChange =>
        !string.IsNullOrWhiteSpace(NewEmail) &&
        !string.IsNullOrWhiteSpace(Password) &&
        IsValidEmail(NewEmail);

    [RelayCommand]
    private async Task LoadStatusAsync()
    {
        var user = await _userService.GetCurrentUserAsync();
        CurrentEmail = user.Email;

        var pending = await _userService.GetPendingEmailChangeAsync();
        HasPendingChange = pending.HasPending;
        if (pending.HasPending)
        {
            PendingEmail = pending.NewEmail;
            PendingExpiresAt = pending.ExpiresAt;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRequestChange))]
    private async Task RequestChangeAsync()
    {
        IsBusy = true;

        try
        {
            await _userService.RequestEmailChangeAsync(NewEmail, Password);

            await _toastService.ShowSuccessAsync("Verifizierungslink wurde gesendet");
            HasPendingChange = true;
            PendingEmail = NewEmail;
            NewEmail = string.Empty;
            Password = string.Empty;
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResendVerificationAsync()
    {
        await _userService.ResendEmailVerificationAsync();
        await _toastService.ShowSuccessAsync("Neuer Link wurde gesendet");
    }

    [RelayCommand]
    private async Task CancelChangeAsync()
    {
        var confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Aenderung abbrechen?",
            "Die Email-Aenderung wird abgebrochen.",
            "Ja",
            "Nein");

        if (confirm)
        {
            await _userService.CancelEmailChangeAsync();
            HasPendingChange = false;
            PendingEmail = string.Empty;
            await _toastService.ShowSuccessAsync("Aenderung abgebrochen");
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Gueltige neue Email | Verifizierungslink wird gesendet |
| TC-002 | Ungueltige Email-Format | Validierungsfehler |
| TC-003 | Email bereits registriert | Fehlermeldung |
| TC-004 | Falsches Passwort | Fehlermeldung |
| TC-005 | Link erneut senden | Neuer Link wird gesendet |
| TC-006 | Aenderung abbrechen | Pending-Status entfernt |
| TC-007 | Link klicken | Email wird geaendert |
| TC-008 | Abgelaufener Link | Fehlermeldung |

## Story Points

3

## Prioritaet

Mittel
