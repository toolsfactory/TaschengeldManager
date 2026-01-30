using System.ComponentModel.DataAnnotations;

namespace TaschengeldManager.Core.DTOs.Family;

/// <summary>
/// Request to accept an invitation.
/// </summary>
public record AcceptInvitationRequest
{
    /// <summary>
    /// Invitation token from the link.
    /// </summary>
    [Required]
    public required string Token { get; init; }
}
