namespace TaschengeldManager.Mobile.Services.Storage;

/// <summary>
/// Service for secure storage of sensitive data (tokens, credentials)
/// </summary>
public interface ISecureStorageService
{
    /// <summary>
    /// Store a value securely
    /// </summary>
    Task SetAsync(string key, string value);

    /// <summary>
    /// Retrieve a securely stored value
    /// </summary>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Remove a securely stored value
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Check if a key exists
    /// </summary>
    Task<bool> ContainsKeyAsync(string key);

    /// <summary>
    /// Clear all stored values
    /// </summary>
    Task ClearAllAsync();
}

/// <summary>
/// Keys for secure storage
/// </summary>
public static class SecureStorageKeys
{
    public const string AccessToken = "access_token";
    public const string RefreshToken = "refresh_token";
    public const string BiometricToken = "biometric_token";
    public const string DeviceId = "device_id";
    public const string UserId = "user_id";
    public const string UserRole = "user_role";
}
