using Microsoft.AspNetCore.Diagnostics;
using TaschengeldManager.Api.Extensions;

namespace TaschengeldManager.Api.Middleware;

/// <summary>
/// Global exception handler that provides consistent error responses across the API.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, errorCode, message) = MapException(exception);

        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}, ErrorCode: {ErrorCode}",
            httpContext.TraceIdentifier,
            errorCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new ApiError
        {
            Error = message,
            Code = errorCode,
            Details = _environment.IsDevelopment()
                ? new Dictionary<string, string[]>
                {
                    ["traceId"] = [httpContext.TraceIdentifier],
                    ["exceptionType"] = [exception.GetType().Name]
                }
                : new Dictionary<string, string[]>
                {
                    ["traceId"] = [httpContext.TraceIdentifier]
                }
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }

    private static (int StatusCode, string ErrorCode, string Message) MapException(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException ex => (
                StatusCodes.Status403Forbidden,
                "FORBIDDEN",
                ex.Message),

            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                "INVALID_OPERATION",
                ex.Message),

            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "INVALID_ARGUMENT",
                ex.Message),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "NOT_FOUND",
                "Die angeforderte Ressource wurde nicht gefunden."),

            OperationCanceledException => (
                StatusCodes.Status499ClientClosedRequest,
                "REQUEST_CANCELLED",
                "Die Anfrage wurde abgebrochen."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR",
                "Ein interner Fehler ist aufgetreten. Bitte versuchen Sie es später erneut.")
        };
    }
}
