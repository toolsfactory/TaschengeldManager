# Story M010-S01: Offline-Erkennung und Banner

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Benutzer** moechte ich **sehen koennen, wenn ich offline bin**, damit **ich weiss, dass meine Daten moeglicherweise nicht aktuell sind**.

## Akzeptanzkriterien

- [ ] Gegeben eine fehlende Internetverbindung, wenn der Benutzer die App nutzt, dann wird ein Offline-Banner am oberen Bildschirmrand angezeigt
- [ ] Gegeben ein angezeigtes Offline-Banner, wenn die Verbindung wiederhergestellt wird, dann verschwindet das Banner automatisch
- [ ] Gegeben eine instabile Verbindung, wenn der Status wechselt, dann wird das Banner entsprechend aktualisiert
- [ ] Gegeben Offline-Status, wenn der Benutzer eine Aktion ausfuehrt die Online benoetigt, dann wird eine entsprechende Meldung angezeigt

## UI-Entwurf

### Offline-Banner
```
+------------------------------------+
| [!] Offline - Daten evtl. veraltet |
+------------------------------------+
|  TaschengeldManager         [Gear] |
+------------------------------------+
|                                    |
|  Hallo, Max!                       |
|                                    |
|  Kontostand (Stand: vor 2 Std.)    |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
|                                    |
|  ...                               |
+------------------------------------+
```

### Verbindung wiederhergestellt
```
+------------------------------------+
| [Check] Wieder online              |
+------------------------------------+
|  TaschengeldManager         [Gear] |
+------------------------------------+
```
(Banner verschwindet nach 2 Sekunden)

## Technische Notizen

- Service: `IConnectivityService` wrappend `Connectivity.Current`
- MAUI Connectivity API verwenden
- Banner als globale Komponente in AppShell
- Debouncing bei instabiler Verbindung (500ms)
- Status wird in ViewModel als Observable Property gehalten

## Implementierungshinweise

```csharp
public interface IConnectivityService
{
    bool IsConnected { get; }
    event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;
}

public class ConnectivityService : IConnectivityService
{
    public ConnectivityService()
    {
        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    public bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        ConnectivityChanged?.Invoke(this, e);
    }
}

// AppShell.xaml.cs oder BaseViewModel
public partial class BaseViewModel : ObservableObject
{
    private readonly IConnectivityService _connectivity;

    [ObservableProperty]
    private bool _isOffline;

    protected BaseViewModel(IConnectivityService connectivity)
    {
        _connectivity = connectivity;
        IsOffline = !connectivity.IsConnected;

        connectivity.ConnectivityChanged += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsOffline = e.NetworkAccess != NetworkAccess.Internet;
            });
        };
    }
}
```

### XAML Banner
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <Grid RowDefinitions="Auto,*">
        <!-- Offline Banner -->
        <Border Grid.Row="0"
                BackgroundColor="#FFA000"
                IsVisible="{Binding IsOffline}"
                Padding="10,5">
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                <Label Text="[!]" FontAttributes="Bold" TextColor="White"/>
                <Label Text="Offline - Daten evtl. veraltet" TextColor="White"/>
            </HorizontalStackLayout>
        </Border>

        <!-- Page Content -->
        <ContentView Grid.Row="1">
            <!-- ... -->
        </ContentView>
    </Grid>
</ContentPage>
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Flugmodus aktivieren | Banner erscheint sofort |
| TC-002 | WLAN deaktivieren | Banner erscheint |
| TC-003 | Verbindung wiederherstellen | Banner verschwindet |
| TC-004 | Instabile Verbindung | Kein Flackern (Debouncing) |
| TC-005 | App-Start ohne Netz | Banner sofort sichtbar |

## Story Points

2

## Prioritaet

Hoch
