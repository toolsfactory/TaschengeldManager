namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Response with generated backup codes.
/// </summary>
public record BackupCodesResponse
{
    /// <summary>
    /// List of backup codes (show only once!).
    /// </summary>
    public required IReadOnlyList<string> BackupCodes { get; init; }

    /// <summary>
    /// Message for user.
    /// </summary>
    public string Message { get; init; } = "Bewahre diese Codes sicher auf. Sie werden nur einmal angezeigt!";
}
