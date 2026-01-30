namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Types of transactions on an account.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Deposit from parent.
    /// </summary>
    Deposit = 0,

    /// <summary>
    /// Withdrawal/expense by child.
    /// </summary>
    Withdrawal = 1,

    /// <summary>
    /// Automatic recurring allowance payment.
    /// </summary>
    Allowance = 2,

    /// <summary>
    /// Gift from a relative.
    /// </summary>
    Gift = 3,

    /// <summary>
    /// Interest payment.
    /// </summary>
    Interest = 4,

    /// <summary>
    /// Correction by parent.
    /// </summary>
    Correction = 5
}
