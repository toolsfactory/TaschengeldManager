using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.RecurringPayment;

/// <summary>
/// DTO for recurring payment information.
/// </summary>
public record RecurringPaymentDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Account ID receiving the payment.
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// Name of the child receiving the payment.
    /// </summary>
    public string ChildName { get; init; } = string.Empty;

    /// <summary>
    /// Payment amount.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Payment interval.
    /// </summary>
    public PaymentInterval Interval { get; init; }

    /// <summary>
    /// Day of week for weekly/biweekly payments.
    /// </summary>
    public DayOfWeek? DayOfWeek { get; init; }

    /// <summary>
    /// Day of month for monthly payments.
    /// </summary>
    public int? DayOfMonth { get; init; }

    /// <summary>
    /// Next scheduled execution date.
    /// </summary>
    public DateTime NextExecutionDate { get; init; }

    /// <summary>
    /// Last execution date.
    /// </summary>
    public DateTime? LastExecutionDate { get; init; }

    /// <summary>
    /// Whether the payment is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// When the payment was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
