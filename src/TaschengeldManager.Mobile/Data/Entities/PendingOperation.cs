using SQLite;

namespace TaschengeldManager.Mobile.Data.Entities;

/// <summary>
/// Represents an operation queued for sync when offline
/// </summary>
[Table("PendingOperations")]
public class PendingOperation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string OperationType { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public Guid? EntityId { get; set; }

    /// <summary>
    /// JSON serialized payload for the operation
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int RetryCount { get; set; }

    public string? LastError { get; set; }

    public DateTime? LastAttemptAt { get; set; }
}

/// <summary>
/// Operation types for pending sync
/// </summary>
public static class OperationTypes
{
    public const string CreateExpense = "CreateExpense";
    public const string CreateMoneyRequest = "CreateMoneyRequest";
    public const string WithdrawMoneyRequest = "WithdrawMoneyRequest";
}
