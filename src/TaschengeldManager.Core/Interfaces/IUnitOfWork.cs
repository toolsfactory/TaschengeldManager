using TaschengeldManager.Core.Interfaces.Repositories;

namespace TaschengeldManager.Core.Interfaces;

/// <summary>
/// Unit of Work pattern interface for transaction management.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IFamilyRepository Families { get; }
    IFamilyInvitationRepository FamilyInvitations { get; }
    IAccountRepository Accounts { get; }
    ITransactionRepository Transactions { get; }
    ISessionRepository Sessions { get; }
    IPasskeyRepository Passkeys { get; }
    IBiometricTokenRepository BiometricTokens { get; }
    ILoginAttemptRepository LoginAttempts { get; }
    IParentApprovalRequestRepository ParentApprovalRequests { get; }
    ITotpBackupCodeRepository TotpBackupCodes { get; }
    IRecurringPaymentRepository RecurringPayments { get; }
    IMoneyRequestRepository MoneyRequests { get; }

    /// <summary>
    /// Save all changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
