# Story M003-S01: AppShell mit rollenbasierter Navigation

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Benutzer mit einer bestimmten Rolle** möchte ich **nur die für mich relevanten Navigationspunkte sehen**, damit **die App übersichtlich bleibt und ich schnell zu meinen Funktionen komme**.

## Akzeptanzkriterien

- [ ] Gegeben ein Eltern-Login, wenn das Dashboard geladen wird, dann werden Eltern-spezifische Tabs angezeigt
- [ ] Gegeben ein Kind-Login, wenn das Dashboard geladen wird, dann werden Kind-spezifische Tabs angezeigt
- [ ] Gegeben ein Verwandten-Login, wenn das Dashboard geladen wird, dann werden Verwandten-spezifische Tabs angezeigt
- [ ] Gegeben ein Rollenwechsel, wenn er stattfindet, dann wird die Navigation dynamisch angepasst

## UI-Entwurf

```
Eltern-Navigation:
┌─────────────────────────────┐
│  [Dashboard] [Kinder] [⚙️]  │
└─────────────────────────────┘

Kind-Navigation:
┌─────────────────────────────┐
│  [Dashboard] [Ausgaben] [⚙️]│
└─────────────────────────────┘

Verwandten-Navigation:
┌─────────────────────────────┐
│  [Dashboard] [Geschenke][⚙️]│
└─────────────────────────────┘
```

## Technische Hinweise

### AppShell.xaml
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:TaschengeldManager.Mobile.Views"
       xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
       x:Class="TaschengeldManager.Mobile.AppShell"
       x:DataType="vm:AppShellViewModel"
       FlyoutBehavior="Disabled">

    <!-- Login (ohne TabBar) -->
    <ShellContent Route="login"
                  ContentTemplate="{DataTemplate views:LoginPage}"
                  Shell.TabBarIsVisible="False" />

    <!-- Eltern-Navigation -->
    <TabBar Route="parent-main"
            IsVisible="{Binding IsParent}">
        <Tab Title="Dashboard" Icon="home.png">
            <ShellContent Route="dashboard"
                          ContentTemplate="{DataTemplate views:ParentDashboardPage}" />
        </Tab>
        <Tab Title="Kinder" Icon="children.png">
            <ShellContent Route="children"
                          ContentTemplate="{DataTemplate views:ChildrenListPage}" />
        </Tab>
        <Tab Title="Einstellungen" Icon="settings.png">
            <ShellContent Route="settings"
                          ContentTemplate="{DataTemplate views:SettingsPage}" />
        </Tab>
    </TabBar>

    <!-- Kind-Navigation -->
    <TabBar Route="child-main"
            IsVisible="{Binding IsChild}">
        <Tab Title="Dashboard" Icon="home.png">
            <ShellContent Route="dashboard"
                          ContentTemplate="{DataTemplate views:ChildDashboardPage}" />
        </Tab>
        <Tab Title="Ausgaben" Icon="expense.png">
            <ShellContent Route="transactions"
                          ContentTemplate="{DataTemplate views:TransactionsPage}" />
        </Tab>
        <Tab Title="Einstellungen" Icon="settings.png">
            <ShellContent Route="settings"
                          ContentTemplate="{DataTemplate views:ChildSettingsPage}" />
        </Tab>
    </TabBar>

    <!-- Verwandten-Navigation -->
    <TabBar Route="relative-main"
            IsVisible="{Binding IsRelative}">
        <Tab Title="Dashboard" Icon="home.png">
            <ShellContent Route="dashboard"
                          ContentTemplate="{DataTemplate views:RelativeDashboardPage}" />
        </Tab>
        <Tab Title="Geschenke" Icon="gift.png">
            <ShellContent Route="gifts"
                          ContentTemplate="{DataTemplate views:GiftsPage}" />
        </Tab>
        <Tab Title="Einstellungen" Icon="settings.png">
            <ShellContent Route="settings"
                          ContentTemplate="{DataTemplate views:SettingsPage}" />
        </Tab>
    </TabBar>

</Shell>
```

### AppShellViewModel.cs
```csharp
public partial class AppShellViewModel : ObservableObject
{
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsChild))]
    [NotifyPropertyChangedFor(nameof(IsRelative))]
    private bool _isParent;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsParent))]
    [NotifyPropertyChangedFor(nameof(IsRelative))]
    private bool _isChild;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsParent))]
    [NotifyPropertyChangedFor(nameof(IsChild))]
    private bool _isRelative;

    public AppShellViewModel(ITokenService tokenService)
    {
        _tokenService = tokenService;

        // Auf Token-Änderungen reagieren
        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, async (r, m) =>
        {
            await UpdateRoleAsync();
        });
    }

    public async Task InitializeAsync()
    {
        await UpdateRoleAsync();
    }

    private async Task UpdateRoleAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        var role = claims?.Role ?? string.Empty;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsParent = role == "Parent";
            IsChild = role == "Child";
            IsRelative = role == "Relative";
        });
    }
}
```

### AppShell.xaml.cs - Routen registrieren
```csharp
public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        // Gemeinsame Routen
        Routing.RegisterRoute("transaction-detail", typeof(TransactionDetailPage));
        Routing.RegisterRoute("profile", typeof(ProfilePage));

        // Eltern-spezifische Routen
        Routing.RegisterRoute("child-detail", typeof(ChildDetailPage));
        Routing.RegisterRoute("add-child", typeof(AddChildPage));
        Routing.RegisterRoute("allowance-settings", typeof(AllowanceSettingsPage));

        // Kind-spezifische Routen
        Routing.RegisterRoute("add-expense", typeof(AddExpensePage));
        Routing.RegisterRoute("category-select", typeof(CategorySelectPage));

        // Verwandten-spezifische Routen
        Routing.RegisterRoute("send-gift", typeof(SendGiftPage));
        Routing.RegisterRoute("gift-history", typeof(GiftHistoryPage));
    }
}
```

### Navigation nach Login
```csharp
// Nach erfolgreichem Login
public async Task NavigateToMainAsync(string role)
{
    var route = role switch
    {
        "Parent" => "//parent-main/dashboard",
        "Child" => "//child-main/dashboard",
        "Relative" => "//relative-main/dashboard",
        _ => "//login"
    };

    await Shell.Current.GoToAsync(route);
}
```

### IRoleNavigationService
```csharp
public interface IRoleNavigationService
{
    Task NavigateToRoleBasedDashboardAsync();
    string GetMainRoute();
}

public class RoleNavigationService : IRoleNavigationService
{
    private readonly ITokenService _tokenService;

    public async Task NavigateToRoleBasedDashboardAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        var route = GetRouteForRole(claims?.Role);
        await Shell.Current.GoToAsync(route);
    }

    public string GetMainRoute()
    {
        // Synchrone Variante für Shell-Initialisierung
        return "//parent-main/dashboard"; // Default
    }

    private static string GetRouteForRole(string? role) => role switch
    {
        "Parent" => "//parent-main/dashboard",
        "Child" => "//child-main/dashboard",
        "Relative" => "//relative-main/dashboard",
        _ => "//login"
    };
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-01 | Eltern-Login | Eltern-TabBar angezeigt |
| TC-M003-02 | Kind-Login | Kind-TabBar angezeigt |
| TC-M003-03 | Verwandten-Login | Verwandten-TabBar angezeigt |
| TC-M003-04 | Logout und anderer Login | Navigation wechselt |

## Story Points
3

## Priorität
Hoch
