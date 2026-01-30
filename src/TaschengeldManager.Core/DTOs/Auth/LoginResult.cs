namespace TaschengeldManager.Core.DTOs.Auth;

/// <summary>
/// Result of a login attempt. Either a successful login or MFA is required.
/// </summary>
public record LoginResult
{
    /// <summary>
    /// Whether login was successful (without MFA) or MFA is required.
    /// </summary>
    public required bool RequiresMfa { get; init; }

    /// <summary>
    /// The login response (only set when RequiresMfa is false).
    /// </summary>
    public LoginResponse? LoginResponse { get; init; }

    /// <summary>
    /// The MFA required response (only set when RequiresMfa is true).
    /// </summary>
    public MfaRequiredResponse? MfaResponse { get; init; }

    /// <summary>
    /// Creates a successful login result.
    /// </summary>
    public static LoginResult Success(LoginResponse response) => new()
    {
        RequiresMfa = false,
        LoginResponse = response
    };

    /// <summary>
    /// Creates an MFA required result.
    /// </summary>
    public static LoginResult MfaRequired(MfaRequiredResponse response) => new()
    {
        RequiresMfa = true,
        MfaResponse = response
    };
}
