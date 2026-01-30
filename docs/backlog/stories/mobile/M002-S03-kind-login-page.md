# Story M002-S03: Kind-Login-Page

## Epic
M002 - Authentifizierung

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **mich mit meinem Spitznamen und einer PIN anmelden können**, damit **ich einfach auf mein Taschengeld-Konto zugreifen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Kind-Login-Seite, wenn sie angezeigt wird, dann ist eine kindgerechte Oberfläche sichtbar
- [ ] Gegeben der Familien-Code, wenn er eingegeben wird, dann werden die Kinder der Familie geladen
- [ ] Gegeben die Kinder-Avatare, wenn sie angezeigt werden, dann kann das Kind sich auswählen
- [ ] Gegeben die PIN-Eingabe, wenn die korrekte PIN eingegeben wird, dann wird das Kind eingeloggt
- [ ] Gegeben eine falsche PIN, wenn sie eingegeben wird, dann erscheint eine kindgerechte Fehlermeldung

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│      Hallo! Wer bist du?    │
│                             │
│  Familien-Code:             │
│  ┌──┐ ┌──┐ ┌──┐ ┌──┐ ┌──┐ ┌──┐│
│  │  │ │  │ │  │ │  │ │  │ │  ││
│  └──┘ └──┘ └──┘ └──┘ └──┘ └──┘│
│                             │
│  ─────── Wähle dich ───────  │
│                             │
│  ┌─────┐  ┌─────┐  ┌─────┐  │
│  │ 🦁  │  │ 🐰  │  │ 🦊  │  │
│  │Max  │  │Lisa │  │Tom  │  │
│  └─────┘  └─────┘  └─────┘  │
│                             │
│  ─────── Deine PIN ─────── │
│                             │
│  ┌───┐ ┌───┐ ┌───┐ ┌───┐   │
│  │ ● │ │ ● │ │ ○ │ │ ○ │   │
│  └───┘ └───┘ └───┘ └───┘   │
│                             │
│  ┌───┐ ┌───┐ ┌───┐         │
│  │ 1 │ │ 2 │ │ 3 │         │
│  └───┘ └───┘ └───┘         │
│  ┌───┐ ┌───┐ ┌───┐         │
│  │ 4 │ │ 5 │ │ 6 │         │
│  └───┘ └───┘ └───┘         │
│  ┌───┐ ┌───┐ ┌───┐         │
│  │ 7 │ │ 8 │ │ 9 │         │
│  └───┘ └───┘ └───┘         │
│        ┌───┐ ┌───┐         │
│        │ 0 │ │ ← │         │
│        └───┘ └───┘         │
│                             │
│  Eltern-Login →             │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### ChildLoginPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.ChildLoginPage"
             x:DataType="vm:ChildLoginViewModel"
             Shell.NavBarIsVisible="False"
             BackgroundColor="{StaticResource PrimaryLight}">

    <Grid RowDefinitions="Auto,Auto,*,Auto" Padding="24">

        <!-- Titel -->
        <Label Grid.Row="0"
               Text="Hallo! Wer bist du?"
               FontSize="28"
               FontAttributes="Bold"
               HorizontalTextAlignment="Center"
               Margin="0,24,0,16" />

        <!-- Familien-Code -->
        <VerticalStackLayout Grid.Row="1" Spacing="8">
            <Label Text="Familien-Code:"
                   FontSize="16"
                   HorizontalTextAlignment="Center" />
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                <Entry WidthRequest="40"
                       MaxLength="1"
                       Keyboard="Text"
                       HorizontalTextAlignment="Center"
                       Text="{Binding FamilyCodeChar1}" />
                <!-- Weitere 5 Entry-Felder -->
            </HorizontalStackLayout>
        </VerticalStackLayout>

        <!-- Kinder-Auswahl -->
        <CollectionView Grid.Row="2"
                        ItemsSource="{Binding Children}"
                        SelectionMode="Single"
                        SelectedItem="{Binding SelectedChild}"
                        IsVisible="{Binding HasChildren}">
            <CollectionView.ItemsLayout>
                <GridItemsLayout Orientation="Vertical"
                                 Span="3"
                                 HorizontalItemSpacing="16"
                                 VerticalItemSpacing="16" />
            </CollectionView.ItemsLayout>
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="vm:ChildViewModel">
                    <Frame Padding="8"
                           CornerRadius="12"
                           BackgroundColor="{Binding IsSelected,
                               Converter={StaticResource BoolToColorConverter},
                               ConverterParameter='Primary|Surface'}">
                        <VerticalStackLayout HorizontalOptions="Center">
                            <Label Text="{Binding AvatarEmoji}"
                                   FontSize="48"
                                   HorizontalTextAlignment="Center" />
                            <Label Text="{Binding Nickname}"
                                   FontSize="14"
                                   FontAttributes="Bold"
                                   HorizontalTextAlignment="Center" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <!-- PIN-Eingabe und Numpad -->
        <VerticalStackLayout Grid.Row="3" Spacing="16">

            <!-- PIN-Anzeige -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="12">
                <Frame WidthRequest="40" HeightRequest="40"
                       CornerRadius="20"
                       BackgroundColor="{Binding Pin1Filled,
                           Converter={StaticResource BoolToColorConverter}}" />
                <Frame WidthRequest="40" HeightRequest="40"
                       CornerRadius="20"
                       BackgroundColor="{Binding Pin2Filled,
                           Converter={StaticResource BoolToColorConverter}}" />
                <Frame WidthRequest="40" HeightRequest="40"
                       CornerRadius="20"
                       BackgroundColor="{Binding Pin3Filled,
                           Converter={StaticResource BoolToColorConverter}}" />
                <Frame WidthRequest="40" HeightRequest="40"
                       CornerRadius="20"
                       BackgroundColor="{Binding Pin4Filled,
                           Converter={StaticResource BoolToColorConverter}}" />
            </HorizontalStackLayout>

            <!-- Fehleranzeige -->
            <Label Text="{Binding ErrorMessage}"
                   TextColor="Red"
                   IsVisible="{Binding HasError}"
                   HorizontalTextAlignment="Center" />

            <!-- Numpad -->
            <Grid ColumnDefinitions="*,*,*"
                  RowDefinitions="60,60,60,60"
                  ColumnSpacing="16"
                  RowSpacing="8"
                  HorizontalOptions="Center">

                <Button Grid.Row="0" Grid.Column="0" Text="1"
                        Command="{Binding EnterDigitCommand}"
                        CommandParameter="1"
                        Style="{StaticResource NumpadButton}" />
                <!-- Weitere Buttons 2-9 -->
                <Button Grid.Row="3" Grid.Column="1" Text="0"
                        Command="{Binding EnterDigitCommand}"
                        CommandParameter="0"
                        Style="{StaticResource NumpadButton}" />
                <Button Grid.Row="3" Grid.Column="2" Text="←"
                        Command="{Binding DeleteDigitCommand}"
                        Style="{StaticResource NumpadButton}" />
            </Grid>

            <!-- Eltern-Login Link -->
            <Label Text="Eltern-Login"
                   TextColor="{StaticResource Primary}"
                   TextDecorations="Underline"
                   HorizontalTextAlignment="Center"
                   Margin="0,16,0,24">
                <Label.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding NavigateToParentLoginCommand}" />
                </Label.GestureRecognizers>
            </Label>

        </VerticalStackLayout>

    </Grid>

</ContentPage>
```

### ChildLoginViewModel.cs
```csharp
public partial class ChildLoginViewModel : ObservableObject
{
    private readonly IAuthApi _authApi;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _familyCode = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ChildViewModel> _children = new();

    [ObservableProperty]
    private ChildViewModel? _selectedChild;

    [ObservableProperty]
    private string _pin = string.Empty;

    public bool Pin1Filled => Pin.Length >= 1;
    public bool Pin2Filled => Pin.Length >= 2;
    public bool Pin3Filled => Pin.Length >= 3;
    public bool Pin4Filled => Pin.Length >= 4;

    public bool HasChildren => Children.Any();

    [RelayCommand]
    private void EnterDigit(string digit)
    {
        if (Pin.Length < 4)
        {
            Pin += digit;
            OnPropertyChanged(nameof(Pin1Filled));
            OnPropertyChanged(nameof(Pin2Filled));
            OnPropertyChanged(nameof(Pin3Filled));
            OnPropertyChanged(nameof(Pin4Filled));

            if (Pin.Length == 4)
            {
                _ = LoginAsync();
            }
        }
    }

    [RelayCommand]
    private void DeleteDigit()
    {
        if (Pin.Length > 0)
        {
            Pin = Pin[..^1];
            OnPropertyChanged(nameof(Pin1Filled));
            OnPropertyChanged(nameof(Pin2Filled));
            OnPropertyChanged(nameof(Pin3Filled));
            OnPropertyChanged(nameof(Pin4Filled));
        }
    }

    private async Task LoginAsync()
    {
        if (SelectedChild == null) return;

        var response = await _authApi.ChildLoginAsync(new ChildLoginRequest
        {
            FamilyCode = FamilyCode,
            Nickname = SelectedChild.Nickname,
            Pin = Pin
        });

        if (response.IsSuccessStatusCode)
        {
            await _navigationService.NavigateToAsync("//main/child-dashboard");
        }
        else
        {
            ErrorMessage = "Hoppla! Die PIN ist falsch. Versuch es nochmal!";
            Pin = string.Empty;
        }
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-10 | Familien-Code eingeben | Kinder werden geladen |
| TC-M002-11 | Kind auswählen | Kind ist markiert |
| TC-M002-12 | Korrekte PIN eingeben | Login erfolgreich |
| TC-M002-13 | Falsche PIN eingeben | Kindgerechte Fehlermeldung |
| TC-M002-14 | PIN löschen | Letzte Ziffer entfernt |

## Story Points
3

## Priorität
Hoch
