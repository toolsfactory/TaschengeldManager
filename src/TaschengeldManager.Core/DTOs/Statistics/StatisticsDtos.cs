namespace TaschengeldManager.Core.DTOs.Statistics;

/// <summary>
/// Period information for statistics queries.
/// </summary>
public record PeriodInfo
{
    public required DateOnly From { get; init; }
    public required DateOnly To { get; init; }
}

/// <summary>
/// Category expense item.
/// </summary>
public record CategoryExpenseItem
{
    public required string Name { get; init; }
    public required decimal Amount { get; init; }
    public required decimal Percentage { get; init; }
}

/// <summary>
/// S090: Expenses by category response for a child.
/// </summary>
public record ExpensesByCategoryResponse
{
    public required PeriodInfo Period { get; init; }
    public required decimal Total { get; init; }
    public required IReadOnlyList<CategoryExpenseItem> Categories { get; init; }
}

/// <summary>
/// Balance history data point.
/// </summary>
public record BalanceDataPoint
{
    public required DateOnly Date { get; init; }
    public required decimal Balance { get; init; }
}

/// <summary>
/// S091: Balance history response for a child.
/// </summary>
public record BalanceHistoryResponse
{
    public required string Period { get; init; }
    public required IReadOnlyList<BalanceDataPoint> DataPoints { get; init; }
    public required string Trend { get; init; }
    public required decimal ChangePercent { get; init; }
}

/// <summary>
/// Monthly summary for comparison.
/// </summary>
public record MonthlySummary
{
    public required string Month { get; init; }
    public required decimal Expenses { get; init; }
    public required decimal Income { get; init; }
}

/// <summary>
/// S092: Month comparison response for a child.
/// </summary>
public record MonthComparisonResponse
{
    public required MonthlySummary CurrentMonth { get; init; }
    public required MonthlySummary PreviousMonth { get; init; }
    public required decimal ExpenseChange { get; init; }
    public required decimal IncomeChange { get; init; }
}

/// <summary>
/// Child summary for family dashboard.
/// </summary>
public record ChildSummary
{
    public required Guid ChildId { get; init; }
    public required string Name { get; init; }
    public required decimal Balance { get; init; }
    public required decimal BalanceChange { get; init; }
    public required decimal ExpensesThisMonth { get; init; }
}

/// <summary>
/// S093: Family dashboard response for parents.
/// </summary>
public record FamilyDashboardResponse
{
    public required string FamilyName { get; init; }
    public required decimal TotalBalance { get; init; }
    public required decimal TotalExpensesThisMonth { get; init; }
    public required IReadOnlyList<ChildSummary> Children { get; init; }
}

/// <summary>
/// Income breakdown by type.
/// </summary>
public record IncomeBreakdown
{
    public required decimal Total { get; init; }
    public required decimal Allowance { get; init; }
    public required decimal Gifts { get; init; }
    public required decimal Interest { get; init; }
}

/// <summary>
/// Expense breakdown.
/// </summary>
public record ExpenseBreakdown
{
    public required decimal Total { get; init; }
}

/// <summary>
/// Monthly income/expense summary.
/// </summary>
public record MonthlyIncomeExpense
{
    public required string Month { get; init; }
    public required IncomeBreakdown Income { get; init; }
    public required ExpenseBreakdown Expenses { get; init; }
    public required decimal NetBalance { get; init; }
}

/// <summary>
/// S094: Income/expenses response for a child (parent view).
/// </summary>
public record IncomeExpensesResponse
{
    public required Guid ChildId { get; init; }
    public required string ChildName { get; init; }
    public required IReadOnlyList<MonthlyIncomeExpense> Months { get; init; }
}

/// <summary>
/// Child expense in a category.
/// </summary>
public record ChildCategoryExpense
{
    public required Guid ChildId { get; init; }
    public required string Name { get; init; }
    public required decimal Amount { get; init; }
}

/// <summary>
/// Category with breakdown by child.
/// </summary>
public record CategoryWithChildBreakdown
{
    public required string Name { get; init; }
    public required decimal Total { get; init; }
    public required IReadOnlyList<ChildCategoryExpense> ByChild { get; init; }
}

/// <summary>
/// S095: Family expenses by category response for parents.
/// </summary>
public record FamilyCategoryAnalysisResponse
{
    public required PeriodInfo Period { get; init; }
    public required decimal TotalExpenses { get; init; }
    public required IReadOnlyList<CategoryWithChildBreakdown> Categories { get; init; }
}
