using Refit;
using TaschengeldManager.Core.DTOs.Auth;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.DTOs.MoneyRequest;
using TaschengeldManager.Core.DTOs.RecurringPayment;

namespace TaschengeldManager.Mobile.Services.Api;

/// <summary>
/// Refit API client interface for TaschengeldManager backend
/// </summary>
public interface ITaschengeldApi
{
    #region Auth Endpoints

    [Post("/api/auth/register")]
    Task<RegisterResponse> RegisterAsync([Body] RegisterRequest request, CancellationToken ct = default);

    [Post("/api/auth/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request, CancellationToken ct = default);

    [Post("/api/auth/login/child")]
    Task<LoginResponse> LoginChildAsync([Body] ChildLoginRequest request, CancellationToken ct = default);

    [Post("/api/auth/login/biometric")]
    Task<LoginResponse> LoginBiometricAsync([Body] BiometricLoginRequest request, CancellationToken ct = default);

    [Post("/api/auth/refresh")]
    Task<LoginResponse> RefreshTokenAsync([Body] RefreshTokenRequest request, CancellationToken ct = default);

    [Post("/api/auth/logout")]
    Task LogoutAsync([Body] LogoutRequest request, CancellationToken ct = default);

    [Post("/api/auth/logout/all")]
    Task LogoutAllAsync(CancellationToken ct = default);

    [Get("/api/auth/me")]
    Task<UserDto> GetCurrentUserAsync(CancellationToken ct = default);

    [Post("/api/auth/mfa/totp/setup")]
    Task<SetupTotpResponse> SetupTotpAsync(CancellationToken ct = default);

    [Post("/api/auth/mfa/totp/activate")]
    Task ActivateTotpAsync([Body] ActivateTotpRequest request, CancellationToken ct = default);

    [Post("/api/auth/mfa/verify")]
    Task<LoginResponse> VerifyTotpAsync([Body] VerifyTotpRequest request, CancellationToken ct = default);

    [Post("/api/auth/mfa/biometric/enable")]
    Task<EnableBiometricResponse> EnableBiometricAsync([Body] EnableBiometricRequest request, CancellationToken ct = default);

    [Delete("/api/auth/mfa/biometric/{deviceId}")]
    Task DisableBiometricAsync(string deviceId, CancellationToken ct = default);

    #endregion

    #region Account Endpoints

    [Get("/api/account/{id}")]
    Task<AccountDto> GetAccountAsync(Guid id, CancellationToken ct = default);

    [Get("/api/account/me")]
    Task<AccountDto> GetMyAccountAsync(CancellationToken ct = default);

    [Get("/api/account/family/{familyId}")]
    Task<List<AccountDto>> GetFamilyAccountsAsync(Guid familyId, CancellationToken ct = default);

    [Post("/api/account/{accountId}/deposit")]
    Task<TransactionDto> DepositAsync(Guid accountId, [Body] DepositRequest request, CancellationToken ct = default);

    [Post("/api/account/withdraw")]
    Task<TransactionDto> WithdrawAsync([Body] WithdrawRequest request, CancellationToken ct = default);

    [Post("/api/account/gift")]
    Task<TransactionDto> GiftAsync([Body] GiftRequest request, CancellationToken ct = default);

    [Get("/api/account/{accountId}/transactions")]
    Task<List<TransactionDto>> GetTransactionsAsync(Guid accountId, int? limit = null, int? offset = null, CancellationToken ct = default);

    [Get("/api/account/gifts")]
    Task<List<TransactionDto>> GetMyGiftsAsync(CancellationToken ct = default);

    [Put("/api/account/{accountId}/interest")]
    Task<AccountDto> SetInterestAsync(Guid accountId, [Body] SetInterestRequest request, CancellationToken ct = default);

    #endregion

    #region Family Endpoints

    [Post("/api/family")]
    Task<FamilyDto> CreateFamilyAsync([Body] CreateFamilyRequest request, CancellationToken ct = default);

    [Get("/api/family/{id}")]
    Task<FamilyDto> GetFamilyAsync(Guid id, CancellationToken ct = default);

    [Get("/api/family")]
    Task<List<FamilyDto>> GetMyFamiliesAsync(CancellationToken ct = default);

    [Get("/api/family/{familyId}/members")]
    Task<List<FamilyMemberDto>> GetFamilyMembersAsync(Guid familyId, CancellationToken ct = default);

    [Get("/api/family/{familyId}/children")]
    Task<List<ChildDto>> GetChildrenAsync(Guid familyId, CancellationToken ct = default);

    [Post("/api/family/{familyId}/children")]
    Task<ChildDto> AddChildAsync(Guid familyId, [Body] AddChildRequest request, CancellationToken ct = default);

    [Delete("/api/family/{familyId}/children/{childId}")]
    Task RemoveChildAsync(Guid familyId, Guid childId, CancellationToken ct = default);

    [Post("/api/family/{familyId}/invitations")]
    Task<InvitationDto> InviteAsync(Guid familyId, [Body] InviteRequest request, CancellationToken ct = default);

    [Get("/api/family/{familyId}/invitations")]
    Task<List<InvitationDto>> GetInvitationsAsync(Guid familyId, CancellationToken ct = default);

    [Post("/api/family/invitations/accept")]
    Task AcceptInvitationAsync([Body] AcceptInvitationRequest request, CancellationToken ct = default);

    [Delete("/api/family/{familyId}/invitations/{invitationId}")]
    Task WithdrawInvitationAsync(Guid familyId, Guid invitationId, CancellationToken ct = default);

    #endregion

    #region Money Request Endpoints

    [Get("/api/money-requests/{id}")]
    Task<MoneyRequestDto> GetMoneyRequestAsync(Guid id, CancellationToken ct = default);

    [Get("/api/money-requests/my")]
    Task<List<MoneyRequestDto>> GetMyRequestsAsync(CancellationToken ct = default);

    [Get("/api/money-requests/family")]
    Task<List<MoneyRequestDto>> GetFamilyRequestsAsync([Query] string? status = null, CancellationToken ct = default);

    [Post("/api/money-requests")]
    Task<MoneyRequestDto> CreateRequestAsync([Body] CreateMoneyRequestRequest request, CancellationToken ct = default);

    [Post("/api/money-requests/{id}/respond")]
    Task<MoneyRequestDto> RespondToRequestAsync(Guid id, [Body] RespondToRequestRequest request, CancellationToken ct = default);

    [Post("/api/money-requests/{id}/withdraw")]
    Task WithdrawRequestAsync(Guid id, CancellationToken ct = default);

    #endregion

    #region Recurring Payment Endpoints

    [Get("/api/recurring-payments")]
    Task<List<RecurringPaymentDto>> GetRecurringPaymentsAsync(CancellationToken ct = default);

    [Get("/api/recurring-payments/{id}")]
    Task<RecurringPaymentDto> GetRecurringPaymentAsync(Guid id, CancellationToken ct = default);

    [Post("/api/recurring-payments")]
    Task<RecurringPaymentDto> CreateRecurringPaymentAsync([Body] CreateRecurringPaymentRequest request, CancellationToken ct = default);

    [Put("/api/recurring-payments/{id}")]
    Task<RecurringPaymentDto> UpdateRecurringPaymentAsync(Guid id, [Body] UpdateRecurringPaymentRequest request, CancellationToken ct = default);

    [Post("/api/recurring-payments/{id}/pause")]
    Task PauseRecurringPaymentAsync(Guid id, CancellationToken ct = default);

    [Post("/api/recurring-payments/{id}/resume")]
    Task ResumeRecurringPaymentAsync(Guid id, CancellationToken ct = default);

    [Delete("/api/recurring-payments/{id}")]
    Task DeleteRecurringPaymentAsync(Guid id, CancellationToken ct = default);

    #endregion

    #region Session Endpoints

    [Get("/api/session")]
    Task<List<SessionDto>> GetSessionsAsync(CancellationToken ct = default);

    [Delete("/api/session/{sessionId}")]
    Task RevokeSessionAsync(Guid sessionId, CancellationToken ct = default);

    [Delete("/api/session/others")]
    Task RevokeOtherSessionsAsync(CancellationToken ct = default);

    #endregion
}

// Local DTOs that mirror API definitions
public record LogoutRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record ActivateTotpRequest
{
    public string SetupToken { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
