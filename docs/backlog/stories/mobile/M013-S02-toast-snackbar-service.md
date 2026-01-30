# Story M013-S02: Toast/Snackbar Service

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **Benutzer** moechte ich **kurze Feedback-Nachrichten sehen, wenn eine Aktion erfolgreich war oder fehlgeschlagen ist**, damit **ich weiss, was passiert ist**.

## Akzeptanzkriterien

- [ ] Gegeben eine erfolgreiche Aktion, wenn sie abgeschlossen ist, dann wird eine gruene Erfolgsmeldung angezeigt
- [ ] Gegeben ein Fehler, wenn er auftritt, dann wird eine rote Fehlermeldung angezeigt
- [ ] Gegeben eine Warnung, wenn sie angezeigt wird, dann erscheint sie in Orange
- [ ] Gegeben eine Info-Nachricht, wenn sie angezeigt wird, dann erscheint sie in Blau
- [ ] Gegeben eine Toast-Nachricht, wenn sie angezeigt wird, dann verschwindet sie nach einer bestimmten Zeit automatisch

## UI-Entwurf

### Toast-Typen
```
+------------------------------------+
|                                    |
|  ... App Content ...               |
|                                    |
+------------------------------------+
| [Check] Ausgabe erfolgreich        |
|         gespeichert                | <- Gruen, 3 Sekunden
+------------------------------------+

+------------------------------------+
| [X] Verbindung fehlgeschlagen      |
|     Bitte pruefe dein Internet     | <- Rot, 5 Sekunden
+------------------------------------+

+------------------------------------+
| [!] Offline - Daten werden         |
|     spaeter synchronisiert         | <- Orange, 4 Sekunden
+------------------------------------+

+------------------------------------+
| [i] Neue Version verfuegbar        | <- Blau, 3 Sekunden
+------------------------------------+
```

### Position und Animation
```
Toast erscheint am unteren Bildschirmrand:

+------------------------------------+
|                                    |
|                                    |
|                                    |
|                                    |
|                                    |
+------------------------------------+
| +--------------------------------+ |
| | [Check] Gespeichert            | | <- Slide-Up Animation
| +--------------------------------+ |
+------------------------------------+
|  Tab Bar                           |
+------------------------------------+
```

## Technische Notizen

- Service: `IToastService`
- Community Toolkit: `CommunityToolkit.Maui` fuer Snackbar
- Dauer konfigurierbar nach Typ
- Queue fuer mehrere Toasts

## Implementierungshinweise

```csharp
public interface IToastService
{
    Task ShowSuccessAsync(string message, int durationMs = 3000);
    Task ShowErrorAsync(string message, int durationMs = 5000);
    Task ShowWarningAsync(string message, int durationMs = 4000);
    Task ShowInfoAsync(string message, int durationMs = 3000);
    Task ShowAsync(ToastOptions options);
}

public class ToastOptions
{
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; } = ToastType.Info;
    public int DurationMs { get; set; } = 3000;
    public string? ActionText { get; set; }
    public Action? ActionCallback { get; set; }
}

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

// Implementierung mit CommunityToolkit.Maui
public class ToastService : IToastService
{
    public async Task ShowSuccessAsync(string message, int durationMs = 3000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Success,
            DurationMs = durationMs
        });
    }

    public async Task ShowErrorAsync(string message, int durationMs = 5000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Error,
            DurationMs = durationMs
        });
    }

    public async Task ShowWarningAsync(string message, int durationMs = 4000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Warning,
            DurationMs = durationMs
        });
    }

    public async Task ShowInfoAsync(string message, int durationMs = 3000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Info,
            DurationMs = durationMs
        });
    }

    public async Task ShowAsync(ToastOptions options)
    {
        var (backgroundColor, textColor, icon) = GetStyleForType(options.Type);

        var snackbar = Snackbar.Make(
            $"{icon} {options.Message}",
            actionButtonText: options.ActionText,
            action: options.ActionCallback,
            duration: TimeSpan.FromMilliseconds(options.DurationMs),
            visualOptions: new SnackbarOptions
            {
                BackgroundColor = backgroundColor,
                TextColor = textColor,
                ActionButtonTextColor = textColor,
                CornerRadius = new CornerRadius(8),
                Font = Font.SystemFontOfSize(14)
            });

        await snackbar.Show();
    }

    private (Color Background, Color Text, string Icon) GetStyleForType(ToastType type)
    {
        return type switch
        {
            ToastType.Success => (Color.FromArgb("#4CAF50"), Colors.White, "[Check]"),
            ToastType.Error => (Color.FromArgb("#F44336"), Colors.White, "[X]"),
            ToastType.Warning => (Color.FromArgb("#FF9800"), Colors.White, "[!]"),
            ToastType.Info => (Color.FromArgb("#2196F3"), Colors.White, "[i]"),
            _ => (Colors.Gray, Colors.White, "")
        };
    }
}

// MauiProgram.cs Registration
builder.Services.AddSingleton<IToastService, ToastService>();

// Verwendung im ViewModel
public partial class CreateExpenseViewModel : BaseViewModel
{
    private readonly IToastService _toastService;

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            await _expenseService.CreateAsync(/* ... */);
            await _toastService.ShowSuccessAsync("Ausgabe erfolgreich gespeichert");
            await Shell.Current.GoToAsync("..");
        }
        catch (ValidationException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
        }
    }
}
```

### Snackbar mit Aktion

```csharp
// Toast mit Undo-Option
await _toastService.ShowAsync(new ToastOptions
{
    Message = "Transaktion geloescht",
    Type = ToastType.Success,
    DurationMs = 5000,
    ActionText = "Rueckgaengig",
    ActionCallback = async () =>
    {
        await _transactionService.RestoreAsync(transactionId);
        await _toastService.ShowSuccessAsync("Wiederhergestellt");
    }
});
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Erfolgs-Toast | Gruen, 3 Sekunden, Check-Icon |
| TC-002 | Fehler-Toast | Rot, 5 Sekunden, X-Icon |
| TC-003 | Warnung-Toast | Orange, 4 Sekunden, !-Icon |
| TC-004 | Info-Toast | Blau, 3 Sekunden, i-Icon |
| TC-005 | Toast mit Aktion | Button wird angezeigt und funktioniert |
| TC-006 | Automatisches Ausblenden | Verschwindet nach Dauer |
| TC-007 | Mehrere Toasts | Werden nacheinander angezeigt |

## Story Points

2

## Prioritaet

Hoch
