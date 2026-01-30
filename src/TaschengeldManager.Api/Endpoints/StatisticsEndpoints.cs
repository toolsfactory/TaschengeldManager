using System.Security.Claims;
using TaschengeldManager.Api.Extensions;
using TaschengeldManager.Core.DTOs.Statistics;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Api.Endpoints;

/// <summary>
/// Statistics and analytics endpoints.
/// </summary>
public static class StatisticsEndpoints
{
    public static IEndpointRouteBuilder MapStatisticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/statistics")
            .WithTags("Statistics")
            .RequireAuthorization();

        // Child endpoints (own statistics)
        group.MapGet("/me/expenses-by-category", GetMyExpensesByCategory)
            .WithSummary("Get expenses by category for the current user (child)")
            .Produces<ExpensesByCategoryResponse>(200)
            .Produces(404);

        group.MapGet("/me/balance-history", GetMyBalanceHistory)
            .WithSummary("Get balance history for the current user (child)")
            .Produces<BalanceHistoryResponse>(200)
            .Produces(404);

        group.MapGet("/me/month-comparison", GetMyMonthComparison)
            .WithSummary("Get month comparison for the current user (child)")
            .Produces<MonthComparisonResponse>(200)
            .Produces(404);

        // Account-specific endpoints (for parents)
        group.MapGet("/accounts/{accountId:guid}/expenses-by-category", GetExpensesByCategory)
            .WithSummary("Get expenses by category for an account (S090)")
            .Produces<ExpensesByCategoryResponse>(200)
            .Produces(403)
            .Produces(404);

        group.MapGet("/accounts/{accountId:guid}/balance-history", GetBalanceHistory)
            .WithSummary("Get balance history for an account (S091)")
            .Produces<BalanceHistoryResponse>(200)
            .Produces(403)
            .Produces(404);

        group.MapGet("/accounts/{accountId:guid}/month-comparison", GetMonthComparison)
            .WithSummary("Get month comparison for an account (S092)")
            .Produces<MonthComparisonResponse>(200)
            .Produces(403)
            .Produces(404);

        group.MapGet("/accounts/{accountId:guid}/income-expenses", GetIncomeExpenses)
            .WithSummary("Get income/expenses for an account (S094)")
            .Produces<IncomeExpensesResponse>(200)
            .Produces(403)
            .Produces(404);

        // Family endpoints (for parents)
        group.MapGet("/family/{familyId:guid}/dashboard", GetFamilyDashboard)
            .WithSummary("Get family dashboard (S093)")
            .Produces<FamilyDashboardResponse>(200)
            .Produces(403);

        group.MapGet("/family/{familyId:guid}/expenses-by-category", GetFamilyCategoryAnalysis)
            .WithSummary("Get family expenses by category (S095)")
            .Produces<FamilyCategoryAnalysisResponse>(200)
            .Produces(403);

        return app;
    }

    private static async Task<IResult> GetMyExpensesByCategory(
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var fromDate = from ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
        var toDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetMyExpensesByCategoryAsync(userId, fromDate, toDate, cancellationToken));
    }

    private static async Task<IResult> GetMyBalanceHistory(
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        string? period,
        string? granularity,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var periodValue = period ?? "6months";
        var granularityValue = granularity ?? "weekly";

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetMyBalanceHistoryAsync(userId, periodValue, granularityValue, cancellationToken));
    }

    private static async Task<IResult> GetMyMonthComparison(
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetMyMonthComparisonAsync(userId, cancellationToken));
    }

    private static async Task<IResult> GetExpensesByCategory(
        Guid accountId,
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var fromDate = from ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
        var toDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetExpensesByCategoryAsync(
                userId, new AccountId(accountId), fromDate, toDate, cancellationToken));
    }

    private static async Task<IResult> GetBalanceHistory(
        Guid accountId,
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        string? period,
        string? granularity,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var periodValue = period ?? "6months";
        var granularityValue = granularity ?? "weekly";

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetBalanceHistoryAsync(
                userId, new AccountId(accountId), periodValue, granularityValue, cancellationToken));
    }

    private static async Task<IResult> GetMonthComparison(
        Guid accountId,
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetMonthComparisonAsync(
                userId, new AccountId(accountId), cancellationToken));
    }

    private static async Task<IResult> GetIncomeExpenses(
        Guid accountId,
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        int? months,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var monthsValue = months ?? 6;

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetIncomeExpensesAsync(
                userId, new AccountId(accountId), monthsValue, cancellationToken));
    }

    private static async Task<IResult> GetFamilyDashboard(
        Guid familyId,
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetFamilyDashboardAsync(
                userId, new FamilyId(familyId), cancellationToken));
    }

    private static async Task<IResult> GetFamilyCategoryAnalysis(
        Guid familyId,
        ClaimsPrincipal user,
        IStatisticsService statisticsService,
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var fromDate = from ?? DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
        var toDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return await ResultExtensions.ExecuteAsync(
            async () => await statisticsService.GetFamilyCategoryAnalysisAsync(
                userId, new FamilyId(familyId), fromDate, toDate, cancellationToken));
    }
}
