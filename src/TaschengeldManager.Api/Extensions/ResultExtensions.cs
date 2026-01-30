using Toolsfactory.Common;
using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace TaschengeldManager.Api.Extensions;

/// <summary>
/// Standardized API error response.
/// </summary>
public record ApiError
{
    /// <summary>
    /// Error message.
    /// </summary>
    public required string Error { get; init; }

    /// <summary>
    /// Optional error code for programmatic handling.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Optional additional details.
    /// </summary>
    public IDictionary<string, string[]>? Details { get; init; }
}

/// <summary>
/// Extension methods for standardized result handling in endpoints.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes an async operation and handles common exceptions with standardized responses.
    /// </summary>
    public static async Task<HttpResult> ExecuteAsync(Func<Task<HttpResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(
                new ApiError { Error = ex.Message, Code = "UNAUTHORIZED" },
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_OPERATION" });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_ARGUMENT" });
        }
    }

    /// <summary>
    /// Executes an async operation that returns a value and handles common exceptions.
    /// </summary>
    public static async Task<HttpResult> ExecuteAsync<T>(
        Func<Task<T?>> operation,
        Func<T, HttpResult>? onSuccess = null) where T : class
    {
        try
        {
            var result = await operation();
            if (result == null)
            {
                return Results.NotFound(new ApiError { Error = "Resource not found", Code = "NOT_FOUND" });
            }

            return onSuccess != null ? onSuccess(result) : Results.Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(
                new ApiError { Error = ex.Message, Code = "UNAUTHORIZED" },
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_OPERATION" });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_ARGUMENT" });
        }
    }

    /// <summary>
    /// Executes an async operation that creates a resource.
    /// </summary>
    public static async Task<HttpResult> ExecuteCreateAsync<T>(
        Func<Task<T>> operation,
        Func<T, string> getLocation) where T : class
    {
        try
        {
            var result = await operation();
            return Results.Created(getLocation(result), result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(
                new ApiError { Error = ex.Message, Code = "UNAUTHORIZED" },
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_OPERATION" });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_ARGUMENT" });
        }
    }

    /// <summary>
    /// Executes an async void operation (delete, update without return).
    /// </summary>
    public static async Task<HttpResult> ExecuteNoContentAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Results.Json(
                new ApiError { Error = ex.Message, Code = "UNAUTHORIZED" },
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_OPERATION" });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ApiError { Error = ex.Message, Code = "INVALID_ARGUMENT" });
        }
    }

    #region Result<T> Extensions (Toolsfactory.Common.Result)

    /// <summary>
    /// Converts a Result&lt;T&gt; to an HTTP response.
    /// </summary>
    public static HttpResult ToResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return MapErrorsToResponse(result.Errors);
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to an HTTP created response.
    /// </summary>
    public static HttpResult ToCreatedResponse<T>(this Result<T> result, Func<T, string> getLocation)
    {
        if (result.IsSuccess)
        {
            return Results.Created(getLocation(result.Value), result.Value);
        }

        return MapErrorsToResponse(result.Errors);
    }

    /// <summary>
    /// Converts a Result to an HTTP no-content response.
    /// </summary>
    public static HttpResult ToNoContentResponse(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MapErrorsToResponse(result.Errors);
    }

    /// <summary>
    /// Standard error codes used with Toolsfactory.Common.Result.
    /// </summary>
    public static class ErrorCodes
    {
        public const int NotFound = 404;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int ValidationError = 400;
        public const int InvalidOperation = 422;
        public const int Conflict = 409;
        public const int InternalError = 500;
    }

    /// <summary>
    /// Maps errors to the appropriate HTTP response.
    /// </summary>
    private static HttpResult MapErrorsToResponse(IReadOnlyCollection<Error> errors)
    {
        var firstError = errors.FirstOrDefault();
        if (firstError == null)
        {
            return Results.Json(
                new ApiError { Error = "An unknown error occurred", Code = "UNKNOWN_ERROR" },
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var apiError = new ApiError
        {
            Error = firstError.Message,
            Code = firstError.Code.ToString(),
            Details = errors.Count > 1
                ? new Dictionary<string, string[]> { ["additionalErrors"] = errors.Skip(1).Select(e => e.Message).ToArray() }
                : null
        };

        return firstError.Code switch
        {
            ErrorCodes.NotFound => Results.NotFound(apiError),
            ErrorCodes.Unauthorized => Results.Unauthorized(),
            ErrorCodes.Forbidden => Results.Json(apiError, statusCode: StatusCodes.Status403Forbidden),
            ErrorCodes.ValidationError => Results.BadRequest(apiError),
            ErrorCodes.InvalidOperation => Results.BadRequest(apiError),
            ErrorCodes.Conflict => Results.Conflict(apiError),
            _ => Results.Json(apiError, statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    #endregion
}
