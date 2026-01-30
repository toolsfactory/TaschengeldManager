# Story M011-S06: Benachrichtigung - Taschengeld eingegangen

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Kind** moechte ich **eine Push-Benachrichtigung erhalten, wenn mein automatisches Taschengeld gutgeschrieben wurde**, damit **ich weiss, dass ich neues Geld habe**.

## Akzeptanzkriterien

- [ ] Gegeben eine automatische Taschengeld-Zahlung, wenn sie ausgefuehrt wird, dann erhaelt das Kind eine Push-Benachrichtigung
- [ ] Gegeben eine Push-Benachrichtigung, wenn das Kind darauf tippt, dann wird es zum Dashboard mit aktuellem Kontostand geleitet
- [ ] Gegeben Benachrichtigungen deaktiviert, wenn Taschengeld eingegangen ist, dann erhaelt das Kind keine Push
- [ ] Gegeben Zinsen werden gutgeschrieben, wenn die Gutschrift erfolgt, dann erhaelt das Kind eine separate Benachrichtigung

## UI-Entwurf

### Push-Benachrichtigung: Taschengeld
```
+------------------------------------+
| TaschengeldManager                 |
| Taschengeld eingegangen!           |
| Dein woechentliches Taschengeld    |
| von 10,00 EUR wurde gutgeschrieben.|
+------------------------------------+
```

### Push-Benachrichtigung: Zinsen
```
+------------------------------------+
| TaschengeldManager                 |
| Zinsen gutgeschrieben              |
| Du hast 0,50 EUR Zinsen fuer       |
| Januar erhalten!                   |
+------------------------------------+
```

## Technische Notizen

- Push-Typen: `allowance_received`, `interest_credited`
- Wird vom Backend-Job ausgeloest (Scheduler)
- Navigation zum Dashboard

## Implementierungshinweise

### Backend: Taschengeld-Job mit Push
```csharp
public class AllowancePaymentJob : IJob
{
    private readonly IAllowanceService _allowanceService;
    private readonly IPushNotificationSender _pushSender;
    private readonly INotificationSettingsRepository _settingsRepository;

    public async Task Execute(IJobExecutionContext context)
    {
        var duePayments = await _allowanceService.GetDuePaymentsAsync();

        foreach (var payment in duePayments)
        {
            // Zahlung ausfuehren
            await _allowanceService.ExecutePaymentAsync(payment);

            // Push senden
            var settings = await _settingsRepository.GetAsync(payment.ChildId);
            if (settings.AllowanceNotificationsEnabled)
            {
                var frequencyText = payment.Frequency switch
                {
                    AllowanceFrequency.Weekly => "woechentliches",
                    AllowanceFrequency.BiWeekly => "zweiwöchentliches",
                    AllowanceFrequency.Monthly => "monatliches",
                    _ => string.Empty
                };

                await _pushSender.SendAsync(new PushNotification
                {
                    UserId = payment.ChildId,
                    Type = "allowance_received",
                    Title = "Taschengeld eingegangen!",
                    Body = $"Dein {frequencyText} Taschengeld von {payment.Amount:C} wurde gutgeschrieben.",
                    Data = new Dictionary<string, string>
                    {
                        ["type"] = "allowance_received",
                        ["amount"] = payment.Amount.ToString("F2")
                    }
                });
            }
        }
    }
}

// Zinsen-Job
public class InterestCalculationJob : IJob
{
    private readonly IInterestService _interestService;
    private readonly IPushNotificationSender _pushSender;
    private readonly INotificationSettingsRepository _settingsRepository;

    public async Task Execute(IJobExecutionContext context)
    {
        var interestPayments = await _interestService.CalculateMonthlyInterestAsync();

        foreach (var interest in interestPayments)
        {
            // Zinsen gutschreiben
            await _interestService.CreditInterestAsync(interest);

            // Push senden
            var settings = await _settingsRepository.GetAsync(interest.ChildId);
            if (settings.InterestNotificationsEnabled)
            {
                var monthName = DateTime.Now.AddMonths(-1).ToString("MMMM", new CultureInfo("de-DE"));

                await _pushSender.SendAsync(new PushNotification
                {
                    UserId = interest.ChildId,
                    Type = "interest_credited",
                    Title = "Zinsen gutgeschrieben",
                    Body = $"Du hast {interest.Amount:C} Zinsen fuer {monthName} erhalten!",
                    Data = new Dictionary<string, string>
                    {
                        ["type"] = "interest_credited",
                        ["amount"] = interest.Amount.ToString("F2"),
                        ["month"] = monthName
                    }
                });
            }
        }
    }
}
```

### Mobile: Push-Handler
```csharp
public partial class PushNotificationHandler
{
    private async Task HandleAllowanceReceivedAsync(IDictionary<string, string> data)
    {
        // Navigation zum Dashboard
        await Shell.Current.GoToAsync("//dashboard");
    }

    private async Task HandleInterestCreditedAsync(IDictionary<string, string> data)
    {
        // Navigation zum Dashboard
        await Shell.Current.GoToAsync("//dashboard");
    }
}
```

## Payload-Strukturen

### Taschengeld
```json
{
  "notification": {
    "title": "Taschengeld eingegangen!",
    "body": "Dein woechentliches Taschengeld von 10,00 EUR wurde gutgeschrieben."
  },
  "data": {
    "type": "allowance_received",
    "amount": "10.00",
    "frequency": "weekly"
  },
  "android": {
    "priority": "high",
    "notification": {
      "channel_id": "allowance",
      "icon": "ic_money"
    }
  }
}
```

### Zinsen
```json
{
  "notification": {
    "title": "Zinsen gutgeschrieben",
    "body": "Du hast 0,50 EUR Zinsen fuer Januar erhalten!"
  },
  "data": {
    "type": "interest_credited",
    "amount": "0.50",
    "month": "Januar"
  },
  "android": {
    "priority": "normal",
    "notification": {
      "channel_id": "allowance",
      "icon": "ic_trending_up"
    }
  }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Woechentliches Taschengeld | Push mit "woechentliches" |
| TC-002 | Monatliches Taschengeld | Push mit "monatliches" |
| TC-003 | Zinsen am Monatsende | Push mit Monatsname |
| TC-004 | Auf Push tippen | Navigation zum Dashboard |
| TC-005 | Benachrichtigung deaktiviert | Keine Push |

## Story Points

1

## Prioritaet

Mittel
