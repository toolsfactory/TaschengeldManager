namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Response with TOTP setup information.
/// </summary>
public record SetupTotpResponse
{
    /// <summary>
    /// Secret key for manual entry.
    /// </summary>
    public required string Secret { get; init; }

    /// <summary>
    /// QR code data URL for authenticator apps.
    /// </summary>
    public required string QrCodeDataUrl { get; init; }

    /// <summary>
    /// Setup token for verification step.
    /// </summary>
    public required string SetupToken { get; init; }
}
