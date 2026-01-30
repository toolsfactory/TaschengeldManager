namespace TaschengeldManager.Mobile.Services.Feedback;

/// <summary>
/// Global exception handler interface (M013-S01)
/// </summary>
public interface IGlobalExceptionHandler
{
    /// <summary>
    /// Initializes the exception handler and hooks into system events
    /// </summary>
    void Initialize();

    /// <summary>
    /// Handles an exception by logging, reporting, and showing user feedback
    /// </summary>
    Task HandleExceptionAsync(Exception exception, string? context = null);
}
