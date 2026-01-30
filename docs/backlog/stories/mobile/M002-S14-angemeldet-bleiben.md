# Story M002-S14: Angemeldet bleiben

## Epic
M002 - Authentifizierung

## Status
Offen

## User Story

Als **Benutzer** möchte ich **die Option haben, angemeldet zu bleiben**, damit **ich mich nicht bei jedem App-Start erneut anmelden muss**.

## Akzeptanzkriterien

- [ ] Gegeben die Login-Seite, wenn "Angemeldet bleiben" aktiviert ist, dann bleibt der Benutzer nach App-Neustart eingeloggt
- [ ] Gegeben die Option deaktiviert, wenn die App geschlossen wird, dann muss sich der Benutzer erneut anmelden
- [ ] Gegeben "Angemeldet bleiben", wenn 30 Tage vergangen sind, dann wird der Benutzer automatisch ausgeloggt
- [ ] Gegeben die Einstellungen, wenn der Benutzer die Option ändert, dann wird dies sofort wirksam

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│  [Login-Formular]           │
│                             │
│  ┌───────────────────────┐  │
│  │ E-Mail                │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │ Passwort              │  │
│  └───────────────────────┘  │
│                             │
│  ☑ Angemeldet bleiben       │
│                             │
│  ┌───────────────────────┐  │
│  │      Anmelden         │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘

      Einstellungen
┌─────────────────────────────┐
│                             │
│  Sicherheit                 │
│  ┌───────────────────────┐  │
│  │ Angemeldet bleiben [✓]│  │
│  └───────────────────────┘  │
│  Bei Deaktivierung wirst du │
│  beim nächsten App-Start    │
│  ausgeloggt.                │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### Preferences-Keys
```csharp
public static class PreferenceKeys
{
    public const string RememberMe = "remember_me";
    public const string LastLoginTime = "last_login_time";
    public const string SessionExpirationDays = "session_expiration_days";
}
```

### ISessionPersistenceService Interface
```csharp
public interface ISessionPersistenceService
{
    bool IsRememberMeEnabled { get; }
    void SetRememberMe(bool enabled);
    bool ShouldAutoLogin();
    void RecordLogin();
    void ClearSession();
}
```

### SessionPersistenceService Implementation
```csharp
public class SessionPersistenceService : ISessionPersistenceService
{
    private const int MaxSessionDays = 30;

    public bool IsRememberMeEnabled =>
        Preferences.Default.Get(PreferenceKeys.RememberMe, false);

    public void SetRememberMe(bool enabled)
    {
        Preferences.Default.Set(PreferenceKeys.RememberMe, enabled);

        if (!enabled)
        {
            // Bei Deaktivierung: Session-Daten löschen
            Preferences.Default.Remove(PreferenceKeys.LastLoginTime);
        }
    }

    public bool ShouldAutoLogin()
    {
        if (!IsRememberMeEnabled)
            return false;

        var lastLogin = Preferences.Default.Get(
            PreferenceKeys.LastLoginTime,
            DateTime.MinValue.ToString("O"));

        if (DateTime.TryParse(lastLogin, out var loginTime))
        {
            var daysSinceLogin = (DateTime.UtcNow - loginTime).TotalDays;
            return daysSinceLogin <= MaxSessionDays;
        }

        return false;
    }

    public void RecordLogin()
    {
        if (IsRememberMeEnabled)
        {
            Preferences.Default.Set(
                PreferenceKeys.LastLoginTime,
                DateTime.UtcNow.ToString("O"));
        }
    }

    public void ClearSession()
    {
        Preferences.Default.Remove(PreferenceKeys.LastLoginTime);
    }
}
```

### LoginViewModel mit RememberMe
```csharp
public partial class LoginViewModel : ObservableObject
{
    private readonly ISessionPersistenceService _sessionPersistence;

    [ObservableProperty]
    private bool _rememberMe;

    public LoginViewModel(
        IAuthApi authApi,
        ITokenService tokenService,
        INavigationService navigationService,
        ISessionPersistenceService sessionPersistence)
    {
        _sessionPersistence = sessionPersistence;
        RememberMe = _sessionPersistence.IsRememberMeEnabled;
    }

    partial void OnRememberMeChanged(bool value)
    {
        _sessionPersistence.SetRememberMe(value);
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // ... Login-Logik ...

        if (response.IsSuccessStatusCode)
        {
            // Session-Persistenz aktualisieren
            _sessionPersistence.SetRememberMe(RememberMe);
            _sessionPersistence.RecordLogin();

            await _navigationService.NavigateToAsync("//main/dashboard");
        }
    }
}
```

### App.xaml.cs - Auto-Login Prüfung
```csharp
public partial class App : Application
{
    private readonly ITokenService _tokenService;
    private readonly ISessionPersistenceService _sessionPersistence;
    private readonly IBiometricService _biometricService;

    protected override async void OnStart()
    {
        base.OnStart();

        // Prüfen ob Auto-Login möglich
        if (await ShouldAutoLoginAsync())
        {
            await PerformAutoLoginAsync();
        }
        else
        {
            await NavigateToLoginAsync();
        }
    }

    private async Task<bool> ShouldAutoLoginAsync()
    {
        // Prüfen ob "Angemeldet bleiben" aktiv und nicht abgelaufen
        if (!_sessionPersistence.ShouldAutoLogin())
            return false;

        // Prüfen ob gültige Tokens vorhanden
        if (!await _tokenService.IsAuthenticatedAsync())
            return false;

        return true;
    }

    private async Task PerformAutoLoginAsync()
    {
        // Biometrie-Login wenn aktiviert
        if (await _biometricService.IsBiometricEnabledAsync())
        {
            MainPage = new BiometricLoginPage();
        }
        else
        {
            // Direkt zum Dashboard
            MainPage = new AppShell();
            await Shell.Current.GoToAsync("//main/dashboard");
        }
    }

    private async Task NavigateToLoginAsync()
    {
        // Tokens und Session löschen falls vorhanden
        await _tokenService.ClearTokensAsync();
        _sessionPersistence.ClearSession();

        MainPage = new AppShell();
    }
}
```

### SettingsPage - RememberMe Toggle
```xml
<Frame Padding="16" CornerRadius="8">
    <Grid ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto">
        <Label Text="Angemeldet bleiben"
               FontSize="16"
               VerticalOptions="Center" />
        <Switch Grid.Column="1"
                IsToggled="{Binding IsRememberMeEnabled}"
                OnColor="{StaticResource Primary}" />
        <Label Grid.Row="1" Grid.ColumnSpan="2"
               Text="Bei Deaktivierung wirst du beim nächsten App-Start ausgeloggt."
               FontSize="12"
               TextColor="{StaticResource TextSecondaryLight}"
               Margin="0,4,0,0" />
    </Grid>
</Frame>
```

### Logout mit RememberMe-Reset
```csharp
public async Task LogoutAsync()
{
    // RememberMe deaktivieren bei explizitem Logout
    _sessionPersistence.ClearSession();

    await _tokenService.ClearTokensAsync();
    await _navigationService.NavigateToAsync("//login");
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-57 | RememberMe aktiviert, App-Neustart | Auto-Login |
| TC-M002-58 | RememberMe deaktiviert, App-Neustart | Login erforderlich |
| TC-M002-59 | 30 Tage nach Login | Automatischer Logout |
| TC-M002-60 | RememberMe in Einstellungen ändern | Sofort wirksam |

## Story Points
1

## Priorität
Niedrig
