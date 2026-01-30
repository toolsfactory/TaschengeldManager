namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for interest calculation and crediting.
/// </summary>
public interface IInterestService
{
    /// <summary>
    /// Calculate and credit interest for all eligible accounts.
    /// Called by the background job.
    /// </summary>
    /// <returns>Number of accounts that received interest.</returns>
    Task<int> ProcessDueInterestAsync(CancellationToken cancellationToken = default);
}
