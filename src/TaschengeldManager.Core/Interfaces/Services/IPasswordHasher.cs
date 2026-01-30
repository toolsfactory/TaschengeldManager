namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for password hashing using Argon2id.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify a password against a hash.
    /// </summary>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Hash a PIN.
    /// </summary>
    string HashPin(string pin);

    /// <summary>
    /// Verify a PIN against a hash.
    /// </summary>
    bool VerifyPin(string pin, string hash);
}
