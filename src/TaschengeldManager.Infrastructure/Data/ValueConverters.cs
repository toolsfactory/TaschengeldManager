using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data;


// Value converters for strongly typed IDs
public class UserIdConverter() : ValueConverter<UserId, Guid>(
    id => id.Value,
    guid => new UserId(guid));

public class FamilyIdConverter() : ValueConverter<FamilyId, Guid>(
    id => id.Value,
    guid => new FamilyId(guid));

public class AccountIdConverter() : ValueConverter<AccountId, Guid>(
    id => id.Value,
    guid => new AccountId(guid));

public class TransactionIdConverter() : ValueConverter<TransactionId, Guid>(
    id => id.Value,
    guid => new TransactionId(guid));

public class SessionIdConverter() : ValueConverter<SessionId, Guid>(
    id => id.Value,
    guid => new SessionId(guid));

public class PasskeyIdConverter() : ValueConverter<PasskeyId, Guid>(
    id => id.Value,
    guid => new PasskeyId(guid));

public class BiometricTokenIdConverter() : ValueConverter<BiometricTokenId, Guid>(
    id => id.Value,
    guid => new BiometricTokenId(guid));

public class TotpBackupCodeIdConverter() : ValueConverter<TotpBackupCodeId, Guid>(
    id => id.Value,
    guid => new TotpBackupCodeId(guid));

public class LoginAttemptIdConverter() : ValueConverter<LoginAttemptId, Guid>(
    id => id.Value,
    guid => new LoginAttemptId(guid));

public class FamilyInvitationIdConverter() : ValueConverter<FamilyInvitationId, Guid>(
    id => id.Value,
    guid => new FamilyInvitationId(guid));

public class ParentApprovalRequestIdConverter() : ValueConverter<ParentApprovalRequestId, Guid>(
    id => id.Value,
    guid => new ParentApprovalRequestId(guid));

public class MoneyRequestIdConverter() : ValueConverter<MoneyRequestId, Guid>(
    id => id.Value,
    guid => new MoneyRequestId(guid));

public class RecurringPaymentIdConverter() : ValueConverter<RecurringPaymentId, Guid>(
    id => id.Value,
    guid => new RecurringPaymentId(guid));
