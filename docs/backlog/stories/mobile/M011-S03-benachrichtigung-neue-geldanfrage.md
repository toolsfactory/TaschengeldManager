# Story M011-S03: Benachrichtigung - Neue Geldanfrage

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Elternteil** moechte ich **eine Push-Benachrichtigung erhalten, wenn mein Kind eine Geldanfrage stellt**, damit **ich zeitnah reagieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind erstellt eine Geldanfrage, wenn die Anfrage gespeichert wird, dann erhalten alle Eltern der Familie eine Push-Benachrichtigung
- [ ] Gegeben eine Push-Benachrichtigung, wenn der Elternteil darauf tippt, dann wird er direkt zur Anfrage-Detail-Seite geleitet
- [ ] Gegeben Benachrichtigungen deaktiviert fuer diesen Typ, wenn eine Anfrage erstellt wird, dann erhaelt der Elternteil keine Push
- [ ] Gegeben mehrere Eltern, wenn eine Anfrage erstellt wird, dann erhalten alle Eltern die Benachrichtigung

## UI-Entwurf

### Push-Benachrichtigung
```
+------------------------------------+
| TaschengeldManager                 |
| Neue Geldanfrage von Max           |
| Max moechte 15,00 EUR fuer         |
| "Neues Videospiel"                 |
+------------------------------------+
```

### In-App nach Tippen
```
Navigation zur: /requests/{requestId}
```

## Technische Notizen

- Push-Typ: `money_request_new`
- Backend sendet Push nach erfolgreicher Anfrage-Erstellung
- Deep-Link: `taschengeld://request/{requestId}`
- Service: `IPushNotificationHandler`

## Implementierungshinweise

### Backend: Push-Versand
```csharp
public class MoneyRequestCreatedHandler : INotificationHandler<MoneyRequestCreatedEvent>
{
    private readonly IPushNotificationSender _pushSender;
    private readonly IUserRepository _userRepository;
    private readonly INotificationSettingsRepository _settingsRepository;

    public async Task Handle(MoneyRequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Alle Eltern der Familie holen
        var parents = await _userRepository.GetParentsByFamilyIdAsync(notification.FamilyId);

        foreach (var parent in parents)
        {
            // Pruefen ob Benachrichtigung aktiviert
            var settings = await _settingsRepository.GetAsync(parent.Id);
            if (!settings.MoneyRequestNotificationsEnabled) continue;

            await _pushSender.SendAsync(new PushNotification
            {
                UserId = parent.Id,
                Type = "money_request_new",
                Title = $"Neue Geldanfrage von {notification.ChildName}",
                Body = $"{notification.ChildName} moechte {notification.Amount:C} fuer \"{notification.Reason}\"",
                Data = new Dictionary<string, string>
                {
                    ["requestId"] = notification.RequestId.ToString(),
                    ["type"] = "money_request_new"
                }
            });
        }
    }
}

// Push Sender Service
public class FirebasePushSender : IPushNotificationSender
{
    private readonly FirebaseMessaging _messaging;
    private readonly IPushTokenRepository _tokenRepository;

    public async Task SendAsync(PushNotification notification)
    {
        var tokens = await _tokenRepository.GetTokensByUserIdAsync(notification.UserId);

        foreach (var token in tokens)
        {
            var message = new Message
            {
                Token = token.Token,
                Notification = new Notification
                {
                    Title = notification.Title,
                    Body = notification.Body
                },
                Data = notification.Data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        ClickAction = "OPEN_REQUEST",
                        ChannelId = "money_requests"
                    }
                }
            };

            await _messaging.SendAsync(message);
        }
    }
}
```

### Mobile: Push-Handler
```csharp
public class PushNotificationHandler : IPushNotificationHandler
{
    public PushNotificationHandler(IPushNotificationService pushService)
    {
        pushService.NotificationReceived += OnNotificationReceived;
    }

    private async void OnNotificationReceived(object? sender, PushNotificationReceivedEventArgs e)
    {
        if (e.Data == null) return;

        var type = e.Data.GetValueOrDefault("type");

        switch (type)
        {
            case "money_request_new":
                await HandleMoneyRequestNewAsync(e.Data);
                break;
            // ... weitere Typen
        }
    }

    private async Task HandleMoneyRequestNewAsync(IDictionary<string, string> data)
    {
        if (data.TryGetValue("requestId", out var requestId))
        {
            await Shell.Current.GoToAsync($"//requests/detail?id={requestId}");
        }
    }
}

// Deep-Link Registrierung (Android)
// AndroidManifest.xml
<activity android:name=".MainActivity">
    <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="taschengeld" android:host="request" />
    </intent-filter>
</activity>
```

## Payload-Struktur

```json
{
  "notification": {
    "title": "Neue Geldanfrage von Max",
    "body": "Max moechte 15,00 EUR fuer \"Neues Videospiel\""
  },
  "data": {
    "type": "money_request_new",
    "requestId": "guid",
    "childName": "Max",
    "amount": "15.00",
    "reason": "Neues Videospiel"
  },
  "android": {
    "priority": "high",
    "notification": {
      "channel_id": "money_requests",
      "click_action": "OPEN_REQUEST"
    }
  }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Kind erstellt Anfrage | Eltern erhalten Push |
| TC-002 | Auf Push tippen | Navigation zur Anfrage |
| TC-003 | Benachrichtigung deaktiviert | Keine Push |
| TC-004 | 2 Eltern in Familie | Beide erhalten Push |
| TC-005 | App im Vordergrund | In-App Handling |

## Story Points

1

## Prioritaet

Hoch
