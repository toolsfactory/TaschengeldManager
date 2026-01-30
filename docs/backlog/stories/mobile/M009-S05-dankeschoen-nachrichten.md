# Story M009-S05: Dankeschoen-Nachrichten empfangen

## Epic

M009 - Geschenke (Verwandten-Rolle)

## User Story

Als **Verwandter** moechte ich **Dankeschoen-Nachrichten von Kindern empfangen koennen**, damit **ich weiss, dass mein Geschenk angekommen ist und sich das Kind gefreut hat**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind, das ein Geschenk erhalten hat, wenn es eine Dankeschoen-Nachricht sendet, dann erhaelt der Verwandte diese
- [ ] Gegeben eine neue Dankeschoen-Nachricht, wenn der Verwandte die App oeffnet, dann sieht er diese im Benachrichtigungs-Center
- [ ] Gegeben Push-Benachrichtigungen aktiviert, wenn eine Dankeschoen-Nachricht eingeht, dann erhaelt der Verwandte eine Push
- [ ] Gegeben eine Dankeschoen-Nachricht, wenn der Verwandte sie oeffnet, dann sieht er Absender, Nachricht und das zugehoerige Geschenk

## UI-Entwurf

### Push-Benachrichtigung
```
+------------------------------------+
| TaschengeldManager                 |
| Max sagt Danke fuer dein Geschenk! |
| "Danke Oma, ich habe mich riesig   |
|  gefreut!"                         |
+------------------------------------+
```

### Dankeschoen-Detail
```
+------------------------------------+
|  <- Zurueck    Dankeschoen         |
+------------------------------------+
|                                    |
|         [Heart-Icon]               |
|                                    |
|  Max sagt Danke!                   |
|                                    |
|  +--------------------------------+|
|  |                                ||
|  | "Danke Oma, ich habe mich      ||
|  |  riesig gefreut! Das Spiel     ||
|  |  habe ich mir schon gekauft!"  ||
|  |                                ||
|  +--------------------------------+|
|                                    |
|  Fuer dein Geschenk vom 15.01.2026 |
|  20,00 EUR - "Zum Geburtstag!"     |
|                                    |
|  Empfangen: 16.01.2026, 14:30 Uhr  |
|                                    |
+------------------------------------+
```

### Kind-Perspektive: Danke senden
```
+------------------------------------+
|  <- Zurueck    Danke sagen         |
+------------------------------------+
|                                    |
|  Geschenk von Oma Maria            |
|  20,00 EUR                         |
|  "Zum Geburtstag!"                 |
|                                    |
|  Moechtest du Danke sagen?         |
|                                    |
|  +--------------------------------+|
|  |                                ||
|  | Danke Oma, ich habe mich       ||
|  | riesig gefreut!                ||
|  |                                ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |    [Heart] Danke senden        ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Kind sendet Danke
```
POST /api/gifts/{giftId}/thanks
Authorization: Bearer {token}
Content-Type: application/json

{
  "message": "Danke Oma, ich habe mich riesig gefreut!"
}

Response 201:
{
  "id": "guid",
  "giftId": "guid",
  "message": "Danke Oma, ich habe mich riesig gefreut!",
  "createdAt": "2026-01-16T14:30:00Z"
}
```

### Verwandter ruft Nachrichten ab
```
GET /api/thanks-messages
Authorization: Bearer {token}

Response 200:
{
  "messages": [
    {
      "id": "guid",
      "childName": "Max",
      "childAvatarUrl": "string | null",
      "message": "Danke Oma, ich habe mich riesig gefreut!",
      "createdAt": "2026-01-16T14:30:00Z",
      "gift": {
        "id": "guid",
        "amount": 20.00,
        "message": "Zum Geburtstag!",
        "createdAt": "2026-01-15T10:00:00Z"
      },
      "isRead": false
    }
  ]
}
```

## Technische Notizen

- ViewModel (Kind): `SendThanksViewModel`
- ViewModel (Verwandter): `ThanksMessagesViewModel`
- Service: `IThanksService.SendThanksAsync()`, `IThanksService.GetMessagesAsync()`
- Push-Benachrichtigung an Verwandten bei neuer Nachricht
- Nachricht max. 500 Zeichen

## Implementierungshinweise

```csharp
// Kind sendet Danke
public partial class SendThanksViewModel : BaseViewModel
{
    [ObservableProperty]
    private GiftDto _gift;

    [ObservableProperty]
    private string _message = string.Empty;

    [RelayCommand]
    private async Task SendThanksAsync()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            await _toastService.ShowErrorAsync("Bitte schreibe eine Nachricht");
            return;
        }

        await _thanksService.SendThanksAsync(Gift.Id, Message);
        await _toastService.ShowSuccessAsync("Dankeschoen gesendet!");
        await Shell.Current.GoToAsync("..");
    }
}

// Verwandter empfaengt Nachrichten
public partial class ThanksMessagesViewModel : BaseViewModel
{
    public ObservableCollection<ThanksMessage> Messages { get; } = new();

    [ObservableProperty]
    private int _unreadCount;

    [RelayCommand]
    private async Task LoadMessagesAsync()
    {
        var messages = await _thanksService.GetMessagesAsync();
        Messages.Clear();
        foreach (var msg in messages)
        {
            Messages.Add(msg);
        }
        UnreadCount = messages.Count(m => !m.IsRead);
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Kind sendet Dankeschoen | Nachricht wird erstellt |
| TC-002 | Leere Nachricht | Validierungsfehler |
| TC-003 | Verwandter oeffnet Nachricht | Als gelesen markiert |
| TC-004 | Push bei neuer Nachricht | Verwandter erhaelt Push |

## Story Points

1

## Prioritaet

Mittel
