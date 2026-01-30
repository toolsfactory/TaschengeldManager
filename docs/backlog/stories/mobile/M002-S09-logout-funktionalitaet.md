# Story M002-S09: Logout-Funktionalität

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **angemeldeter Benutzer** möchte ich **mich abmelden können**, damit **mein Konto auf diesem Gerät geschützt ist, wenn ich es nicht mehr nutze**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggter Benutzer, wenn er Logout wählt, dann werden alle lokalen Tokens gelöscht
- [ ] Gegeben ein Logout, wenn er durchgeführt wird, dann wird der Benutzer zum Login-Bildschirm weitergeleitet
- [ ] Gegeben ein Logout, wenn Biometrie aktiviert war, dann bleibt diese Einstellung erhalten (optional deaktivieren)
- [ ] Gegeben ein Logout, wenn er durchgeführt wird, dann wird die Server-Session invalidiert

## UI-Entwurf

```
┌─────────────────────────────┐
│  Einstellungen              │
├─────────────────────────────┤
│                             │
│  Konto                      │
│  ┌───────────────────────┐  │
│  │ max@example.com       │  │
│  │ Eltern-Konto          │  │
│  └───────────────────────┘  │
│                             │
│  Sicherheit                 │
│  ┌───────────────────────┐  │
│  │ Biometrie         [✓] │  │
│  └───────────────────────┘  │
│                             │
│  ────────────────────────   │
│                             │
│  ┌───────────────────────┐  │
│  │      Abmelden         │  │
│  └───────────────────────┘  │
│                             │
│                             │
│                             │
└─────────────────────────────┘

        Bestätigung
┌─────────────────────────────┐
│                             │
│   Möchtest du dich wirklich │
│   abmelden?                 │
│                             │
│  ┌───────────┐ ┌──────────┐ │
│  │ Abbrechen │ │ Abmelden │ │
│  └───────────┘ └──────────┘ │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### IAuthService Interface
```csharp
public interface IAuthService
{
    Task LogoutAsync(bool clearBiometric = false);
    Task<bool> LogoutFromServerAsync();
}
```

### AuthService Implementation
```csharp
public class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;
    private readonly ITokenService _tokenService;
    private readonly IBiometricService _biometricService;
    private readonly IDatabaseService _databaseService;
    private readonly INavigationService _navigationService;

    public AuthService(
        IAuthApi authApi,
        ITokenService tokenService,
        IBiometricService biometricService,
        IDatabaseService databaseService,
        INavigationService navigationService)
    {
        _authApi = authApi;
        _tokenService = tokenService;
        _biometricService = biometricService;
        _databaseService = databaseService;
        _navigationService = navigationService;
    }

    public async Task LogoutAsync(bool clearBiometric = false)
    {
        try
        {
            // Server-Session invalidieren
            await LogoutFromServerAsync();
        }
        catch
        {
            // Fehler beim Server-Logout ignorieren
            // Lokaler Logout wird trotzdem durchgeführt
        }

        // Tokens löschen
        await _tokenService.ClearTokensAsync();

        // Biometrie optional deaktivieren
        if (clearBiometric)
        {
            await _biometricService.DisableBiometricLoginAsync();
        }

        // Lokale Daten löschen (optional, je nach Anforderung)
        // await _databaseService.ClearAllDataAsync();

        // Zum Login navigieren
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await _navigationService.NavigateToAsync("//login");
        });
    }

    public async Task<bool> LogoutFromServerAsync()
    {
        var refreshToken = await _tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        try
        {
            var response = await _authApi.LogoutAsync(new LogoutRequest
            {
                RefreshToken = refreshToken
            });

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
```

### SettingsViewModel.cs
```csharp
public partial class SettingsViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _userEmail = string.Empty;

    [ObservableProperty]
    private string _userRole = string.Empty;

    [ObservableProperty]
    private bool _isBiometricEnabled;

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
    private async Task LogoutAndClearBiometricAsync()
    {
        var result = await Shell.Current.DisplayActionSheet(
            "Abmelden",
            "Abbrechen",
            null,
            "Abmelden",
            "Abmelden und Biometrie deaktivieren");

        switch (result)
        {
            case "Abmelden":
                await _authService.LogoutAsync(clearBiometric: false);
                break;
            case "Abmelden und Biometrie deaktivieren":
                await _authService.LogoutAsync(clearBiometric: true);
                break;
        }
    }
}
```

### SettingsPage.xaml (Logout-Bereich)
```xml
<VerticalStackLayout Spacing="16">

    <!-- Konto Info -->
    <Frame Padding="16" CornerRadius="8">
        <VerticalStackLayout Spacing="4">
            <Label Text="Konto"
                   FontSize="12"
                   TextColor="{StaticResource TextSecondaryLight}" />
            <Label Text="{Binding UserEmail}"
                   FontSize="16"
                   FontAttributes="Bold" />
            <Label Text="{Binding UserRole}"
                   FontSize="14"
                   TextColor="{StaticResource TextSecondaryLight}" />
        </VerticalStackLayout>
    </Frame>

    <!-- Sicherheitseinstellungen -->
    <Frame Padding="16" CornerRadius="8">
        <Grid ColumnDefinitions="*,Auto">
            <Label Text="Biometrie-Login"
                   VerticalOptions="Center" />
            <Switch Grid.Column="1"
                    IsToggled="{Binding IsBiometricEnabled}"
                    OnColor="{StaticResource Primary}" />
        </Grid>
    </Frame>

    <!-- Logout Button -->
    <Button Text="Abmelden"
            Command="{Binding LogoutCommand}"
            BackgroundColor="Transparent"
            TextColor="Red"
            BorderColor="Red"
            BorderWidth="1"
            CornerRadius="8"
            Margin="0,24,0,0" />

</VerticalStackLayout>
```

### API-Endpunkt
```csharp
public interface IAuthApi
{
    [Post("/api/auth/logout")]
    Task<ApiResponse<EmptyResponse>> LogoutAsync([Body] LogoutRequest request);
}

public record LogoutRequest(string RefreshToken);
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-37 | Logout bestätigen | Tokens gelöscht, Navigation zu Login |
| TC-M002-38 | Logout abbrechen | Benutzer bleibt eingeloggt |
| TC-M002-39 | Logout mit Biometrie deaktivieren | Biometrie-Daten gelöscht |
| TC-M002-40 | Server nicht erreichbar | Lokaler Logout erfolgreich |

## Story Points
1

## Priorität
Hoch
