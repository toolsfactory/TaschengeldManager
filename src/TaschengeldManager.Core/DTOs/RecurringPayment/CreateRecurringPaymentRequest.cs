using System.ComponentModel.DataAnnotations;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.RecurringPayment;

/// <summary>
/// Request to create a new recurring payment.
/// </summary>
public record CreateRecurringPaymentRequest
{
    /// <summary>
    /// Account ID of the child receiving the payment.
    /// </summary>
    [Required]
    public Guid AccountId { get; init; }

    /// <summary>
    /// Amount to pay each interval.
    /// </summary>
    [Required]
    [Range(0.01, 10000)]
    public decimal Amount { get; init; }

    /// <summary>
    /// Payment interval.
    /// </summary>
    [Required]
    public PaymentInterval Interval { get; init; }

    /// <summary>
    /// Day of week for weekly/biweekly payments (0 = Sunday, 6 = Saturday).
    /// Required when Interval is Weekly or Biweekly.
    /// </summary>
    public DayOfWeek? DayOfWeek { get; init; }

    /// <summary>
    /// Day of month for monthly payments (1-28).
    /// Required when Interval is Monthly.
    /// </summary>
    [Range(1, 28)]
    public int? DayOfMonth { get; init; }

    /// <summary>
    /// Optional description for the payment.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; init; }

    /// <summary>
    /// Start date for the recurring payment (defaults to next occurrence).
    /// </summary>
    public DateTime? StartDate { get; init; }
}
