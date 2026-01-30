# Story M002-S10: Passwort-vergessen Flow

## Epic
M002 - Authentifizierung

## Status
Offen

## User Story

Als **Benutzer, der sein Passwort vergessen hat** möchte ich **mein Passwort zurücksetzen können**, damit **ich wieder Zugang zu meinem Konto erhalte**.

## Akzeptanzkriterien

- [ ] Gegeben die Login-Seite, wenn "Passwort vergessen?" geklickt wird, dann wird die Passwort-Reset-Seite angezeigt
- [ ] Gegeben eine gültige E-Mail, wenn sie eingegeben wird, dann wird eine E-Mail mit Reset-Link versendet
- [ ] Gegeben eine unbekannte E-Mail, wenn sie eingegeben wird, dann wird aus Sicherheitsgründen trotzdem eine Erfolgsmeldung angezeigt
- [ ] Gegeben der Reset-Link, wenn er geklickt wird, dann kann ein neues Passwort gesetzt werden

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Passwort zurück- │
│            setzen           │
├─────────────────────────────┤
│                             │
│        [🔑 Icon]            │
│                             │
│   Passwort vergessen?       │
│                             │
│   Gib deine E-Mail-Adresse  │
│   ein und wir senden dir    │
│   einen Link zum Zurück-    │
│   setzen deines Passworts.  │
│                             │
│  ┌───────────────────────┐  │
│  │ E-Mail                │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │   Link senden         │  │
│  └───────────────────────┘  │
│                             │
│  Zurück zum Login →         │
│                             │
└─────────────────────────────┘

        Erfolg
┌─────────────────────────────┐
│                             │
│        [✉️ Icon]            │
│                             │
│   E-Mail gesendet!          │
│                             │
│   Falls ein Konto mit       │
│   dieser E-Mail existiert,  │
│   haben wir dir einen Link  │
│   zum Zurücksetzen gesendet.│
│                             │
│   Prüfe auch deinen Spam-   │
│   Ordner.                   │
│                             │
│  ┌───────────────────────┐  │
│  │   Zurück zum Login    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### ForgotPasswordPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.ForgotPasswordPage"
             x:DataType="vm:ForgotPasswordViewModel"
             Title="Passwort zurücksetzen">

    <VerticalStackLayout Padding="24"
                         Spacing="16"
                         VerticalOptions="Center">

        <!-- Icon -->
        <Image Source="key.png"
               HeightRequest="80"
               Aspect="AspectFit"
               IsVisible="{Binding IsNotSent}" />

        <Image Source="email_sent.png"
               HeightRequest="80"
               Aspect="AspectFit"
               IsVisible="{Binding IsSent}" />

        <!-- Titel -->
        <Label Text="{Binding Title}"
               Style="{StaticResource HeadlineLabel}"
               HorizontalTextAlignment="Center" />

        <!-- Beschreibung -->
        <Label Text="{Binding Description}"
               FontSize="14"
               HorizontalTextAlignment="Center"
               Margin="0,0,0,16" />

        <!-- E-Mail Eingabe (nur wenn nicht gesendet) -->
        <Entry Placeholder="E-Mail"
               Text="{Binding Email}"
               Keyboard="Email"
               IsVisible="{Binding IsNotSent}"
               ReturnType="Send"
               ReturnCommand="{Binding SendResetLinkCommand}" />

        <!-- Fehler -->
        <Label Text="{Binding ErrorMessage}"
               TextColor="Red"
               IsVisible="{Binding HasError}"
               HorizontalTextAlignment="Center" />

        <!-- Senden Button -->
        <Button Text="Link senden"
                Command="{Binding SendResetLinkCommand}"
                Style="{StaticResource PrimaryButton}"
                IsVisible="{Binding IsNotSent}" />

        <!-- Zurück zum Login Button (nach Erfolg) -->
        <Button Text="Zurück zum Login"
                Command="{Binding NavigateToLoginCommand}"
                Style="{StaticResource PrimaryButton}"
                IsVisible="{Binding IsSent}" />

        <ActivityIndicator IsRunning="{Binding IsBusy}"
                           IsVisible="{Binding IsBusy}" />

        <!-- Link zurück zum Login -->
        <Label Text="Zurück zum Login"
               TextColor="{StaticResource Primary}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center"
               IsVisible="{Binding IsNotSent}">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding NavigateToLoginCommand}" />
            </Label.GestureRecognizers>
        </Label>

    </VerticalStackLayout>

</ContentPage>
```

### ForgotPasswordViewModel.cs
```csharp
public partial class ForgotPasswordViewModel : ObservableObject
{
    private readonly IAuthApi _authApi;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSend))]
    private string _email = string.Empty;

    [ObservableProperty]
    private bool _isSent;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public bool IsNotSent => !IsSent;
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool CanSend => IsValidEmail(Email) && !IsBusy;

    public string Title => IsSent ? "E-Mail gesendet!" : "Passwort vergessen?";

    public string Description => IsSent
        ? "Falls ein Konto mit dieser E-Mail existiert, haben wir dir einen Link zum Zurücksetzen gesendet. Prüfe auch deinen Spam-Ordner."
        : "Gib deine E-Mail-Adresse ein und wir senden dir einen Link zum Zurücksetzen deines Passworts.";

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task SendResetLinkAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // API-Aufruf (zeigt immer Erfolg aus Sicherheitsgründen)
            await _authApi.ForgotPasswordAsync(new ForgotPasswordRequest
            {
                Email = Email
            });

            IsSent = true;
            OnPropertyChanged(nameof(IsNotSent));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
        }
        catch
        {
            // Auch bei Fehlern Erfolg anzeigen (Sicherheit)
            IsSent = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToLoginAsync()
    {
        await _navigationService.GoBackAsync();
    }

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) &&
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
}
```

### API-Interface
```csharp
public interface IAuthApi
{
    [Post("/api/auth/forgot-password")]
    Task<ApiResponse<EmptyResponse>> ForgotPasswordAsync([Body] ForgotPasswordRequest request);

    [Post("/api/auth/reset-password")]
    Task<ApiResponse<EmptyResponse>> ResetPasswordAsync([Body] ResetPasswordRequest request);
}

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(
    string Token,
    string NewPassword);
```

### Deep Link Handling (Reset-Link)
```csharp
// In App.xaml.cs
protected override async void OnAppLinkRequestReceived(Uri uri)
{
    base.OnAppLinkRequestReceived(uri);

    // URI: taschengeldmanager://reset-password?token=xxx
    if (uri.Host == "reset-password")
    {
        var token = HttpUtility.ParseQueryString(uri.Query).Get("token");
        if (!string.IsNullOrEmpty(token))
        {
            await Shell.Current.GoToAsync($"reset-password?token={token}");
        }
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-41 | Gültige E-Mail eingeben | Erfolgsmeldung angezeigt |
| TC-M002-42 | Unbekannte E-Mail | Erfolgsmeldung (Sicherheit) |
| TC-M002-43 | Ungültige E-Mail-Format | Button deaktiviert |
| TC-M002-44 | Deep Link mit Token | Reset-Seite öffnet sich |

## Story Points
2

## Priorität
Mittel
