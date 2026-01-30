using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service for child management operations.
/// </summary>
public class ChildManagementService : IChildManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChildManagementService> _logger;
    private readonly ICacheService _cache;

    public ChildManagementService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<ChildManagementService> logger,
        ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ChildDto> AddChildAsync(UserId parentUserId, FamilyId familyId, AddChildRequest request, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        if (!IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can add children");
        }

        // Check for duplicate nickname in family
        var existingChild = family.Children.FirstOrDefault(c => c.Nickname.Equals(request.Nickname, StringComparison.OrdinalIgnoreCase));
        if (existingChild != null)
        {
            throw new InvalidOperationException("A child with this nickname already exists in the family");
        }

        // Create child user
        var child = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Nickname = request.Nickname,
            PinHash = _passwordHasher.HashPin(request.Pin),
            Role = UserRole.Child,
            FamilyId = familyId,
            MfaEnabled = false
        };

        await _unitOfWork.Users.AddAsync(child, cancellationToken);

        // Create account for child
        var account = new Account
        {
            Id = new AccountId(Guid.NewGuid()),
            UserId = child.Id,
            Balance = request.InitialBalance,
            InterestEnabled = false
        };

        await _unitOfWork.Accounts.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate family cache
        await InvalidateFamilyCacheAsync(family, cancellationToken);

        _logger.LogInformation("Child {ChildId} added to family {FamilyId} by parent {ParentId}", child.Id.Value, familyId.Value, parentUserId.Value);

        return new ChildDto
        {
            Id = child.Id.Value,
            Nickname = child.Nickname,
            AccountId = account.Id.Value,
            Balance = account.Balance,
            IsLocked = child.IsLocked,
            MfaEnabled = child.MfaEnabled,
            CreatedAt = child.CreatedAt
        };
    }

    public async Task RemoveChildAsync(UserId parentUserId, FamilyId familyId, UserId childId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        if (!IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can remove children");
        }

        var child = family.Children.FirstOrDefault(c => c.Id == childId);
        if (child == null)
        {
            throw new InvalidOperationException("Child not found in family");
        }

        // Don't delete, just unlink from family
        child.FamilyId = null;
        child.IsLocked = true;
        child.LockReason = "Removed from family";
        child.LockedAt = DateTime.UtcNow;
        child.LockedByUserId = parentUserId;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate family cache
        await InvalidateFamilyCacheAsync(family, cancellationToken);

        _logger.LogInformation("Child {ChildId} removed from family {FamilyId} by parent {ParentId}", childId.Value, familyId.Value, parentUserId.Value);
    }

    public async Task<IReadOnlyList<ChildDto>> GetChildrenAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !HasFamilyAccess(family, userId))
        {
            return [];
        }

        // Batch load all accounts for children to avoid N+1 queries
        var childIds = family.Children.Select(c => c.Id).ToList();
        var accounts = await _unitOfWork.Accounts.GetByUserIdsAsync(childIds, cancellationToken);
        var accountsByUserId = accounts.ToDictionary(a => a.UserId);

        return family.Children.Select(child =>
        {
            accountsByUserId.TryGetValue(child.Id, out var account);
            return new ChildDto
            {
                Id = child.Id.Value,
                Nickname = child.Nickname,
                AccountId = account?.Id.Value,
                Balance = account?.Balance ?? 0,
                IsLocked = child.IsLocked,
                MfaEnabled = child.MfaEnabled,
                CreatedAt = child.CreatedAt
            };
        }).ToList();
    }

    public async Task ChangeChildPinAsync(UserId parentUserId, FamilyId familyId, UserId childId, ChangeChildPinRequest request, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        if (!IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can change child PINs");
        }

        // Verify parent password
        var parent = await _unitOfWork.Users.GetByIdAsync(parentUserId, cancellationToken);
        if (parent == null || !_passwordHasher.VerifyPassword(request.ParentPassword, parent.PasswordHash!))
        {
            throw new InvalidOperationException("Invalid parent password");
        }

        var child = family.Children.FirstOrDefault(c => c.Id == childId);
        if (child == null)
        {
            throw new InvalidOperationException("Child not found in family");
        }

        // Update child PIN
        child.PinHash = _passwordHasher.HashPin(request.NewPin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PIN changed for child {ChildId} by parent {ParentId}", childId.Value, parentUserId.Value);
    }

    private static bool IsParent(Family family, UserId userId)
    {
        return family.Parents.Any(p => p.UserId == userId);
    }

    private static bool HasFamilyAccess(Family family, UserId userId)
    {
        return family.Parents.Any(p => p.UserId == userId) ||
               family.Relatives.Any(r => r.UserId == userId) ||
               family.Children.Any(c => c.Id == userId);
    }

    private async Task InvalidateFamilyCacheAsync(Family family, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.Family(family.Id), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.FamilyByCode(family.FamilyCode), cancellationToken);

        foreach (var parent in family.Parents)
        {
            await _cache.RemoveAsync(CacheKeys.FamiliesForUser(parent.UserId), cancellationToken);
        }

        foreach (var child in family.Children)
        {
            await _cache.RemoveAsync(CacheKeys.FamiliesForUser(child.Id), cancellationToken);
        }

        foreach (var relative in family.Relatives)
        {
            await _cache.RemoveAsync(CacheKeys.FamiliesForUser(relative.UserId), cancellationToken);
        }
    }
}
