# Story M012-S06: Account loeschen

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **meinen Account loeschen koennen**, damit **alle meine Daten entfernt werden, wenn ich die App nicht mehr nutzen moechte**.

## Akzeptanzkriterien

- [ ] Gegeben ein Benutzer, der seinen Account loeschen will, wenn er die Option waehlt, dann wird ein Warnhinweis angezeigt
- [ ] Gegeben ein Benutzer moechte loeschen, wenn er sein Passwort korrekt eingibt, dann wird der Account markiert zur Loeschung
- [ ] Gegeben ein zur Loeschung markierter Account, wenn 30 Tage vergehen, dann wird er endgueltig geloescht
- [ ] Gegeben ein zur Loeschung markierter Account, wenn der Benutzer sich innerhalb von 30 Tagen anmeldet, dann kann er die Loeschung abbrechen
- [ ] Gegeben ein Elternteil als einziger Erwachsener, wenn er loeschen will, dann muss zuerst die Familie aufgeloest oder uebergeben werden

## UI-Entwurf

### Account loeschen
```
+------------------------------------+
|  <- Zurueck     Account loeschen   |
+------------------------------------+
|                                    |
|         [Warning-Icon]             |
|                                    |
|  Account loeschen                  |
|                                    |
|  Wenn du deinen Account loeschst:  |
|  - Alle deine Daten werden         |
|    unwiderruflich entfernt         |
|  - Du verlierst Zugang zu deiner   |
|    Familie                         |
|  - Transaktionshistorie wird       |
|    geloescht                       |
|                                    |
|  Die Loeschung erfolgt nach einer  |
|  Karenzzeit von 30 Tagen. Du       |
|  kannst sie bis dahin abbrechen.   |
|                                    |
|  Passwort zur Bestaetigung         |
|  +--------------------------------+|
|  | ************             [Eye] ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Trash] Account loeschen       ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Bestaetigung
```
+------------------------------------+
|        Account loeschen?           |
+------------------------------------+
|                                    |
|  Bist du sicher, dass du deinen    |
|  Account loeschen moechtest?       |
|                                    |
|  Diese Aktion kann innerhalb von   |
|  30 Tagen rueckgaengig gemacht     |
|  werden.                           |
|                                    |
|  +--------------------------------+|
|  |     Ja, Account loeschen       ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |         Abbrechen              ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Loeschung angefordert
```
+------------------------------------+
|  <- Zurueck     Account-Status     |
+------------------------------------+
|                                    |
|         [Clock-Icon]               |
|                                    |
|  Loeschung angefordert             |
|                                    |
|  Dein Account wird am              |
|  19.02.2026 endgueltig geloescht.  |
|                                    |
|  Du kannst die Loeschung bis       |
|  dahin jederzeit abbrechen.        |
|                                    |
|  +--------------------------------+|
|  |   [Undo] Loeschung abbrechen   ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Loeschung anfordern
```
POST /api/users/me/delete
Authorization: Bearer {token}
Content-Type: application/json

{
  "password": "passwort"
}

Response 200:
{
  "success": true,
  "scheduledDeletionDate": "2026-02-19T00:00:00Z",
  "message": "Account wird am 19.02.2026 geloescht"
}

Response 400:
{
  "error": "Du bist der einzige Elternteil. Bitte uebertrage die Familie erst."
}

Response 401:
{
  "error": "Falsches Passwort"
}
```

### Loeschung abbrechen
```
POST /api/users/me/delete/cancel
Authorization: Bearer {token}

Response 200:
{
  "success": true,
  "message": "Loeschung wurde abgebrochen"
}
```

### Loeschungs-Status pruefen
```
GET /api/users/me/delete/status
Authorization: Bearer {token}

Response 200 (keine Loeschung):
{
  "isDeletionScheduled": false
}

Response 200 (Loeschung geplant):
{
  "isDeletionScheduled": true,
  "scheduledDeletionDate": "2026-02-19T00:00:00Z",
  "daysRemaining": 30
}
```

## Technische Notizen

- Soft-Delete: Account wird markiert, nicht sofort geloescht
- Karenzzeit: 30 Tage
- Backend-Job: Fuehrt endgueltige Loeschung durch
- Bei Login waehrend Karenzzeit: Warnung anzeigen
- DSGVO-konform: Alle personenbezogenen Daten werden entfernt

## Implementierungshinweise

```csharp
public partial class DeleteAccountViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteAccountCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isDeletionScheduled;

    [ObservableProperty]
    private DateTime? _scheduledDeletionDate;

    [ObservableProperty]
    private int _daysRemaining;

    private bool CanDeleteAccount => !string.IsNullOrEmpty(Password);

    [RelayCommand]
    private async Task LoadStatusAsync()
    {
        var status = await _userService.GetDeletionStatusAsync();
        IsDeletionScheduled = status.IsDeletionScheduled;
        ScheduledDeletionDate = status.ScheduledDeletionDate;
        DaysRemaining = status.DaysRemaining;
    }

    [RelayCommand(CanExecute = nameof(CanDeleteAccount))]
    private async Task DeleteAccountAsync()
    {
        var confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Account loeschen?",
            "Bist du sicher, dass du deinen Account loeschen moechtest? Diese Aktion kann innerhalb von 30 Tagen rueckgaengig gemacht werden.",
            "Ja, Account loeschen",
            "Abbrechen");

        if (!confirm) return;

        IsBusy = true;

        try
        {
            var result = await _userService.RequestAccountDeletionAsync(Password);

            await _toastService.ShowInfoAsync($"Account wird am {result.ScheduledDeletionDate:dd.MM.yyyy} geloescht");

            IsDeletionScheduled = true;
            ScheduledDeletionDate = result.ScheduledDeletionDate;
            Password = string.Empty;
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
        }
        catch (AuthenticationException)
        {
            await _toastService.ShowErrorAsync("Falsches Passwort");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelDeletionAsync()
    {
        var confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Loeschung abbrechen?",
            "Moechtest du die geplante Account-Loeschung abbrechen?",
            "Ja",
            "Nein");

        if (!confirm) return;

        await _userService.CancelAccountDeletionAsync();
        IsDeletionScheduled = false;
        ScheduledDeletionDate = null;

        await _toastService.ShowSuccessAsync("Loeschung wurde abgebrochen");
    }
}

// Backend: Loeschungs-Job
public class AccountDeletionJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var accountsToDelete = await _userRepository
            .GetAccountsScheduledForDeletionAsync(DateTime.UtcNow);

        foreach (var account in accountsToDelete)
        {
            await DeleteAccountDataAsync(account);
        }
    }

    private async Task DeleteAccountDataAsync(User user)
    {
        // Alle personenbezogenen Daten loeschen
        await _transactionRepository.DeleteByUserIdAsync(user.Id);
        await _requestRepository.DeleteByUserIdAsync(user.Id);
        await _pushTokenRepository.DeleteByUserIdAsync(user.Id);
        await _notificationRepository.DeleteByUserIdAsync(user.Id);

        // Account anonymisieren statt loeschen (fuer Integritaet)
        user.Email = $"deleted_{user.Id}@deleted.local";
        user.FirstName = "Geloescht";
        user.LastName = "Geloescht";
        user.PasswordHash = null;
        user.IsDeleted = true;

        await _userRepository.UpdateAsync(user);
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Korrektes Passwort | Loeschung wird geplant |
| TC-002 | Falsches Passwort | Fehlermeldung |
| TC-003 | Einziger Elternteil | Fehlermeldung mit Hinweis |
| TC-004 | Loeschung abbrechen | Status zurueckgesetzt |
| TC-005 | Login waehrend Karenzzeit | Warnung wird angezeigt |
| TC-006 | Nach 30 Tagen | Daten werden geloescht |

## Story Points

2

## Prioritaet

Niedrig
