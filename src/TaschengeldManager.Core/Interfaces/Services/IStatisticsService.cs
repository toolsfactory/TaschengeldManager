using TaschengeldManager.Core.DTOs.Statistics;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for statistics and analytics operations.
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// S090: Get expenses by category for a child.
    /// </summary>
    Task<ExpensesByCategoryResponse> GetExpensesByCategoryAsync(
        UserId userId,
        AccountId accountId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// S091: Get balance history for a child.
    /// </summary>
    Task<BalanceHistoryResponse> GetBalanceHistoryAsync(
        UserId userId,
        AccountId accountId,
        string period,
        string granularity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// S092: Get month comparison for a child.
    /// </summary>
    Task<MonthComparisonResponse> GetMonthComparisonAsync(
        UserId userId,
        AccountId accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// S093: Get family dashboard for parents.
    /// </summary>
    Task<FamilyDashboardResponse> GetFamilyDashboardAsync(
        UserId parentUserId,
        FamilyId familyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// S094: Get income/expenses for a child (parent view).
    /// </summary>
    Task<IncomeExpensesResponse> GetIncomeExpensesAsync(
        UserId parentUserId,
        AccountId accountId,
        int months,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// S095: Get family expenses by category for parents.
    /// </summary>
    Task<FamilyCategoryAnalysisResponse> GetFamilyCategoryAnalysisAsync(
        UserId parentUserId,
        FamilyId familyId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expenses by category for the current user's account (child view).
    /// </summary>
    Task<ExpensesByCategoryResponse> GetMyExpensesByCategoryAsync(
        UserId userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get balance history for the current user's account (child view).
    /// </summary>
    Task<BalanceHistoryResponse> GetMyBalanceHistoryAsync(
        UserId userId,
        string period,
        string granularity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get month comparison for the current user's account (child view).
    /// </summary>
    Task<MonthComparisonResponse> GetMyMonthComparisonAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
