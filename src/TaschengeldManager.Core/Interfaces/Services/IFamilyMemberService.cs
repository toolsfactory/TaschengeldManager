using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for family member operations (parents and relatives).
/// </summary>
public interface IFamilyMemberService
{
    /// <summary>
    /// Get all members (parents and relatives) of a family.
    /// </summary>
    Task<IReadOnlyList<FamilyMemberDto>> GetMembersAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a relative account directly.
    /// </summary>
    Task<FamilyMemberDto> CreateRelativeAsync(UserId parentUserId, FamilyId familyId, CreateRelativeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a parent from family.
    /// </summary>
    Task RemoveParentAsync(UserId parentUserId, FamilyId familyId, UserId targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a relative from family.
    /// </summary>
    Task RemoveRelativeAsync(UserId parentUserId, FamilyId familyId, UserId relativeUserId, CancellationToken cancellationToken = default);
}
