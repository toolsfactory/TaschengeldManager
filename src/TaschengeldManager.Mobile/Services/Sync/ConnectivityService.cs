namespace TaschengeldManager.Mobile.Services.Sync;

/// <summary>
/// Connectivity service using MAUI Connectivity API
/// </summary>
public class ConnectivityService : IConnectivityService, IDisposable
{
    public bool IsConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;

    public ConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, Microsoft.Maui.Networking.ConnectivityChangedEventArgs e)
    {
        ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs
        {
            IsConnected = e.NetworkAccess == NetworkAccess.Internet,
            NetworkAccess = e.NetworkAccess
        });
    }

    public void Dispose()
    {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }
}
