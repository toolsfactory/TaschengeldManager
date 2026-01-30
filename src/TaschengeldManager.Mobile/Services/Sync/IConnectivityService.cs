namespace TaschengeldManager.Mobile.Services.Sync;

/// <summary>
/// Service for monitoring network connectivity
/// </summary>
public interface IConnectivityService
{
    /// <summary>
    /// Gets whether the device currently has internet connectivity
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Event fired when connectivity changes
    /// </summary>
    event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;
}

/// <summary>
/// Event args for connectivity changes
/// </summary>
public class ConnectivityChangedEventArgs : EventArgs
{
    public bool IsConnected { get; init; }
    public NetworkAccess NetworkAccess { get; init; }
}
