using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entities.
/// </summary>
public interface IUserRepository : IRepository<User, UserId>
{
    /// <summary>
    /// Get user by email (case-insensitive).
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by nickname within a family.
    /// </summary>
    Task<User?> GetByNicknameInFamilyAsync(FamilyId familyId, string nickname, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if email is already registered.
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user with their passkeys.
    /// </summary>
    Task<User?> GetWithPasskeysAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user with their sessions.
    /// </summary>
    Task<User?> GetWithSessionsAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get children of a family.
    /// </summary>
    Task<IReadOnlyList<User>> GetChildrenByFamilyAsync(FamilyId familyId, CancellationToken cancellationToken = default);
}
