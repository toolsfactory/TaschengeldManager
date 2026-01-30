# Story M003-S06: Bottom Navigation Bar

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **über eine untere Navigationsleiste zwischen den Hauptbereichen wechseln können**, damit **ich schnell und intuitiv durch die App navigieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben die App, wenn der Benutzer eingeloggt ist, dann wird eine Bottom Navigation Bar angezeigt
- [ ] Gegeben die Navigation, wenn ein Tab ausgewählt wird, dann wird er visuell hervorgehoben
- [ ] Gegeben die Navigation, wenn zwischen Tabs gewechselt wird, dann erfolgt eine flüssige Animation
- [ ] Gegeben die Navigation, wenn der aktive Tab erneut gedrückt wird, dann wird zum Anfang gescrollt

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│  [Content Area]             │
│                             │
├─────────────────────────────┤
│  [🏠]    [📋]    [⚙️]      │
│  Home    Liste   Settings   │
│   ●                         │
└─────────────────────────────┘
```

## Technische Hinweise

### AppShell.xaml mit TabBar Styling
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:views="clr-namespace:TaschengeldManager.Mobile.Views"
       x:Class="TaschengeldManager.Mobile.AppShell"
       FlyoutBehavior="Disabled"
       Shell.TabBarBackgroundColor="{AppThemeBinding Light={StaticResource BackgroundLight}, Dark={StaticResource BackgroundDark}}"
       Shell.TabBarTitleColor="{StaticResource TextSecondaryLight}"
       Shell.TabBarUnselectedColor="{StaticResource TextSecondaryLight}"
       Shell.TabBarForegroundColor="{StaticResource Primary}">

    <!-- Styles für TabBar -->
    <Shell.Resources>
        <Style TargetType="TabBar">
            <Setter Property="Shell.TabBarBackgroundColor"
                    Value="{AppThemeBinding Light={StaticResource BackgroundLight},
                                            Dark={StaticResource BackgroundDark}}" />
        </Style>
    </Shell.Resources>

    <!-- Parent Navigation -->
    <TabBar Route="parent-main">
        <Tab Title="Dashboard"
             Icon="{AppThemeBinding Light=home_light.png, Dark=home_dark.png}">
            <ShellContent ContentTemplate="{DataTemplate views:ParentDashboardPage}" />
        </Tab>
        <Tab Title="Kinder"
             Icon="{AppThemeBinding Light=children_light.png, Dark=children_dark.png}">
            <ShellContent ContentTemplate="{DataTemplate views:ChildrenListPage}" />
        </Tab>
        <Tab Title="Einstellungen"
             Icon="{AppThemeBinding Light=settings_light.png, Dark=settings_dark.png}">
            <ShellContent ContentTemplate="{DataTemplate views:SettingsPage}" />
        </Tab>
    </TabBar>

</Shell>
```

### Platform-spezifisches Styling

#### Android (Platforms/Android/Resources/values/styles.xml)
```xml
<style name="MainTheme" parent="Theme.MaterialComponents.DayNight.NoActionBar">
    <!-- Tab Bar Elevation -->
    <item name="android:elevation">8dp</item>
</style>
```

#### iOS (Platforms/iOS/AppDelegate.cs)
```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    // TabBar Appearance
    UITabBar.Appearance.TintColor = UIColor.FromRGB(76, 175, 80); // Primary
    UITabBar.Appearance.UnselectedItemTintColor = UIColor.Gray;

    if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
    {
        var appearance = new UITabBarAppearance();
        appearance.ConfigureWithOpaqueBackground();
        UITabBar.Appearance.StandardAppearance = appearance;
        UITabBar.Appearance.ScrollEdgeAppearance = appearance;
    }

    return base.FinishedLaunching(application, launchOptions);
}
```

### Custom Tab Icons mit Badge
```csharp
public class BadgeTabRenderer
{
    public static void SetBadge(string tabTitle, int count)
    {
        // Platform-spezifische Badge-Implementierung
#if ANDROID
        // Android Badge via ShortcutBadger oder ähnliches
#elif IOS
        if (UIApplication.SharedApplication.KeyWindow?.RootViewController
            is UITabBarController tabBarController)
        {
            var tabIndex = GetTabIndex(tabTitle);
            if (tabIndex >= 0 && tabIndex < tabBarController.TabBar.Items.Length)
            {
                tabBarController.TabBar.Items[tabIndex].BadgeValue =
                    count > 0 ? count.ToString() : null;
            }
        }
#endif
    }
}
```

### Scroll-to-Top bei erneutem Tab-Klick
```csharp
public partial class AppShell : Shell
{
    private string? _lastSelectedRoute;

    public AppShell()
    {
        InitializeComponent();
        Navigated += OnNavigated;
    }

    private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        var currentRoute = Current.CurrentState.Location.ToString();

        if (currentRoute == _lastSelectedRoute)
        {
            // Gleicher Tab erneut geklickt -> Scroll to top
            ScrollCurrentPageToTop();
        }

        _lastSelectedRoute = currentRoute;
    }

    private void ScrollCurrentPageToTop()
    {
        if (Current.CurrentPage is ContentPage page)
        {
            // ScrollView finden und nach oben scrollen
            var scrollView = FindScrollView(page.Content);
            scrollView?.ScrollToAsync(0, 0, true);
        }
    }

    private ScrollView? FindScrollView(IView? view)
    {
        if (view is ScrollView scrollView)
            return scrollView;

        if (view is Layout layout)
        {
            foreach (var child in layout.Children)
            {
                var result = FindScrollView(child);
                if (result != null)
                    return result;
            }
        }

        if (view is RefreshView refreshView)
        {
            return FindScrollView(refreshView.Content);
        }

        return null;
    }
}
```

### Tab-Animation
```xml
<!-- In Resources/Styles/Styles.xaml -->
<Style TargetType="Tab">
    <Setter Property="Shell.TabBarForegroundColor" Value="{StaticResource Primary}" />
</Style>

<!-- Einfache Animation via VisualStateManager -->
<ContentPage>
    <ContentPage.Behaviors>
        <toolkit:EventToCommandBehavior EventName="Appearing"
                                         Command="{Binding AppearingCommand}" />
    </ContentPage.Behaviors>

    <VerticalStackLayout x:Name="MainContent">
        <VerticalStackLayout.Behaviors>
            <toolkit:AnimationBehavior EventName="Loaded">
                <toolkit:AnimationBehavior.AnimationType>
                    <toolkit:FadeAnimation Opacity="1" Length="200" />
                </toolkit:AnimationBehavior.AnimationType>
            </toolkit:AnimationBehavior>
        </VerticalStackLayout.Behaviors>
        <!-- Content -->
    </VerticalStackLayout>
</ContentPage>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-21 | Tab wechseln | Aktiver Tab hervorgehoben |
| TC-M003-22 | Aktiven Tab erneut drücken | Scroll to top |
| TC-M003-23 | Dark Mode aktivieren | Icons passen sich an |
| TC-M003-24 | Badge setzen | Badge wird angezeigt |

## Story Points
2

## Priorität
Hoch
