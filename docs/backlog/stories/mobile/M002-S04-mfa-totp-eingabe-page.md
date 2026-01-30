# Story M002-S04: MFA/TOTP-Eingabe-Page

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Elternteil mit aktivierter Zwei-Faktor-Authentifizierung** möchte ich **meinen TOTP-Code eingeben können**, damit **mein Konto zusätzlich geschützt ist**.

## Akzeptanzkriterien

- [ ] Gegeben ein Login mit MFA, wenn der Benutzer sich anmeldet, dann wird die TOTP-Seite angezeigt
- [ ] Gegeben die TOTP-Seite, wenn der korrekte 6-stellige Code eingegeben wird, dann wird eingeloggt
- [ ] Gegeben ein falscher Code, wenn er eingegeben wird, dann erscheint eine Fehlermeldung
- [ ] Gegeben die Code-Eingabe, wenn 6 Ziffern eingegeben sind, dann wird automatisch validiert
- [ ] Gegeben Probleme mit dem Code, wenn der Benutzer Hilfe braucht, dann gibt es eine Option für Backup-Codes

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Bestätigung      │
├─────────────────────────────┤
│                             │
│         [🔐 Icon]           │
│                             │
│   Zwei-Faktor-              │
│   Authentifizierung         │
│                             │
│   Gib den 6-stelligen Code  │
│   aus deiner Authenticator- │
│   App ein.                  │
│                             │
│  ┌──┐ ┌──┐ ┌──┐ ┌──┐ ┌──┐ ┌──┐│
│  │  │ │  │ │  │ │  │ │  │ │  ││
│  └──┘ └──┘ └──┘ └──┘ └──┘ └──┘│
│                             │
│  [Fehleranzeige]            │
│                             │
│  ┌───────────────────────┐  │
│  │     Bestätigen        │  │
│  └───────────────────────┘  │
│                             │
│  Probleme mit dem Code?     │
│  → Backup-Code verwenden    │
│                             │
│  ────────────────────────   │
│                             │
│  Code erneut senden (30s)   │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### TotpVerificationPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.TotpVerificationPage"
             x:DataType="vm:TotpVerificationViewModel"
             Title="Bestätigung">

    <VerticalStackLayout Padding="24"
                         Spacing="16"
                         VerticalOptions="Center">

        <!-- Icon -->
        <Image Source="shield_lock.png"
               HeightRequest="80"
               Aspect="AspectFit"
               HorizontalOptions="Center" />

        <!-- Titel -->
        <Label Text="Zwei-Faktor-Authentifizierung"
               Style="{StaticResource HeadlineLabel}"
               HorizontalTextAlignment="Center" />

        <!-- Beschreibung -->
        <Label Text="Gib den 6-stelligen Code aus deiner Authenticator-App ein."
               FontSize="14"
               HorizontalTextAlignment="Center"
               Margin="0,0,0,16" />

        <!-- Code-Eingabe -->
        <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
            <Entry x:Name="Digit1"
                   WidthRequest="45"
                   HeightRequest="55"
                   FontSize="24"
                   HorizontalTextAlignment="Center"
                   Keyboard="Numeric"
                   MaxLength="1"
                   Text="{Binding Digit1}"
                   Unfocused="OnDigitUnfocused" />
            <Entry x:Name="Digit2"
                   WidthRequest="45"
                   HeightRequest="55"
                   FontSize="24"
                   HorizontalTextAlignment="Center"
                   Keyboard="Numeric"
                   MaxLength="1"
                   Text="{Binding Digit2}" />
            <Entry x:Name="Digit3"
                   WidthRequest="45"
                   HeightRequest="55"
                   FontSize="24"
                   HorizontalTextAlignment="Center"
                   Keyboard="Numeric"
                   MaxLength="1"
                   Text="{Binding Digit3}" />
            <Entry x:Name="Digit4"
                   WidthRequest="45"
                   HeightRequest="55"
                   FontSize="24"
                   HorizontalTextAlignment="Center"
                   Keyboard="Numeric"
                   MaxLength="1"
                   Text="{Binding Digit4}" />
            <Entry x:Name="Digit5"
                   WidthRequest="45"
                   HeightRequest="55"
                   FontSize="24"
                   HorizontalTextAlignment="Center"
                   Keyboard="Numeric"
                   MaxLength="1"
                   Text="{Binding Digit5}" />
            <Entry x:Name="Digit6"
                   WidthRequest="45"
                   HeightRequest="55"
                   FontSize="24"
                   HorizontalTextAlignment="Center"
                   Keyboard="Numeric"
                   MaxLength="1"
                   Text="{Binding Digit6}" />
        </HorizontalStackLayout>

        <!-- Fehleranzeige -->
        <Label Text="{Binding ErrorMessage}"
               TextColor="Red"
               IsVisible="{Binding HasError}"
               HorizontalTextAlignment="Center" />

        <!-- Bestätigen Button -->
        <Button Text="Bestätigen"
                Command="{Binding VerifyCommand}"
                Style="{StaticResource PrimaryButton}"
                IsEnabled="{Binding CanVerify}" />

        <ActivityIndicator IsRunning="{Binding IsBusy}"
                           IsVisible="{Binding IsBusy}" />

        <!-- Backup-Code Option -->
        <Label Text="Backup-Code verwenden"
               TextColor="{StaticResource Primary}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center"
               Margin="0,16,0,0">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding UseBackupCodeCommand}" />
            </Label.GestureRecognizers>
        </Label>

        <!-- Hilfetext -->
        <Label Text="Der Code ist nur 30 Sekunden gültig"
               FontSize="12"
               TextColor="{StaticResource TextSecondaryLight}"
               HorizontalTextAlignment="Center" />

    </VerticalStackLayout>

</ContentPage>
```

### TotpVerificationViewModel.cs
```csharp
public partial class TotpVerificationViewModel : ObservableObject
{
    private readonly IAuthApi _authApi;
    private readonly INavigationService _navigationService;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanVerify))]
    [NotifyPropertyChangedFor(nameof(FullCode))]
    private string _digit1 = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanVerify))]
    [NotifyPropertyChangedFor(nameof(FullCode))]
    private string _digit2 = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanVerify))]
    [NotifyPropertyChangedFor(nameof(FullCode))]
    private string _digit3 = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanVerify))]
    [NotifyPropertyChangedFor(nameof(FullCode))]
    private string _digit4 = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanVerify))]
    [NotifyPropertyChangedFor(nameof(FullCode))]
    private string _digit5 = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanVerify))]
    [NotifyPropertyChangedFor(nameof(FullCode))]
    private string _digit6 = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    // Temporärer Token vom Login-Schritt
    public string PendingToken { get; set; } = string.Empty;

    public string FullCode => $"{Digit1}{Digit2}{Digit3}{Digit4}{Digit5}{Digit6}";

    public bool CanVerify => FullCode.Length == 6 && FullCode.All(char.IsDigit);
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    partial void OnDigit1Changed(string value) => AutoFocusNext(value, 1);
    partial void OnDigit2Changed(string value) => AutoFocusNext(value, 2);
    // ... weitere OnDigitXChanged Methoden

    private void AutoFocusNext(string value, int currentIndex)
    {
        if (!string.IsNullOrEmpty(value) && currentIndex < 6)
        {
            // Focus auf nächstes Feld setzen (via Messenger oder Event)
            WeakReferenceMessenger.Default.Send(
                new FocusDigitMessage(currentIndex + 1));
        }

        // Auto-Verify wenn komplett
        if (CanVerify)
        {
            _ = VerifyAsync();
        }
    }

    [RelayCommand]
    private async Task VerifyAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var response = await _authApi.VerifyTotpAsync(new TotpVerifyRequest
            {
                PendingToken = PendingToken,
                TotpCode = FullCode
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
                ErrorMessage = "Der Code ist ungültig. Bitte versuche es erneut.";
                ClearCode();
            }
        }
        catch
        {
            ErrorMessage = "Ein Fehler ist aufgetreten.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UseBackupCodeAsync()
    {
        await _navigationService.NavigateToAsync("backup-code-entry");
    }

    private void ClearCode()
    {
        Digit1 = Digit2 = Digit3 = Digit4 = Digit5 = Digit6 = string.Empty;
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-15 | Korrekten 6-stelligen Code eingeben | Login erfolgreich |
| TC-M002-16 | Falschen Code eingeben | Fehlermeldung, Code geleert |
| TC-M002-17 | Auto-Focus nach Zifferneingabe | Nächstes Feld fokussiert |
| TC-M002-18 | Backup-Code Link klicken | Navigation zu Backup-Code-Seite |

## Story Points
2

## Priorität
Hoch
