# Story M011-S07: Benachrichtigungs-Einstellungen

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Benutzer** moechte ich **einstellen koennen, welche Push-Benachrichtigungen ich erhalten moechte**, damit **ich nur relevante Benachrichtigungen bekomme und nicht gestoert werde**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er die Einstellungen oeffnet, dann sieht er alle Benachrichtigungs-Optionen
- [ ] Gegeben verschiedene Benachrichtigungs-Typen, wenn der Benutzer einen Toggle aendert, dann wird die Einstellung gespeichert
- [ ] Gegeben ein deaktivierter Benachrichtigungs-Typ, wenn ein entsprechendes Ereignis eintritt, dann wird keine Push gesendet
- [ ] Gegeben geaenderte Einstellungen, wenn der Benutzer die Seite erneut oeffnet, dann sind die Einstellungen gespeichert

## UI-Entwurf

### Kind-Einstellungen
```
+------------------------------------+
|  <- Zurueck    Benachrichtigungen  |
+------------------------------------+
|                                    |
|  Push-Benachrichtigungen           |
|  +--------------------------------+|
|  | Alle Benachrichtigungen  [ON] ||
|  +--------------------------------+|
|                                    |
|  Einzelne Kategorien               |
|  +--------------------------------+|
|  | Anfrage genehmigt/      [ON]  ||
|  | abgelehnt                      ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Geschenke erhalten       [ON] ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Taschengeld eingegangen  [ON] ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Zinsen gutgeschrieben    [OFF]||
|  +--------------------------------+|
|                                    |
|  Ruhezeiten                        |
|  +--------------------------------+|
|  | Nicht stoeren           [OFF] ||
|  | 22:00 - 07:00 Uhr              ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Eltern-Einstellungen
```
+------------------------------------+
|  <- Zurueck    Benachrichtigungen  |
+------------------------------------+
|                                    |
|  Push-Benachrichtigungen           |
|  +--------------------------------+|
|  | Alle Benachrichtigungen  [ON] ||
|  +--------------------------------+|
|                                    |
|  Einzelne Kategorien               |
|  +--------------------------------+|
|  | Neue Geldanfragen        [ON] ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Kind hat Geschenk        [ON] ||
|  | erhalten                       ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Niedriger Kontostand     [OFF]||
|  | (Kind < 5 EUR)                 ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Einstellungen abrufen
```
GET /api/notifications/settings
Authorization: Bearer {token}

Response 200 (Kind):
{
  "allEnabled": true,
  "settings": {
    "requestDecision": true,
    "giftReceived": true,
    "allowanceReceived": true,
    "interestCredited": false
  },
  "quietHours": {
    "enabled": false,
    "start": "22:00",
    "end": "07:00"
  }
}

Response 200 (Eltern):
{
  "allEnabled": true,
  "settings": {
    "newMoneyRequest": true,
    "childGiftReceived": true,
    "lowBalance": false,
    "lowBalanceThreshold": 5.00
  },
  "quietHours": {
    "enabled": false,
    "start": "22:00",
    "end": "07:00"
  }
}
```

### Einstellungen speichern
```
PUT /api/notifications/settings
Authorization: Bearer {token}
Content-Type: application/json

{
  "allEnabled": true,
  "settings": {
    "requestDecision": true,
    "giftReceived": true,
    "allowanceReceived": true,
    "interestCredited": false
  },
  "quietHours": {
    "enabled": true,
    "start": "22:00",
    "end": "07:00"
  }
}

Response 200:
{
  "success": true,
  "updatedAt": "2026-01-20T10:00:00Z"
}
```

## Technische Notizen

- Settings werden im Backend gespeichert (nicht nur lokal)
- Ruhezeiten: Backend prueft vor Push-Versand
- "Alle aus" deaktiviert alle Einzeloptionen
- Service: `INotificationSettingsService`

## Implementierungshinweise

```csharp
public class NotificationSettings
{
    public bool AllEnabled { get; set; } = true;

    // Kind-spezifisch
    public bool RequestDecisionEnabled { get; set; } = true;
    public bool GiftReceivedEnabled { get; set; } = true;
    public bool AllowanceReceivedEnabled { get; set; } = true;
    public bool InterestCreditedEnabled { get; set; } = true;

    // Eltern-spezifisch
    public bool NewMoneyRequestEnabled { get; set; } = true;
    public bool ChildGiftReceivedEnabled { get; set; } = true;
    public bool LowBalanceEnabled { get; set; } = false;
    public decimal LowBalanceThreshold { get; set; } = 5.00m;

    // Ruhezeiten
    public bool QuietHoursEnabled { get; set; } = false;
    public TimeSpan QuietHoursStart { get; set; } = new TimeSpan(22, 0, 0);
    public TimeSpan QuietHoursEnd { get; set; } = new TimeSpan(7, 0, 0);
}

// ViewModel
public partial class NotificationSettingsViewModel : BaseViewModel
{
    private readonly INotificationSettingsService _settingsService;

    [ObservableProperty]
    private bool _allEnabled;

    [ObservableProperty]
    private bool _requestDecisionEnabled;

    [ObservableProperty]
    private bool _giftReceivedEnabled;

    // ... weitere Properties ...

    partial void OnAllEnabledChanged(bool value)
    {
        if (!value)
        {
            // Alle deaktivieren
            RequestDecisionEnabled = false;
            GiftReceivedEnabled = false;
            AllowanceReceivedEnabled = false;
            InterestCreditedEnabled = false;
        }
        SaveSettingsAsync().ConfigureAwait(false);
    }

    partial void OnRequestDecisionEnabledChanged(bool value)
    {
        SaveSettingsAsync().ConfigureAwait(false);
    }

    private async Task SaveSettingsAsync()
    {
        var settings = new NotificationSettings
        {
            AllEnabled = AllEnabled,
            RequestDecisionEnabled = RequestDecisionEnabled,
            GiftReceivedEnabled = GiftReceivedEnabled,
            AllowanceReceivedEnabled = AllowanceReceivedEnabled,
            InterestCreditedEnabled = InterestCreditedEnabled,
            QuietHoursEnabled = QuietHoursEnabled,
            QuietHoursStart = QuietHoursStart,
            QuietHoursEnd = QuietHoursEnd
        };

        await _settingsService.SaveSettingsAsync(settings);
    }
}

// Backend: Ruhezeiten-Pruefung
public class PushNotificationSender : IPushNotificationSender
{
    public async Task SendAsync(PushNotification notification)
    {
        var settings = await _settingsRepository.GetAsync(notification.UserId);

        // Ruhezeiten pruefen
        if (settings.QuietHoursEnabled && IsInQuietHours(settings))
        {
            // In Queue fuer spaeter speichern oder verwerfen
            return;
        }

        // Normal senden
        // ...
    }

    private bool IsInQuietHours(NotificationSettings settings)
    {
        var now = DateTime.Now.TimeOfDay;
        var start = settings.QuietHoursStart;
        var end = settings.QuietHoursEnd;

        if (start < end)
        {
            return now >= start && now < end;
        }
        else // Ueber Mitternacht
        {
            return now >= start || now < end;
        }
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Einstellungen laden | Aktuelle Werte werden angezeigt |
| TC-002 | Toggle aendern | Aenderung wird gespeichert |
| TC-003 | "Alle aus" aktivieren | Alle Toggles werden deaktiviert |
| TC-004 | Push bei deaktiviertem Typ | Keine Push wird gesendet |
| TC-005 | Ruhezeit aktiv | Push wird nicht gesendet |
| TC-006 | Einstellungen nach Neustart | Werte bleiben erhalten |

## Story Points

2

## Prioritaet

Mittel
