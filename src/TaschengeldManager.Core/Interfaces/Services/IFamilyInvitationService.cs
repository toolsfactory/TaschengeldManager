using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for family invitation operations.
/// </summary>
public interface IFamilyInvitationService
{
    /// <summary>
    /// Invite someone to the family.
    /// </summary>
    Task<InvitationDto> InviteToFamilyAsync(UserId parentUserId, FamilyId familyId, InviteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Accept an invitation.
    /// </summary>
    Task AcceptInvitationAsync(UserId userId, AcceptInvitationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject an invitation.
    /// </summary>
    Task RejectInvitationAsync(UserId userId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Withdraw an invitation.
    /// </summary>
    Task WithdrawInvitationAsync(UserId parentUserId, FamilyInvitationId invitationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending invitations for a family.
    /// </summary>
    Task<IReadOnlyList<InvitationDto>> GetPendingInvitationsAsync(UserId parentUserId, FamilyId familyId, CancellationToken cancellationToken = default);
}
