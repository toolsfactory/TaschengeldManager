using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for MoneyRequest entities.
/// </summary>
public interface IMoneyRequestRepository : IRepository<MoneyRequest, MoneyRequestId>
{
    /// <summary>
    /// Get all requests for a specific child.
    /// </summary>
    Task<IReadOnlyList<MoneyRequest>> GetByChildIdAsync(UserId childUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all pending requests for children in a family.
    /// </summary>
    Task<IReadOnlyList<MoneyRequest>> GetPendingForFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all requests (any status) for children in a family.
    /// </summary>
    Task<IReadOnlyList<MoneyRequest>> GetAllForFamilyAsync(FamilyId familyId, RequestStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get request with all related details.
    /// </summary>
    Task<MoneyRequest?> GetWithDetailsAsync(MoneyRequestId requestId, CancellationToken cancellationToken = default);
}
