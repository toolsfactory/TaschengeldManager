using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Core.Interfaces;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Service for core family operations.
/// </summary>
/// <remarks>
/// Related services:
/// - FamilyInvitationService: Invitation management
/// - ChildManagementService: Child management and PIN
/// - FamilyMemberService: Parent and relative management
/// </remarks>
public class FamilyService : IFamilyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FamilyService> _logger;
    private readonly ICacheService _cache;

    private static readonly TimeSpan FamilyCacheExpiration = TimeSpan.FromMinutes(10);

    public FamilyService(
        IUnitOfWork unitOfWork,
        ILogger<FamilyService> logger,
        ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cache = cache;
    }

    public async Task<FamilyDto> CreateFamilyAsync(UserId userId, CreateFamilyRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.Role != UserRole.Parent)
        {
            throw new InvalidOperationException("Only parents can create families");
        }

        var family = new Family
        {
            Id = new FamilyId(Guid.NewGuid()),
            Name = request.Name,
            FamilyCode = GenerateFamilyCode(),
            CreatedByUserId = userId
        };

        await _unitOfWork.Families.AddAsync(family, cancellationToken);

        // Add creator as primary parent
        family.Parents.Add(new FamilyParent
        {
            FamilyId = family.Id,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsPrimary = true
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Family {FamilyId} created by user {UserId}", family.Id.Value, userId.Value);

        // Invalidate user's families cache
        await _cache.RemoveAsync(CacheKeys.FamiliesForUser(userId), cancellationToken);

        return await GetFamilyDtoAsync(family, cancellationToken);
    }

    public async Task<FamilyDto?> GetFamilyAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        var cacheKey = CacheKeys.Family(familyId);
        var cachedDto = await _cache.GetAsync<FamilyDto>(cacheKey, cancellationToken);

        if (cachedDto != null)
        {
            // Verify user still has access (in case permissions changed)
            var family = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
            if (family != null && HasFamilyAccess(family, userId))
            {
                return cachedDto;
            }
            return null;
        }

        var familyEntity = await _unitOfWork.Families.GetWithMembersAsync(familyId, cancellationToken);
        if (familyEntity == null)
        {
            return null;
        }

        // Check if user has access to this family
        if (!HasFamilyAccess(familyEntity, userId))
        {
            return null;
        }

        var dto = await GetFamilyDtoAsync(familyEntity, cancellationToken);

        // Cache the result
        await _cache.SetAsync(cacheKey, dto, FamilyCacheExpiration, cancellationToken);

        return dto;
    }

    public async Task<IReadOnlyList<FamilyDto>> GetFamiliesForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        var cacheKey = CacheKeys.FamiliesForUser(userId);
        var cachedResult = await _cache.GetAsync<List<FamilyDto>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return [];
        }

        var families = new List<Family>();

        // Get families where user is a parent
        if (user.Role == UserRole.Parent)
        {
            var parentFamilies = await _unitOfWork.Families.GetFamiliesForParentAsync(userId, cancellationToken);
            families.AddRange(parentFamilies);
        }
        // Get families where user is a relative
        else if (user.Role == UserRole.Relative)
        {
            var relativeFamilies = await _unitOfWork.Families.GetFamiliesForRelativeAsync(userId, cancellationToken);
            families.AddRange(relativeFamilies);
        }
        // Get family where user is a child
        else if (user.Role == UserRole.Child && user.FamilyId.HasValue)
        {
            var childFamily = await _unitOfWork.Families.GetWithMembersAsync(user.FamilyId.Value, cancellationToken);
            if (childFamily != null)
            {
                families.Add(childFamily);
            }
        }

        var result = new List<FamilyDto>();
        foreach (var family in families)
        {
            result.Add(await GetFamilyDtoAsync(family, cancellationToken));
        }

        // Cache the result
        await _cache.SetAsync(cacheKey, result, FamilyCacheExpiration, cancellationToken);

        return result;
    }

    private static string GenerateFamilyCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Avoid confusing chars like O, 0, I, 1
        var code = new char[6];
        var bytes = RandomNumberGenerator.GetBytes(6);

        for (int i = 0; i < 6; i++)
        {
            code[i] = chars[bytes[i] % chars.Length];
        }

        return new string(code);
    }

    private static bool HasFamilyAccess(Family family, UserId userId)
    {
        return family.Parents.Any(p => p.UserId == userId) ||
               family.Relatives.Any(r => r.UserId == userId) ||
               family.Children.Any(c => c.Id == userId);
    }

    private async Task<FamilyDto> GetFamilyDtoAsync(Family family, CancellationToken cancellationToken)
    {
        var parents = family.Parents.Select(p => new FamilyMemberDto
        {
            Id = p.UserId.Value,
            Nickname = p.User?.Nickname ?? "Unknown",
            Email = p.User?.Email,
            Role = UserRole.Parent,
            JoinedAt = p.JoinedAt,
            IsPrimary = p.IsPrimary
        }).ToList();

        var relatives = family.Relatives.Select(r => new FamilyMemberDto
        {
            Id = r.UserId.Value,
            Nickname = r.User?.Nickname ?? "Unknown",
            Email = r.User?.Email,
            Role = UserRole.Relative,
            JoinedAt = r.JoinedAt,
            RelationshipDescription = r.RelationshipDescription
        }).ToList();

        // Batch load all accounts for children to avoid N+1 queries
        var childIds = family.Children.Select(c => c.Id).ToList();
        var accounts = await _unitOfWork.Accounts.GetByUserIdsAsync(childIds, cancellationToken);
        var accountsByUserId = accounts.ToDictionary(a => a.UserId);

        var children = family.Children.Select(child =>
        {
            accountsByUserId.TryGetValue(child.Id, out var account);
            return new ChildDto
            {
                Id = child.Id.Value,
                Nickname = child.Nickname,
                AccountId = account?.Id.Value,
                Balance = account?.Balance,
                IsLocked = child.IsLocked,
                MfaEnabled = child.MfaEnabled,
                CreatedAt = child.CreatedAt
            };
        }).ToList();

        return new FamilyDto
        {
            Id = family.Id.Value,
            Name = family.Name,
            FamilyCode = family.FamilyCode,
            CreatedAt = family.CreatedAt,
            Parents = parents,
            Relatives = relatives,
            Children = children
        };
    }
}
