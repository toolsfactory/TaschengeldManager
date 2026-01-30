using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.RecurringPayment;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service implementation for recurring payment management.
/// </summary>
public class RecurringPaymentService : IRecurringPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecurringPaymentService> _logger;

    public RecurringPaymentService(IUnitOfWork unitOfWork, ILogger<RecurringPaymentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RecurringPaymentDto> CreateAsync(UserId userId, CreateRecurringPaymentRequest request, CancellationToken cancellationToken = default)
    {
        // Verify account exists and user has access
        var accountId = new AccountId(request.AccountId);
        var account = await _unitOfWork.Accounts.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        var accountOwner = await _unitOfWork.Users.GetByIdAsync(account.UserId, cancellationToken);
        if (accountOwner == null || accountOwner.FamilyId == null)
        {
            throw new InvalidOperationException("Account owner not found");
        }

        var family = await _unitOfWork.Families.GetWithMembersAsync(accountOwner.FamilyId.Value, cancellationToken);
        if (family == null || !family.Parents.Any(p => p.UserId == userId))
        {
            throw new UnauthorizedAccessException("Only parents can create recurring payments");
        }

        // Validate request
        ValidateRequest(request);

        // Calculate next execution date
        var nextExecutionDate = CalculateNextExecutionDate(
            request.StartDate ?? DateTime.UtcNow,
            request.Interval,
            request.DayOfWeek,
            request.DayOfMonth);

        var payment = new RecurringPayment
        {
            Id = new RecurringPaymentId(Guid.NewGuid()),
            AccountId = accountId,
            Amount = request.Amount,
            Interval = request.Interval,
            DayOfWeek = request.DayOfWeek,
            DayOfMonth = request.DayOfMonth,
            NextExecutionDate = nextExecutionDate,
            IsActive = true,
            Description = request.Description ?? "Taschengeld",
            CreatedByUserId = userId
        };

        await _unitOfWork.RecurringPayments.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Recurring payment {PaymentId} created for account {AccountId} by user {UserId}",
            payment.Id.Value, accountId.Value, userId.Value);

        return MapToDto(payment, accountOwner.Nickname);
    }

    public async Task<RecurringPaymentDto?> GetByIdAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.RecurringPayments.GetWithDetailsAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            return null;
        }

        // Check access
        if (payment.CreatedByUserId != userId)
        {
            return null;
        }

        var childName = payment.Account?.User?.Nickname ?? "Unknown";
        return MapToDto(payment, childName);
    }

    public async Task<IReadOnlyList<RecurringPaymentDto>> GetAllForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var payments = await _unitOfWork.RecurringPayments.GetByCreatorIdAsync(userId, cancellationToken);

        return payments.Select(p => MapToDto(p, p.Account?.User?.Nickname ?? "Unknown")).ToList();
    }

    public async Task<RecurringPaymentDto> UpdateAsync(UserId userId, RecurringPaymentId paymentId, UpdateRecurringPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.RecurringPayments.GetWithDetailsAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            throw new InvalidOperationException("Recurring payment not found");
        }

        if (payment.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        // Update fields if provided
        if (request.Amount.HasValue)
        {
            payment.Amount = request.Amount.Value;
        }

        if (request.Description != null)
        {
            payment.Description = request.Description;
        }

        if (request.IsActive.HasValue)
        {
            payment.IsActive = request.IsActive.Value;
        }

        var intervalChanged = request.Interval.HasValue && request.Interval.Value != payment.Interval;
        var dayChanged = (request.DayOfWeek.HasValue && request.DayOfWeek != payment.DayOfWeek) ||
                         (request.DayOfMonth.HasValue && request.DayOfMonth != payment.DayOfMonth);

        if (intervalChanged)
        {
            payment.Interval = request.Interval!.Value;
        }

        if (request.DayOfWeek.HasValue)
        {
            payment.DayOfWeek = request.DayOfWeek;
        }

        if (request.DayOfMonth.HasValue)
        {
            payment.DayOfMonth = request.DayOfMonth;
        }

        // Recalculate next execution if interval or day changed
        if (intervalChanged || dayChanged)
        {
            payment.NextExecutionDate = CalculateNextExecutionDate(
                DateTime.UtcNow,
                payment.Interval,
                payment.DayOfWeek,
                payment.DayOfMonth);
        }

        payment.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Recurring payment {PaymentId} updated by user {UserId}", paymentId.Value, userId.Value);

        var childName = payment.Account?.User?.Nickname ?? "Unknown";
        return MapToDto(payment, childName);
    }

    public async Task PauseAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.RecurringPayments.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            throw new InvalidOperationException("Recurring payment not found");
        }

        if (payment.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        payment.IsActive = false;
        payment.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Recurring payment {PaymentId} paused by user {UserId}", paymentId.Value, userId.Value);
    }

    public async Task ResumeAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.RecurringPayments.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            throw new InvalidOperationException("Recurring payment not found");
        }

        if (payment.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        payment.IsActive = true;
        payment.NextExecutionDate = CalculateNextExecutionDate(
            DateTime.UtcNow,
            payment.Interval,
            payment.DayOfWeek,
            payment.DayOfMonth);
        payment.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Recurring payment {PaymentId} resumed by user {UserId}", paymentId.Value, userId.Value);
    }

    public async Task DeleteAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.RecurringPayments.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            throw new InvalidOperationException("Recurring payment not found");
        }

        if (payment.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        await _unitOfWork.RecurringPayments.DeleteAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Recurring payment {PaymentId} deleted by user {UserId}", paymentId.Value, userId.Value);
    }

    public async Task<int> ExecuteDuePaymentsAsync(CancellationToken cancellationToken = default)
    {
        var duePayments = await _unitOfWork.RecurringPayments.GetDuePaymentsAsync(DateTime.UtcNow, cancellationToken);

        var executedCount = 0;

        foreach (var payment in duePayments)
        {
            try
            {
                await ExecutePaymentAsync(payment, cancellationToken);
                executedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute recurring payment {PaymentId}", payment.Id.Value);
            }
        }

        _logger.LogInformation("Executed {Count} due recurring payments", executedCount);

        return executedCount;
    }

    private async Task ExecutePaymentAsync(RecurringPayment payment, CancellationToken cancellationToken)
    {
        var account = payment.Account;
        if (account == null)
        {
            _logger.LogWarning("Account not found for recurring payment {PaymentId}", payment.Id.Value);
            return;
        }

        // Create transaction
        var transaction = new Transaction
        {
            Id = new TransactionId(Guid.NewGuid()),
            AccountId = payment.AccountId,
            Amount = payment.Amount,
            Type = TransactionType.Allowance,
            Description = payment.Description ?? "Taschengeld",
            BalanceAfter = account.Balance + payment.Amount,
            CreatedByUserId = payment.CreatedByUserId
        };

        await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);

        // Update account balance
        account.Balance += payment.Amount;

        // Update payment schedule
        payment.LastExecutionDate = DateTime.UtcNow;
        payment.NextExecutionDate = CalculateNextExecutionDate(
            DateTime.UtcNow.AddDays(1), // Start from tomorrow
            payment.Interval,
            payment.DayOfWeek,
            payment.DayOfMonth);
        payment.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Executed recurring payment {PaymentId}: {Amount} to account {AccountId}",
            payment.Id.Value, payment.Amount, payment.AccountId.Value);
    }

    private static void ValidateRequest(CreateRecurringPaymentRequest request)
    {
        if (request.Interval == PaymentInterval.Weekly || request.Interval == PaymentInterval.Biweekly)
        {
            if (!request.DayOfWeek.HasValue)
            {
                throw new InvalidOperationException("DayOfWeek is required for weekly/biweekly payments");
            }
        }
        else if (request.Interval == PaymentInterval.Monthly)
        {
            if (!request.DayOfMonth.HasValue)
            {
                throw new InvalidOperationException("DayOfMonth is required for monthly payments");
            }
        }
    }

    private static DateTime CalculateNextExecutionDate(
        DateTime fromDate,
        PaymentInterval interval,
        DayOfWeek? dayOfWeek,
        int? dayOfMonth)
    {
        var date = fromDate.Date;

        return interval switch
        {
            PaymentInterval.Weekly => GetNextWeekday(date, dayOfWeek!.Value),
            PaymentInterval.Biweekly => GetNextWeekday(date, dayOfWeek!.Value, biweekly: true),
            PaymentInterval.Monthly => GetNextMonthDay(date, dayOfMonth!.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(interval))
        };
    }

    private static DateTime GetNextWeekday(DateTime fromDate, DayOfWeek targetDay, bool biweekly = false)
    {
        var daysUntilTarget = ((int)targetDay - (int)fromDate.DayOfWeek + 7) % 7;
        if (daysUntilTarget == 0)
        {
            daysUntilTarget = 7; // If today is the target day, schedule for next week
        }

        var nextDate = fromDate.AddDays(daysUntilTarget);

        if (biweekly)
        {
            // For biweekly, add an extra week
            nextDate = nextDate.AddDays(7);
        }

        return nextDate;
    }

    private static DateTime GetNextMonthDay(DateTime fromDate, int targetDay)
    {
        var year = fromDate.Year;
        var month = fromDate.Month;

        // If we've passed the target day this month, go to next month
        if (fromDate.Day >= targetDay)
        {
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
        }

        // Handle months with fewer days
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var actualDay = Math.Min(targetDay, daysInMonth);

        return new DateTime(year, month, actualDay);
    }

    private static RecurringPaymentDto MapToDto(RecurringPayment payment, string childName)
    {
        return new RecurringPaymentDto
        {
            Id = payment.Id.Value,
            AccountId = payment.AccountId.Value,
            ChildName = childName,
            Amount = payment.Amount,
            Interval = payment.Interval,
            DayOfWeek = payment.DayOfWeek,
            DayOfMonth = payment.DayOfMonth,
            NextExecutionDate = payment.NextExecutionDate,
            LastExecutionDate = payment.LastExecutionDate,
            IsActive = payment.IsActive,
            Description = payment.Description,
            CreatedAt = payment.CreatedAt
        };
    }
}
