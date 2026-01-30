using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Family entities.
/// </summary>
public interface IFamilyRepository : IRepository<Family, FamilyId>
{
    /// <summary>
    /// Get family by family code.
    /// </summary>
    Task<Family?> GetByFamilyCodeAsync(string familyCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get family with all members.
    /// </summary>
    Task<Family?> GetWithMembersAsync(FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get families where user is a parent.
    /// </summary>
    Task<IReadOnlyList<Family>> GetFamiliesForParentAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get families where user is a relative.
    /// </summary>
    Task<IReadOnlyList<Family>> GetFamiliesForRelativeAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if family code exists.
    /// </summary>
    Task<bool> FamilyCodeExistsAsync(string familyCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user is parent of family.
    /// </summary>
    Task<bool> IsParentOfFamilyAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user is relative of family.
    /// </summary>
    Task<bool> IsRelativeOfFamilyAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add parent to family.
    /// </summary>
    Task AddParentAsync(FamilyId familyId, UserId userId, bool isPrimary = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add relative to family.
    /// </summary>
    Task AddRelativeAsync(FamilyId familyId, UserId userId, string? relationshipDescription = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove parent from family.
    /// </summary>
    Task RemoveParentAsync(FamilyId familyId, UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove relative from family.
    /// </summary>
    Task RemoveRelativeAsync(FamilyId familyId, UserId userId, CancellationToken cancellationToken = default);
}
