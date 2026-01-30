using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces.Services;
using TaschengeldManager.Infrastructure.Data;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service for seeding development/test data.
/// </summary>
public class DevSeederService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DevSeederService> _logger;

    public DevSeederService(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DevSeederService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Seed default test data if database is empty.
    /// </summary>
    public async Task<SeedResult> SeedAsync(CancellationToken cancellationToken = default)
    {
        var result = new SeedResult();

        if (await _context.Users.AnyAsync(cancellationToken))
        {
            result.Message = "Database already contains data. Use force=true to reset.";
            return result;
        }

        return await SeedAllDataAsync(cancellationToken);
    }

    /// <summary>
    /// Force reset and reseed all data.
    /// </summary>
    public async Task<SeedResult> ResetAndSeedAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Resetting database and reseeding all data");

        // Clear all data in reverse dependency order
        _context.Transactions.RemoveRange(_context.Transactions);
        _context.Accounts.RemoveRange(_context.Accounts);
        _context.Sessions.RemoveRange(_context.Sessions);
        _context.LoginAttempts.RemoveRange(_context.LoginAttempts);
        _context.BiometricTokens.RemoveRange(_context.BiometricTokens);
        _context.Passkeys.RemoveRange(_context.Passkeys);
        _context.TotpBackupCodes.RemoveRange(_context.TotpBackupCodes);
        _context.ParentApprovalRequests.RemoveRange(_context.ParentApprovalRequests);
        _context.FamilyInvitations.RemoveRange(_context.FamilyInvitations);

        // Clear join tables
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM family_parents", cancellationToken);
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM family_relatives", cancellationToken);

        _context.Users.RemoveRange(_context.Users);
        _context.Families.RemoveRange(_context.Families);

        await _context.SaveChangesAsync(cancellationToken);

        // Clear change tracker to avoid tracking conflicts with new entities
        _context.ChangeTracker.Clear();

        return await SeedAllDataAsync(cancellationToken);
    }

    private async Task<SeedResult> SeedAllDataAsync(CancellationToken cancellationToken)
    {
        var result = new SeedResult();

        // Disable automatic change detection to avoid tracking conflicts
        _context.ChangeTracker.AutoDetectChangesEnabled = false;

        // Pre-generate IDs to avoid tracking conflicts
        var parent1Id = new UserId(Guid.NewGuid());
        var parent2Id = new UserId(Guid.NewGuid());

        // Create parent users
        var parent1 = new User
        {
            Id = parent1Id,
            Email = "max.mueller@example.com",
            NormalizedEmail = "MAX.MUELLER@EXAMPLE.COM",
            PasswordHash = _passwordHasher.HashPassword("Test1234!"),
            Nickname = "Max",
            Role = UserRole.Parent,
            MfaEnabled = false
        };

        var parent2 = new User
        {
            Id = parent2Id,
            Email = "anna.mueller@example.com",
            NormalizedEmail = "ANNA.MUELLER@EXAMPLE.COM",
            PasswordHash = _passwordHasher.HashPassword("Test1234!"),
            Nickname = "Anna",
            Role = UserRole.Parent,
            MfaEnabled = false
        };

        await _context.Users.AddRangeAsync(parent1, parent2);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        result.ParentsCreated = 2;

        // Pre-generate family ID
        var familyId = new FamilyId(Guid.NewGuid());

        // Create family
        var family = new Family
        {
            Id = familyId,
            Name = "Familie Müller",
            FamilyCode = "MUEL01",
            CreatedByUserId = parent1Id
        };

        await _context.Families.AddAsync(family, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        // Add parents to family via DbSet (avoid navigation property tracking issues)
        await _context.FamilyParents.AddRangeAsync(
            new FamilyParent
            {
                FamilyId = familyId,
                UserId = parent1Id,
                JoinedAt = DateTime.UtcNow.AddDays(-30),
                IsPrimary = true
            },
            new FamilyParent
            {
                FamilyId = familyId,
                UserId = parent2Id,
                JoinedAt = DateTime.UtcNow.AddDays(-29),
                IsPrimary = false
            });

        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
        result.FamiliesCreated = 1;

        // Pre-generate IDs for children
        var child1Id = new UserId(Guid.NewGuid());
        var child2Id = new UserId(Guid.NewGuid());

        // Create children
        var child1 = new User
        {
            Id = child1Id,
            Nickname = "Tim",
            PinHash = _passwordHasher.HashPin("1234"),
            Role = UserRole.Child,
            FamilyId = familyId,
            MfaEnabled = false,
            SecurityTutorialCompleted = true
        };

        var child2 = new User
        {
            Id = child2Id,
            Nickname = "Lisa",
            PinHash = _passwordHasher.HashPin("5678"),
            Role = UserRole.Child,
            FamilyId = familyId,
            MfaEnabled = false,
            SecurityTutorialCompleted = false
        };

        await _context.Users.AddRangeAsync(child1, child2);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        result.ChildrenCreated = 2;

        // Pre-generate account IDs
        var account1Id = new AccountId(Guid.NewGuid());
        var account2Id = new AccountId(Guid.NewGuid());

        // Create accounts for children
        var account1 = new Account
        {
            Id = account1Id,
            UserId = child1Id,
            Balance = 42.50m,
            InterestEnabled = true,
            InterestRate = 2.5m,
            LastInterestCalculation = DateTime.UtcNow.AddDays(-7)
        };

        var account2 = new Account
        {
            Id = account2Id,
            UserId = child2Id,
            Balance = 15.00m,
            InterestEnabled = false
        };

        await _context.Accounts.AddRangeAsync(account1, account2);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        result.AccountsCreated = 2;

        // Create some transactions for Tim (each with unique pre-generated ID)
        var transactions = new List<Transaction>
        {
            new()
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account1Id,
                Amount = 20.00m,
                Type = TransactionType.Allowance,
                Description = "Taschengeld Woche 1",
                CreatedByUserId = parent1Id,
                BalanceAfter = 20.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-21)
            },
            new()
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account1Id,
                Amount = 20.00m,
                Type = TransactionType.Allowance,
                Description = "Taschengeld Woche 2",
                CreatedByUserId = parent1Id,
                BalanceAfter = 40.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            },
            new()
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account1Id,
                Amount = -5.00m,
                Type = TransactionType.Withdrawal,
                Description = "Eis gekauft",
                Category = "Süßigkeiten",
                CreatedByUserId = child1Id,
                BalanceAfter = 35.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account1Id,
                Amount = 10.00m,
                Type = TransactionType.Gift,
                Description = "Von Oma zum Geburtstag",
                CreatedByUserId = parent1Id, // Would be relative
                BalanceAfter = 45.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account1Id,
                Amount = -2.50m,
                Type = TransactionType.Withdrawal,
                Description = "Comic-Heft",
                Category = "Unterhaltung",
                CreatedByUserId = child1Id,
                BalanceAfter = 42.50m,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        // Transactions for Lisa
        transactions.AddRange(new[]
        {
            new Transaction
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account2Id,
                Amount = 10.00m,
                Type = TransactionType.Allowance,
                Description = "Taschengeld",
                CreatedByUserId = parent1Id,
                BalanceAfter = 10.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Transaction
            {
                Id = new TransactionId(Guid.NewGuid()),
                AccountId = account2Id,
                Amount = 5.00m,
                Type = TransactionType.Deposit,
                Description = "Für Hausarbeit",
                CreatedByUserId = parent2Id,
                BalanceAfter = 15.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        });

        await _context.Transactions.AddRangeAsync(transactions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        result.TransactionsCreated = transactions.Count;

        // Pre-generate ID for relative
        var relativeId = new UserId(Guid.NewGuid());

        // Create a relative
        var relative = new User
        {
            Id = relativeId,
            Email = "oma.mueller@example.com",
            NormalizedEmail = "OMA.MUELLER@EXAMPLE.COM",
            PasswordHash = _passwordHasher.HashPassword("Test1234!"),
            Nickname = "Oma Helga",
            Role = UserRole.Relative,
            MfaEnabled = false
        };

        await _context.Users.AddAsync(relative, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        await _context.FamilyRelatives.AddAsync(new FamilyRelative
        {
            FamilyId = familyId,
            UserId = relativeId,
            JoinedAt = DateTime.UtcNow.AddDays(-20),
            RelationshipDescription = "Großmutter"
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        result.RelativesCreated = 1;

        // Pre-generate ID for parent3
        var parent3Id = new UserId(Guid.NewGuid());

        // Create a second family for testing
        var parent3 = new User
        {
            Id = parent3Id,
            Email = "peter.schmidt@example.com",
            NormalizedEmail = "PETER.SCHMIDT@EXAMPLE.COM",
            PasswordHash = _passwordHasher.HashPassword("Test1234!"),
            Nickname = "Peter",
            Role = UserRole.Parent,
            MfaEnabled = false
        };

        await _context.Users.AddAsync(parent3, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        // Pre-generate family2 ID
        var family2Id = new FamilyId(Guid.NewGuid());

        var family2 = new Family
        {
            Id = family2Id,
            Name = "Familie Schmidt",
            FamilyCode = "SCHM01",
            CreatedByUserId = parent3Id
        };

        await _context.Families.AddAsync(family2, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        await _context.FamilyParents.AddAsync(new FamilyParent
        {
            FamilyId = family2Id,
            UserId = parent3Id,
            JoinedAt = DateTime.UtcNow.AddDays(-10),
            IsPrimary = true
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();

        result.ParentsCreated++;
        result.FamiliesCreated++;

        result.Success = true;
        result.Message = "Seed data created successfully";

        _logger.LogInformation(
            "Seed completed: {Parents} parents, {Children} children, {Relatives} relatives, {Families} families, {Accounts} accounts, {Transactions} transactions",
            result.ParentsCreated, result.ChildrenCreated, result.RelativesCreated,
            result.FamiliesCreated, result.AccountsCreated, result.TransactionsCreated);

        // Re-enable automatic change detection
        _context.ChangeTracker.AutoDetectChangesEnabled = true;

        return result;
    }

    private static string GenerateBase32Secret(int length = 20)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var random = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            chars[i] = alphabet[random[i] % 32];
        }

        return new string(chars);
    }
}

/// <summary>
/// Result of seeding operation.
/// </summary>
public class SeedResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ParentsCreated { get; set; }
    public int ChildrenCreated { get; set; }
    public int RelativesCreated { get; set; }
    public int FamiliesCreated { get; set; }
    public int AccountsCreated { get; set; }
    public int TransactionsCreated { get; set; }

    public TestCredentials Credentials => new()
    {
        Parent1 = new CredentialInfo { Email = "max.mueller@example.com", Password = "Test1234!" },
        Parent2 = new CredentialInfo { Email = "anna.mueller@example.com", Password = "Test1234!" },
        Parent3 = new CredentialInfo { Email = "peter.schmidt@example.com", Password = "Test1234!" },
        Relative = new CredentialInfo { Email = "oma.mueller@example.com", Password = "Test1234!" },
        Child1 = new ChildCredentialInfo { FamilyCode = "MUEL01", Nickname = "Tim", Pin = "1234" },
        Child2 = new ChildCredentialInfo { FamilyCode = "MUEL01", Nickname = "Lisa", Pin = "5678" }
    };
}

public class TestCredentials
{
    public CredentialInfo Parent1 { get; set; } = new();
    public CredentialInfo Parent2 { get; set; } = new();
    public CredentialInfo Parent3 { get; set; } = new();
    public CredentialInfo Relative { get; set; } = new();
    public ChildCredentialInfo Child1 { get; set; } = new();
    public ChildCredentialInfo Child2 { get; set; } = new();
}

public class CredentialInfo
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ChildCredentialInfo
{
    public string FamilyCode { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
}
