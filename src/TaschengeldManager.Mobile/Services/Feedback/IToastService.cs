namespace TaschengeldManager.Mobile.Services.Feedback;

/// <summary>
/// Service interface for showing toast/snackbar notifications (M013-S02)
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Shows a success message (green, 3 seconds by default)
    /// </summary>
    Task ShowSuccessAsync(string message, int durationMs = 3000);

    /// <summary>
    /// Shows an error message (red, 5 seconds by default)
    /// </summary>
    Task ShowErrorAsync(string message, int durationMs = 5000);

    /// <summary>
    /// Shows a warning message (orange, 4 seconds by default)
    /// </summary>
    Task ShowWarningAsync(string message, int durationMs = 4000);

    /// <summary>
    /// Shows an info message (blue, 3 seconds by default)
    /// </summary>
    Task ShowInfoAsync(string message, int durationMs = 3000);

    /// <summary>
    /// Shows a toast with custom options
    /// </summary>
    Task ShowAsync(ToastOptions options);
}

/// <summary>
/// Options for customizing a toast notification
/// </summary>
public class ToastOptions
{
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; } = ToastType.Info;
    public int DurationMs { get; set; } = 3000;
    public string? ActionText { get; set; }
    public Action? ActionCallback { get; set; }
}

/// <summary>
/// Types of toast notifications
/// </summary>
public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}
