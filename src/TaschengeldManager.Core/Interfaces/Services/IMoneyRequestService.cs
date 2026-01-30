using TaschengeldManager.Core.DTOs.MoneyRequest;
using TaschengeldManager.Core.Entities;
using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for money request management.
/// </summary>
public interface IMoneyRequestService
{
    /// <summary>
    /// Create a new money request (by child).
    /// </summary>
    Task<MoneyRequestDto> CreateAsync(UserId childUserId, CreateMoneyRequestRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a money request by ID.
    /// </summary>
    Task<MoneyRequestDto?> GetByIdAsync(UserId userId, MoneyRequestId requestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all requests for the current child.
    /// </summary>
    Task<IReadOnlyList<MoneyRequestDto>> GetMyRequestsAsync(UserId childUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all requests for children in the parent's family.
    /// </summary>
    Task<IReadOnlyList<MoneyRequestDto>> GetFamilyRequestsAsync(UserId parentUserId, RequestStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Respond to a money request (by parent).
    /// </summary>
    Task<MoneyRequestDto> RespondAsync(UserId parentUserId, MoneyRequestId requestId, RespondToRequestRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Withdraw a pending money request (by child).
    /// </summary>
    Task WithdrawAsync(UserId childUserId, MoneyRequestId requestId, CancellationToken cancellationToken = default);
}
