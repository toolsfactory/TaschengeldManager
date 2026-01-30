using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Mobile.Data.Entities;

namespace TaschengeldManager.Mobile.Data;

/// <summary>
/// Mapper for converting between DTOs and cached entities
/// </summary>
public static class CacheMapper
{
    #region Account Mapping

    public static CachedAccount ToCached(this AccountDto dto) => new()
    {
        Id = dto.Id,
        UserId = dto.UserId,
        OwnerName = dto.OwnerName,
        Balance = dto.Balance,
        InterestEnabled = dto.InterestEnabled,
        InterestRate = dto.InterestRate,
        CreatedAt = dto.CreatedAt
    };

    public static AccountDto ToDto(this CachedAccount cached) => new()
    {
        Id = cached.Id,
        UserId = cached.UserId,
        OwnerName = cached.OwnerName,
        Balance = cached.Balance,
        InterestEnabled = cached.InterestEnabled,
        InterestRate = cached.InterestRate,
        CreatedAt = cached.CreatedAt
    };

    #endregion

    #region Transaction Mapping

    /// <summary>
    /// Convert DTO to cached entity. AccountId must be set separately as it's not in the DTO.
    /// </summary>
    public static CachedTransaction ToCached(this TransactionDto dto, Guid accountId) => new()
    {
        Id = dto.Id,
        AccountId = accountId,
        Type = dto.Type,
        Amount = dto.Amount,
        Description = dto.Description,
        Category = dto.Category,
        CreatedByName = dto.CreatedByName,
        BalanceAfter = dto.BalanceAfter,
        CreatedAt = dto.CreatedAt
    };

    public static TransactionDto ToDto(this CachedTransaction cached) => new()
    {
        Id = cached.Id,
        Type = cached.Type,
        Amount = cached.Amount,
        Description = cached.Description,
        Category = cached.Category,
        CreatedByName = cached.CreatedByName,
        BalanceAfter = cached.BalanceAfter,
        CreatedAt = cached.CreatedAt
    };

    #endregion

    #region Family Mapping

    public static CachedFamily ToCached(this FamilyDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        FamilyCode = dto.FamilyCode,
        CreatedAt = dto.CreatedAt
    };

    public static FamilyDto ToDto(this CachedFamily cached) => new()
    {
        Id = cached.Id,
        Name = cached.Name,
        FamilyCode = cached.FamilyCode,
        CreatedAt = cached.CreatedAt
    };

    #endregion

    #region Child Mapping

    /// <summary>
    /// Convert DTO to cached entity. FamilyId must be set separately as it's not in the DTO.
    /// </summary>
    public static CachedChild ToCached(this ChildDto dto, Guid familyId) => new()
    {
        Id = dto.Id,
        FamilyId = familyId,
        AccountId = dto.AccountId,
        Nickname = dto.Nickname,
        Balance = dto.Balance,
        IsLocked = dto.IsLocked,
        MfaEnabled = dto.MfaEnabled,
        CreatedAt = dto.CreatedAt
    };

    public static ChildDto ToDto(this CachedChild cached) => new()
    {
        Id = cached.Id,
        AccountId = cached.AccountId,
        Nickname = cached.Nickname,
        Balance = cached.Balance,
        IsLocked = cached.IsLocked,
        MfaEnabled = cached.MfaEnabled,
        CreatedAt = cached.CreatedAt
    };

    #endregion

    #region Collection Extensions

    public static List<CachedAccount> ToCached(this IEnumerable<AccountDto> dtos) =>
        dtos.Select(d => d.ToCached()).ToList();

    public static List<AccountDto> ToDtos(this IEnumerable<CachedAccount> cached) =>
        cached.Select(c => c.ToDto()).ToList();

    public static List<CachedTransaction> ToCached(this IEnumerable<TransactionDto> dtos, Guid accountId) =>
        dtos.Select(d => d.ToCached(accountId)).ToList();

    public static List<TransactionDto> ToDtos(this IEnumerable<CachedTransaction> cached) =>
        cached.Select(c => c.ToDto()).ToList();

    public static List<CachedChild> ToCached(this IEnumerable<ChildDto> dtos, Guid familyId) =>
        dtos.Select(d => d.ToCached(familyId)).ToList();

    public static List<ChildDto> ToDtos(this IEnumerable<CachedChild> cached) =>
        cached.Select(c => c.ToDto()).ToList();

    #endregion
}
