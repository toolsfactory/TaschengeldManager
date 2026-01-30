using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for Passkey entities.
/// </summary>
public interface IPasskeyRepository : IRepository<Passkey, PasskeyId>
{
    /// <summary>
    /// Get passkey by credential ID.
    /// </summary>
    Task<Passkey?> GetByCredentialIdAsync(string credentialId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all passkeys for a user.
    /// </summary>
    Task<IReadOnlyList<Passkey>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if credential ID exists.
    /// </summary>
    Task<bool> CredentialIdExistsAsync(string credentialId, CancellationToken cancellationToken = default);
}
