using TaschengeldManager.Core.DTOs.RecurringPayment;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for recurring payment management.
/// </summary>
public interface IRecurringPaymentService
{
    /// <summary>
    /// Create a new recurring payment.
    /// </summary>
    Task<RecurringPaymentDto> CreateAsync(UserId userId, CreateRecurringPaymentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a recurring payment by ID.
    /// </summary>
    Task<RecurringPaymentDto?> GetByIdAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all recurring payments for the user.
    /// </summary>
    Task<IReadOnlyList<RecurringPaymentDto>> GetAllForUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a recurring payment.
    /// </summary>
    Task<RecurringPaymentDto> UpdateAsync(UserId userId, RecurringPaymentId paymentId, UpdateRecurringPaymentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pause a recurring payment.
    /// </summary>
    Task PauseAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resume a paused recurring payment.
    /// </summary>
    Task ResumeAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a recurring payment.
    /// </summary>
    Task DeleteAsync(UserId userId, RecurringPaymentId paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute all due recurring payments.
    /// Called by the background job.
    /// </summary>
    Task<int> ExecuteDuePaymentsAsync(CancellationToken cancellationToken = default);
}
