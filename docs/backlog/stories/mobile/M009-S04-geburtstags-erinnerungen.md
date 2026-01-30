# Story M009-S04: Geburtstags-Erinnerungen

## Epic

M009 - Geschenke (Verwandten-Rolle)

## User Story

Als **Verwandter** moechte ich **an die Geburtstage der Kinder erinnert werden**, damit **ich rechtzeitig ein Geschenk senden kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Geburtsdatum, wenn der Geburtstag in den naechsten 7 Tagen ist, dann wird eine Erinnerung angezeigt
- [ ] Gegeben ein anstehender Geburtstag, wenn der Verwandte die App oeffnet, dann sieht er einen Hinweis auf dem Dashboard
- [ ] Gegeben eine Erinnerung, wenn der Verwandte darauf tippt, dann wird er direkt zur Geschenk-senden-Seite geleitet
- [ ] Gegeben Benachrichtigungen aktiviert, wenn ein Geburtstag ansteht, dann erhaelt der Verwandte eine Push-Benachrichtigung
- [ ] Gegeben ein bereits gesendetes Geschenk in dieser Woche, wenn der Geburtstag ansteht, dann wird die Erinnerung angepasst angezeigt

## UI-Entwurf

### Dashboard-Banner
```
+------------------------------------+
|  TaschengeldManager         [Gear] |
+------------------------------------+
|                                    |
|  +--------------------------------+|
|  | [Cake] Geburtstags-Erinnerung  ||
|  |                                ||
|  | Max wird am 25.01. 13 Jahre!   ||
|  |                                ||
|  | [    Jetzt Geschenk senden   ] ||
|  +--------------------------------+|
|                                    |
|  Hallo, Oma Maria!                 |
|  ...                               |
+------------------------------------+
```

### Erinnerungs-Uebersicht
```
+------------------------------------+
|  <- Zurueck       Geburtstage      |
+------------------------------------+
|                                    |
|  Anstehende Geburtstage            |
|                                    |
|  +--------------------------------+|
|  | [Cake] Max                     ||
|  |        25.01. (in 5 Tagen)     ||
|  |        Wird 13 Jahre           ||
|  |        [ Schenken ]            ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Cake] Lisa                    ||
|  |        14.02. (in 25 Tagen)    ||
|  |        Wird 10 Jahre           ||
|  |        [ Schenken ]            ||
|  +--------------------------------+|
|                                    |
|  Vergangene Geburtstage 2026       |
|  +--------------------------------+|
|  | [Check] Tim - 05.01.           ||
|  |         Du hast 30 EUR         ||
|  |         geschenkt              ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkt

```
GET /api/relatives/birthdays
Authorization: Bearer {token}

Response 200:
{
  "upcoming": [
    {
      "childId": "guid",
      "childName": "Max",
      "birthDate": "2013-01-25",
      "upcomingBirthday": "2026-01-25",
      "turnsAge": 13,
      "daysUntil": 5,
      "hasGiftThisYear": false
    }
  ],
  "recent": [
    {
      "childId": "guid",
      "childName": "Tim",
      "birthDate": "2019-01-05",
      "age": 7,
      "giftAmount": 30.00
    }
  ]
}
```

## Technische Notizen

- ViewModel: `BirthdayReminderViewModel`
- Service: `IBirthdayService.GetUpcomingBirthdaysAsync()`
- Push-Benachrichtigung: 7 Tage und 1 Tag vor Geburtstag
- Lokale Berechnung der "Tage bis" Anzeige
- Integration mit M011 (Push-Benachrichtigungen)

## Implementierungshinweise

```csharp
public partial class BirthdayReminderViewModel : BaseViewModel
{
    private readonly IBirthdayService _birthdayService;

    public ObservableCollection<UpcomingBirthday> UpcomingBirthdays { get; } = new();
    public ObservableCollection<RecentBirthday> RecentBirthdays { get; } = new();

    [ObservableProperty]
    private UpcomingBirthday? _nextBirthday;

    [RelayCommand]
    private async Task LoadBirthdaysAsync()
    {
        var result = await _birthdayService.GetUpcomingBirthdaysAsync();

        UpcomingBirthdays.Clear();
        foreach (var birthday in result.Upcoming)
        {
            UpcomingBirthdays.Add(birthday);
        }

        NextBirthday = result.Upcoming.FirstOrDefault();
    }

    [RelayCommand]
    private async Task SendGiftAsync(Guid childId)
    {
        await Shell.Current.GoToAsync($"sendGift?childId={childId}");
    }
}

// Push-Notification Scheduling (Backend)
public class BirthdayNotificationJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var upcomingBirthdays = await GetBirthdaysInNextDays(7);
        foreach (var birthday in upcomingBirthdays)
        {
            await SendReminderToRelatives(birthday);
        }
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Kind hat in 5 Tagen Geburtstag | Banner wird auf Dashboard angezeigt |
| TC-002 | Kind hat in 30 Tagen Geburtstag | Kein Banner, nur in Uebersicht |
| TC-003 | Geburtstag heute | Besondere Anzeige "Heute!" |
| TC-004 | Bereits Geschenk gesendet | Banner zeigt "Du hast bereits geschenkt" |
| TC-005 | Push 7 Tage vorher | Push-Benachrichtigung wird gesendet |

## Story Points

2

## Prioritaet

Mittel
