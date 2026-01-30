namespace TaschengeldManager.Core.Enums;

/// <summary>
/// Interval for recurring payments.
/// </summary>
public enum PaymentInterval
{
    /// <summary>
    /// Weekly payment (same day each week).
    /// </summary>
    Weekly = 1,

    /// <summary>
    /// Biweekly payment (every 14 days).
    /// </summary>
    Biweekly = 2,

    /// <summary>
    /// Monthly payment (same day each month).
    /// </summary>
    Monthly = 3
}
