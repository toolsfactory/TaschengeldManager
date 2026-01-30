using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for core family operations.
/// </summary>
/// <remarks>
/// Related services:
/// - IFamilyInvitationService: Invitation management
/// - IChildManagementService: Child management and PIN
/// - IFamilyMemberService: Parent and relative management
/// </remarks>
public interface IFamilyService
{
    /// <summary>
    /// Create a new family.
    /// </summary>
    Task<FamilyDto> CreateFamilyAsync(UserId userId, CreateFamilyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get family by ID.
    /// </summary>
    Task<FamilyDto?> GetFamilyAsync(UserId userId, FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get families for a user.
    /// </summary>
    Task<IReadOnlyList<FamilyDto>> GetFamiliesForUserAsync(UserId userId, CancellationToken cancellationToken = default);
}
