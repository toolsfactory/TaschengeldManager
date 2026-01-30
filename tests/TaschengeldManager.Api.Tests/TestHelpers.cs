using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Infrastructure.Data;
using TaschengeldManager.Infrastructure.Services;

namespace TaschengeldManager.Api.Tests;

/// <summary>
/// Helper methods for integration tests.
/// </summary>
public static class TestHelpers
{
    private const string TestJwtKey = "TestSecretKeyForIntegrationTestsOnly123456789!";
    private const string TestIssuer = "TaschengeldManager.Tests";
    private const string TestAudience = "TaschengeldManager.Tests";

    /// <summary>
    /// Generates a valid JWT token for testing.
    /// </summary>
    public static string GenerateTestToken(UserId userId, string? email, UserRole role, FamilyId? familyId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.Value.ToString()),
            new(ClaimTypes.Role, role.ToString())
        };

        if (!string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        if (familyId.HasValue)
        {
            claims.Add(new Claim("FamilyId", familyId.Value.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Sets the authorization header with a bearer token.
    /// </summary>
    public static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Sets the authorization header for a specific user.
    /// </summary>
    public static void AuthenticateAs(this HttpClient client, User user)
    {
        var token = GenerateTestToken(user.Id, user.Email, user.Role, user.FamilyId);
        client.SetBearerToken(token);
    }

    /// <summary>
    /// Generates a valid JWT token for testing (overload accepting Guid for backwards compatibility).
    /// </summary>
    public static string GenerateTestToken(Guid userId, string? email, UserRole role, Guid? familyId = null)
    {
        return GenerateTestToken(
            new UserId(userId),
            email,
            role,
            familyId.HasValue ? new FamilyId(familyId.Value) : null);
    }

    /// <summary>
    /// Clears the authorization header.
    /// </summary>
    public static void ClearAuthentication(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}

/// <summary>
/// Test data builder for creating test entities.
/// </summary>
public class TestDataBuilder
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher _passwordHasher = new();

    public TestDataBuilder(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a test family.
    /// </summary>
    public async Task<Family> CreateFamilyAsync(string name = "Test Familie")
    {
        var family = new Family
        {
            Id = new FamilyId(Guid.NewGuid()),
            Name = name,
            FamilyCode = GenerateFamilyCode(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Families.Add(family);
        await _context.SaveChangesAsync();
        return family;
    }

    /// <summary>
    /// Creates a parent user.
    /// </summary>
    public async Task<User> CreateParentAsync(
        string email = "parent@test.de",
        string password = "Test123!",
        Family? family = null,
        string nickname = "TestParent")
    {
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            PasswordHash = _passwordHasher.HashPassword(password),
            Nickname = nickname,
            Role = UserRole.Parent,
            FamilyId = family?.Id,
            Family = family,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // If there's a family, add to FamilyParent junction table
        if (family != null)
        {
            _context.Set<FamilyParent>().Add(new FamilyParent
            {
                FamilyId = family.Id,
                UserId = user.Id
            });
        }

        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Creates a child user with an account.
    /// </summary>
    public async Task<(User Child, Account Account)> CreateChildWithAccountAsync(
        Family family,
        string nickname = "TestKind",
        string pin = "1234",
        decimal initialBalance = 0m)
    {
        var child = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = null, // Children don't have email
            Nickname = nickname,
            PinHash = _passwordHasher.HashPassword(pin),
            Role = UserRole.Child,
            FamilyId = family.Id,
            Family = family,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(child);

        var account = new Account
        {
            Id = new AccountId(Guid.NewGuid()),
            UserId = child.Id,
            User = child,
            Balance = initialBalance,
            InterestRate = null,
            InterestEnabled = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return (child, account);
    }

    /// <summary>
    /// Creates a relative user.
    /// </summary>
    public async Task<User> CreateRelativeAsync(
        Family family,
        string email = "oma@test.de",
        string password = "Test123!",
        string firstName = "Oma",
        string lastName = "Müller")
    {
        var user = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            PasswordHash = _passwordHasher.HashPassword(password),
            Nickname = $"{firstName} {lastName}",
            Role = UserRole.Relative,
            FamilyId = family.Id,
            Family = family,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Add to FamilyRelative junction table
        _context.Set<FamilyRelative>().Add(new FamilyRelative
        {
            FamilyId = family.Id,
            UserId = user.Id,
            RelationshipDescription = firstName
        });

        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Creates a transaction.
    /// </summary>
    public async Task<Transaction> CreateTransactionAsync(
        Account account,
        decimal amount,
        TransactionType type,
        User? createdBy = null,
        string? description = null)
    {
        var transaction = new Transaction
        {
            Id = new TransactionId(Guid.NewGuid()),
            AccountId = account.Id,
            Account = account,
            Amount = amount,
            Type = type,
            Description = description ?? $"Test {type}",
            CreatedByUserId = createdBy?.Id,
            CreatedByUser = createdBy,
            BalanceAfter = account.Balance + amount,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);

        // Update account balance
        if (type == TransactionType.Deposit || type == TransactionType.Gift ||
            type == TransactionType.Interest || type == TransactionType.Allowance)
        {
            account.Balance += amount;
        }
        else if (type == TransactionType.Withdrawal)
        {
            account.Balance -= amount;
        }

        await _context.SaveChangesAsync();
        return transaction;
    }

    private static string GenerateFamilyCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
