# Story M002-S08: Biometrie-Login (Fingerprint)

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Benutzer mit aktivierter Biometrie** möchte ich **mich mit meinem Fingerabdruck oder Face ID anmelden können**, damit **ich schnell und ohne Passworteingabe in die App komme**.

## Akzeptanzkriterien

- [ ] Gegeben ein aktiviertes Biometrie-Login, wenn die App gestartet wird, dann wird automatisch die Biometrie-Authentifizierung angeboten
- [ ] Gegeben die Biometrie-Abfrage, wenn die Authentifizierung erfolgreich ist, dann wird der Benutzer eingeloggt
- [ ] Gegeben eine fehlgeschlagene Biometrie, wenn der Benutzer abbricht, dann kann er sich mit Passwort anmelden
- [ ] Gegeben mehrere fehlgeschlagene Versuche, wenn das Limit erreicht wird, dann wird nur Passwort-Login angeboten

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│     [Logo/App-Name]         │
│   TaschengeldManager        │
│                             │
├─────────────────────────────┤
│                             │
│      Willkommen zurück,     │
│         Max!                │
│                             │
│        [Fingerprint]        │
│         Animation           │
│                             │
│   Lege deinen Finger auf    │
│   den Sensor                │
│                             │
│   ─────── oder ───────      │
│                             │
│   Mit Passwort anmelden →   │
│                             │
│   Anderes Konto nutzen →    │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### BiometricLoginPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.BiometricLoginPage"
             x:DataType="vm:BiometricLoginViewModel"
             Shell.NavBarIsVisible="False">

    <VerticalStackLayout Padding="24"
                         Spacing="24"
                         VerticalOptions="Center">

        <!-- Logo -->
        <Image Source="logo.png"
               HeightRequest="80"
               Aspect="AspectFit" />

        <Label Text="TaschengeldManager"
               Style="{StaticResource HeadlineLabel}"
               HorizontalTextAlignment="Center" />

        <!-- Begrüßung -->
        <Label HorizontalTextAlignment="Center" FontSize="18">
            <Label.FormattedText>
                <FormattedString>
                    <Span Text="Willkommen zurück, " />
                    <Span Text="{Binding UserName}"
                          FontAttributes="Bold" />
                    <Span Text="!" />
                </FormattedString>
            </Label.FormattedText>
        </Label>

        <!-- Fingerprint Animation -->
        <Image Source="{Binding BiometricIcon}"
               HeightRequest="100"
               Aspect="AspectFit"
               HorizontalOptions="Center">
            <Image.Behaviors>
                <toolkit:AnimationBehavior>
                    <toolkit:AnimationBehavior.AnimationType>
                        <toolkit:PulseAnimation />
                    </toolkit:AnimationBehavior.AnimationType>
                </toolkit:AnimationBehavior>
            </Image.Behaviors>
        </Image>

        <!-- Status Text -->
        <Label Text="{Binding StatusText}"
               FontSize="14"
               HorizontalTextAlignment="Center"
               TextColor="{Binding StatusColor}" />

        <!-- Fehler -->
        <Label Text="{Binding ErrorMessage}"
               TextColor="Red"
               IsVisible="{Binding HasError}"
               HorizontalTextAlignment="Center" />

        <!-- Erneut versuchen Button -->
        <Button Text="Erneut versuchen"
                Command="{Binding RetryBiometricCommand}"
                IsVisible="{Binding CanRetry}"
                Style="{StaticResource PrimaryButton}" />

        <!-- Separator -->
        <BoxView HeightRequest="1"
                 Color="{StaticResource TextSecondaryLight}"
                 Margin="40,0" />

        <!-- Alternative Login Optionen -->
        <Label Text="Mit Passwort anmelden"
               TextColor="{StaticResource Primary}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding SwitchToPasswordLoginCommand}" />
            </Label.GestureRecognizers>
        </Label>

        <Label Text="Anderes Konto nutzen"
               TextColor="{StaticResource TextSecondaryLight}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding SwitchAccountCommand}" />
            </Label.GestureRecognizers>
        </Label>

    </VerticalStackLayout>

</ContentPage>
```

### BiometricLoginViewModel.cs
```csharp
public partial class BiometricLoginViewModel : ObservableObject
{
    private readonly IBiometricService _biometricService;
    private readonly IAuthApi _authApi;
    private readonly ITokenService _tokenService;
    private readonly INavigationService _navigationService;
    private int _failedAttempts;
    private const int MaxAttempts = 3;

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _statusText = "Lege deinen Finger auf den Sensor";

    [ObservableProperty]
    private Color _statusColor = Colors.Gray;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _canRetry;

    public string BiometricIcon => DeviceInfo.Platform == DevicePlatform.iOS
        ? "face_id.png"
        : "fingerprint.png";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public BiometricLoginViewModel(
        IBiometricService biometricService,
        IAuthApi authApi,
        ITokenService tokenService,
        INavigationService navigationService)
    {
        _biometricService = biometricService;
        _authApi = authApi;
        _tokenService = tokenService;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        UserName = claims?.Email?.Split('@').FirstOrDefault() ?? "Benutzer";

        await AuthenticateWithBiometricAsync();
    }

    private async Task AuthenticateWithBiometricAsync()
    {
        try
        {
            StatusText = "Authentifizierung läuft...";
            StatusColor = Colors.Gray;
            ErrorMessage = string.Empty;
            CanRetry = false;

            var biometricType = await _biometricService.GetBiometricTypeAsync();
            var reason = biometricType == BiometricType.FaceId
                ? "Schau auf dein Gerät, um dich anzumelden"
                : "Lege deinen Finger auf den Sensor, um dich anzumelden";

            var authenticated = await _biometricService.AuthenticateAsync(reason);

            if (authenticated)
            {
                await PerformLoginAsync();
            }
            else
            {
                _failedAttempts++;
                HandleFailedAttempt();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Ein Fehler ist aufgetreten. Bitte melde dich mit Passwort an.";
            CanRetry = false;
        }
    }

    private async Task PerformLoginAsync()
    {
        var credentials = await _biometricService.GetStoredCredentialsAsync();
        if (credentials == null)
        {
            ErrorMessage = "Gespeicherte Anmeldedaten nicht gefunden.";
            await _navigationService.NavigateToAsync("//login");
            return;
        }

        StatusText = "Anmeldung...";
        StatusColor = Colors.Green;

        var response = await _authApi.LoginAsync(new LoginRequest
        {
            Email = credentials.Value.Email,
            Password = credentials.Value.Password
        });

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            await _tokenService.SaveTokensAsync(
                response.Content.Token,
                response.Content.RefreshToken);

            await _navigationService.NavigateToAsync("//main/dashboard");
        }
        else
        {
            ErrorMessage = "Anmeldung fehlgeschlagen. Bitte melde dich mit Passwort an.";
            await _biometricService.DisableBiometricLoginAsync();
            await _navigationService.NavigateToAsync("//login");
        }
    }

    private void HandleFailedAttempt()
    {
        if (_failedAttempts >= MaxAttempts)
        {
            ErrorMessage = "Zu viele fehlgeschlagene Versuche. Bitte melde dich mit Passwort an.";
            CanRetry = false;
        }
        else
        {
            StatusText = "Authentifizierung fehlgeschlagen";
            StatusColor = Colors.Orange;
            CanRetry = true;
        }
    }

    [RelayCommand]
    private async Task RetryBiometricAsync()
    {
        await AuthenticateWithBiometricAsync();
    }

    [RelayCommand]
    private async Task SwitchToPasswordLoginAsync()
    {
        await _navigationService.NavigateToAsync("//login");
    }

    [RelayCommand]
    private async Task SwitchAccountAsync()
    {
        await _tokenService.ClearTokensAsync();
        await _biometricService.DisableBiometricLoginAsync();
        await _navigationService.NavigateToAsync("//login");
    }
}
```

### App-Start mit Biometrie
```csharp
// In App.xaml.cs
public partial class App : Application
{
    protected override async void OnStart()
    {
        var biometricService = Handler.MauiContext.Services.GetService<IBiometricService>();
        var tokenService = Handler.MauiContext.Services.GetService<ITokenService>();

        // Prüfen ob Biometrie aktiviert und Token vorhanden
        if (await biometricService.IsBiometricEnabledAsync() &&
            await tokenService.IsAuthenticatedAsync())
        {
            MainPage = new BiometricLoginPage();
        }
        else if (await tokenService.IsAuthenticatedAsync())
        {
            MainPage = new AppShell();
            await Shell.Current.GoToAsync("//main/dashboard");
        }
        else
        {
            MainPage = new AppShell();
        }
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-32 | Biometrie erfolgreich | Benutzer eingeloggt |
| TC-M002-33 | Biometrie fehlgeschlagen | Retry möglich |
| TC-M002-34 | Max. Versuche erreicht | Nur Passwort-Login |
| TC-M002-35 | Passwort-Login wählen | Weiterleitung zu Login |
| TC-M002-36 | Anderes Konto | Tokens gelöscht, Login |

## Story Points
5

## Priorität
Mittel
