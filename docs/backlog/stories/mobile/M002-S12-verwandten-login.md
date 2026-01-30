# Story M002-S12: Verwandten-Login

## Epic
M002 - Authentifizierung

## Status
Offen

## User Story

Als **eingeladener Verwandter** möchte ich **mich mit meinem Einladungslink registrieren und anmelden können**, damit **ich meinen Enkeln/Nichten/Neffen Geldgeschenke senden kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Einladungslink, wenn er geöffnet wird, dann wird die Registrierungsseite für Verwandte angezeigt
- [ ] Gegeben die Registrierung, wenn sie abgeschlossen ist, dann ist der Verwandte mit der Familie verknüpft
- [ ] Gegeben ein bereits registrierter Verwandter, wenn er den Link erneut öffnet, dann wird er zum Login weitergeleitet
- [ ] Gegeben der Verwandten-Login, wenn er erfolgreich ist, dann sieht der Verwandte nur die zugeordneten Kinder

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│     [Logo/App-Name]         │
│   TaschengeldManager        │
│                             │
├─────────────────────────────┤
│                             │
│   Du wurdest eingeladen!    │
│                             │
│   Familie Müller möchte     │
│   dich als Oma/Opa/Onkel/   │
│   Tante hinzufügen.         │
│                             │
│  ┌───────────────────────┐  │
│  │ Vorname               │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │ Nachname              │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │ E-Mail                │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │ Passwort              │  │
│  └───────────────────────┘  │
│                             │
│  Beziehung zu den Kindern:  │
│  ○ Oma    ○ Opa            │
│  ○ Tante  ○ Onkel          │
│  ○ Andere: [___________]   │
│                             │
│  ┌───────────────────────┐  │
│  │   Einladung annehmen  │  │
│  └───────────────────────┘  │
│                             │
│  Bereits registriert?       │
│  → Zum Login                │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### Deep Link Handling
```csharp
// URI: taschengeldmanager://invite?token=xxx&family=FamilyName
protected override async void OnAppLinkRequestReceived(Uri uri)
{
    base.OnAppLinkRequestReceived(uri);

    if (uri.Host == "invite")
    {
        var query = HttpUtility.ParseQueryString(uri.Query);
        var token = query.Get("token");
        var familyName = query.Get("family");

        if (!string.IsNullOrEmpty(token))
        {
            var parameters = new Dictionary<string, object>
            {
                { "InviteToken", token },
                { "FamilyName", familyName ?? "" }
            };

            await Shell.Current.GoToAsync("relative-registration", parameters);
        }
    }
}
```

### RelativeRegistrationPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.RelativeRegistrationPage"
             x:DataType="vm:RelativeRegistrationViewModel"
             Title="Einladung">

    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="16">

            <!-- Logo -->
            <Image Source="logo.png"
                   HeightRequest="60"
                   Aspect="AspectFit" />

            <!-- Einladungstext -->
            <Label HorizontalTextAlignment="Center" FontSize="18">
                <Label.FormattedText>
                    <FormattedString>
                        <Span Text="Du wurdest eingeladen!" FontAttributes="Bold" />
                    </FormattedString>
                </Label.FormattedText>
            </Label>

            <Label HorizontalTextAlignment="Center"
                   TextColor="{StaticResource TextSecondaryLight}">
                <Label.FormattedText>
                    <FormattedString>
                        <Span Text="Familie " />
                        <Span Text="{Binding FamilyName}" FontAttributes="Bold" />
                        <Span Text=" möchte dich als Verwandten hinzufügen." />
                    </FormattedString>
                </Label.FormattedText>
            </Label>

            <!-- Eingabefelder -->
            <Entry Placeholder="Vorname" Text="{Binding FirstName}" />
            <Entry Placeholder="Nachname" Text="{Binding LastName}" />
            <Entry Placeholder="E-Mail" Text="{Binding Email}" Keyboard="Email" />
            <Entry Placeholder="Passwort" Text="{Binding Password}" IsPassword="True" />

            <!-- Beziehungsauswahl -->
            <Label Text="Beziehung zu den Kindern:"
                   FontSize="14"
                   Margin="0,8,0,0" />

            <FlexLayout Wrap="Wrap" JustifyContent="Start">
                <RadioButton Content="Oma"
                             Value="Grandmother"
                             IsChecked="{Binding IsGrandmother}"
                             GroupName="Relation" />
                <RadioButton Content="Opa"
                             Value="Grandfather"
                             IsChecked="{Binding IsGrandfather}"
                             GroupName="Relation" />
                <RadioButton Content="Tante"
                             Value="Aunt"
                             IsChecked="{Binding IsAunt}"
                             GroupName="Relation" />
                <RadioButton Content="Onkel"
                             Value="Uncle"
                             IsChecked="{Binding IsUncle}"
                             GroupName="Relation" />
                <RadioButton Content="Andere"
                             Value="Other"
                             IsChecked="{Binding IsOther}"
                             GroupName="Relation" />
            </FlexLayout>

            <Entry Placeholder="Beziehung (z.B. Patenonkel)"
                   Text="{Binding CustomRelation}"
                   IsVisible="{Binding IsOther}" />

            <!-- Fehler -->
            <Label Text="{Binding ErrorMessage}"
                   TextColor="Red"
                   IsVisible="{Binding HasError}"
                   HorizontalTextAlignment="Center" />

            <!-- Registrieren Button -->
            <Button Text="Einladung annehmen"
                    Command="{Binding AcceptInvitationCommand}"
                    Style="{StaticResource PrimaryButton}" />

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

### RelativeRegistrationViewModel.cs
```csharp
public partial class RelativeRegistrationViewModel : ObservableObject, IQueryAttributable
{
    private readonly IAuthApi _authApi;
    private readonly INavigationService _navigationService;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    private string _inviteToken = string.Empty;

    [ObservableProperty]
    private string _familyName = string.Empty;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _selectedRelation = "Grandmother";

    [ObservableProperty]
    private string _customRelation = string.Empty;

    [ObservableProperty]
    private bool _isOther;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("InviteToken", out var token))
            InviteToken = token.ToString() ?? "";

        if (query.TryGetValue("FamilyName", out var family))
            FamilyName = family.ToString() ?? "";
    }

    [RelayCommand]
    private async Task AcceptInvitationAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var relation = IsOther ? CustomRelation : SelectedRelation;

            var response = await _authApi.RegisterRelativeAsync(new RegisterRelativeRequest
            {
                InviteToken = InviteToken,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                Relation = relation
            });

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                await _tokenService.SaveTokensAsync(
                    response.Content.Token,
                    response.Content.RefreshToken);

                await _navigationService.NavigateToAsync("//main/relative-dashboard");
            }
            else
            {
                ErrorMessage = "Registrierung fehlgeschlagen. Bitte prüfe deine Eingaben.";
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
}
```

### API-Interface
```csharp
public interface IAuthApi
{
    [Post("/api/auth/register-relative")]
    Task<ApiResponse<RegisterResponse>> RegisterRelativeAsync([Body] RegisterRelativeRequest request);
}

public record RegisterRelativeRequest(
    string InviteToken,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Relation);
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-49 | Deep Link öffnen | Registrierungsseite mit Familiename |
| TC-M002-50 | Registrierung abschließen | Verwandter eingeloggt |
| TC-M002-51 | Ungültiger Token | Fehlermeldung |
| TC-M002-52 | Bereits registriert | Login-Link funktioniert |

## Story Points
3

## Priorität
Mittel
