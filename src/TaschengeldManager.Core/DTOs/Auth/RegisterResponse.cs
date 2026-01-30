namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Response after successful registration.
/// </summary>
public record RegisterResponse
{
    /// <summary>
    /// User ID.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Temporary token for MFA setup.
    /// </summary>
    public required string MfaSetupToken { get; init; }

    /// <summary>
    /// Whether MFA setup is required before login.
    /// </summary>
    public bool MfaSetupRequired { get; init; } = true;
}
