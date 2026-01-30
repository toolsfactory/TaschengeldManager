using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Statistics;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatisticsService> _logger;

    private const string DefaultCategory = "Sonstiges";

    public StatisticsService(
        IUnitOfWork unitOfWork,
        ILogger<StatisticsService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ExpensesByCategoryResponse> GetExpensesByCategoryAsync(
        UserId userId,
        AccountId accountId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        if (!await HasAccountAccessAsync(userId, account, cancellationToken))
        {
            throw new UnauthorizedAccessException("No access to this account");
        }

        return await GetExpensesByCategoryInternalAsync(accountId, from, to, cancellationToken);
    }

    public async Task<ExpensesByCategoryResponse> GetMyExpensesByCategoryAsync(
        UserId userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        return await GetExpensesByCategoryInternalAsync(account.Id, from, to, cancellationToken);
    }

    private async Task<ExpensesByCategoryResponse> GetExpensesByCategoryInternalAsync(
        AccountId accountId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        var fromDateTime = from.ToDateTime(TimeOnly.MinValue);
        var toDateTime = to.ToDateTime(TimeOnly.MaxValue);

        var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(accountId, fromDateTime, toDateTime, cancellationToken);

        var expenses = transactions
            .Where(t => t.Amount < 0)
            .ToList();

        var total = Math.Abs(expenses.Sum(t => t.Amount));

        var categoryGroups = expenses
            .GroupBy(t => t.Category ?? DefaultCategory)
            .Select(g => new CategoryExpenseItem
            {
                Name = g.Key,
                Amount = Math.Abs(g.Sum(t => t.Amount)),
                Percentage = total > 0 ? Math.Round(Math.Abs(g.Sum(t => t.Amount)) / total * 100, 1) : 0
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        return new ExpensesByCategoryResponse
        {
            Period = new PeriodInfo { From = from, To = to },
            Total = total,
            Categories = categoryGroups
        };
    }

    public async Task<BalanceHistoryResponse> GetBalanceHistoryAsync(
        UserId userId,
        AccountId accountId,
        string period,
        string granularity,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        if (!await HasAccountAccessAsync(userId, account, cancellationToken))
        {
            throw new UnauthorizedAccessException("No access to this account");
        }

        return await GetBalanceHistoryInternalAsync(accountId, period, granularity, cancellationToken);
    }

    public async Task<BalanceHistoryResponse> GetMyBalanceHistoryAsync(
        UserId userId,
        string period,
        string granularity,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        return await GetBalanceHistoryInternalAsync(account.Id, period, granularity, cancellationToken);
    }

    private async Task<BalanceHistoryResponse> GetBalanceHistoryInternalAsync(
        AccountId accountId,
        string period,
        string granularity,
        CancellationToken cancellationToken)
    {
        var (fromDateTime, toDateTime) = GetPeriodDates(period);

        var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(accountId, fromDateTime, toDateTime, cancellationToken);
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);

        var dataPoints = GenerateBalanceDataPoints(transactions, account!.Balance, fromDateTime, toDateTime, granularity);

        var trend = CalculateTrend(dataPoints);
        var changePercent = CalculateChangePercent(dataPoints);

        return new BalanceHistoryResponse
        {
            Period = period,
            DataPoints = dataPoints,
            Trend = trend,
            ChangePercent = changePercent
        };
    }

    public async Task<MonthComparisonResponse> GetMonthComparisonAsync(
        UserId userId,
        AccountId accountId,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        if (!await HasAccountAccessAsync(userId, account, cancellationToken))
        {
            throw new UnauthorizedAccessException("No access to this account");
        }

        return await GetMonthComparisonInternalAsync(accountId, cancellationToken);
    }

    public async Task<MonthComparisonResponse> GetMyMonthComparisonAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        return await GetMonthComparisonInternalAsync(account.Id, cancellationToken);
    }

    private async Task<MonthComparisonResponse> GetMonthComparisonInternalAsync(
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddSeconds(-1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var previousMonthEnd = currentMonthStart.AddSeconds(-1);

        var currentTransactions = await _unitOfWork.Transactions.GetByDateRangeAsync(
            accountId, currentMonthStart, currentMonthEnd, cancellationToken);
        var previousTransactions = await _unitOfWork.Transactions.GetByDateRangeAsync(
            accountId, previousMonthStart, previousMonthEnd, cancellationToken);

        var currentSummary = CalculateMonthlySummary(currentTransactions, currentMonthStart);
        var previousSummary = CalculateMonthlySummary(previousTransactions, previousMonthStart);

        var expenseChange = previousSummary.Expenses > 0
            ? Math.Round((currentSummary.Expenses - previousSummary.Expenses) / previousSummary.Expenses * 100, 1)
            : 0;
        var incomeChange = previousSummary.Income > 0
            ? Math.Round((currentSummary.Income - previousSummary.Income) / previousSummary.Income * 100, 1)
            : 0;

        return new MonthComparisonResponse
        {
            CurrentMonth = currentSummary,
            PreviousMonth = previousSummary,
            ExpenseChange = expenseChange,
            IncomeChange = incomeChange
        };
    }

    public async Task<FamilyDashboardResponse> GetFamilyDashboardAsync(
        UserId parentUserId,
        FamilyId familyId,
        CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            throw new UnauthorizedAccessException("No access to this family");
        }

        var accounts = await _unitOfWork.Accounts.GetByFamilyAsync(familyId, cancellationToken);
        var accountIds = accounts.Select(a => a.Id).ToList();

        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddSeconds(-1);
        var previousMonthEnd = currentMonthStart.AddSeconds(-1);

        var currentTransactions = await _unitOfWork.Transactions.GetByAccountsAndDateRangeAsync(
            accountIds, currentMonthStart, currentMonthEnd, cancellationToken);
        var previousTransactions = await _unitOfWork.Transactions.GetByAccountsAndDateRangeAsync(
            accountIds, previousMonthStart, previousMonthEnd, cancellationToken);

        var children = new List<ChildSummary>();
        foreach (var account in accounts)
        {
            var currentAccountTxs = currentTransactions.Where(t => t.AccountId == account.Id).ToList();
            var previousAccountTxs = previousTransactions.Where(t => t.AccountId == account.Id).ToList();

            var currentExpenses = Math.Abs(currentAccountTxs.Where(t => t.Amount < 0).Sum(t => t.Amount));
            var previousBalance = account.Balance - currentAccountTxs.Sum(t => t.Amount);
            var balanceChange = previousBalance > 0
                ? Math.Round((account.Balance - previousBalance) / previousBalance * 100, 1)
                : 0;

            children.Add(new ChildSummary
            {
                ChildId = account.UserId.Value,
                Name = account.User?.Nickname ?? "Unknown",
                Balance = account.Balance,
                BalanceChange = balanceChange,
                ExpensesThisMonth = currentExpenses
            });
        }

        return new FamilyDashboardResponse
        {
            FamilyName = family.Name,
            TotalBalance = accounts.Sum(a => a.Balance),
            TotalExpensesThisMonth = Math.Abs(currentTransactions.Where(t => t.Amount < 0).Sum(t => t.Amount)),
            Children = children
        };
    }

    public async Task<IncomeExpensesResponse> GetIncomeExpensesAsync(
        UserId parentUserId,
        AccountId accountId,
        int months,
        CancellationToken cancellationToken = default)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (owner?.FamilyId == null)
        {
            throw new InvalidOperationException("Account owner not in a family");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(owner.FamilyId.Value, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            throw new UnauthorizedAccessException("No access to this account");
        }

        var now = DateTime.UtcNow;
        var fromDate = new DateTime(now.Year, now.Month, 1).AddMonths(-months + 1);
        var toDate = now;

        var transactions = await _unitOfWork.Transactions.GetByDateRangeAsync(accountId, fromDate, toDate, cancellationToken);

        var monthlyData = new List<MonthlyIncomeExpense>();
        for (int i = 0; i < months; i++)
        {
            var monthStart = fromDate.AddMonths(i);
            var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);
            var monthTransactions = transactions
                .Where(t => t.CreatedAt >= monthStart && t.CreatedAt <= monthEnd)
                .ToList();

            var income = CalculateIncomeBreakdown(monthTransactions);
            var expenses = new ExpenseBreakdown
            {
                Total = Math.Abs(monthTransactions.Where(t => t.Amount < 0).Sum(t => t.Amount))
            };

            monthlyData.Add(new MonthlyIncomeExpense
            {
                Month = monthStart.ToString("yyyy-MM"),
                Income = income,
                Expenses = expenses,
                NetBalance = income.Total - expenses.Total
            });
        }

        return new IncomeExpensesResponse
        {
            ChildId = account.UserId.Value,
            ChildName = owner.Nickname,
            Months = monthlyData
        };
    }

    public async Task<FamilyCategoryAnalysisResponse> GetFamilyCategoryAnalysisAsync(
        UserId parentUserId,
        FamilyId familyId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == parentUserId))
        {
            throw new UnauthorizedAccessException("No access to this family");
        }

        var accounts = await _unitOfWork.Accounts.GetByFamilyAsync(familyId, cancellationToken);
        var accountIds = accounts.Select(a => a.Id).ToList();

        var fromDateTime = from.ToDateTime(TimeOnly.MinValue);
        var toDateTime = to.ToDateTime(TimeOnly.MaxValue);

        var transactions = await _unitOfWork.Transactions.GetByAccountsAndDateRangeAsync(
            accountIds, fromDateTime, toDateTime, cancellationToken);

        var expenses = transactions.Where(t => t.Amount < 0).ToList();
        var totalExpenses = Math.Abs(expenses.Sum(t => t.Amount));

        var categoryGroups = expenses
            .GroupBy(t => t.Category ?? DefaultCategory)
            .Select(g => new CategoryWithChildBreakdown
            {
                Name = g.Key,
                Total = Math.Abs(g.Sum(t => t.Amount)),
                ByChild = g
                    .GroupBy(t => t.AccountId)
                    .Select(cg =>
                    {
                        var account = accounts.First(a => a.Id == cg.Key);
                        return new ChildCategoryExpense
                        {
                            ChildId = account.UserId.Value,
                            Name = account.User?.Nickname ?? "Unknown",
                            Amount = Math.Abs(cg.Sum(t => t.Amount))
                        };
                    })
                    .OrderByDescending(c => c.Amount)
                    .ToList()
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return new FamilyCategoryAnalysisResponse
        {
            Period = new PeriodInfo { From = from, To = to },
            TotalExpenses = totalExpenses,
            Categories = categoryGroups
        };
    }

    private static MonthlySummary CalculateMonthlySummary(IEnumerable<Transaction> transactions, DateTime monthStart)
    {
        var txList = transactions.ToList();
        return new MonthlySummary
        {
            Month = monthStart.ToString("yyyy-MM"),
            Expenses = Math.Abs(txList.Where(t => t.Amount < 0).Sum(t => t.Amount)),
            Income = txList.Where(t => t.Amount > 0).Sum(t => t.Amount)
        };
    }

    private static IncomeBreakdown CalculateIncomeBreakdown(IEnumerable<Transaction> transactions)
    {
        var income = transactions.Where(t => t.Amount > 0).ToList();
        return new IncomeBreakdown
        {
            Total = income.Sum(t => t.Amount),
            Allowance = income.Where(t => t.Type == TransactionType.Allowance).Sum(t => t.Amount),
            Gifts = income.Where(t => t.Type == TransactionType.Gift).Sum(t => t.Amount),
            Interest = income.Where(t => t.Type == TransactionType.Interest).Sum(t => t.Amount)
        };
    }

    private static (DateTime from, DateTime to) GetPeriodDates(string period)
    {
        var now = DateTime.UtcNow;
        return period.ToLower() switch
        {
            "3months" => (now.AddMonths(-3), now),
            "6months" => (now.AddMonths(-6), now),
            "1year" or "12months" => (now.AddYears(-1), now),
            _ => (now.AddMonths(-6), now)
        };
    }

    private static List<BalanceDataPoint> GenerateBalanceDataPoints(
        IReadOnlyList<Transaction> transactions,
        decimal currentBalance,
        DateTime from,
        DateTime to,
        string granularity)
    {
        var dataPoints = new List<BalanceDataPoint>();
        var orderedTransactions = transactions.OrderByDescending(t => t.CreatedAt).ToList();

        var interval = granularity.ToLower() switch
        {
            "daily" => TimeSpan.FromDays(1),
            "weekly" => TimeSpan.FromDays(7),
            "monthly" => TimeSpan.FromDays(30),
            _ => TimeSpan.FromDays(7)
        };

        var balance = currentBalance;
        var date = to;

        while (date >= from)
        {
            dataPoints.Add(new BalanceDataPoint
            {
                Date = DateOnly.FromDateTime(date),
                Balance = balance
            });

            var nextDate = date.Add(-interval);
            var transactionsInPeriod = orderedTransactions
                .Where(t => t.CreatedAt <= date && t.CreatedAt > nextDate)
                .Sum(t => t.Amount);

            balance -= transactionsInPeriod;
            date = nextDate;
        }

        dataPoints.Reverse();
        return dataPoints;
    }

    private static string CalculateTrend(List<BalanceDataPoint> dataPoints)
    {
        if (dataPoints.Count < 2)
            return "stable";

        var first = dataPoints.First().Balance;
        var last = dataPoints.Last().Balance;

        if (last > first * 1.05m)
            return "increasing";
        if (last < first * 0.95m)
            return "decreasing";

        return "stable";
    }

    private static decimal CalculateChangePercent(List<BalanceDataPoint> dataPoints)
    {
        if (dataPoints.Count < 2)
            return 0;

        var first = dataPoints.First().Balance;
        var last = dataPoints.Last().Balance;

        if (first == 0)
            return 0;

        return Math.Round((last - first) / first * 100, 1);
    }

    private async Task<bool> HasAccountAccessAsync(UserId userId, Account account, CancellationToken cancellationToken)
    {
        if (account.UserId == userId)
            return true;

        var owner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (owner?.FamilyId == null)
            return false;

        var family = await _unitOfWork.Families.GetWithMembersAsync(owner.FamilyId.Value, cancellationToken);
        if (family == null)
            return false;

        return family.Parents.Any(p => p.UserId == userId) ||
               family.Relatives.Any(r => r.UserId == userId);
    }
}
