using System.Security.Claims;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Api.Filters;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Account management endpoints.
/// </summary>
public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account")
            .WithTags("Account")

            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetAccount)
            .WithSummary("Get an account by ID")
            .Produces<AccountDto>(200)
            .Produces(404);

        group.MapGet("/me", GetMyAccount)
            .WithSummary("Get the current user's account (for children)")
            .Produces<AccountDto>(200)
            .Produces(404);

        group.MapGet("/family/{familyId:guid}", GetFamilyAccounts)
            .WithSummary("Get all accounts for a family (for parents)")
            .Produces<IReadOnlyList<AccountDto>>(200);

        group.MapPost("/{accountId:guid}/deposit", Deposit)
            .WithSummary("Deposit money into an account (by parent)")
            .WithValidation<DepositRequest>()
            .Produces<TransactionDto>(201)
            .Produces(400)
            .Produces(403);

        group.MapPost("/withdraw", Withdraw)
            .WithSummary("Withdraw/spend money from account (by child)")
            .Produces<TransactionDto>(201)
            .Produces(400);

        group.MapPost("/gift", Gift)
            .WithSummary("Gift money to a child (by parent or relative)")
            .Produces<TransactionDto>(201)
            .Produces(400)
            .Produces(403);

        group.MapGet("/transactions", GetMyTransactions)
            .WithSummary("Get transactions for the current user's account (for children)")
            .Produces<IReadOnlyList<TransactionDto>>(200);

        group.MapGet("/{accountId:guid}/transactions", GetTransactions)
            .WithSummary("Get transactions for an account")
            .Produces<IReadOnlyList<TransactionDto>>(200);

        group.MapGet("/gifts", GetMyGifts)
            .WithSummary("Get own gifts (for relatives)")
            .Produces<IReadOnlyList<TransactionDto>>(200);

        group.MapPut("/{accountId:guid}/interest", SetInterest)
            .WithSummary("Set interest settings for an account (by parent)")
            .Produces<AccountDto>(200)
            .Produces(400)
            .Produces(403);

        return app;
    }

    private static async Task<IResult> GetAccount(
        Guid id,
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        return await ResultExtensions.ExecuteAsync(
            async () => await accountService.GetAccountAsync(userId, new AccountId(id), cancellationToken));
    }

    private static async Task<IResult> GetMyAccount(
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        return await ResultExtensions.ExecuteAsync(
            async () => await accountService.GetMyAccountAsync(userId, cancellationToken));
    }

    private static async Task<IResult> GetFamilyAccounts(
        Guid familyId,
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var accounts = await accountService.GetFamilyAccountsAsync(userId, new FamilyId(familyId), cancellationToken);
        return Results.Ok(accounts);
    }

    private static async Task<IResult> Deposit(
        Guid accountId,
        DepositRequest request,
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        return await ResultExtensions.ExecuteCreateAsync(
            async () => await accountService.DepositAsync(userId, new AccountId(accountId), request, cancellationToken),
            _ => $"/api/account/{accountId}/transactions");
    }

    private static async Task<IResult> Withdraw(
        WithdrawRequest request,
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        return await ResultExtensions.ExecuteCreateAsync(
            async () => await accountService.WithdrawAsync(userId, request, cancellationToken),
            _ => "/api/account/me");
    }

    private static async Task<IResult> Gift(
        GiftRequest request,
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        return await ResultExtensions.ExecuteCreateAsync(
            async () => await accountService.GiftAsync(userId, request, cancellationToken),
            _ => $"/api/account/{request.AccountId}");
    }

    private static async Task<IResult> GetMyTransactions(
        ClaimsPrincipal user,
        IAccountService accountService,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var limit = pageSize ?? 20;
        var offset = ((page ?? 1) - 1) * limit;
        var transactions = await accountService.GetMyTransactionsAsync(userId, limit, offset, cancellationToken);
        return Results.Ok(transactions);
    }

    private static async Task<IResult> GetTransactions(
        Guid accountId,
        ClaimsPrincipal user,
        IAccountService accountService,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var transactions = await accountService.GetTransactionsAsync(userId, new AccountId(accountId), limit, offset, cancellationToken);
        return Results.Ok(transactions);
    }

    private static async Task<IResult> GetMyGifts(
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var gifts = await accountService.GetMyGiftsAsync(userId, cancellationToken);
        return Results.Ok(gifts);
    }

    private static async Task<IResult> SetInterest(
        Guid accountId,
        SetInterestRequest request,
        ClaimsPrincipal user,
        IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        return await ResultExtensions.ExecuteAsync(async () =>
        {
            var result = await accountService.SetInterestAsync(userId, new AccountId(accountId), request, cancellationToken);
            return Results.Ok(result);
        });
    }
}
