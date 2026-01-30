# Story M009-S06: Wiederkehrendes Geschenk

## Epic

M009 - Geschenke (Verwandten-Rolle)

## User Story

Als **Verwandter** moechte ich **ein wiederkehrendes Geschenk einrichten koennen (z.B. zum Geburtstag)**, damit **das Kind automatisch zu bestimmten Anlaessen ein Geschenk von mir erhaelt**.

## Akzeptanzkriterien

- [ ] Gegeben ein Verwandter beim Geschenk-senden, wenn er "Wiederkehrend" aktiviert, dann kann er einen Rhythmus waehlen
- [ ] Gegeben ein Rhythmus "Jaehrlich zum Geburtstag", wenn der Geburtstag des Kindes kommt, dann wird das Geschenk automatisch gesendet
- [ ] Gegeben ein wiederkehrendes Geschenk, wenn der Verwandte seine Einstellungen oeffnet, dann sieht er alle aktiven wiederkehrenden Geschenke
- [ ] Gegeben ein aktives wiederkehrendes Geschenk, wenn der Verwandte es bearbeitet, dann kann er Betrag, Nachricht oder Rhythmus aendern
- [ ] Gegeben ein aktives wiederkehrendes Geschenk, wenn der Verwandte es deaktiviert, dann wird es nicht mehr automatisch gesendet

## UI-Entwurf

### Geschenk einrichten
```
+------------------------------------+
|  <- Zurueck    Geschenk an Max     |
+------------------------------------+
|                                    |
|  Betrag                            |
|  +--------------------------------+|
|  |          50,00 EUR             ||
|  +--------------------------------+|
|                                    |
|  Persoenliche Nachricht            |
|  +--------------------------------+|
|  | Alles Gute zum Geburtstag!     ||
|  +--------------------------------+|
|                                    |
|  Wiederkehrend                     |
|  +--------------------------------+|
|  | [Toggle: ON]                   ||
|  |                                ||
|  | Rhythmus:                      ||
|  | ( ) Einmalig                   ||
|  | (x) Jaehrlich zum Geburtstag   ||
|  | ( ) Jaehrlich zu Weihnachten   ||
|  | ( ) Monatlich                  ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |   [Gift] Geschenk einrichten   ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

### Wiederkehrende Geschenke verwalten
```
+------------------------------------+
|  <- Zurueck   Wiederkehrende       |
|               Geschenke            |
+------------------------------------+
|                                    |
|  Aktive wiederkehrende Geschenke   |
|                                    |
|  +--------------------------------+|
|  | Max - Geburtstag               ||
|  | 50,00 EUR jaehrlich            ||
|  | Naechstes: 25.01.2026          ||
|  | [Bearbeiten] [Deaktivieren]    ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | Lisa - Weihnachten             ||
|  | 30,00 EUR jaehrlich            ||
|  | Naechstes: 24.12.2026          ||
|  | [Bearbeiten] [Deaktivieren]    ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |   [+] Neues wiederkehrendes    ||
|  |       Geschenk                 ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Wiederkehrendes Geschenk erstellen
```
POST /api/recurring-gifts
Authorization: Bearer {token}
Content-Type: application/json

{
  "childId": "guid",
  "amount": 50.00,
  "message": "Alles Gute zum Geburtstag!",
  "schedule": {
    "type": "birthday" | "christmas" | "monthly" | "custom",
    "customDate": "2026-01-25" // nur bei custom
  }
}

Response 201:
{
  "id": "guid",
  "childId": "guid",
  "amount": 50.00,
  "message": "Alles Gute zum Geburtstag!",
  "schedule": {
    "type": "birthday",
    "nextExecution": "2026-01-25"
  },
  "isActive": true,
  "createdAt": "2026-01-20T10:00:00Z"
}
```

### Wiederkehrende Geschenke abrufen
```
GET /api/recurring-gifts
Authorization: Bearer {token}

Response 200:
{
  "recurringGifts": [
    {
      "id": "guid",
      "childId": "guid",
      "childName": "Max",
      "amount": 50.00,
      "message": "Alles Gute zum Geburtstag!",
      "schedule": {
        "type": "birthday",
        "nextExecution": "2026-01-25"
      },
      "isActive": true,
      "lastExecuted": "2025-01-25T00:00:00Z"
    }
  ]
}
```

### Wiederkehrendes Geschenk bearbeiten
```
PUT /api/recurring-gifts/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "amount": 60.00,
  "message": "Neuer Text",
  "isActive": true
}
```

### Wiederkehrendes Geschenk deaktivieren
```
DELETE /api/recurring-gifts/{id}
Authorization: Bearer {token}

Response 204 No Content
```

## Technische Notizen

- ViewModel: `RecurringGiftViewModel`, `ManageRecurringGiftsViewModel`
- Service: `IRecurringGiftService`
- Backend-Job: `RecurringGiftExecutionJob` laeuft taeglich
- Schedule-Typen:
  - `birthday`: Am Geburtstag des Kindes
  - `christmas`: Am 24.12.
  - `monthly`: Am 1. jedes Monats
  - `custom`: An einem festen Datum

## Implementierungshinweise

```csharp
public enum RecurringScheduleType
{
    Birthday,
    Christmas,
    Monthly,
    Custom
}

public partial class RecurringGiftViewModel : BaseViewModel
{
    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private bool _isRecurring;

    [ObservableProperty]
    private RecurringScheduleType _scheduleType = RecurringScheduleType.Birthday;

    [ObservableProperty]
    private DateTime? _customDate;

    [RelayCommand]
    private async Task CreateRecurringGiftAsync()
    {
        var request = new CreateRecurringGiftRequest
        {
            ChildId = _childId,
            Amount = Amount,
            Message = Message,
            Schedule = new ScheduleDto
            {
                Type = ScheduleType,
                CustomDate = CustomDate
            }
        };

        await _recurringGiftService.CreateAsync(request);
        await _toastService.ShowSuccessAsync("Wiederkehrendes Geschenk eingerichtet!");
    }
}

// Backend Job
public class RecurringGiftExecutionJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dueGifts = await _repository.GetDueRecurringGiftsAsync(DateTime.Today);

        foreach (var recurringGift in dueGifts)
        {
            await _giftService.SendGiftAsync(
                recurringGift.ChildId,
                recurringGift.Amount,
                recurringGift.Message
            );

            recurringGift.LastExecuted = DateTime.Today;
            recurringGift.NextExecution = CalculateNextExecution(recurringGift);
            await _repository.UpdateAsync(recurringGift);
        }
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Jaehrlich zum Geburtstag | Geschenk wird am Geburtstag gesendet |
| TC-002 | Weihnachtsgeschenk | Geschenk wird am 24.12. gesendet |
| TC-003 | Monatlich | Geschenk wird am 1. jedes Monats gesendet |
| TC-004 | Deaktivieren | Kein Geschenk mehr automatisch |
| TC-005 | Betrag aendern | Naechstes Geschenk mit neuem Betrag |
| TC-006 | Custom-Datum | Geschenk wird am gewaehlten Datum gesendet |

## Story Points

3

## Prioritaet

Mittel
