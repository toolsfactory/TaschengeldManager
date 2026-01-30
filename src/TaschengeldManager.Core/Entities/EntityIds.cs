using StronglyTypedIds;

// EF Core ValueConverter sind manuell in Infrastructure/Data/ApplicationDbContext.cs implementiert
[assembly: StronglyTypedIdDefaults(Template.Guid)]

namespace TaschengeldManager.Core.Entities;


/// <summary>
/// Strongly typed ID for User entities.
/// </summary>
[StronglyTypedId]
public partial struct UserId { }

/// <summary>
/// Strongly typed ID for Family entities.
/// </summary>
[StronglyTypedId]
public partial struct FamilyId { }

/// <summary>
/// Strongly typed ID for Account entities.
/// </summary>
[StronglyTypedId]
public partial struct AccountId { }

/// <summary>
/// Strongly typed ID for Transaction entities.
/// </summary>
[StronglyTypedId]
public partial struct TransactionId { }

/// <summary>
/// Strongly typed ID for Session entities.
/// </summary>
[StronglyTypedId]
public partial struct SessionId { }

/// <summary>
/// Strongly typed ID for Passkey entities.
/// </summary>
[StronglyTypedId]
public partial struct PasskeyId { }

/// <summary>
/// Strongly typed ID for BiometricToken entities.
/// </summary>
[StronglyTypedId]
public partial struct BiometricTokenId { }

/// <summary>
/// Strongly typed ID for TotpBackupCode entities.
/// </summary>
[StronglyTypedId]
public partial struct TotpBackupCodeId { }

/// <summary>
/// Strongly typed ID for LoginAttempt entities.
/// </summary>
[StronglyTypedId]
public partial struct LoginAttemptId { }

/// <summary>
/// Strongly typed ID for FamilyInvitation entities.
/// </summary>
[StronglyTypedId]
public partial struct FamilyInvitationId { }

/// <summary>
/// Strongly typed ID for ParentApprovalRequest entities.
/// </summary>
[StronglyTypedId]
public partial struct ParentApprovalRequestId { }

/// <summary>
/// Strongly typed ID for MoneyRequest entities.
/// </summary>
[StronglyTypedId]
public partial struct MoneyRequestId { }

/// <summary>
/// Strongly typed ID for RecurringPayment entities.
/// </summary>
[StronglyTypedId]
public partial struct RecurringPaymentId { }
