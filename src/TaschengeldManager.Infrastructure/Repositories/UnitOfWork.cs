using Microsoft.EntityFrameworkCore.Storage;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Repositories;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IFamilyRepository? _families;
    private IFamilyInvitationRepository? _familyInvitations;
    private IAccountRepository? _accounts;
    private ITransactionRepository? _transactions;
    private ISessionRepository? _sessions;
    private IPasskeyRepository? _passkeys;
    private IBiometricTokenRepository? _biometricTokens;
    private ILoginAttemptRepository? _loginAttempts;
    private IParentApprovalRequestRepository? _parentApprovalRequests;
    private ITotpBackupCodeRepository? _totpBackupCodes;
    private IRecurringPaymentRepository? _recurringPayments;
    private IMoneyRequestRepository? _moneyRequests;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IFamilyRepository Families => _families ??= new FamilyRepository(_context);
    public IFamilyInvitationRepository FamilyInvitations => _familyInvitations ??= new FamilyInvitationRepository(_context);
    public IAccountRepository Accounts => _accounts ??= new AccountRepository(_context);
    public ITransactionRepository Transactions => _transactions ??= new TransactionRepository(_context);
    public ISessionRepository Sessions => _sessions ??= new SessionRepository(_context);
    public IPasskeyRepository Passkeys => _passkeys ??= new PasskeyRepository(_context);
    public IBiometricTokenRepository BiometricTokens => _biometricTokens ??= new BiometricTokenRepository(_context);
    public ILoginAttemptRepository LoginAttempts => _loginAttempts ??= new LoginAttemptRepository(_context);
    public IParentApprovalRequestRepository ParentApprovalRequests => _parentApprovalRequests ??= new ParentApprovalRequestRepository(_context);
    public ITotpBackupCodeRepository TotpBackupCodes => _totpBackupCodes ??= new TotpBackupCodeRepository(_context);
    public IRecurringPaymentRepository RecurringPayments => _recurringPayments ??= new RecurringPaymentRepository(_context);
    public IMoneyRequestRepository MoneyRequests => _moneyRequests ??= new MoneyRequestRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
