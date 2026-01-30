# Story M001-S08: Connectivity-Service

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **über den Verbindungsstatus informiert werden**, damit **ich weiß, wann ich online oder offline bin und meine Daten synchronisiert werden**.

## Akzeptanzkriterien

- [ ] Gegeben der ConnectivityService, wenn die Verbindung wechselt, dann wird ein Event ausgelöst
- [ ] Gegeben keine Internetverbindung, wenn der Benutzer eine Online-Aktion ausführt, dann wird eine Meldung angezeigt
- [ ] Gegeben die App, wenn sie startet, dann wird der aktuelle Verbindungsstatus geprüft
- [ ] Gegeben der Verbindungsstatus, wenn er sich ändert, dann können ViewModels darauf reagieren

## Technische Hinweise

### IConnectivityService Interface
```csharp
public interface IConnectivityService
{
    bool IsConnected { get; }
    event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;
    NetworkAccess CurrentNetworkAccess { get; }
    IEnumerable<ConnectionProfile> ConnectionProfiles { get; }
}
```

### ConnectivityService Implementation
```csharp
public class ConnectivityService : IConnectivityService, IDisposable
{
    public event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;

    public bool IsConnected =>
        Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    public NetworkAccess CurrentNetworkAccess =>
        Connectivity.Current.NetworkAccess;

    public IEnumerable<ConnectionProfile> ConnectionProfiles =>
        Connectivity.Current.ConnectionProfiles;

    public ConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        ConnectivityChanged?.Invoke(this, e);
    }

    public void Dispose()
    {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }
}
```

### Verwendung im ViewModel
```csharp
public partial class DashboardViewModel : ObservableObject
{
    private readonly IConnectivityService _connectivity;

    [ObservableProperty]
    private bool _isOnline;

    [ObservableProperty]
    private string _connectivityMessage = string.Empty;

    public DashboardViewModel(IConnectivityService connectivity)
    {
        _connectivity = connectivity;
        _connectivity.ConnectivityChanged += OnConnectivityChanged;

        IsOnline = _connectivity.IsConnected;
        UpdateConnectivityMessage();
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsOnline = e.NetworkAccess == NetworkAccess.Internet;
            UpdateConnectivityMessage();
        });
    }

    private void UpdateConnectivityMessage()
    {
        ConnectivityMessage = IsOnline
            ? string.Empty
            : "Du bist offline. Einige Funktionen sind eingeschränkt.";
    }

    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        if (!_connectivity.IsConnected)
        {
            await Shell.Current.DisplayAlert(
                "Keine Verbindung",
                "Bitte stelle eine Internetverbindung her, um die Daten zu aktualisieren.",
                "OK");
            return;
        }

        // Daten laden...
    }
}
```

### XAML Connectivity-Anzeige
```xml
<!-- Offline-Banner -->
<Frame IsVisible="{Binding IsOnline, Converter={StaticResource InverseBoolConverter}}"
       BackgroundColor="{StaticResource Warning}"
       Padding="8"
       CornerRadius="0"
       HasShadow="False">
    <Label Text="{Binding ConnectivityMessage}"
           TextColor="White"
           HorizontalTextAlignment="Center"
           FontSize="12" />
</Frame>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-26 | App startet mit Internet | IsConnected = true |
| TC-M001-27 | Verbindung unterbrochen | Event wird ausgelöst |
| TC-M001-28 | Verbindung wiederhergestellt | IsConnected wird true |
| TC-M001-29 | Offline-Action versuchen | Fehlermeldung angezeigt |

## Story Points
2

## Priorität
Hoch
