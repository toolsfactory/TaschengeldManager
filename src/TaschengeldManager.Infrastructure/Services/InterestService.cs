using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service for calculating and crediting interest on accounts.
/// </summary>
public class InterestService : IInterestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InterestService> _logger;

    public InterestService(
        IUnitOfWork unitOfWork,
        ILogger<InterestService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> ProcessDueInterestAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _unitOfWork.Accounts.GetWithInterestEnabledAsync(cancellationToken);
        var processedCount = 0;
        var now = DateTime.UtcNow;

        foreach (var account in accounts)
        {
            if (!IsInterestDue(account, now))
            {
                continue;
            }

            var interest = CalculateInterest(account);
            if (interest <= 0)
            {
                continue;
            }

            await CreditInterestAsync(account, interest, now, cancellationToken);
            processedCount++;
        }

        if (processedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Credited interest to {Count} accounts", processedCount);
        }

        return processedCount;
    }

    /// <summary>
    /// Check if interest is due based on the interval setting.
    /// </summary>
    private bool IsInterestDue(Account account, DateTime now)
    {
        var lastCalculation = account.LastInterestCalculation ?? account.CreatedAt;

        return account.InterestInterval switch
        {
            InterestInterval.Monthly => IsMonthEndDue(lastCalculation, now),
            InterestInterval.Yearly => IsYearEndDue(lastCalculation, now),
            _ => false
        };
    }

    /// <summary>
    /// Check if monthly interest is due (last day of month or first day after month end).
    /// </summary>
    private static bool IsMonthEndDue(DateTime lastCalculation, DateTime now)
    {
        // Interest is due if we're past the month end and haven't calculated for this period yet
        var lastCalculationMonth = new DateTime(lastCalculation.Year, lastCalculation.Month, 1);
        var currentMonth = new DateTime(now.Year, now.Month, 1);

        // Check if we're in a new month compared to last calculation
        return currentMonth > lastCalculationMonth;
    }

    /// <summary>
    /// Check if yearly interest is due (after year end).
    /// </summary>
    private static bool IsYearEndDue(DateTime lastCalculation, DateTime now)
    {
        // Interest is due if we're past the year end and haven't calculated for this year yet
        return now.Year > lastCalculation.Year;
    }

    /// <summary>
    /// Calculate interest based on the account settings.
    /// Formula: Interest = Balance × (Rate / 100) × (1 / Periods per year)
    /// </summary>
    private decimal CalculateInterest(Account account)
    {
        if (account.Balance <= 0 || account.InterestRate == null || account.InterestRate <= 0)
        {
            return 0;
        }

        var rate = account.InterestRate.Value / 100m;
        var periodsPerYear = account.InterestInterval switch
        {
            InterestInterval.Monthly => 12m,
            InterestInterval.Yearly => 1m,
            _ => 12m
        };

        var interest = account.Balance * rate / periodsPerYear;

        // Round to 2 decimal places (always round down for interest)
        return Math.Floor(interest * 100) / 100;
    }

    /// <summary>
    /// Credit interest to the account and create a transaction.
    /// </summary>
    private async Task CreditInterestAsync(Account account, decimal interest, DateTime now, CancellationToken cancellationToken)
    {
        // Update account balance
        account.Balance += interest;
        account.LastInterestCalculation = now;

        // Determine the period description
        var periodDescription = account.InterestInterval switch
        {
            InterestInterval.Monthly => GetMonthlyPeriodDescription(now),
            InterestInterval.Yearly => GetYearlyPeriodDescription(now),
            _ => $"Zinsen {now:yyyy-MM}"
        };

        // Create interest transaction
        var transaction = new Transaction
        {
            AccountId = account.Id,
            Amount = interest,
            Type = TransactionType.Interest,
            Description = periodDescription,
            CreatedByUserId = null, // System-generated
            BalanceAfter = account.Balance
        };

        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);

        _logger.LogInformation(
            "Credited {Interest:F2} EUR interest to account {AccountId} ({OwnerName}). Rate: {Rate}%, Interval: {Interval}",
            interest,
            account.Id,
            account.User?.Nickname ?? "Unknown",
            account.InterestRate,
            account.InterestInterval);
    }

    /// <summary>
    /// Get the description for monthly interest (e.g., "Zinsen Januar 2025").
    /// </summary>
    private static string GetMonthlyPeriodDescription(DateTime now)
    {
        // Interest is credited for the previous month
        var previousMonth = now.AddMonths(-1);
        var monthName = GetGermanMonthName(previousMonth.Month);
        return $"Zinsen {monthName} {previousMonth.Year}";
    }

    /// <summary>
    /// Get the description for yearly interest (e.g., "Zinsen 2024").
    /// </summary>
    private static string GetYearlyPeriodDescription(DateTime now)
    {
        // Interest is credited for the previous year
        return $"Zinsen {now.Year - 1}";
    }

    /// <summary>
    /// Get German month name.
    /// </summary>
    private static string GetGermanMonthName(int month)
    {
        return month switch
        {
            1 => "Januar",
            2 => "Februar",
            3 => "März",
            4 => "April",
            5 => "Mai",
            6 => "Juni",
            7 => "Juli",
            8 => "August",
            9 => "September",
            10 => "Oktober",
            11 => "November",
            12 => "Dezember",
            _ => month.ToString()
        };
    }
}
