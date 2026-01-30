using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for child management operations.
/// </summary>
public interface IChildManagementService
{
    /// <summary>
    /// Add a child to the family.
    /// </summary>
    Task<ChildDto> AddChildAsync(UserId parentUserId, FamilyId familyId, AddChildRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a child from the family.
    /// </summary>
    Task RemoveChildAsync(UserId parentUserId, FamilyId familyId, UserId childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all children in a family.
    /// </summary>
    Task<IReadOnlyList<ChildDto>> GetChildrenAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Change a child's PIN.
    /// </summary>
    Task ChangeChildPinAsync(UserId parentUserId, FamilyId familyId, UserId childId, ChangeChildPinRequest request, CancellationToken cancellationToken = default);
}
