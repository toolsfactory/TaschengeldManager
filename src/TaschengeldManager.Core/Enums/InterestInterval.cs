namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Interval for interest calculation and crediting.
/// </summary>
public enum InterestInterval
{
    /// <summary>
    /// Interest credited monthly.
    /// </summary>
    Monthly = 1,

    /// <summary>
    /// Interest credited yearly (at year end).
    /// </summary>
    Yearly = 2
}
