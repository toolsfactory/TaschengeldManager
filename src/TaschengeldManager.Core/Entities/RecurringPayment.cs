using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Entities;

/// <summary>
/// Represents a recurring allowance payment configuration.
/// </summary>
public class RecurringPayment : BaseEntity<RecurringPaymentId>
{
    /// <summary>
    /// Account ID of the child receiving the payment.
    /// </summary>
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }

    /// <summary>
    /// Amount to be paid each interval.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Payment interval (weekly, biweekly, monthly).
    /// </summary>
    public PaymentInterval Interval { get; set; }

    /// <summary>
    /// Day of week for weekly/biweekly payments (0 = Sunday, 6 = Saturday).
    /// </summary>
    public DayOfWeek? DayOfWeek { get; set; }

    /// <summary>
    /// Day of month for monthly payments (1-28, null for weekly/biweekly).
    /// Use 28 max to avoid issues with short months.
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// Next scheduled execution date.
    /// </summary>
    public DateTime NextExecutionDate { get; set; }

    /// <summary>
    /// Last execution date (null if never executed).
    /// </summary>
    public DateTime? LastExecutionDate { get; set; }

    /// <summary>
    /// Whether this payment is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional description for the payment.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// User ID who created/owns this payment configuration.
    /// </summary>
    public UserId CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}
