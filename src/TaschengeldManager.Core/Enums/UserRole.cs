namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Represents the role of a user in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Parent with full access to family and children's accounts.
    /// </summary>
    Parent = 0,

    /// <summary>
    /// Child with access only to their own account.
    /// </summary>
    Child = 1,

    /// <summary>
    /// Relative with limited access - can only gift money and see own transfers.
    /// </summary>
    Relative = 2
}
