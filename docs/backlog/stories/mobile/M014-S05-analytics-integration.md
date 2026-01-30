# Story M014-S05: Analytics Integration

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **Produktmanager** moechte ich **anonymisierte Nutzungsdaten sammeln**, damit **ich verstehe, wie die App genutzt wird und wo Verbesserungspotential besteht**.

## Akzeptanzkriterien

- [ ] Gegeben ein App-Start, wenn er erfolgt, dann wird ein anonymisiertes Event gesendet
- [ ] Gegeben eine Benutzeraktion (z.B. Ausgabe erfassen), wenn sie abgeschlossen ist, dann wird sie als Event erfasst
- [ ] Gegeben ein Benutzer, wenn er Analytics deaktiviert hat, dann werden keine Events gesendet
- [ ] Gegeben Analytics-Events, wenn sie gesendet werden, dann enthalten sie keine personenbezogenen Daten

## Erfasste Events

| Event | Beschreibung | Parameter |
|-------|--------------|-----------|
| `app_open` | App gestartet | - |
| `login_success` | Erfolgreiche Anmeldung | role |
| `logout` | Abmeldung | - |
| `expense_created` | Ausgabe erfasst | category |
| `request_submitted` | Geldanfrage gestellt | - |
| `request_approved` | Anfrage genehmigt | - |
| `request_rejected` | Anfrage abgelehnt | - |
| `gift_sent` | Geschenk gesendet | - |
| `allowance_setup` | Taschengeld eingerichtet | frequency |
| `screen_view` | Seite angezeigt | screen_name |

## Technische Implementierung

### NuGet Package

```xml
<ItemGroup>
    <PackageReference Include="Plugin.Firebase.Analytics" Version="2.x.x" />
</ItemGroup>
```

### Analytics Service

```csharp
public interface IAnalyticsService
{
    void SetEnabled(bool enabled);
    bool IsEnabled { get; }
    void SetUserRole(string role);
    void TrackEvent(string eventName, IDictionary<string, string>? parameters = null);
    void TrackScreenView(string screenName);
}

public class FirebaseAnalyticsService : IAnalyticsService
{
    public bool IsEnabled => Preferences.Get("AnalyticsEnabled", true);

    public void SetEnabled(bool enabled)
    {
        Preferences.Set("AnalyticsEnabled", enabled);
        CrossFirebaseAnalytics.Current.SetAnalyticsCollectionEnabled(enabled);
    }

    public void SetUserRole(string role)
    {
        if (!IsEnabled) return;

        // Nur Rolle, keine persoenlichen Daten
        CrossFirebaseAnalytics.Current.SetUserProperty("user_role", role);
    }

    public void TrackEvent(string eventName, IDictionary<string, string>? parameters = null)
    {
        if (!IsEnabled) return;

        // Event-Namen sanitizen (nur Kleinbuchstaben, Unterstriche)
        var sanitizedName = SanitizeEventName(eventName);

        if (parameters != null)
        {
            CrossFirebaseAnalytics.Current.LogEvent(sanitizedName, parameters);
        }
        else
        {
            CrossFirebaseAnalytics.Current.LogEvent(sanitizedName);
        }
    }

    public void TrackScreenView(string screenName)
    {
        if (!IsEnabled) return;

        CrossFirebaseAnalytics.Current.SetCurrentScreen(screenName, screenName);
    }

    private string SanitizeEventName(string name)
    {
        return name.ToLowerInvariant()
            .Replace("-", "_")
            .Replace(" ", "_");
    }
}
```

### Integration in ViewModels

```csharp
// BaseViewModel
public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly IAnalyticsService _analytics;

    protected virtual string ScreenName => GetType().Name.Replace("ViewModel", "");

    public virtual void OnAppearing()
    {
        _analytics.TrackScreenView(ScreenName);
    }
}

// Beispiel: Ausgabe erfassen
public partial class CreateExpenseViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task SaveAsync()
    {
        await _expenseService.CreateAsync(/* ... */);

        // Analytics Event - nur Kategorie, kein Betrag!
        _analytics.TrackEvent("expense_created", new Dictionary<string, string>
        {
            ["category"] = Category
        });
    }
}

// Beispiel: Geldanfrage
public partial class CreateRequestViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task SubmitAsync()
    {
        await _requestService.CreateAsync(/* ... */);

        // Keine Betraege oder Gruende tracken!
        _analytics.TrackEvent("request_submitted");
    }
}

// Beispiel: Login
public class AuthService : IAuthService
{
    private readonly IAnalyticsService _analytics;

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var result = await _apiClient.LoginAsync(/* ... */);

        if (result.IsSuccess)
        {
            _analytics.TrackEvent("login_success", new Dictionary<string, string>
            {
                ["role"] = result.User.Role
            });

            _analytics.SetUserRole(result.User.Role);
        }

        return result;
    }
}
```

### Page-Tracking automatisieren

```csharp
// Behavior fuer automatisches Screen-Tracking
public class AnalyticsPageBehavior : Behavior<ContentPage>
{
    private IAnalyticsService? _analytics;
    private string? _screenName;

    protected override void OnAttachedTo(ContentPage page)
    {
        base.OnAttachedTo(page);

        _analytics = IPlatformApplication.Current?.Services.GetService<IAnalyticsService>();
        _screenName = page.GetType().Name.Replace("Page", "");

        page.Appearing += OnPageAppearing;
    }

    protected override void OnDetachingFrom(ContentPage page)
    {
        page.Appearing -= OnPageAppearing;
        base.OnDetachingFrom(page);
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        _analytics?.TrackScreenView(_screenName ?? "Unknown");
    }
}

// XAML Verwendung
<ContentPage xmlns:behaviors="clr-namespace:TaschengeldManager.Behaviors">
    <ContentPage.Behaviors>
        <behaviors:AnalyticsPageBehavior/>
    </ContentPage.Behaviors>
    <!-- Content -->
</ContentPage>
```

## Datenschutz-Richtlinien

### NICHT erfasst werden:
- Benutzernamen / Echte Namen
- E-Mail-Adressen
- Transaktionsbetraege
- Nachrichten-Inhalte
- Anfrage-Gruende
- Genaue Zeitstempel (nur Datum)

### Erfasst werden:
- Benutzerrolle (Parent/Child/Relative)
- Kategorien (ohne Betraege)
- Haeufigkeit (weekly/monthly etc.)
- Screen-Namen
- Allgemeine Aktionen

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | App-Start | `app_open` Event gesendet |
| TC-002 | Login | `login_success` mit Rolle |
| TC-003 | Ausgabe erfassen | `expense_created` mit Kategorie |
| TC-004 | Analytics deaktiviert | Keine Events gesendet |
| TC-005 | Screen-Wechsel | `screen_view` Event |
| TC-006 | Betraege | Werden NICHT erfasst |

## Story Points

2

## Prioritaet

Niedrig
