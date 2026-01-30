namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Types of biometric authentication.
/// </summary>
public enum BiometryType
{
    /// <summary>
    /// No biometry available.
    /// </summary>
    None = 0,

    /// <summary>
    /// Fingerprint authentication.
    /// </summary>
    Fingerprint = 1,

    /// <summary>
    /// Face ID authentication.
    /// </summary>
    FaceId = 2,

    /// <summary>
    /// Iris scan authentication.
    /// </summary>
    Iris = 3
}
