# Story M011-S05: Benachrichtigung - Neues Geschenk

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Kind** moechte ich **eine Push-Benachrichtigung erhalten, wenn ich ein Geschenk von einem Verwandten bekomme**, damit **ich mich sofort darueber freuen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Verwandter sendet ein Geschenk, wenn das Geschenk verarbeitet wird, dann erhaelt das Kind eine Push-Benachrichtigung
- [ ] Gegeben ein Geschenk mit persoenlicher Nachricht, wenn die Push angezeigt wird, dann ist die Nachricht sichtbar
- [ ] Gegeben eine Push-Benachrichtigung, wenn das Kind darauf tippt, dann wird es zur Geschenk-Detail-Seite geleitet
- [ ] Gegeben Benachrichtigungen deaktiviert, wenn ein Geschenk eingeht, dann erhaelt das Kind keine Push

## UI-Entwurf

### Push-Benachrichtigung
```
+------------------------------------+
| TaschengeldManager                 |
| Geschenk von Oma Maria!            |
| Du hast 20,00 EUR erhalten:        |
| "Alles Gute zum Geburtstag!"       |
+------------------------------------+
```

### Geschenk-Detail (nach Tippen)
```
+------------------------------------+
|  <- Zurueck       Geschenk         |
+------------------------------------+
|                                    |
|         [Gift-Icon]                |
|                                    |
|  Geschenk von Oma Maria            |
|                                    |
|  +--------------------------------+|
|  |        20,00 EUR               ||
|  +--------------------------------+|
|                                    |
|  "Alles Gute zum Geburtstag,       |
|   mein Schatz!"                    |
|                                    |
|  Erhalten am: 15.01.2026, 14:30    |
|                                    |
|  +--------------------------------+|
|  |    [Heart] Danke sagen         ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## Technische Notizen

- Push-Typ: `gift_received`
- Deep-Link: `taschengeld://gift/{giftId}`
- Zeigt auch Eltern an (optional, je nach Einstellung)

## Implementierungshinweise

### Backend: Push-Versand
```csharp
public class GiftSentHandler : INotificationHandler<GiftSentEvent>
{
    private readonly IPushNotificationSender _pushSender;
    private readonly INotificationSettingsRepository _settingsRepository;
    private readonly IUserRepository _userRepository;

    public async Task Handle(GiftSentEvent notification, CancellationToken cancellationToken)
    {
        // Push an Kind
        var childSettings = await _settingsRepository.GetAsync(notification.ChildId);
        if (childSettings.GiftNotificationsEnabled)
        {
            await _pushSender.SendAsync(new PushNotification
            {
                UserId = notification.ChildId,
                Type = "gift_received",
                Title = $"Geschenk von {notification.SenderName}!",
                Body = string.IsNullOrEmpty(notification.Message)
                    ? $"Du hast {notification.Amount:C} erhalten."
                    : $"Du hast {notification.Amount:C} erhalten: \"{TruncateMessage(notification.Message, 50)}\"",
                Data = new Dictionary<string, string>
                {
                    ["giftId"] = notification.GiftId.ToString(),
                    ["type"] = "gift_received",
                    ["senderName"] = notification.SenderName,
                    ["amount"] = notification.Amount.ToString("F2")
                }
            });
        }

        // Optional: Push an Eltern
        var parents = await _userRepository.GetParentsByFamilyIdAsync(notification.FamilyId);
        foreach (var parent in parents)
        {
            var parentSettings = await _settingsRepository.GetAsync(parent.Id);
            if (parentSettings.ChildGiftNotificationsEnabled)
            {
                await _pushSender.SendAsync(new PushNotification
                {
                    UserId = parent.Id,
                    Type = "child_gift_received",
                    Title = $"{notification.ChildName} hat ein Geschenk erhalten",
                    Body = $"{notification.SenderName} hat {notification.Amount:C} geschenkt.",
                    Data = new Dictionary<string, string>
                    {
                        ["giftId"] = notification.GiftId.ToString(),
                        ["type"] = "child_gift_received"
                    }
                });
            }
        }
    }

    private string TruncateMessage(string message, int maxLength)
    {
        if (message.Length <= maxLength) return message;
        return message.Substring(0, maxLength - 3) + "...";
    }
}
```

### Mobile: Push-Handler
```csharp
public partial class PushNotificationHandler
{
    private async Task HandleGiftReceivedAsync(IDictionary<string, string> data)
    {
        if (data.TryGetValue("giftId", out var giftId))
        {
            await Shell.Current.GoToAsync($"//gifts/detail?id={giftId}");
        }
    }
}
```

## Payload-Struktur

```json
{
  "notification": {
    "title": "Geschenk von Oma Maria!",
    "body": "Du hast 20,00 EUR erhalten: \"Alles Gute zum Geburtstag!\""
  },
  "data": {
    "type": "gift_received",
    "giftId": "guid",
    "senderName": "Oma Maria",
    "amount": "20.00",
    "message": "Alles Gute zum Geburtstag, mein Schatz!"
  },
  "android": {
    "priority": "high",
    "notification": {
      "channel_id": "gifts",
      "icon": "ic_gift"
    }
  }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Geschenk mit Nachricht | Push zeigt Nachricht |
| TC-002 | Geschenk ohne Nachricht | Push zeigt nur Betrag |
| TC-003 | Lange Nachricht (>50 Zeichen) | Nachricht wird gekuerzt |
| TC-004 | Auf Push tippen | Navigation zur Geschenk-Detail |
| TC-005 | Benachrichtigung deaktiviert | Keine Push |
| TC-006 | Eltern-Benachrichtigung aktiviert | Eltern erhalten auch Push |

## Story Points

1

## Prioritaet

Mittel
