# Story M012-S05: MFA aktivieren/deaktivieren

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **die Zwei-Faktor-Authentifizierung aktivieren oder deaktivieren koennen**, damit **ich mein Konto zusaetzlich absichern kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Benutzer ohne MFA, wenn er MFA aktivieren will, dann wird ihm ein QR-Code fuer eine Authenticator-App angezeigt
- [ ] Gegeben ein angezeigter QR-Code, wenn der Benutzer einen gueltigen Code eingibt, dann wird MFA aktiviert
- [ ] Gegeben ein Benutzer mit aktivem MFA, wenn er MFA deaktivieren will, dann muss er einen gueltigen Code und sein Passwort eingeben
- [ ] Gegeben erfolgreiche MFA-Aktivierung, wenn der Benutzer sich das naechste Mal anmeldet, dann wird ein Code abgefragt
- [ ] Gegeben MFA-Setup, wenn der Benutzer den QR-Code nicht scannen kann, dann wird ein manueller Schluessel angezeigt

## UI-Entwurf

### MFA-Status (Deaktiviert)
```
+------------------------------------+
|  <- Zurueck   Zwei-Faktor-Auth     |
+------------------------------------+
|                                    |
|  Status: Deaktiviert               |
|                                    |
|  Zwei-Faktor-Authentifizierung     |
|  erhoehe die Sicherheit deines     |
|  Kontos durch einen zusaetzlichen  |
|  Code bei der Anmeldung.           |
|                                    |
|  +--------------------------------+|
|  |   [Shield] MFA aktivieren      ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### MFA-Setup
```
+------------------------------------+
|  <- Zurueck    MFA einrichten      |
+------------------------------------+
|                                    |
|  Schritt 1: App installieren       |
|  Installiere eine Authenticator-   |
|  App wie Google Authenticator      |
|  oder Authy.                       |
|                                    |
|  Schritt 2: QR-Code scannen        |
|  +--------------------------------+|
|  |                                ||
|  |       [ QR-CODE ]              ||
|  |                                ||
|  +--------------------------------+|
|                                    |
|  Manueller Schluessel:             |
|  JBSW Y3DP EHPK 3PXP              |
|  [Kopieren]                        |
|                                    |
|  Schritt 3: Code eingeben          |
|  +--------------------------------+|
|  |         123456                 ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |      [Check] Aktivieren        ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### MFA-Status (Aktiviert)
```
+------------------------------------+
|  <- Zurueck   Zwei-Faktor-Auth     |
+------------------------------------+
|                                    |
|  [Shield-Check]                    |
|  Status: Aktiviert                 |
|                                    |
|  Dein Konto ist durch Zwei-Faktor- |
|  Authentifizierung geschuetzt.     |
|                                    |
|  +--------------------------------+|
|  |   [X] MFA deaktivieren         ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### MFA Deaktivieren
```
+------------------------------------+
|  <- Zurueck   MFA deaktivieren     |
+------------------------------------+
|                                    |
|  [Warning] Sicherheitshinweis      |
|  Das Deaktivieren von MFA macht    |
|  dein Konto weniger sicher.        |
|                                    |
|  Authentifizierungs-Code           |
|  +--------------------------------+|
|  |         123456                 ||
|  +--------------------------------+|
|                                    |
|  Passwort                          |
|  +--------------------------------+|
|  | ************             [Eye] ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |      [X] Deaktivieren          ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkte

### MFA-Setup starten
```
POST /api/users/me/mfa/setup
Authorization: Bearer {token}

Response 200:
{
  "secret": "JBSWY3DPEHPK3PXP",
  "qrCodeUrl": "otpauth://totp/TaschengeldManager:max@example.com?secret=JBSWY3DPEHPK3PXP&issuer=TaschengeldManager",
  "qrCodeImage": "data:image/png;base64,..."
}
```

### MFA aktivieren
```
POST /api/users/me/mfa/enable
Authorization: Bearer {token}
Content-Type: application/json

{
  "code": "123456"
}

Response 200:
{
  "success": true,
  "backupCodes": ["12345678", "23456789", "34567890"]
}

Response 400:
{
  "error": "Ungueltiger Code"
}
```

### MFA deaktivieren
```
POST /api/users/me/mfa/disable
Authorization: Bearer {token}
Content-Type: application/json

{
  "code": "123456",
  "password": "passwort"
}

Response 200:
{
  "success": true
}

Response 400:
{
  "error": "Ungueltiger Code oder Passwort"
}
```

## Technische Notizen

- TOTP (Time-based One-Time Password) mit 6 Ziffern
- Backup-Codes: 8 Stueck, einmal verwendbar
- QR-Code: Generiert mit QRCoder-Bibliothek
- Secret: Base32-kodiert, 16 Zeichen

## Implementierungshinweise

```csharp
public partial class MfaSettingsViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool _isMfaEnabled;

    [ObservableProperty]
    private string _qrCodeImage = string.Empty;

    [ObservableProperty]
    private string _manualKey = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnableMfaCommand))]
    private string _verificationCode = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private List<string> _backupCodes = new();

    private bool CanEnableMfa => VerificationCode.Length == 6;

    [RelayCommand]
    private async Task LoadStatusAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        IsMfaEnabled = user.MfaEnabled;
    }

    [RelayCommand]
    private async Task StartSetupAsync()
    {
        var setup = await _authService.StartMfaSetupAsync();

        QrCodeImage = setup.QrCodeImage;
        ManualKey = FormatManualKey(setup.Secret);

        await Shell.Current.GoToAsync("mfaSetup");
    }

    [RelayCommand(CanExecute = nameof(CanEnableMfa))]
    private async Task EnableMfaAsync()
    {
        IsBusy = true;

        try
        {
            var result = await _authService.EnableMfaAsync(VerificationCode);
            BackupCodes = result.BackupCodes;

            await Shell.Current.GoToAsync("mfaBackupCodes");
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync("Ungueltiger Code");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DisableMfaAsync()
    {
        IsBusy = true;

        try
        {
            await _authService.DisableMfaAsync(VerificationCode, Password);
            IsMfaEnabled = false;

            await _toastService.ShowSuccessAsync("MFA deaktiviert");
            await Shell.Current.GoToAsync("..");
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync("Ungueltiger Code oder Passwort");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CopyManualKeyAsync()
    {
        await Clipboard.SetTextAsync(ManualKey.Replace(" ", ""));
        await _toastService.ShowSuccessAsync("Schluessel kopiert");
    }

    private string FormatManualKey(string key)
    {
        // "JBSWY3DPEHPK3PXP" -> "JBSW Y3DP EHPK 3PXP"
        return string.Join(" ", Enumerable.Range(0, key.Length / 4)
            .Select(i => key.Substring(i * 4, 4)));
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Setup starten | QR-Code und Schluessel werden angezeigt |
| TC-002 | Gueltiger Code | MFA wird aktiviert |
| TC-003 | Ungueltiger Code | Fehlermeldung |
| TC-004 | Backup-Codes anzeigen | 8 Codes werden angezeigt |
| TC-005 | MFA deaktivieren mit Code + Passwort | Erfolgreich deaktiviert |
| TC-006 | Falsches Passwort beim Deaktivieren | Fehlermeldung |
| TC-007 | Manuellen Schluessel kopieren | In Zwischenablage |

## Story Points

2

## Prioritaet

Mittel
