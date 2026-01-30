# Story M002-S07: Biometrie-Aktivierung Dialog

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **die biometrische Anmeldung aktivieren können**, damit **ich mich schneller und sicherer anmelden kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein erfolgreicher Login, wenn Biometrie verfügbar ist, dann wird der Aktivierungsdialog angezeigt
- [ ] Gegeben der Dialog, wenn der Benutzer zustimmt, dann wird Biometrie aktiviert und die Credentials gespeichert
- [ ] Gegeben der Dialog, wenn der Benutzer ablehnt, dann wird die Entscheidung gespeichert und nicht erneut gefragt
- [ ] Gegeben die Einstellungen, wenn Biometrie aktiviert ist, dann kann sie dort deaktiviert werden

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│         [Fingerprint]       │
│                             │
│   Mit Fingerabdruck         │
│   anmelden?                 │
│                             │
│   Melde dich beim nächsten  │
│   Mal schnell und sicher    │
│   mit deinem Fingerabdruck  │
│   an.                       │
│                             │
│  ┌───────────────────────┐  │
│  │    Ja, aktivieren     │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │   Nein, danke         │  │
│  └───────────────────────┘  │
│                             │
│  ☐ Nicht erneut fragen      │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### BiometricEnrollmentPopup.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<toolkit:Popup xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
               xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
               xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
               x:Class="TaschengeldManager.Mobile.Views.BiometricEnrollmentPopup"
               Color="Transparent">

    <Frame Padding="24"
           CornerRadius="16"
           BackgroundColor="{AppThemeBinding Light={StaticResource BackgroundLight},
                                             Dark={StaticResource BackgroundDark}}"
           WidthRequest="300">
        <VerticalStackLayout Spacing="16">

            <!-- Icon -->
            <Image Source="fingerprint.png"
                   HeightRequest="64"
                   Aspect="AspectFit"
                   HorizontalOptions="Center" />

            <!-- Titel -->
            <Label Text="Mit Fingerabdruck anmelden?"
                   FontSize="20"
                   FontAttributes="Bold"
                   HorizontalTextAlignment="Center" />

            <!-- Beschreibung -->
            <Label Text="Melde dich beim nächsten Mal schnell und sicher mit deinem Fingerabdruck an."
                   FontSize="14"
                   HorizontalTextAlignment="Center"
                   TextColor="{StaticResource TextSecondaryLight}" />

            <!-- Aktivieren Button -->
            <Button Text="Ja, aktivieren"
                    Command="{Binding EnableBiometricCommand}"
                    Style="{StaticResource PrimaryButton}" />

            <!-- Ablehnen Button -->
            <Button Text="Nein, danke"
                    Command="{Binding DeclineBiometricCommand}"
                    Style="{StaticResource SecondaryButton}" />

            <!-- Nicht erneut fragen Checkbox -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                <CheckBox IsChecked="{Binding DontAskAgain}" />
                <Label Text="Nicht erneut fragen"
                       VerticalOptions="Center"
                       FontSize="12" />
            </HorizontalStackLayout>

        </VerticalStackLayout>
    </Frame>

</toolkit:Popup>
```

### IBiometricService Interface
```csharp
public interface IBiometricService
{
    Task<bool> IsAvailableAsync();
    Task<BiometricType> GetBiometricTypeAsync();
    Task<bool> AuthenticateAsync(string reason);
    Task EnableBiometricLoginAsync(string email, string password);
    Task DisableBiometricLoginAsync();
    Task<(string Email, string Password)?> GetStoredCredentialsAsync();
    Task<bool> IsBiometricEnabledAsync();
}

public enum BiometricType
{
    None,
    Fingerprint,
    FaceId,
    Iris
}
```

### BiometricService Implementation
```csharp
public class BiometricService : IBiometricService
{
    private const string BiometricEnabledKey = "biometric_enabled";
    private const string BiometricEmailKey = "biometric_email";
    private const string BiometricPasswordKey = "biometric_password";

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            return await CrossFingerprint.Current.IsAvailableAsync(true);
        }
        catch
        {
            return false;
        }
    }

    public async Task<BiometricType> GetBiometricTypeAsync()
    {
        var authType = await CrossFingerprint.Current.GetAuthenticationTypeAsync();
        return authType switch
        {
            AuthenticationType.Fingerprint => BiometricType.Fingerprint,
            AuthenticationType.Face => BiometricType.FaceId,
            _ => BiometricType.None
        };
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        var result = await CrossFingerprint.Current.AuthenticateAsync(
            new AuthenticationRequestConfiguration(
                "TaschengeldManager",
                reason));

        return result.Authenticated;
    }

    public async Task EnableBiometricLoginAsync(string email, string password)
    {
        await SecureStorage.Default.SetAsync(BiometricEmailKey, email);
        await SecureStorage.Default.SetAsync(BiometricPasswordKey, password);
        Preferences.Default.Set(BiometricEnabledKey, true);
    }

    public async Task DisableBiometricLoginAsync()
    {
        SecureStorage.Default.Remove(BiometricEmailKey);
        SecureStorage.Default.Remove(BiometricPasswordKey);
        Preferences.Default.Set(BiometricEnabledKey, false);
        await Task.CompletedTask;
    }

    public async Task<(string Email, string Password)?> GetStoredCredentialsAsync()
    {
        var email = await SecureStorage.Default.GetAsync(BiometricEmailKey);
        var password = await SecureStorage.Default.GetAsync(BiometricPasswordKey);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return null;

        return (email, password);
    }

    public Task<bool> IsBiometricEnabledAsync()
    {
        return Task.FromResult(Preferences.Default.Get(BiometricEnabledKey, false));
    }
}
```

### Enrollment nach Login
```csharp
// In LoginViewModel nach erfolgreichem Login
private async Task ShowBiometricEnrollmentIfNeeded(string email, string password)
{
    // Prüfen, ob Biometrie verfügbar und noch nicht aktiviert
    if (!await _biometricService.IsAvailableAsync())
        return;

    if (await _biometricService.IsBiometricEnabledAsync())
        return;

    if (Preferences.Default.Get("biometric_dont_ask", false))
        return;

    // Popup anzeigen
    var popup = new BiometricEnrollmentPopup(
        new BiometricEnrollmentViewModel(_biometricService, email, password));

    await Shell.Current.ShowPopupAsync(popup);
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-28 | Biometrie verfügbar, Login erfolgreich | Dialog wird angezeigt |
| TC-M002-29 | Benutzer aktiviert Biometrie | Credentials gespeichert |
| TC-M002-30 | Benutzer lehnt ab | Dialog schließt |
| TC-M002-31 | "Nicht erneut fragen" aktiviert | Dialog erscheint nicht mehr |

## Story Points
3

## Priorität
Mittel
