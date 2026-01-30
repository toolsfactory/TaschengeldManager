# Story M014-S04: Crash-Reporting (Sentry)

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **Entwickler** moechte ich **automatisch ueber App-Abstuerze und Fehler informiert werden**, damit **ich Probleme schnell erkennen und beheben kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein App-Absturz, wenn er auftritt, dann wird ein Report an Sentry gesendet
- [ ] Gegeben eine unbehandelte Exception, wenn sie auftritt, dann wird sie automatisch erfasst
- [ ] Gegeben ein Crash-Report, wenn er gesendet wird, dann enthaelt er App-Version, Geraeteinfo und Stacktrace
- [ ] Gegeben ein Benutzer, wenn er Crash-Reporting deaktiviert hat, dann werden keine Reports gesendet
- [ ] Gegeben personenbezogene Daten, wenn ein Report gesendet wird, dann werden sie anonymisiert

## Technische Implementierung

### NuGet Package

```xml
<ItemGroup>
    <PackageReference Include="Sentry.Maui" Version="4.x.x" />
</ItemGroup>
```

### MauiProgram.cs Setup

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder
        .UseMauiApp<App>()
        .UseSentry(options =>
        {
            options.Dsn = "https://examplePublicKey@o0.ingest.sentry.io/0";

            // Environment
            options.Environment = AppConstants.Environment; // "production", "staging", "development"

            // Release Version
            options.Release = $"com.taschengeld.app@{AppInfo.VersionString}";

            // Performance Monitoring (optional)
            options.TracesSampleRate = 0.1; // 10% der Transaktionen tracken

            // Breadcrumbs
            options.MaxBreadcrumbs = 50;

            // Keine personenbezogenen Daten
            options.SendDefaultPii = false;

            // Debug-Modus fuer Entwicklung
#if DEBUG
            options.Debug = true;
            options.DiagnosticLevel = SentryLevel.Warning;
#endif
        });

    return builder.Build();
}
```

### Crash-Reporting Service

```csharp
public interface ICrashReportingService
{
    void SetEnabled(bool enabled);
    bool IsEnabled { get; }
    void SetUser(string? userId, string? role);
    void ClearUser();
    void CaptureException(Exception exception, IDictionary<string, string>? extras = null);
    void AddBreadcrumb(string message, string category, BreadcrumbLevel level = BreadcrumbLevel.Info);
}

public class SentryCrashReportingService : ICrashReportingService
{
    public bool IsEnabled => Preferences.Get("CrashReportingEnabled", true);

    public void SetEnabled(bool enabled)
    {
        Preferences.Set("CrashReportingEnabled", enabled);

        if (!enabled)
        {
            SentrySdk.EndSession();
        }
        else
        {
            SentrySdk.StartSession();
        }
    }

    public void SetUser(string? userId, string? role)
    {
        if (!IsEnabled) return;

        SentrySdk.ConfigureScope(scope =>
        {
            scope.User = new SentryUser
            {
                // Anonymisierte ID (Hash)
                Id = userId != null ? HashUserId(userId) : null,
                // Keine Email, kein Name
            };

            if (role != null)
            {
                scope.SetTag("user_role", role);
            }
        });
    }

    public void ClearUser()
    {
        SentrySdk.ConfigureScope(scope =>
        {
            scope.User = null;
        });
    }

    public void CaptureException(Exception exception, IDictionary<string, string>? extras = null)
    {
        if (!IsEnabled) return;

        SentrySdk.ConfigureScope(scope =>
        {
            if (extras != null)
            {
                foreach (var (key, value) in extras)
                {
                    scope.SetExtra(key, value);
                }
            }
        });

        SentrySdk.CaptureException(exception);
    }

    public void AddBreadcrumb(string message, string category, BreadcrumbLevel level = BreadcrumbLevel.Info)
    {
        if (!IsEnabled) return;

        SentrySdk.AddBreadcrumb(
            message: message,
            category: category,
            level: MapLevel(level));
    }

    private string HashUserId(string userId)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(userId));
        return Convert.ToBase64String(hash)[..16]; // Gekuerzter Hash
    }

    private Sentry.BreadcrumbLevel MapLevel(BreadcrumbLevel level)
    {
        return level switch
        {
            BreadcrumbLevel.Debug => Sentry.BreadcrumbLevel.Debug,
            BreadcrumbLevel.Info => Sentry.BreadcrumbLevel.Info,
            BreadcrumbLevel.Warning => Sentry.BreadcrumbLevel.Warning,
            BreadcrumbLevel.Error => Sentry.BreadcrumbLevel.Error,
            _ => Sentry.BreadcrumbLevel.Info
        };
    }
}

public enum BreadcrumbLevel
{
    Debug,
    Info,
    Warning,
    Error
}
```

### Breadcrumbs fuer Navigation und Aktionen

```csharp
// BaseViewModel
public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly ICrashReportingService _crashReporting;

    protected void TrackAction(string action)
    {
        _crashReporting.AddBreadcrumb(action, "user_action");
    }
}

// Navigation Tracking
public partial class AppShell : Shell
{
    private readonly ICrashReportingService _crashReporting;

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        _crashReporting.AddBreadcrumb(
            $"Navigating to {args.Target.Location}",
            "navigation");
    }
}

// Verwendung im ViewModel
public partial class CreateExpenseViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task SaveAsync()
    {
        TrackAction("create_expense_started");

        try
        {
            await _expenseService.CreateAsync(/* ... */);
            TrackAction("create_expense_success");
        }
        catch (Exception ex)
        {
            TrackAction("create_expense_failed");
            throw;
        }
    }
}
```

### Kontextdaten (Anonymisiert)

```csharp
// Bei App-Start
SentrySdk.ConfigureScope(scope =>
{
    // Geraeteinformationen
    scope.SetTag("device_platform", DeviceInfo.Platform.ToString());
    scope.SetTag("device_manufacturer", DeviceInfo.Manufacturer);
    scope.SetTag("device_model", DeviceInfo.Model);
    scope.SetTag("os_version", DeviceInfo.VersionString);

    // App-Informationen
    scope.SetTag("app_version", AppInfo.VersionString);
    scope.SetTag("app_build", AppInfo.BuildString);

    // Keine personenbezogenen Daten!
});
```

## Datenschutz-Hinweis

Folgende Daten werden **NICHT** gesendet:
- Benutzername / Echte Namen
- E-Mail-Adressen
- Transaktionsdetails / Betraege
- Nachrichten-Inhalte
- Genauer Standort

Folgende Daten werden gesendet:
- Anonymisierte User-ID (Hash)
- Benutzerrolle (Parent/Child/Relative)
- App-Version
- Geraetetyp und OS-Version
- Stacktrace bei Fehlern
- Navigations-Breadcrumbs

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | App-Absturz | Report wird an Sentry gesendet |
| TC-002 | Unbehandelte Exception | Wird erfasst |
| TC-003 | Crash-Reporting deaktiviert | Kein Report gesendet |
| TC-004 | Breadcrumbs | Navigation ist im Report sichtbar |
| TC-005 | User-ID | Wird gehasht, nicht im Klartext |
| TC-006 | Release-Info | Version und Build sind korrekt |

## Story Points

2

## Prioritaet

Hoch
