# Story M001-S05: Navigation Service mit Shell

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **einen Navigation Service mit Shell-Navigation implementieren**, damit **ich programmatisch zwischen Seiten navigieren kann und die Navigation testbar ist**.

## Akzeptanzkriterien

- [ ] Gegeben der NavigationService, wenn NavigateToAsync aufgerufen wird, dann navigiert die App zur Zielseite
- [ ] Gegeben die Navigation, wenn Parameter übergeben werden, dann sind sie in der Zielseite verfügbar
- [ ] Gegeben die Navigation, wenn GoBackAsync aufgerufen wird, dann kehrt die App zur vorherigen Seite zurück
- [ ] Gegeben die Shell, wenn Routen registriert werden, dann sind sie über URI erreichbar

## Technische Hinweise

### INavigationService Interface
```csharp
public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, IDictionary<string, object> parameters);
    Task GoBackAsync();
    Task GoToRootAsync();
}
```

### NavigationService Implementation
```csharp
public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task GoToRootAsync()
    {
        await Shell.Current.GoToAsync("//");
    }
}
```

### AppShell.xaml
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:TaschengeldManager.Mobile.Views"
       x:Class="TaschengeldManager.Mobile.AppShell"
       FlyoutBehavior="Disabled">

    <!-- Login Route -->
    <ShellContent Route="login"
                  ContentTemplate="{DataTemplate views:LoginPage}" />

    <!-- Hauptnavigation (nach Login) -->
    <TabBar Route="main">
        <Tab Title="Dashboard" Icon="home.png">
            <ShellContent Route="dashboard"
                          ContentTemplate="{DataTemplate views:DashboardPage}" />
        </Tab>
        <Tab Title="Transaktionen" Icon="list.png">
            <ShellContent Route="transactions"
                          ContentTemplate="{DataTemplate views:TransactionsPage}" />
        </Tab>
        <Tab Title="Einstellungen" Icon="settings.png">
            <ShellContent Route="settings"
                          ContentTemplate="{DataTemplate views:SettingsPage}" />
        </Tab>
    </TabBar>

</Shell>
```

### AppShell.xaml.cs - Routen registrieren
```csharp
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Detail-Routen registrieren
        Routing.RegisterRoute("transaction-detail", typeof(TransactionDetailPage));
        Routing.RegisterRoute("add-transaction", typeof(AddTransactionPage));
        Routing.RegisterRoute("child-detail", typeof(ChildDetailPage));
    }
}
```

### ViewModel mit Navigation
```csharp
public partial class LoginViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public LoginViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // Nach erfolgreichem Login
        await _navigationService.NavigateToAsync("//main/dashboard");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-14 | NavigateToAsync aufrufen | Seite wird angezeigt |
| TC-M001-15 | Navigation mit Parametern | Parameter verfügbar |
| TC-M001-16 | GoBackAsync aufrufen | Vorherige Seite angezeigt |
| TC-M001-17 | Registrierte Route aufrufen | Navigation erfolgreich |

## Story Points
3

## Priorität
Hoch
