using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for RecurringPayment entities.
/// </summary>
public interface IRecurringPaymentRepository : IRepository<RecurringPayment, RecurringPaymentId>
{
    /// <summary>
    /// Get all active recurring payments for a specific account.
    /// </summary>
    Task<IReadOnlyList<RecurringPayment>> GetByAccountIdAsync(AccountId accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all recurring payments created by a user.
    /// </summary>
    Task<IReadOnlyList<RecurringPayment>> GetByCreatorIdAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all payments due for execution (NextExecutionDate <= specified date).
    /// </summary>
    Task<IReadOnlyList<RecurringPayment>> GetDuePaymentsAsync(DateTime upToDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recurring payment with account and user details.
    /// </summary>
    Task<RecurringPayment?> GetWithDetailsAsync(RecurringPaymentId paymentId, CancellationToken cancellationToken = default);
}
