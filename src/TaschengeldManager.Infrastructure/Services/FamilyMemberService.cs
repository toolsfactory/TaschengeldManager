using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service for family member operations (parents and relatives).
/// </summary>
public class FamilyMemberService : IFamilyMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly ILogger<FamilyMemberService> _logger;
    private readonly ICacheService _cache;

    public FamilyMemberService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        ILogger<FamilyMemberService> logger,
        ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IReadOnlyList<FamilyMemberDto>> GetMembersAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !HasFamilyAccess(family, userId))
        {
            return [];
        }

        var members = new List<FamilyMemberDto>();

        // Add parents
        foreach (var parent in family.Parents)
        {
            members.Add(new FamilyMemberDto
            {
                Id = parent.UserId.Value,
                Nickname = parent.User?.Nickname ?? "Unknown",
                Email = parent.User?.Email,
                Role = UserRole.Parent,
                JoinedAt = parent.JoinedAt,
                IsPrimary = parent.IsPrimary
            });
        }

        // Add relatives
        foreach (var relative in family.Relatives)
        {
            members.Add(new FamilyMemberDto
            {
                Id = relative.UserId.Value,
                Nickname = relative.User?.Nickname ?? "Unknown",
                Email = relative.User?.Email,
                Role = UserRole.Relative,
                JoinedAt = relative.JoinedAt,
                RelationshipDescription = relative.RelationshipDescription
            });
        }

        return members;
    }

    public async Task<FamilyMemberDto> CreateRelativeAsync(UserId parentUserId, FamilyId familyId, CreateRelativeRequest request, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null || !IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can create relatives");
        }

        // Check if email already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this email already exists");
        }

        // Create relative user
        var relative = new User
        {
            Id = new UserId(Guid.NewGuid()),
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Nickname = request.Nickname,
            Role = UserRole.Relative,
            MfaEnabled = false
        };

        await _unitOfWork.Users.AddAsync(relative, cancellationToken);

        // Add to family
        family.Relatives.Add(new FamilyRelative
        {
            FamilyId = familyId,
            UserId = relative.Id,
            JoinedAt = DateTime.UtcNow,
            RelationshipDescription = request.RelationshipDescription
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate family cache
        await InvalidateFamilyCacheAsync(family, cancellationToken);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(request.Email, request.Nickname, cancellationToken);

        _logger.LogInformation("Relative {RelativeId} created for family {FamilyId} by parent {ParentId}", relative.Id.Value, familyId.Value, parentUserId.Value);

        return new FamilyMemberDto
        {
            Id = relative.Id.Value,
            Nickname = relative.Nickname,
            Email = relative.Email,
            Role = relative.Role,
            JoinedAt = DateTime.UtcNow,
            IsPrimary = false,
            RelationshipDescription = request.RelationshipDescription
        };
    }

    public async Task RemoveParentAsync(UserId parentUserId, FamilyId familyId, UserId targetUserId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        if (!IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can remove other parents");
        }

        var targetParent = family.Parents.FirstOrDefault(p => p.UserId == targetUserId);
        if (targetParent == null)
        {
            throw new InvalidOperationException("Target user is not a parent in this family");
        }

        if (targetParent.IsPrimary)
        {
            throw new InvalidOperationException("Cannot remove the primary parent");
        }

        family.Parents.Remove(targetParent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate family cache
        await InvalidateFamilyCacheAsync(family, cancellationToken);

        _logger.LogInformation("Parent {TargetId} removed from family {FamilyId} by parent {ParentId}", targetUserId.Value, familyId.Value, parentUserId.Value);
    }

    public async Task RemoveRelativeAsync(UserId parentUserId, FamilyId familyId, UserId relativeUserId, CancellationToken cancellationToken = default)
    {
        var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (family == null)
        {
            throw new InvalidOperationException("Family not found");
        }

        if (!IsParent(family, parentUserId))
        {
            throw new UnauthorizedAccessException("Only parents can remove relatives");
        }

        var relative = family.Relatives.FirstOrDefault(r => r.UserId == relativeUserId);
        if (relative == null)
        {
            throw new InvalidOperationException("Relative not found in family");
        }

        family.Relatives.Remove(relative);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate family cache
        await InvalidateFamilyCacheAsync(family, cancellationToken);

        _logger.LogInformation("Relative {RelativeId} removed from family {FamilyId} by parent {ParentId}", relativeUserId.Value, familyId.Value, parentUserId.Value);
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
