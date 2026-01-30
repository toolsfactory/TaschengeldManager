using System.ComponentModel.DataAnnotations;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.RecurringPayment;

/// <summary>
/// Request to update an existing recurring payment.
/// </summary>
public record UpdateRecurringPaymentRequest
{
    /// <summary>
    /// New amount (optional).
    /// </summary>
    [Range(0.01, 10000)]
    public decimal? Amount { get; init; }

    /// <summary>
    /// New payment interval (optional).
    /// </summary>
    public PaymentInterval? Interval { get; init; }

    /// <summary>
    /// New day of week for weekly/biweekly payments.
    /// </summary>
    public DayOfWeek? DayOfWeek { get; init; }

    /// <summary>
    /// New day of month for monthly payments.
    /// </summary>
    [Range(1, 28)]
    public int? DayOfMonth { get; init; }

    /// <summary>
    /// New description.
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; init; }

    /// <summary>
    /// Activate or deactivate the payment.
    /// </summary>
    public bool? IsActive { get; init; }
}
