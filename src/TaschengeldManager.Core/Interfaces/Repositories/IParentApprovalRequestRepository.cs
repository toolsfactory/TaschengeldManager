using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for ParentApprovalRequest entities.
/// </summary>
public interface IParentApprovalRequestRepository : IRepository<ParentApprovalRequest, ParentApprovalRequestId>
{
    /// <summary>
    /// Get pending requests for a family (for parents to see).
    /// </summary>
    Task<IReadOnlyList<ParentApprovalRequest>> GetPendingByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending request for a child.
    /// </summary>
    Task<ParentApprovalRequest?> GetPendingByChildAsync(UserId childUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count recent requests by child (for rate limiting).
    /// </summary>
    Task<int> GetRecentCountByChildAsync(UserId childUserId, TimeSpan window, CancellationToken cancellationToken = default);

    /// <summary>
    /// Expire old pending requests.
    /// </summary>
    Task ExpireOldRequestsAsync(CancellationToken cancellationToken = default);
}
