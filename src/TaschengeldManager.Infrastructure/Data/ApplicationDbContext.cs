using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data;

/// <summary>
/// Application database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Family> Families => Set<Family>();
    public DbSet<FamilyParent> FamilyParents => Set<FamilyParent>();
    public DbSet<FamilyRelative> FamilyRelatives => Set<FamilyRelative>();
    public DbSet<FamilyInvitation> FamilyInvitations => Set<FamilyInvitation>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Passkey> Passkeys => Set<Passkey>();
    public DbSet<BiometricToken> BiometricTokens => Set<BiometricToken>();
    public DbSet<TotpBackupCode> TotpBackupCodes => Set<TotpBackupCode>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();
    public DbSet<ParentApprovalRequest> ParentApprovalRequests => Set<ParentApprovalRequest>();
    public DbSet<RecurringPayment> RecurringPayments => Set<RecurringPayment>();
    public DbSet<MoneyRequest> MoneyRequests => Set<MoneyRequest>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure strongly typed ID conversions using custom value converters
        configurationBuilder.Properties<UserId>()
            .HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<FamilyId>()
            .HaveConversion<FamilyIdConverter>();
        configurationBuilder.Properties<AccountId>()
            .HaveConversion<AccountIdConverter>();
        configurationBuilder.Properties<TransactionId>()
            .HaveConversion<TransactionIdConverter>();
        configurationBuilder.Properties<SessionId>()
            .HaveConversion<SessionIdConverter>();
        configurationBuilder.Properties<PasskeyId>()
            .HaveConversion<PasskeyIdConverter>();
        configurationBuilder.Properties<BiometricTokenId>()
            .HaveConversion<BiometricTokenIdConverter>();
        configurationBuilder.Properties<TotpBackupCodeId>()
            .HaveConversion<TotpBackupCodeIdConverter>();
        configurationBuilder.Properties<LoginAttemptId>()
            .HaveConversion<LoginAttemptIdConverter>();
        configurationBuilder.Properties<FamilyInvitationId>()
            .HaveConversion<FamilyInvitationIdConverter>();
        configurationBuilder.Properties<ParentApprovalRequestId>()
            .HaveConversion<ParentApprovalRequestIdConverter>();
        configurationBuilder.Properties<MoneyRequestId>()
            .HaveConversion<MoneyRequestIdConverter>();
        configurationBuilder.Properties<RecurringPaymentId>()
            .HaveConversion<RecurringPaymentIdConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IHasTimestamps>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
