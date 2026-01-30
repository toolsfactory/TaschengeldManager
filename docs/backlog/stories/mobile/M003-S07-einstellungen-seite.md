# Story M003-S07: Einstellungen-Seite

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **auf einer Einstellungsseite meine Präferenzen verwalten können**, damit **ich die App nach meinen Wünschen anpassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Einstellungen, wenn sie geöffnet werden, dann sind alle verfügbaren Optionen sichtbar
- [ ] Gegeben eine Einstellung, wenn sie geändert wird, dann wird sie sofort gespeichert
- [ ] Gegeben die Sicherheitseinstellungen, wenn Biometrie verfügbar ist, dann kann sie ein/ausgeschaltet werden
- [ ] Gegeben die Einstellungen, wenn Abmelden gewählt wird, dann wird der Benutzer ausgeloggt

## UI-Entwurf

```
┌─────────────────────────────┐
│  Einstellungen              │
├─────────────────────────────┤
│                             │
│  Konto                      │
│  ┌───────────────────────┐  │
│  │ 👤 max@example.com    │  │
│  │    Eltern-Konto    >  │  │
│  └───────────────────────┘  │
│                             │
│  Sicherheit                 │
│  ┌───────────────────────┐  │
│  │ Biometrie         [✓] │  │
│  ├───────────────────────┤  │
│  │ Angemeldet bleiben[✓] │  │
│  ├───────────────────────┤  │
│  │ Aktive Sessions    >  │  │
│  ├───────────────────────┤  │
│  │ Passwort ändern    >  │  │
│  └───────────────────────┘  │
│                             │
│  Benachrichtigungen         │
│  ┌───────────────────────┐  │
│  │ Push-Nachrichten  [✓] │  │
│  ├───────────────────────┤  │
│  │ Taschengeld-       [✓]│  │
│  │ Erinnerungen          │  │
│  └───────────────────────┘  │
│                             │
│  App                        │
│  ┌───────────────────────┐  │
│  │ Dark Mode      [Auto] │  │
│  ├───────────────────────┤  │
│  │ Über die App       >  │  │
│  ├───────────────────────┤  │
│  │ Hilfe & Support    >  │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │      Abmelden         │  │
│  └───────────────────────┘  │
│                             │
│  Version 1.0.0 (123)        │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### SettingsPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.SettingsPage"
             x:DataType="vm:SettingsViewModel"
             Title="Einstellungen">

    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="16">

            <!-- Konto Sektion -->
            <Label Text="Konto"
                   Style="{StaticResource SectionHeaderLabel}" />

            <Frame Padding="0" CornerRadius="8">
                <Grid Padding="16" ColumnDefinitions="Auto,*,Auto">
                    <Frame WidthRequest="48" HeightRequest="48"
                           CornerRadius="24"
                           BackgroundColor="{StaticResource PrimaryLight}"
                           Padding="0">
                        <Label Text="👤"
                               FontSize="24"
                               HorizontalOptions="Center"
                               VerticalOptions="Center" />
                    </Frame>
                    <VerticalStackLayout Grid.Column="1" Margin="12,0">
                        <Label Text="{Binding UserEmail}"
                               FontSize="16"
                               FontAttributes="Bold" />
                        <Label Text="{Binding UserRole}"
                               FontSize="14"
                               TextColor="{StaticResource TextSecondaryLight}" />
                    </VerticalStackLayout>
                    <Label Grid.Column="2"
                           Text=">"
                           FontSize="18"
                           TextColor="{StaticResource TextSecondaryLight}"
                           VerticalOptions="Center" />
                    <Grid.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding NavigateToProfileCommand}" />
                    </Grid.GestureRecognizers>
                </Grid>
            </Frame>

            <!-- Sicherheit Sektion -->
            <Label Text="Sicherheit"
                   Style="{StaticResource SectionHeaderLabel}" />

            <Frame Padding="0" CornerRadius="8">
                <VerticalStackLayout>
                    <!-- Biometrie -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto"
                          IsVisible="{Binding IsBiometricAvailable}">
                        <Label Text="Biometrie-Login"
                               VerticalOptions="Center" />
                        <Switch Grid.Column="1"
                                IsToggled="{Binding IsBiometricEnabled}"
                                OnColor="{StaticResource Primary}" />
                    </Grid>
                    <BoxView HeightRequest="1"
                             Color="{StaticResource Divider}"
                             IsVisible="{Binding IsBiometricAvailable}" />

                    <!-- Angemeldet bleiben -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Angemeldet bleiben"
                               VerticalOptions="Center" />
                        <Switch Grid.Column="1"
                                IsToggled="{Binding IsRememberMeEnabled}"
                                OnColor="{StaticResource Primary}" />
                    </Grid>
                    <BoxView HeightRequest="1" Color="{StaticResource Divider}" />

                    <!-- Aktive Sessions -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Aktive Sessions"
                               VerticalOptions="Center" />
                        <Label Grid.Column="1"
                               Text=">"
                               FontSize="18"
                               TextColor="{StaticResource TextSecondaryLight}"
                               VerticalOptions="Center" />
                        <Grid.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding NavigateToSessionsCommand}" />
                        </Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" Color="{StaticResource Divider}" />

                    <!-- Passwort ändern -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Passwort ändern"
                               VerticalOptions="Center" />
                        <Label Grid.Column="1"
                               Text=">"
                               FontSize="18"
                               TextColor="{StaticResource TextSecondaryLight}"
                               VerticalOptions="Center" />
                        <Grid.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding NavigateToChangePasswordCommand}" />
                        </Grid.GestureRecognizers>
                    </Grid>
                </VerticalStackLayout>
            </Frame>

            <!-- Benachrichtigungen Sektion -->
            <Label Text="Benachrichtigungen"
                   Style="{StaticResource SectionHeaderLabel}" />

            <Frame Padding="0" CornerRadius="8">
                <VerticalStackLayout>
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Push-Nachrichten"
                               VerticalOptions="Center" />
                        <Switch Grid.Column="1"
                                IsToggled="{Binding IsPushEnabled}"
                                OnColor="{StaticResource Primary}" />
                    </Grid>
                    <BoxView HeightRequest="1" Color="{StaticResource Divider}" />

                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Taschengeld-Erinnerungen"
                               VerticalOptions="Center" />
                        <Switch Grid.Column="1"
                                IsToggled="{Binding IsAllowanceReminderEnabled}"
                                OnColor="{StaticResource Primary}" />
                    </Grid>
                </VerticalStackLayout>
            </Frame>

            <!-- App Sektion -->
            <Label Text="App"
                   Style="{StaticResource SectionHeaderLabel}" />

            <Frame Padding="0" CornerRadius="8">
                <VerticalStackLayout>
                    <!-- Dark Mode -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Erscheinungsbild"
                               VerticalOptions="Center" />
                        <Picker Grid.Column="1"
                                SelectedItem="{Binding SelectedAppearance}"
                                ItemsSource="{Binding AppearanceOptions}"
                                WidthRequest="100" />
                    </Grid>
                    <BoxView HeightRequest="1" Color="{StaticResource Divider}" />

                    <!-- Über die App -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Über die App"
                               VerticalOptions="Center" />
                        <Label Grid.Column="1"
                               Text=">"
                               FontSize="18"
                               TextColor="{StaticResource TextSecondaryLight}"
                               VerticalOptions="Center" />
                        <Grid.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding NavigateToAboutCommand}" />
                        </Grid.GestureRecognizers>
                    </Grid>
                    <BoxView HeightRequest="1" Color="{StaticResource Divider}" />

                    <!-- Hilfe -->
                    <Grid Padding="16" ColumnDefinitions="*,Auto">
                        <Label Text="Hilfe &amp; Support"
                               VerticalOptions="Center" />
                        <Label Grid.Column="1"
                               Text=">"
                               FontSize="18"
                               TextColor="{StaticResource TextSecondaryLight}"
                               VerticalOptions="Center" />
                        <Grid.GestureRecognizers>
                            <TapGestureRecognizer Command="{Binding NavigateToHelpCommand}" />
                        </Grid.GestureRecognizers>
                    </Grid>
                </VerticalStackLayout>
            </Frame>

            <!-- Abmelden Button -->
            <Button Text="Abmelden"
                    Command="{Binding LogoutCommand}"
                    BackgroundColor="Transparent"
                    TextColor="Red"
                    BorderColor="Red"
                    BorderWidth="1"
                    CornerRadius="8"
                    Margin="0,16,0,0" />

            <!-- Version -->
            <Label Text="{Binding VersionText}"
                   FontSize="12"
                   TextColor="{StaticResource TextSecondaryLight}"
                   HorizontalTextAlignment="Center"
                   Margin="0,8" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### SettingsViewModel.cs
```csharp
public partial class SettingsViewModel : ObservableObject
{
    private readonly ITokenService _tokenService;
    private readonly IBiometricService _biometricService;
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    private string _userRole = string.Empty;

    [ObservableProperty]
    private bool _isBiometricAvailable;

    [ObservableProperty]
    private bool _isBiometricEnabled;

    [ObservableProperty]
    private bool _isRememberMeEnabled;

    [ObservableProperty]
    private bool _isPushEnabled;

    [ObservableProperty]
    private bool _isAllowanceReminderEnabled;

    [ObservableProperty]
    private string _selectedAppearance = "System";

    public List<string> AppearanceOptions { get; } = new()
    {
        "System", "Hell", "Dunkel"
    };

    public string VersionText =>
        $"Version {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";

    public async Task InitializeAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        UserEmail = claims?.Email ?? "";
        UserRole = GetRoleDisplayName(claims?.Role);

        IsBiometricAvailable = await _biometricService.IsAvailableAsync();
        IsBiometricEnabled = await _biometricService.IsBiometricEnabledAsync();

        IsRememberMeEnabled = _settingsService.IsRememberMeEnabled;
        IsPushEnabled = _settingsService.IsPushEnabled;
        IsAllowanceReminderEnabled = _settingsService.IsAllowanceReminderEnabled;
        SelectedAppearance = _settingsService.Appearance;
    }

    private static string GetRoleDisplayName(string? role) => role switch
    {
        "Parent" => "Eltern-Konto",
        "Child" => "Kind-Konto",
        "Relative" => "Verwandten-Konto",
        _ => "Unbekannt"
    };

    partial void OnIsBiometricEnabledChanged(bool value)
    {
        if (value)
        {
            _ = _biometricService.EnableBiometricLoginAsync();
        }
        else
        {
            _ = _biometricService.DisableBiometricLoginAsync();
        }
    }

    partial void OnSelectedAppearanceChanged(string value)
    {
        _settingsService.Appearance = value;
        ApplyAppearance(value);
    }

    private static void ApplyAppearance(string appearance)
    {
        Application.Current!.UserAppTheme = appearance switch
        {
            "Hell" => AppTheme.Light,
            "Dunkel" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var result = await Shell.Current.DisplayAlert(
            "Abmelden",
            "Möchtest du dich wirklich abmelden?",
            "Abmelden",
            "Abbrechen");

        if (result)
        {
            await _authService.LogoutAsync();
        }
    }

    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await _navigationService.NavigateToAsync("profile");
    }

    [RelayCommand]
    private async Task NavigateToSessionsAsync()
    {
        await _navigationService.NavigateToAsync("sessions");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-25 | Einstellungen öffnen | Alle Optionen sichtbar |
| TC-M003-26 | Biometrie umschalten | Einstellung gespeichert |
| TC-M003-27 | Erscheinungsbild ändern | Theme wechselt sofort |
| TC-M003-28 | Abmelden | Logout und Navigation zum Login |

## Story Points
2

## Priorität
Mittel
