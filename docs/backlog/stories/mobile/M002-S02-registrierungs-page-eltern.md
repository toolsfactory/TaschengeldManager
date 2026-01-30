# Story M002-S02: Registrierungs-Page für Eltern

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **neuer Benutzer** möchte ich **mich als Elternteil registrieren können**, damit **ich ein Familienkonto anlegen und die App nutzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Registrierungsseite, wenn alle Pflichtfelder ausgefüllt sind, dann kann registriert werden
- [ ] Gegeben ein ungültiges E-Mail-Format, wenn validiert wird, dann erscheint eine Fehlermeldung
- [ ] Gegeben ein zu kurzes Passwort, wenn validiert wird, dann erscheint ein Hinweis auf die Mindestlänge
- [ ] Gegeben nicht übereinstimmende Passwörter, wenn validiert wird, dann wird ein Fehler angezeigt
- [ ] Gegeben eine erfolgreiche Registrierung, wenn abgeschlossen, dann wird der Benutzer eingeloggt

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Registrierung  │
├─────────────────────────────┤
│                             │
│  Erstelle dein Konto        │
│                             │
│  ┌───────────────────────┐  │
│  │ Vorname *             │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │ Nachname *            │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │ E-Mail *              │  │
│  └───────────────────────┘  │
│  [Validierungshinweis]      │
│                             │
│  ┌───────────────────────┐  │
│  │ Passwort *        [👁] │  │
│  └───────────────────────┘  │
│  Min. 8 Zeichen             │
│                             │
│  ┌───────────────────────┐  │
│  │ Passwort bestätigen * │  │
│  └───────────────────────┘  │
│                             │
│  ☐ Ich akzeptiere die AGB   │
│    und Datenschutzrichtlinie│
│                             │
│  ┌───────────────────────┐  │
│  │    Registrieren       │  │
│  └───────────────────────┘  │
│                             │
│  Bereits registriert?       │
│  → Zum Login                │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### RegisterPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.RegisterPage"
             x:DataType="vm:RegisterViewModel"
             Title="Registrierung">

    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="12">

            <Label Text="Erstelle dein Konto"
                   Style="{StaticResource HeadlineLabel}" />

            <!-- Vorname -->
            <Entry Placeholder="Vorname *"
                   Text="{Binding FirstName}"
                   ReturnType="Next" />

            <!-- Nachname -->
            <Entry Placeholder="Nachname *"
                   Text="{Binding LastName}"
                   ReturnType="Next" />

            <!-- E-Mail -->
            <Entry Placeholder="E-Mail *"
                   Text="{Binding Email}"
                   Keyboard="Email"
                   ReturnType="Next" />
            <Label Text="{Binding EmailValidationMessage}"
                   TextColor="Red"
                   FontSize="12"
                   IsVisible="{Binding HasEmailError}" />

            <!-- Passwort -->
            <Grid ColumnDefinitions="*,Auto">
                <Entry Placeholder="Passwort *"
                       Text="{Binding Password}"
                       IsPassword="{Binding IsPasswordHidden}"
                       ReturnType="Next" />
                <ImageButton Grid.Column="1"
                             Source="{Binding PasswordVisibilityIcon}"
                             Command="{Binding TogglePasswordVisibilityCommand}" />
            </Grid>
            <Label Text="Mindestens 8 Zeichen"
                   FontSize="12"
                   TextColor="{Binding PasswordStrengthColor}" />

            <!-- Passwort bestätigen -->
            <Entry Placeholder="Passwort bestätigen *"
                   Text="{Binding ConfirmPassword}"
                   IsPassword="True"
                   ReturnType="Done" />
            <Label Text="Passwörter stimmen nicht überein"
                   TextColor="Red"
                   FontSize="12"
                   IsVisible="{Binding PasswordsDoNotMatch}" />

            <!-- AGB Checkbox -->
            <HorizontalStackLayout Spacing="8">
                <CheckBox IsChecked="{Binding AcceptedTerms}" />
                <Label VerticalOptions="Center">
                    <Label.FormattedText>
                        <FormattedString>
                            <Span Text="Ich akzeptiere die " />
                            <Span Text="AGB"
                                  TextColor="{StaticResource Primary}"
                                  TextDecorations="Underline">
                                <Span.GestureRecognizers>
                                    <TapGestureRecognizer Command="{Binding ShowTermsCommand}" />
                                </Span.GestureRecognizers>
                            </Span>
                            <Span Text=" und " />
                            <Span Text="Datenschutzrichtlinie"
                                  TextColor="{StaticResource Primary}"
                                  TextDecorations="Underline">
                                <Span.GestureRecognizers>
                                    <TapGestureRecognizer Command="{Binding ShowPrivacyCommand}" />
                                </Span.GestureRecognizers>
                            </Span>
                        </FormattedString>
                    </Label.FormattedText>
                </Label>
            </HorizontalStackLayout>

            <!-- Fehleranzeige -->
            <Label Text="{Binding ErrorMessage}"
                   TextColor="Red"
                   IsVisible="{Binding HasError}"
                   HorizontalTextAlignment="Center" />

            <!-- Registrieren Button -->
            <Button Text="Registrieren"
                    Command="{Binding RegisterCommand}"
                    Style="{StaticResource PrimaryButton}"
                    Margin="0,16,0,0" />

            <ActivityIndicator IsRunning="{Binding IsBusy}"
                               IsVisible="{Binding IsBusy}" />

            <!-- Login Link -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="4">
                <Label Text="Bereits registriert?" />
                <Label Text="Zum Login"
                       TextColor="{StaticResource Primary}"
                       TextDecorations="Underline">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer Command="{Binding NavigateToLoginCommand}" />
                    </Label.GestureRecognizers>
                </Label>
            </HorizontalStackLayout>

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### RegisterViewModel.cs
```csharp
public partial class RegisterViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _firstName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _lastName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    [NotifyPropertyChangedFor(nameof(HasEmailError))]
    [NotifyPropertyChangedFor(nameof(EmailValidationMessage))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    [NotifyPropertyChangedFor(nameof(PasswordStrengthColor))]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    [NotifyPropertyChangedFor(nameof(PasswordsDoNotMatch))]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private bool _acceptedTerms;

    public bool HasEmailError =>
        !string.IsNullOrEmpty(Email) && !IsValidEmail(Email);

    public string EmailValidationMessage =>
        HasEmailError ? "Bitte gib eine gültige E-Mail-Adresse ein" : string.Empty;

    public bool PasswordsDoNotMatch =>
        !string.IsNullOrEmpty(ConfirmPassword) && Password != ConfirmPassword;

    public Color PasswordStrengthColor =>
        Password.Length >= 8 ? Colors.Green : Colors.Gray;

    private bool CanRegister() =>
        !string.IsNullOrWhiteSpace(FirstName) &&
        !string.IsNullOrWhiteSpace(LastName) &&
        IsValidEmail(Email) &&
        Password.Length >= 8 &&
        Password == ConfirmPassword &&
        AcceptedTerms;

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task RegisterAsync()
    {
        // Registrierungslogik...
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-05 | Alle Felder korrekt ausgefüllt | Registrierung erfolgreich |
| TC-M002-06 | Ungültige E-Mail | Validierungsfehler angezeigt |
| TC-M002-07 | Passwort zu kurz | Hinweis erscheint |
| TC-M002-08 | Passwörter unterschiedlich | Fehler angezeigt |
| TC-M002-09 | AGB nicht akzeptiert | Button deaktiviert |

## Story Points
3

## Priorität
Hoch
