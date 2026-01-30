using Microsoft.Extensions.Logging;

namespace TaschengeldManager.Mobile.Services.Feedback;

/// <summary>
/// Global exception handler implementation (M013-S01)
/// </summary>
public class GlobalExceptionHandler : IGlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IToastService _toastService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IToastService toastService)
    {
        _logger = logger;
        _toastService = toastService;
    }

    public void Initialize()
    {
        // .NET Unhandled Exceptions
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        // Task Unobserved Exceptions
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

#if ANDROID
        // Android-specific handler
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#endif
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            _ = HandleExceptionAsync(exception, "UnhandledException");
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved(); // Prevents app crash
        _ = HandleExceptionAsync(e.Exception, "UnobservedTaskException");
    }

#if ANDROID
    private void OnAndroidUnhandledException(object? sender, Android.Runtime.RaiseThrowableEventArgs e)
    {
        _ = HandleExceptionAsync(e.Exception, "AndroidUnhandledException");
    }
#endif

    public async Task HandleExceptionAsync(Exception exception, string? context = null)
    {
        // Log the exception
        _logger.LogError(exception, "Unhandled exception in {Context}", context ?? "Unknown");

        // TODO: Send to crash reporting (Sentry/AppCenter) when M014-S04 is implemented
        // _crashReporting.CaptureException(exception, new Dictionary<string, string>
        // {
        //     ["Context"] = context ?? "Unknown",
        //     ["AppVersion"] = AppInfo.VersionString,
        //     ["Platform"] = DeviceInfo.Platform.ToString()
        // });

        // Show user-friendly message
        var message = GetUserFriendlyMessage(exception);

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await _toastService.ShowErrorAsync(message);
            });
        }
        catch
        {
            // If we can't show a toast, at least log it
            _logger.LogWarning("Could not show error toast to user");
        }
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            HttpRequestException => "Verbindung zum Server fehlgeschlagen. Bitte pruefe deine Internetverbindung.",
            TaskCanceledException or OperationCanceledException => "Die Anfrage hat zu lange gedauert. Bitte versuche es erneut.",
            UnauthorizedAccessException => "Du bist nicht berechtigt fuer diese Aktion. Bitte melde dich erneut an.",
            InvalidOperationException ioe when ioe.Message.Contains("401") => "Deine Sitzung ist abgelaufen. Bitte melde dich erneut an.",
            ArgumentException ae => ae.Message,
            _ => "Ein unerwarteter Fehler ist aufgetreten. Bitte versuche es erneut."
        };
    }
}
