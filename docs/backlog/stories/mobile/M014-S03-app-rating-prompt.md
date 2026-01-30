# Story M014-S03: App-Rating Prompt

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **App-Betreiber** moechte ich **zufriedene Benutzer bitten, die App im Store zu bewerten**, damit **wir mehr positive Bewertungen erhalten und die App sichtbarer wird**.

## Akzeptanzkriterien

- [ ] Gegeben ein aktiver Benutzer, wenn er die App 7 Tage genutzt hat, dann wird eine Bewertungsanfrage angezeigt
- [ ] Gegeben mindestens 5 erfolgreiche Transaktionen, wenn der Benutzer diese Schwelle erreicht, dann qualifiziert er sich fuer die Anfrage
- [ ] Gegeben eine Bewertungsanfrage, wenn der Benutzer "Spaeter" waehlt, dann wird er fruehestens nach 30 Tagen erneut gefragt
- [ ] Gegeben eine Bewertungsanfrage, wenn der Benutzer "Nicht mehr fragen" waehlt, dann wird er nie wieder gefragt
- [ ] Gegeben ein Android-Geraet, wenn der Benutzer bewertet, dann wird der native In-App-Review Dialog verwendet

## UI-Entwurf

### Bewertungs-Dialog
```
+------------------------------------+
|           Gefaellt dir die App? [X]|
+------------------------------------+
|                                    |
|  [Star][Star][Star][Star][Star]    |
|                                    |
|  Du nutzt TaschengeldManager       |
|  jetzt seit einer Woche!           |
|                                    |
|  Wenn dir die App gefaellt,        |
|  wuerden wir uns ueber eine        |
|  Bewertung freuen.                 |
|                                    |
|  +--------------------------------+|
|  |       App bewerten             ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |     Spaeter erinnern           ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |     Nicht mehr fragen          ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## Technische Implementierung

### Rating-Service

```csharp
public interface IRatingService
{
    Task<bool> ShouldShowRatingPromptAsync();
    Task ShowRatingPromptAsync();
    Task DismissForLaterAsync();
    Task DismissPermanentlyAsync();
    Task RecordTransactionAsync();
}

public class RatingService : IRatingService
{
    private const string FirstUseDateKey = "FirstUseDate";
    private const string TransactionCountKey = "TransactionCount";
    private const string LastRatingPromptKey = "LastRatingPrompt";
    private const string RatingDismissedKey = "RatingDismissed";
    private const string HasRatedKey = "HasRated";

    private const int MinDaysBeforePrompt = 7;
    private const int MinTransactions = 5;
    private const int DaysBetweenPrompts = 30;

    public async Task<bool> ShouldShowRatingPromptAsync()
    {
        // Wurde bereits bewertet oder permanent abgelehnt?
        if (Preferences.Get(HasRatedKey, false) ||
            Preferences.Get(RatingDismissedKey, false))
        {
            return false;
        }

        // Ersten Nutzungstag pruefen/setzen
        var firstUseDate = GetFirstUseDate();
        var daysSinceFirstUse = (DateTime.Today - firstUseDate).Days;

        if (daysSinceFirstUse < MinDaysBeforePrompt)
        {
            return false;
        }

        // Transaktionen pruefen
        var transactionCount = Preferences.Get(TransactionCountKey, 0);
        if (transactionCount < MinTransactions)
        {
            return false;
        }

        // Letzte Anfrage pruefen
        var lastPrompt = Preferences.Get(LastRatingPromptKey, DateTime.MinValue);
        var daysSinceLastPrompt = (DateTime.Today - lastPrompt).Days;

        return daysSinceLastPrompt >= DaysBetweenPrompts;
    }

    public async Task ShowRatingPromptAsync()
    {
        Preferences.Set(LastRatingPromptKey, DateTime.Today);

#if ANDROID
        await ShowAndroidInAppReviewAsync();
#else
        await ShowFallbackRatingDialogAsync();
#endif
    }

#if ANDROID
    private async Task ShowAndroidInAppReviewAsync()
    {
        try
        {
            var activity = Platform.CurrentActivity;
            var manager = ReviewManagerFactory.Create(activity);
            var request = manager.RequestReviewFlow();

            await request.AsTask();

            if (request.Result != null)
            {
                var launchFlow = manager.LaunchReviewFlow(activity, request.Result);
                await launchFlow.AsTask();
                Preferences.Set(HasRatedKey, true);
            }
        }
        catch (Exception)
        {
            // Fallback auf manuellen Store-Link
            await OpenStorePageAsync();
        }
    }
#endif

    private async Task ShowFallbackRatingDialogAsync()
    {
        var result = await Application.Current!.MainPage!.DisplayAlert(
            "App bewerten",
            "Wenn dir TaschengeldManager gefaellt, wuerden wir uns ueber eine Bewertung freuen!",
            "Jetzt bewerten",
            "Spaeter");

        if (result)
        {
            await OpenStorePageAsync();
            Preferences.Set(HasRatedKey, true);
        }
    }

    private async Task OpenStorePageAsync()
    {
        var storeUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "market://details?id=com.taschengeld.app"
            : "https://apps.apple.com/app/idXXXXXXXXX";

        await Browser.OpenAsync(storeUrl, BrowserLaunchMode.External);
    }

    public async Task DismissForLaterAsync()
    {
        Preferences.Set(LastRatingPromptKey, DateTime.Today);
    }

    public async Task DismissPermanentlyAsync()
    {
        Preferences.Set(RatingDismissedKey, true);
    }

    public async Task RecordTransactionAsync()
    {
        var count = Preferences.Get(TransactionCountKey, 0);
        Preferences.Set(TransactionCountKey, count + 1);
    }

    private DateTime GetFirstUseDate()
    {
        var stored = Preferences.Get(FirstUseDateKey, DateTime.MinValue);
        if (stored == DateTime.MinValue)
        {
            stored = DateTime.Today;
            Preferences.Set(FirstUseDateKey, stored);
        }
        return stored;
    }
}
```

### Integration in App

```csharp
// Bei jeder erfolgreichen Transaktion
public class TransactionService : ITransactionService
{
    private readonly IRatingService _ratingService;

    public async Task CreateTransactionAsync(CreateTransactionRequest request)
    {
        // ... Transaktion erstellen ...

        // Zaehler erhoehen
        await _ratingService.RecordTransactionAsync();
    }
}

// Bei App-Start oder Dashboard-Anzeige
public partial class DashboardViewModel : BaseViewModel
{
    private readonly IRatingService _ratingService;

    [RelayCommand]
    private async Task OnAppearingAsync()
    {
        await LoadDataAsync();

        // Rating-Check (nach kurzer Verzoegerung)
        await Task.Delay(2000);

        if (await _ratingService.ShouldShowRatingPromptAsync())
        {
            await ShowRatingDialogAsync();
        }
    }

    private async Task ShowRatingDialogAsync()
    {
        var action = await Application.Current!.MainPage!.DisplayActionSheet(
            "Gefaellt dir die App?",
            null, // Kein Abbrechen-Button
            null,
            "App bewerten",
            "Spaeter erinnern",
            "Nicht mehr fragen");

        switch (action)
        {
            case "App bewerten":
                await _ratingService.ShowRatingPromptAsync();
                break;
            case "Spaeter erinnern":
                await _ratingService.DismissForLaterAsync();
                break;
            case "Nicht mehr fragen":
                await _ratingService.DismissPermanentlyAsync();
                break;
        }
    }
}
```

## Trigger-Bedingungen

| Bedingung | Wert |
|-----------|------|
| Mindestnutzungsdauer | 7 Tage |
| Mindesttransaktionen | 5 |
| Pause zwischen Anfragen | 30 Tage |
| Max. Anfragen | Unbegrenzt (bis bewertet oder abgelehnt) |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Tag 1, 0 Transaktionen | Keine Anfrage |
| TC-002 | Tag 7, 5 Transaktionen | Anfrage wird angezeigt |
| TC-003 | "Spaeter" gewaehlt | Naechste Anfrage in 30 Tagen |
| TC-004 | "Nicht mehr fragen" | Nie wieder angefragt |
| TC-005 | "App bewerten" (Android) | In-App Review wird gestartet |
| TC-006 | Bewertung abgeschlossen | Nie wieder angefragt |

## Story Points

1

## Prioritaet

Niedrig
