using System.Security.Claims;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Core.DTOs.MoneyRequest;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Money request management endpoints.
/// </summary>
public static class MoneyRequestEndpoints
{
    public static IEndpointRouteBuilder MapMoneyRequestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/money-requests")
            .WithTags("MoneyRequests")

            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetById)
            .WithSummary("Get a specific money request by ID")
            .Produces<MoneyRequestDto>(200)
            .Produces(404);

        group.MapGet("/my", GetMyRequests)
            .WithSummary("Get all money requests for the current child")
            .RequireAuthorization(policy => policy.RequireRole("Child"))
            .Produces<IReadOnlyList<MoneyRequestDto>>(200);

        group.MapGet("/family", GetFamilyRequests)
            .WithSummary("Get all money requests for the parent's family")
            .RequireAuthorization(policy => policy.RequireRole("Parent"))
            .Produces<IReadOnlyList<MoneyRequestDto>>(200);

        group.MapPost("/", Create)
            .WithSummary("Create a new money request (by child)")
            .RequireAuthorization(policy => policy.RequireRole("Child"))
            .Produces<MoneyRequestDto>(201)
            .Produces(400);

        group.MapPost("/{id:guid}/respond", Respond)
            .WithSummary("Respond to a money request (by parent)")
            .RequireAuthorization(policy => policy.RequireRole("Parent"))
            .Produces<MoneyRequestDto>(200)
            .Produces(400)
            .Produces(404);

        group.MapPost("/{id:guid}/withdraw", Withdraw)
            .WithSummary("Withdraw a pending money request (by child)")
            .RequireAuthorization(policy => policy.RequireRole("Child"))
            .Produces(204)
            .Produces(400)
            .Produces(404);

        return app;
    }

    private static async Task<IResult> GetById(
        Guid id,
        ClaimsPrincipal user,
        IMoneyRequestService moneyRequestService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var request = await moneyRequestService.GetByIdAsync(userId, new MoneyRequestId(id), cancellationToken);

        if (request == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(request);
    }

    private static async Task<IResult> GetMyRequests(
        ClaimsPrincipal user,
        IMoneyRequestService moneyRequestService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var requests = await moneyRequestService.GetMyRequestsAsync(userId, cancellationToken);
        return Results.Ok(requests);
    }

    private static async Task<IResult> GetFamilyRequests(
        ClaimsPrincipal user,
        IMoneyRequestService moneyRequestService,
        RequestStatus? status,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var requests = await moneyRequestService.GetFamilyRequestsAsync(userId, status, cancellationToken);
        return Results.Ok(requests);
    }

    private static async Task<IResult> Create(
        CreateMoneyRequestRequest request,
        ClaimsPrincipal user,
        IMoneyRequestService moneyRequestService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await moneyRequestService.CreateAsync(userId, request, cancellationToken);
            return Results.Created($"/api/money-requests/{result.Id}", result);
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

    private static async Task<IResult> Respond(
        Guid id,
        RespondToRequestRequest request,
        ClaimsPrincipal user,
        IMoneyRequestService moneyRequestService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            var result = await moneyRequestService.RespondAsync(userId, new MoneyRequestId(id), request, cancellationToken);
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

    private static async Task<IResult> Withdraw(
        Guid id,
        ClaimsPrincipal user,
        IMoneyRequestService moneyRequestService,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = user.GetUserId();
            await moneyRequestService.WithdrawAsync(userId, new MoneyRequestId(id), cancellationToken);
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
