using System.Security.Claims;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Core.DTOs.RecurringPayment;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Recurring payment management endpoints.
/// </summary>
public static class RecurringPaymentEndpoints
{
    public static IEndpointRouteBuilder MapRecurringPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recurring-payments")
            .WithTags("RecurringPayments")

            .RequireAuthorization(policy => policy.RequireRole("Parent"));

        group.MapGet("/", GetAll)
            .WithSummary("Get all recurring payments for the current user")
            .Produces<IReadOnlyList<RecurringPaymentDto>>(200);

        group.MapGet("/{id:guid}", GetById)
            .WithSummary("Get a specific recurring payment by ID")
            .Produces<RecurringPaymentDto>(200)
            .Produces(404);

        group.MapPost("/", Create)
            .WithSummary("Create a new recurring payment")
            .Produces<RecurringPaymentDto>(201)
            .Produces(400);

        group.MapPut("/{id:guid}", Update)
            .WithSummary("Update an existing recurring payment")
            .Produces<RecurringPaymentDto>(200)
            .Produces(400)
            .Produces(404);

        group.MapPost("/{id:guid}/pause", Pause)
            .WithSummary("Pause a recurring payment")
            .Produces(204)
            .Produces(404);

        group.MapPost("/{id:guid}/resume", Resume)
            .WithSummary("Resume a paused recurring payment")
            .Produces(204)
            .Produces(404);

        group.MapDelete("/{id:guid}", Delete)
            .WithSummary("Delete a recurring payment")
            .Produces(204)
            .Produces(404);

        return app;
    }

    private static async Task<IResult> GetAll(
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var payments = await recurringPaymentService.GetAllForUserAsync(userId, cancellationToken);
        return Results.Ok(payments);
    }

    private static async Task<IResult> GetById(
        Guid id,
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var payment = await recurringPaymentService.GetByIdAsync(userId, new RecurringPaymentId(id), cancellationToken);

        if (payment == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(payment);
    }

    private static async Task<IResult> Create(
        CreateRecurringPaymentRequest request,
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await recurringPaymentService.CreateAsync(userId, request, cancellationToken);
            return Results.Created($"/api/recurring-payments/{result.Id}", result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Update(
        Guid id,
        UpdateRecurringPaymentRequest request,
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await recurringPaymentService.UpdateAsync(userId, new RecurringPaymentId(id), request, cancellationToken);
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Pause(
        Guid id,
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await recurringPaymentService.PauseAsync(userId, new RecurringPaymentId(id), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Resume(
        Guid id,
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await recurringPaymentService.ResumeAsync(userId, new RecurringPaymentId(id), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> Delete(
        Guid id,
        ClaimsPrincipal user,
        IRecurringPaymentService recurringPaymentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await recurringPaymentService.DeleteAsync(userId, new RecurringPaymentId(id), cancellationToken);
            return Results.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}
