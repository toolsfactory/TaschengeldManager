# Story M013-S01: Globale Exception-Behandlung

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **Entwickler** moechte ich **dass alle unbehandelten Exceptions zentral abgefangen werden**, damit **die App nicht abstuerzt und Benutzer eine freundliche Fehlermeldung erhalten**.

## Akzeptanzkriterien

- [ ] Gegeben eine unbehandelte Exception, wenn sie auftritt, dann wird sie global abgefangen
- [ ] Gegeben eine abgefangene Exception, wenn sie angezeigt wird, dann sieht der Benutzer eine benutzerfreundliche Meldung
- [ ] Gegeben eine kritische Exception, wenn sie auftritt, dann wird sie an das Crash-Reporting gesendet
- [ ] Gegeben eine Exception, wenn sie geloggt wird, dann enthaelt das Log relevante Kontextinformationen

## Technische Implementierung

### Exception-Handler Setup

```csharp
// MauiProgram.cs
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    // Global Exception Handler registrieren
    builder.Services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();

    var app = builder.Build();

    // Handler initialisieren
    var exceptionHandler = app.Services.GetRequiredService<IGlobalExceptionHandler>();
    exceptionHandler.Initialize();

    return app;
}
```

### Global Exception Handler

```csharp
public interface IGlobalExceptionHandler
{
    void Initialize();
    Task HandleExceptionAsync(Exception exception, string? context = null);
}

public class GlobalExceptionHandler : IGlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly ICrashReportingService _crashReporting;
    private readonly IToastService _toastService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        ICrashReportingService crashReporting,
        IToastService toastService)
    {
        _logger = logger;
        _crashReporting = crashReporting;
        _toastService = toastService;
    }

    public void Initialize()
    {
        // .NET Unhandled Exceptions
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        // Task Unobserved Exceptions
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

#if ANDROID
        // Android-spezifisch
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#endif
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        if (exception != null)
        {
            HandleExceptionAsync(exception, "UnhandledException").ConfigureAwait(false);
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved(); // Verhindert App-Absturz
        HandleExceptionAsync(e.Exception, "UnobservedTaskException").ConfigureAwait(false);
    }

#if ANDROID
    private void OnAndroidUnhandledException(object? sender, Android.Runtime.RaiseThrowableEventArgs e)
    {
        HandleExceptionAsync(e.Exception, "AndroidUnhandledException").ConfigureAwait(false);
    }
#endif

    public async Task HandleExceptionAsync(Exception exception, string? context = null)
    {
        // Loggen
        _logger.LogError(exception, "Unhandled exception in {Context}", context ?? "Unknown");

        // An Crash-Reporting senden
        _crashReporting.CaptureException(exception, new Dictionary<string, string>
        {
            ["Context"] = context ?? "Unknown",
            ["AppVersion"] = AppInfo.VersionString,
            ["Platform"] = DeviceInfo.Platform.ToString()
        });

        // Benutzerfreundliche Meldung anzeigen
        var message = GetUserFriendlyMessage(exception);

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await _toastService.ShowErrorAsync(message);
        });
    }

    private string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            HttpRequestException => "Verbindung zum Server fehlgeschlagen. Bitte pruefe deine Internetverbindung.",
            TaskCanceledException => "Die Anfrage hat zu lange gedauert. Bitte versuche es erneut.",
            UnauthorizedAccessException => "Du bist nicht berechtigt fuer diese Aktion. Bitte melde dich erneut an.",
            ValidationException ve => ve.Message,
            _ => "Ein unerwarteter Fehler ist aufgetreten. Bitte versuche es erneut."
        };
    }
}
```

### BaseViewModel Integration

```csharp
public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly IGlobalExceptionHandler _exceptionHandler;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    protected async Task ExecuteSafeAsync(Func<Task> action, string? context = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            await action();
        }
        catch (Exception ex)
        {
            await _exceptionHandler.HandleExceptionAsync(ex, context);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected async Task<T?> ExecuteSafeAsync<T>(Func<Task<T>> action, string? context = null)
    {
        if (IsBusy) return default;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            return await action();
        }
        catch (Exception ex)
        {
            await _exceptionHandler.HandleExceptionAsync(ex, context);
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// Verwendung im ViewModel
public partial class DashboardViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            Balance = await _accountService.GetBalanceAsync();
            Transactions = await _transactionService.GetRecentAsync();
        }, "Dashboard.LoadData");
    }
}
```

## Exception-Typen

| Exception | Behandlung | Benutzer-Meldung |
|-----------|------------|------------------|
| HttpRequestException | Log + Toast | Verbindungsfehler |
| TaskCanceledException | Log + Toast | Timeout |
| UnauthorizedAccessException | Logout + Redirect | Nicht berechtigt |
| ValidationException | Toast | Spezifische Meldung |
| Sonstige | Log + Crash-Report + Toast | Allgemeiner Fehler |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Unbehandelte Exception | Wird abgefangen, Toast angezeigt |
| TC-002 | HTTP-Fehler | Verbindungsfehler-Meldung |
| TC-003 | Timeout | Timeout-Meldung |
| TC-004 | Task-Exception | Wird observiert, kein Crash |
| TC-005 | Crash-Report | Exception wird an Sentry gesendet |
| TC-006 | Context-Logging | Context im Log vorhanden |

## Story Points

2

## Prioritaet

Hoch
