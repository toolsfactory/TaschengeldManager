# Story M012-S07: Datenschutz-Einstellungen

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **meine Datenschutz-Einstellungen einsehen und verwalten koennen**, damit **ich Kontrolle ueber meine Daten habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er die Datenschutz-Einstellungen oeffnet, dann sieht er alle verfuegbaren Optionen
- [ ] Gegeben die Option Analytics, wenn der Benutzer sie deaktiviert, dann werden keine anonymisierten Nutzungsdaten mehr gesammelt
- [ ] Gegeben ein Benutzer, wenn er die Datenschutzerklaerung oeffnen will, dann wird sie angezeigt
- [ ] Gegeben geaenderte Einstellungen, wenn sie gespeichert werden, dann sind sie sofort wirksam

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck       Datenschutz      |
+------------------------------------+
|                                    |
|  Datensammlung                     |
|  +--------------------------------+|
|  | Analytics                      ||
|  | Anonymisierte Nutzungsdaten    ||
|  | helfen uns, die App zu         ||
|  | verbessern.            [Toggle]||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | Crash-Reports                  ||
|  | Automatische Fehlerberichte    ||
|  | an unser Team senden.  [Toggle]||
|  +--------------------------------+|
|                                    |
|  Rechtliches                       |
|  +--------------------------------+|
|  | Datenschutzerklaerung       > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Nutzungsbedingungen         > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Impressum                   > ||
|  +--------------------------------+|
|                                    |
|  Deine Daten                       |
|  +--------------------------------+|
|  | Daten exportieren           > ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | Account loeschen            > ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkt

### Einstellungen abrufen
```
GET /api/users/me/privacy
Authorization: Bearer {token}

Response 200:
{
  "analyticsEnabled": true,
  "crashReportingEnabled": true,
  "lastUpdated": "2026-01-15T10:00:00Z"
}
```

### Einstellungen speichern
```
PUT /api/users/me/privacy
Authorization: Bearer {token}
Content-Type: application/json

{
  "analyticsEnabled": false,
  "crashReportingEnabled": true
}

Response 200:
{
  "success": true,
  "updatedAt": "2026-01-20T10:00:00Z"
}
```

## Technische Notizen

- ViewModel: `PrivacySettingsViewModel`
- Analytics: Integration mit Firebase Analytics oder aehnlichem
- Crash-Reporting: Sentry oder AppCenter
- Links zu rechtlichen Dokumenten: WebView oder externer Browser

## Implementierungshinweise

```csharp
public partial class PrivacySettingsViewModel : BaseViewModel
{
    private readonly IPrivacyService _privacyService;
    private readonly IAnalyticsService _analyticsService;
    private readonly ICrashReportingService _crashReportingService;

    [ObservableProperty]
    private bool _analyticsEnabled;

    [ObservableProperty]
    private bool _crashReportingEnabled;

    partial void OnAnalyticsEnabledChanged(bool value)
    {
        _analyticsService.SetEnabled(value);
        SaveSettingsAsync().ConfigureAwait(false);
    }

    partial void OnCrashReportingEnabledChanged(bool value)
    {
        _crashReportingService.SetEnabled(value);
        SaveSettingsAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        var settings = await _privacyService.GetSettingsAsync();
        AnalyticsEnabled = settings.AnalyticsEnabled;
        CrashReportingEnabled = settings.CrashReportingEnabled;
    }

    private async Task SaveSettingsAsync()
    {
        await _privacyService.SaveSettingsAsync(new PrivacySettings
        {
            AnalyticsEnabled = AnalyticsEnabled,
            CrashReportingEnabled = CrashReportingEnabled
        });
    }

    [RelayCommand]
    private async Task OpenPrivacyPolicyAsync()
    {
        await Browser.OpenAsync("https://taschengeld.app/privacy", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task OpenTermsOfServiceAsync()
    {
        await Browser.OpenAsync("https://taschengeld.app/terms", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task OpenImprintAsync()
    {
        await Browser.OpenAsync("https://taschengeld.app/imprint", BrowserLaunchMode.SystemPreferred);
    }

    [RelayCommand]
    private async Task NavigateToDataExportAsync()
    {
        await Shell.Current.GoToAsync("dataExport");
    }

    [RelayCommand]
    private async Task NavigateToDeleteAccountAsync()
    {
        await Shell.Current.GoToAsync("deleteAccount");
    }
}

// Analytics Service Integration
public class AnalyticsService : IAnalyticsService
{
    public void SetEnabled(bool enabled)
    {
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(enabled);
        Preferences.Set("AnalyticsEnabled", enabled);
    }

    public bool IsEnabled => Preferences.Get("AnalyticsEnabled", true);
}

// Crash Reporting Integration
public class CrashReportingService : ICrashReportingService
{
    public void SetEnabled(bool enabled)
    {
        SentrySdk.ConfigureScope(scope =>
        {
            if (!enabled)
            {
                scope.Level = SentryLevel.Fatal; // Nur fatale Fehler
            }
        });
        Preferences.Set("CrashReportingEnabled", enabled);
    }

    public bool IsEnabled => Preferences.Get("CrashReportingEnabled", true);
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Einstellungen laden | Aktuelle Werte werden angezeigt |
| TC-002 | Analytics deaktivieren | Keine Tracking-Events mehr |
| TC-003 | Crash-Reporting deaktivieren | Keine Reports mehr |
| TC-004 | Datenschutzerklaerung oeffnen | Link wird geoeffnet |
| TC-005 | Nutzungsbedingungen oeffnen | Link wird geoeffnet |
| TC-006 | Einstellungen nach Neustart | Werte bleiben erhalten |

## Story Points

1

## Prioritaet

Mittel
