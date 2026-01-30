using TaschengeldManager.Core.Enums;

namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Response when MFA verification is required.
/// </summary>
public record MfaRequiredResponse
{
    /// <summary>
    /// Temporary token for MFA verification.
    /// </summary>
    public required string MfaToken { get; init; }

    /// <summary>
    /// Available MFA methods for this user.
    /// </summary>
    public required IReadOnlyList<MfaMethod> AvailableMethods { get; init; }

    /// <summary>
    /// User ID for context.
    /// </summary>
    public required Guid UserId { get; init; }
}
