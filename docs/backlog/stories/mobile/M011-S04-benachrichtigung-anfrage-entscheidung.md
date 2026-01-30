# Story M011-S04: Benachrichtigung - Anfrage-Entscheidung

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Kind** moechte ich **eine Push-Benachrichtigung erhalten, wenn meine Geldanfrage genehmigt oder abgelehnt wurde**, damit **ich sofort ueber die Entscheidung informiert werde**.

## Akzeptanzkriterien

- [ ] Gegeben eine Geldanfrage wird genehmigt, wenn die Entscheidung gespeichert wird, dann erhaelt das Kind eine Push-Benachrichtigung mit Erfolgsmeldung
- [ ] Gegeben eine Geldanfrage wird abgelehnt, wenn die Entscheidung gespeichert wird, dann erhaelt das Kind eine Push-Benachrichtigung mit Ablehnungsgrund
- [ ] Gegeben eine Push-Benachrichtigung, wenn das Kind darauf tippt, dann wird es zur Transaktionsliste (genehmigt) oder Anfrage-Detail (abgelehnt) geleitet
- [ ] Gegeben Benachrichtigungen deaktiviert, wenn eine Entscheidung getroffen wird, dann erhaelt das Kind keine Push

## UI-Entwurf

### Push-Benachrichtigung: Genehmigt
```
+------------------------------------+
| TaschengeldManager                 |
| Anfrage genehmigt!                 |
| Du hast 15,00 EUR fuer "Neues      |
| Videospiel" erhalten.              |
+------------------------------------+
```

### Push-Benachrichtigung: Abgelehnt
```
+------------------------------------+
| TaschengeldManager                 |
| Anfrage abgelehnt                  |
| Deine Anfrage fuer "Neues          |
| Videospiel" wurde abgelehnt.       |
+------------------------------------+
```

## Technische Notizen

- Push-Typen: `money_request_approved`, `money_request_rejected`
- Bei Genehmigung: Navigation zur Transaktionsliste
- Bei Ablehnung: Navigation zur Anfrage mit Ablehnungsgrund
- Optional: Ablehnungsgrund in Push-Body

## Implementierungshinweise

### Backend: Push-Versand
```csharp
public class MoneyRequestDecidedHandler : INotificationHandler<MoneyRequestDecidedEvent>
{
    private readonly IPushNotificationSender _pushSender;
    private readonly INotificationSettingsRepository _settingsRepository;

    public async Task Handle(MoneyRequestDecidedEvent notification, CancellationToken cancellationToken)
    {
        // Pruefen ob Benachrichtigung aktiviert
        var settings = await _settingsRepository.GetAsync(notification.ChildId);
        if (!settings.RequestDecisionNotificationsEnabled) return;

        var isApproved = notification.Status == RequestStatus.Approved;

        await _pushSender.SendAsync(new PushNotification
        {
            UserId = notification.ChildId,
            Type = isApproved ? "money_request_approved" : "money_request_rejected",
            Title = isApproved ? "Anfrage genehmigt!" : "Anfrage abgelehnt",
            Body = isApproved
                ? $"Du hast {notification.Amount:C} fuer \"{notification.Reason}\" erhalten."
                : $"Deine Anfrage fuer \"{notification.Reason}\" wurde abgelehnt.{(notification.RejectionReason != null ? $" Grund: {notification.RejectionReason}" : "")}",
            Data = new Dictionary<string, string>
            {
                ["requestId"] = notification.RequestId.ToString(),
                ["type"] = isApproved ? "money_request_approved" : "money_request_rejected",
                ["status"] = notification.Status.ToString()
            }
        });
    }
}
```

### Mobile: Push-Handler
```csharp
public partial class PushNotificationHandler
{
    private async Task HandleRequestDecisionAsync(IDictionary<string, string> data)
    {
        var type = data.GetValueOrDefault("type");
        var requestId = data.GetValueOrDefault("requestId");

        if (type == "money_request_approved")
        {
            // Bei Genehmigung zur Transaktionsliste
            await Shell.Current.GoToAsync("//transactions");
        }
        else if (type == "money_request_rejected")
        {
            // Bei Ablehnung zur Anfrage-Detail
            await Shell.Current.GoToAsync($"//requests/detail?id={requestId}");
        }
    }
}
```

## Payload-Strukturen

### Genehmigt
```json
{
  "notification": {
    "title": "Anfrage genehmigt!",
    "body": "Du hast 15,00 EUR fuer \"Neues Videospiel\" erhalten."
  },
  "data": {
    "type": "money_request_approved",
    "requestId": "guid",
    "status": "Approved",
    "amount": "15.00"
  },
  "android": {
    "priority": "high",
    "notification": {
      "channel_id": "request_decisions",
      "icon": "ic_check"
    }
  }
}
```

### Abgelehnt
```json
{
  "notification": {
    "title": "Anfrage abgelehnt",
    "body": "Deine Anfrage fuer \"Neues Videospiel\" wurde abgelehnt. Grund: Zu teuer diesen Monat."
  },
  "data": {
    "type": "money_request_rejected",
    "requestId": "guid",
    "status": "Rejected",
    "rejectionReason": "Zu teuer diesen Monat."
  },
  "android": {
    "priority": "high",
    "notification": {
      "channel_id": "request_decisions",
      "icon": "ic_close"
    }
  }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Anfrage genehmigt | Kind erhaelt Erfolgs-Push |
| TC-002 | Anfrage abgelehnt | Kind erhaelt Ablehnungs-Push |
| TC-003 | Ablehnung mit Grund | Grund wird in Body angezeigt |
| TC-004 | Auf Genehmigungs-Push tippen | Navigation zu Transaktionen |
| TC-005 | Auf Ablehnungs-Push tippen | Navigation zu Anfrage-Detail |
| TC-006 | Benachrichtigung deaktiviert | Keine Push |

## Story Points

1

## Prioritaet

Hoch
