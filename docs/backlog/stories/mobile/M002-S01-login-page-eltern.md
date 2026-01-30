# Story M002-S01: Login-Page für Eltern

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Elternteil** möchte ich **mich mit meiner E-Mail-Adresse und Passwort anmelden können**, damit **ich auf mein Familienkonto zugreifen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Login-Seite, wenn sie angezeigt wird, dann sind E-Mail- und Passwort-Eingabefelder sichtbar
- [ ] Gegeben valide Anmeldedaten, wenn der Benutzer sich anmeldet, dann wird er zum Dashboard weitergeleitet
- [ ] Gegeben ungültige Anmeldedaten, wenn der Benutzer sich anmeldet, dann wird eine Fehlermeldung angezeigt
- [ ] Gegeben die Login-Seite, wenn der Benutzer tippt, dann wird ein "Passwort vergessen?"-Link angezeigt
- [ ] Gegeben die Eingabe, wenn das Passwort eingegeben wird, dann kann die Sichtbarkeit umgeschaltet werden

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│     [Logo/App-Name]         │
│   TaschengeldManager        │
│                             │
├─────────────────────────────┤
│                             │
│  Willkommen zurück!         │
│                             │
│  ┌───────────────────────┐  │
│  │ E-Mail                │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │ Passwort          [👁] │  │
│  └───────────────────────┘  │
│                             │
│  Passwort vergessen? →      │
│                             │
│  ┌───────────────────────┐  │
│  │      Anmelden         │  │
│  └───────────────────────┘  │
│                             │
│  ─────── oder ───────       │
│                             │
│  [Fingerprint] Mit Biometrie│
│                             │
│  Noch kein Konto?           │
│  → Jetzt registrieren       │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### LoginPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.LoginPage"
             x:DataType="vm:LoginViewModel"
             Shell.NavBarIsVisible="False">

    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="16"
                             VerticalOptions="Center">

            <!-- Logo -->
            <Image Source="logo.png"
                   HeightRequest="80"
                   Aspect="AspectFit" />

            <Label Text="TaschengeldManager"
                   Style="{StaticResource HeadlineLabel}"
                   HorizontalTextAlignment="Center" />

            <Label Text="Willkommen zurück!"
                   FontSize="18"
                   HorizontalTextAlignment="Center"
                   Margin="0,16,0,0" />

            <!-- E-Mail -->
            <Entry Placeholder="E-Mail"
                   Text="{Binding Email}"
                   Keyboard="Email"
                   ReturnType="Next"
                   Style="{StaticResource DefaultEntry}" />

            <!-- Passwort -->
            <Grid ColumnDefinitions="*,Auto">
                <Entry Placeholder="Passwort"
                       Text="{Binding Password}"
                       IsPassword="{Binding IsPasswordHidden}"
                       ReturnType="Done"
                       Style="{StaticResource DefaultEntry}" />
                <ImageButton Grid.Column="1"
                             Source="{Binding PasswordVisibilityIcon}"
                             Command="{Binding TogglePasswordVisibilityCommand}"
                             WidthRequest="40"
                             HeightRequest="40" />
            </Grid>

            <!-- Passwort vergessen -->
            <Label Text="Passwort vergessen?"
                   TextDecorations="Underline"
                   HorizontalOptions="End">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding ForgotPasswordCommand}" />
                </Label.GestureRecognizers>
            </Label>

            <!-- Fehleranzeige -->
            <Label Text="{Binding ErrorMessage}"
                   TextColor="Red"
                   IsVisible="{Binding HasError}"
                   HorizontalTextAlignment="Center" />

            <!-- Login Button -->
            <Button Text="Anmelden"
                    Command="{Binding LoginCommand}"
                    Style="{StaticResource PrimaryButton}"
                    IsEnabled="{Binding IsNotBusy}" />

            <!-- Loading Indicator -->
            <ActivityIndicator IsRunning="{Binding IsBusy}"
                               IsVisible="{Binding IsBusy}"
                               Color="{StaticResource Primary}" />

            <!-- Biometrie Option -->
            <Button Text="Mit Fingerabdruck anmelden"
                    Command="{Binding BiometricLoginCommand}"
                    IsVisible="{Binding IsBiometricAvailable}"
                    Style="{StaticResource SecondaryButton}" />

            <!-- Registrierung Link -->
            <HorizontalStackLayout HorizontalOptions="Center"
                                   Spacing="4"
                                   Margin="0,24,0,0">
                <Label Text="Noch kein Konto?" />
                <Label Text="Jetzt registrieren"
                       TextColor="{StaticResource Primary}"
                       TextDecorations="Underline">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding NavigateToRegisterCommand}" />
                    </Label.GestureRecognizers>
                </Label>
            </HorizontalStackLayout>

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### LoginViewModel.cs
```csharp
public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthApi _authApi;
    private readonly INavigationService _navigationService;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isPasswordHidden = true;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool IsNotBusy => !IsBusy;
    public string PasswordVisibilityIcon =>
        IsPasswordHidden ? "eye_off.png" : "eye_on.png";

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
        OnPropertyChanged(nameof(PasswordVisibilityIcon));
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var response = await _authApi.LoginAsync(new LoginRequest
            {
                Email = Email,
                Password = Password
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
                ErrorMessage = "E-Mail oder Passwort ist falsch.";
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Ein Fehler ist aufgetreten. Bitte versuche es erneut.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password);
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-01 | Valide Anmeldedaten eingeben | Navigation zum Dashboard |
| TC-M002-02 | Ungültige Anmeldedaten | Fehlermeldung angezeigt |
| TC-M002-03 | Leere Felder | Login-Button deaktiviert |
| TC-M002-04 | Passwort-Sichtbarkeit umschalten | Passwort sichtbar/versteckt |

## Story Points
3

## Priorität
Hoch
