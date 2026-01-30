using FluentValidation;

namespace TaschengeldManager.Api.Filters;

/// <summary>
/// Endpoint filter that validates request bodies using FluentValidation.
/// </summary>
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Find the argument of type T
        var argument = context.Arguments.FirstOrDefault(a => a is T) as T;

        if (argument == null)
        {
            return Results.BadRequest(new { error = "Request body is required" });
        }

        var validationResult = await _validator.ValidateAsync(argument);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}

/// <summary>
/// Extension methods for adding validation to endpoints.
/// </summary>
public static class ValidationFilterExtensions
{
    /// <summary>
    /// Adds FluentValidation to the endpoint for the specified request type.
    /// </summary>
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter<ValidationFilter<T>>();
    }
}
