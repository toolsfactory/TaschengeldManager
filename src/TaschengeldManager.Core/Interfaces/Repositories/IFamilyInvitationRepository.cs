using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for FamilyInvitation entities.
/// </summary>
public interface IFamilyInvitationRepository : IRepository<FamilyInvitation, FamilyInvitationId>
{
    /// <summary>
    /// Get invitation by token hash.
    /// </summary>
    Task<FamilyInvitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending invitations for a family.
    /// </summary>
    Task<IReadOnlyList<FamilyInvitation>> GetPendingByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending invitations for an email.
    /// </summary>
    Task<IReadOnlyList<FamilyInvitation>> GetPendingByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Expire old pending invitations.
    /// </summary>
    Task ExpireOldInvitationsAsync(CancellationToken cancellationToken = default);
}
