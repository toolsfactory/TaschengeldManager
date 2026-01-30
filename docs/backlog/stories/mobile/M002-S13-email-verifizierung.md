# Story M002-S13: Email-Verifizierung

## Epic
M002 - Authentifizierung

## Status
Offen

## User Story

Als **neu registrierter Benutzer** möchte ich **meine E-Mail-Adresse verifizieren**, damit **mein Konto aktiviert wird und ich alle Funktionen nutzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine neue Registrierung, wenn sie abgeschlossen ist, dann wird eine Verifizierungs-E-Mail gesendet
- [ ] Gegeben die App nach Registrierung, wenn die E-Mail nicht verifiziert ist, dann wird ein Banner angezeigt
- [ ] Gegeben der Verifizierungslink, wenn er geklickt wird, dann wird die E-Mail als verifiziert markiert
- [ ] Gegeben keine Verifizierung, wenn bestimmte Aktionen versucht werden, dann werden sie blockiert

## UI-Entwurf

```
┌─────────────────────────────┐
│ ⚠️ Bitte bestätige deine    │
│    E-Mail-Adresse           │
│    [Erneut senden]          │
├─────────────────────────────┤
│                             │
│  Dashboard (eingeschränkt)  │
│                             │
│  ...                        │
│                             │
└─────────────────────────────┘

     Verifizierung erforderlich
┌─────────────────────────────┐
│                             │
│        [✉️ Icon]            │
│                             │
│   Bestätige deine E-Mail    │
│                             │
│   Wir haben eine E-Mail an  │
│   max@example.com gesendet. │
│                             │
│   Klicke auf den Link in    │
│   der E-Mail, um dein Konto │
│   zu aktivieren.            │
│                             │
│  ┌───────────────────────┐  │
│  │   E-Mail erneut       │  │
│  │   senden              │  │
│  └───────────────────────┘  │
│                             │
│   E-Mail nicht erhalten?    │
│   - Prüfe deinen Spam-Ordner│
│   - Warte einige Minuten    │
│                             │
│  E-Mail ändern →            │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### EmailVerificationBanner (Component)
```xml
<!-- In MainLayout oder als Teil der Shell -->
<Frame IsVisible="{Binding IsEmailUnverified}"
       BackgroundColor="{StaticResource Warning}"
       Padding="12"
       CornerRadius="0"
       HasShadow="False">
    <Grid ColumnDefinitions="*,Auto">
        <VerticalStackLayout>
            <Label Text="Bitte bestätige deine E-Mail-Adresse"
                   FontSize="14"
                   FontAttributes="Bold"
                   TextColor="White" />
            <Label Text="Einige Funktionen sind eingeschränkt."
                   FontSize="12"
                   TextColor="White" />
        </VerticalStackLayout>
        <Button Grid.Column="1"
                Text="Senden"
                Command="{Binding ResendVerificationCommand}"
                BackgroundColor="White"
                TextColor="{StaticResource Warning}"
                CornerRadius="4"
                Padding="8,4"
                FontSize="12" />
    </Grid>
</Frame>
```

### EmailVerificationPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.EmailVerificationPage"
             x:DataType="vm:EmailVerificationViewModel"
             Title="E-Mail bestätigen"
             Shell.NavBarIsVisible="False">

    <VerticalStackLayout Padding="24"
                         Spacing="16"
                         VerticalOptions="Center">

        <Image Source="email_verification.png"
               HeightRequest="100"
               Aspect="AspectFit" />

        <Label Text="Bestätige deine E-Mail"
               Style="{StaticResource HeadlineLabel}"
               HorizontalTextAlignment="Center" />

        <Label HorizontalTextAlignment="Center"
               FontSize="14">
            <Label.FormattedText>
                <FormattedString>
                    <Span Text="Wir haben eine E-Mail an " />
                    <Span Text="{Binding Email}" FontAttributes="Bold" />
                    <Span Text=" gesendet." />
                </FormattedString>
            </Label.FormattedText>
        </Label>

        <Label Text="Klicke auf den Link in der E-Mail, um dein Konto zu aktivieren."
               FontSize="14"
               HorizontalTextAlignment="Center"
               TextColor="{StaticResource TextSecondaryLight}" />

        <!-- Status -->
        <Frame BackgroundColor="{StaticResource SuccessLight}"
               Padding="12"
               CornerRadius="8"
               IsVisible="{Binding IsSent}">
            <Label Text="E-Mail wurde erneut gesendet!"
                   TextColor="{StaticResource Success}"
                   HorizontalTextAlignment="Center" />
        </Frame>

        <!-- Erneut senden Button -->
        <Button Text="{Binding ResendButtonText}"
                Command="{Binding ResendVerificationCommand}"
                Style="{StaticResource PrimaryButton}"
                IsEnabled="{Binding CanResend}" />

        <!-- Timer -->
        <Label Text="{Binding CooldownText}"
               FontSize="12"
               TextColor="{StaticResource TextSecondaryLight}"
               HorizontalTextAlignment="Center"
               IsVisible="{Binding IsCooldownActive}" />

        <!-- Hilfetext -->
        <VerticalStackLayout Margin="0,24,0,0">
            <Label Text="E-Mail nicht erhalten?"
                   FontAttributes="Bold"
                   FontSize="14" />
            <Label Text="- Prüfe deinen Spam-Ordner"
                   FontSize="12"
                   TextColor="{StaticResource TextSecondaryLight}" />
            <Label Text="- Warte einige Minuten"
                   FontSize="12"
                   TextColor="{StaticResource TextSecondaryLight}" />
        </VerticalStackLayout>

        <!-- E-Mail ändern -->
        <Label Text="E-Mail-Adresse ändern"
               TextColor="{StaticResource Primary}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center"
               Margin="0,16,0,0">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding ChangeEmailCommand}" />
            </Label.GestureRecognizers>
        </Label>

        <!-- Weiter ohne Verifizierung -->
        <Label Text="Später verifizieren"
               TextColor="{StaticResource TextSecondaryLight}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding SkipForNowCommand}" />
            </Label.GestureRecognizers>
        </Label>

    </VerticalStackLayout>

</ContentPage>
```

### EmailVerificationViewModel.cs
```csharp
public partial class EmailVerificationViewModel : ObservableObject
{
    private readonly IAuthApi _authApi;
    private readonly ITokenService _tokenService;
    private readonly INavigationService _navigationService;
    private int _cooldownSeconds;
    private IDispatcherTimer? _cooldownTimer;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private bool _isSent;

    [ObservableProperty]
    private bool _isCooldownActive;

    public bool CanResend => !IsCooldownActive;

    public string ResendButtonText => IsCooldownActive
        ? $"Erneut senden ({_cooldownSeconds}s)"
        : "E-Mail erneut senden";

    public string CooldownText => $"Du kannst in {_cooldownSeconds} Sekunden erneut senden.";

    public async Task InitializeAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        Email = claims?.Email ?? "";
    }

    [RelayCommand]
    private async Task ResendVerificationAsync()
    {
        if (IsCooldownActive) return;

        try
        {
            var response = await _authApi.ResendVerificationEmailAsync();
            if (response.IsSuccessStatusCode)
            {
                IsSent = true;
                StartCooldown();
            }
        }
        catch
        {
            await Shell.Current.DisplayAlert(
                "Fehler",
                "E-Mail konnte nicht gesendet werden. Bitte versuche es später erneut.",
                "OK");
        }
    }

    private void StartCooldown()
    {
        _cooldownSeconds = 60;
        IsCooldownActive = true;

        _cooldownTimer = Application.Current?.Dispatcher.CreateTimer();
        if (_cooldownTimer != null)
        {
            _cooldownTimer.Interval = TimeSpan.FromSeconds(1);
            _cooldownTimer.Tick += (s, e) =>
            {
                _cooldownSeconds--;
                OnPropertyChanged(nameof(ResendButtonText));
                OnPropertyChanged(nameof(CooldownText));

                if (_cooldownSeconds <= 0)
                {
                    _cooldownTimer.Stop();
                    IsCooldownActive = false;
                    OnPropertyChanged(nameof(CanResend));
                }
            };
            _cooldownTimer.Start();
        }
    }

    [RelayCommand]
    private async Task SkipForNowAsync()
    {
        await _navigationService.NavigateToAsync("//main/dashboard");
    }

    [RelayCommand]
    private async Task ChangeEmailAsync()
    {
        // Dialog zum E-Mail ändern anzeigen
        var newEmail = await Shell.Current.DisplayPromptAsync(
            "E-Mail ändern",
            "Gib deine neue E-Mail-Adresse ein:",
            "Ändern",
            "Abbrechen",
            keyboard: Keyboard.Email);

        if (!string.IsNullOrWhiteSpace(newEmail))
        {
            // API-Aufruf zum E-Mail ändern
            // ...
        }
    }
}
```

### API-Interface
```csharp
public interface IAuthApi
{
    [Post("/api/auth/resend-verification")]
    Task<ApiResponse<EmptyResponse>> ResendVerificationEmailAsync();

    [Post("/api/auth/verify-email")]
    Task<ApiResponse<EmptyResponse>> VerifyEmailAsync([Body] VerifyEmailRequest request);
}

public record VerifyEmailRequest(string Token);
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-53 | E-Mail erneut senden | E-Mail wird gesendet |
| TC-M002-54 | Cooldown aktiv | Button deaktiviert |
| TC-M002-55 | Verifizierungslink klicken | E-Mail verifiziert |
| TC-M002-56 | Ohne Verifizierung fortfahren | Dashboard mit Banner |

## Story Points
2

## Priorität
Mittel
