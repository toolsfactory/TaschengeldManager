using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Cache key constants and helpers for consistent cache key naming.
/// </summary>
public static class CacheKeys
{
    private const string Prefix = "taschengeld";

    // User cache keys
    public static string User(UserId userId) => $"{Prefix}:user:{userId.Value}";
    public static string UserByEmail(string email) => $"{Prefix}:user:email:{email.ToLowerInvariant()}";

    // Family cache keys
    public static string Family(FamilyId familyId) => $"{Prefix}:family:{familyId.Value}";
    public static string FamilyByCode(string familyCode) => $"{Prefix}:family:code:{familyCode.ToUpperInvariant()}";
    public static string FamiliesForUser(UserId userId) => $"{Prefix}:families:user:{userId.Value}";

    // Pattern for invalidation
    public static string FamilyPattern(FamilyId familyId) => $"{Prefix}:family:{familyId.Value}*";
    public static string UserPattern(UserId userId) => $"{Prefix}:user:{userId.Value}*";
    public static string AllFamiliesForUserPattern() => $"{Prefix}:families:user:*";
}
