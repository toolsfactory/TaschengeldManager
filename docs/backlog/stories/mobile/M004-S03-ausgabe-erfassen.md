# Story M004-S03: Ausgabe erfassen

## Epic
M004 - Kontoverwaltung Kind

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **eine Ausgabe schnell und einfach erfassen können**, damit **mein Kontostand immer aktuell ist und ich sehe, wofür ich Geld ausgegeben habe**.

## Akzeptanzkriterien

- [ ] Gegeben die Ausgabe-Erfassung, wenn sie geöffnet wird, dann kann ein Betrag eingegeben werden
- [ ] Gegeben der Betrag, wenn er eingegeben wird, dann kann eine Kategorie ausgewählt werden
- [ ] Gegeben die Eingabe, wenn sie gespeichert wird, dann wird der Kontostand aktualisiert
- [ ] Gegeben ein zu hoher Betrag, wenn er eingegeben wird, dann wird eine Warnung angezeigt

## UI-Entwurf

```
┌─────────────────────────────┐
│  × Ausgabe erfassen     [✓] │
├─────────────────────────────┤
│                             │
│  Wie viel hast du           │
│  ausgegeben?                │
│                             │
│       12,50 €               │
│       -------               │
│                             │
│  Wofür?                     │
│  ┌─────────────────────────┐│
│  │ Beschreibung (optional) ││
│  └─────────────────────────┘│
│                             │
│  Kategorie                  │
│  [🍭] [🎮] [📚] [🎬]       │
│  [🚌] [🎁] [🍔] [...]      │
│                             │
│  Aktueller Kontostand:      │
│  45,00 € → 32,50 €          │
│                             │
│  ┌───────────────────────┐  │
│  │     Ausgabe speichern │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### AddExpensePage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="TaschengeldManager.Mobile.Views.AddExpensePage"
             x:DataType="vm:AddExpenseViewModel"
             Title="Ausgabe erfassen"
             Shell.PresentationMode="ModalAnimated">

    <ContentPage.ToolbarItems>
        <ToolbarItem Text="×"
                     Command="{Binding CancelCommand}"
                     Order="Primary" />
        <ToolbarItem Text="✓"
                     Command="{Binding SaveCommand}"
                     Order="Primary" />
    </ContentPage.ToolbarItems>

    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="24">

            <!-- Betrags-Eingabe -->
            <VerticalStackLayout Spacing="8">
                <Label Text="Wie viel hast du ausgegeben?"
                       FontSize="16"
                       FontAttributes="Bold"
                       HorizontalTextAlignment="Center" />

                <Grid HorizontalOptions="Center">
                    <Entry x:Name="AmountEntry"
                           Text="{Binding AmountText}"
                           Keyboard="Numeric"
                           FontSize="48"
                           FontAttributes="Bold"
                           HorizontalTextAlignment="Center"
                           Placeholder="0,00"
                           WidthRequest="200"
                           MaxLength="10" />
                    <Label Text="€"
                           FontSize="32"
                           VerticalOptions="Center"
                           HorizontalOptions="End"
                           Margin="0,0,-40,0" />
                </Grid>

                <!-- Warnung bei zu hohem Betrag -->
                <Frame IsVisible="{Binding ShowInsufficientFundsWarning}"
                       BackgroundColor="{StaticResource WarningLight}"
                       Padding="12"
                       CornerRadius="8">
                    <Label Text="⚠️ Der Betrag ist höher als dein Kontostand!"
                           FontSize="12"
                           TextColor="{StaticResource Warning}"
                           HorizontalTextAlignment="Center" />
                </Frame>
            </VerticalStackLayout>

            <!-- Beschreibung -->
            <VerticalStackLayout Spacing="8">
                <Label Text="Wofür?"
                       FontSize="16"
                       FontAttributes="Bold" />
                <Entry Placeholder="Beschreibung (optional)"
                       Text="{Binding Description}"
                       MaxLength="100" />
            </VerticalStackLayout>

            <!-- Kategorie-Auswahl -->
            <VerticalStackLayout Spacing="8">
                <Label Text="Kategorie"
                       FontSize="16"
                       FontAttributes="Bold" />

                <FlexLayout Wrap="Wrap"
                            JustifyContent="Start"
                            BindableLayout.ItemsSource="{Binding Categories}">
                    <BindableLayout.ItemTemplate>
                        <DataTemplate x:DataType="vm:CategoryViewModel">
                            <Frame Padding="12"
                                   Margin="4"
                                   CornerRadius="12"
                                   BorderColor="{Binding BorderColor}"
                                   BackgroundColor="{Binding BackgroundColor}"
                                   WidthRequest="60"
                                   HeightRequest="60">
                                <Frame.GestureRecognizers>
                                    <TapGestureRecognizer
                                        Command="{Binding Source={RelativeSource AncestorType={x:Type vm:AddExpenseViewModel}}, Path=SelectCategoryCommand}"
                                        CommandParameter="{Binding Id}" />
                                </Frame.GestureRecognizers>
                                <VerticalStackLayout HorizontalOptions="Center"
                                                     VerticalOptions="Center">
                                    <Label Text="{Binding Emoji}"
                                           FontSize="24"
                                           HorizontalTextAlignment="Center" />
                                    <Label Text="{Binding Name}"
                                           FontSize="8"
                                           HorizontalTextAlignment="Center"
                                           LineBreakMode="TailTruncation" />
                                </VerticalStackLayout>
                            </Frame>
                        </DataTemplate>
                    </BindableLayout.ItemTemplate>
                </FlexLayout>
            </VerticalStackLayout>

            <!-- Kontostand-Vorschau -->
            <Frame Padding="16"
                   CornerRadius="12"
                   BackgroundColor="{StaticResource SurfaceLight}">
                <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                    <Label Text="Aktueller Kontostand:"
                           FontSize="14"
                           VerticalOptions="Center" />
                    <Label Text="{Binding CurrentBalance, StringFormat='{0:C}'}"
                           FontSize="14"
                           FontAttributes="Bold"
                           VerticalOptions="Center" />
                    <Label Text="→"
                           FontSize="14"
                           VerticalOptions="Center" />
                    <Label Text="{Binding NewBalance, StringFormat='{0:C}'}"
                           FontSize="14"
                           FontAttributes="Bold"
                           TextColor="{Binding NewBalanceColor}"
                           VerticalOptions="Center" />
                </HorizontalStackLayout>
            </Frame>

            <!-- Speichern Button -->
            <Button Text="Ausgabe speichern"
                    Command="{Binding SaveCommand}"
                    Style="{StaticResource PrimaryButton}"
                    IsEnabled="{Binding CanSave}" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### AddExpenseViewModel.cs
```csharp
public partial class AddExpenseViewModel : ObservableObject
{
    private readonly IAccountApi _accountApi;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Amount))]
    [NotifyPropertyChangedFor(nameof(NewBalance))]
    [NotifyPropertyChangedFor(nameof(ShowInsufficientFundsWarning))]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    private string _amountText = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private ObservableCollection<CategoryViewModel> _categories = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    private Guid? _selectedCategoryId;

    [ObservableProperty]
    private decimal _currentBalance;

    [ObservableProperty]
    private bool _isBusy;

    public decimal Amount
    {
        get
        {
            if (decimal.TryParse(AmountText.Replace(",", "."),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
            {
                return amount;
            }
            return 0;
        }
    }

    public decimal NewBalance => CurrentBalance - Amount;

    public Color NewBalanceColor => NewBalance >= 0
        ? Colors.Green
        : Colors.Red;

    public bool ShowInsufficientFundsWarning => Amount > CurrentBalance;

    public bool CanSave =>
        Amount > 0 &&
        SelectedCategoryId.HasValue &&
        !IsBusy;

    public async Task InitializeAsync()
    {
        // Aktuellen Kontostand laden
        var accountResponse = await _accountApi.GetAccountAsync();
        if (accountResponse.IsSuccessStatusCode && accountResponse.Content != null)
        {
            CurrentBalance = accountResponse.Content.Balance;
        }

        // Kategorien laden
        var categoriesResponse = await _accountApi.GetExpenseCategoriesAsync();
        if (categoriesResponse.IsSuccessStatusCode && categoriesResponse.Content != null)
        {
            Categories = new ObservableCollection<CategoryViewModel>(
                categoriesResponse.Content.Select(c => new CategoryViewModel(c)));
        }
    }

    [RelayCommand]
    private void SelectCategory(Guid categoryId)
    {
        // Alte Auswahl zurücksetzen
        foreach (var cat in Categories)
        {
            cat.IsSelected = cat.Id == categoryId;
        }

        SelectedCategoryId = categoryId;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!CanSave) return;

        try
        {
            IsBusy = true;

            var response = await _accountApi.CreateTransactionAsync(new CreateTransactionRequest
            {
                Amount = -Amount, // Negativ für Ausgabe
                Description = string.IsNullOrWhiteSpace(Description)
                    ? Categories.First(c => c.Id == SelectedCategoryId).Name
                    : Description,
                CategoryId = SelectedCategoryId!.Value,
                Date = DateTime.Now
            });

            if (response.IsSuccessStatusCode)
            {
                // Messenger für Dashboard-Update
                WeakReferenceMessenger.Default.Send(new TransactionCreatedMessage());

                await _navigationService.GoBackAsync();

                // Erfolgs-Toast
                await Toast.Make("Ausgabe gespeichert!", ToastDuration.Short).Show();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Fehler",
                    "Die Ausgabe konnte nicht gespeichert werden.",
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.GoBackAsync();
    }
}
```

### Kategorie-ViewModel
```csharp
public partial class CategoryViewModel : ObservableObject
{
    public Guid Id { get; }
    public string Name { get; }
    public string Emoji { get; }
    public string ColorHex { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BorderColor))]
    [NotifyPropertyChangedFor(nameof(BackgroundColor))]
    private bool _isSelected;

    public Color BorderColor => IsSelected
        ? Color.FromArgb("#4CAF50") // Primary
        : Colors.Transparent;

    public Color BackgroundColor => IsSelected
        ? Color.FromArgb("#E8F5E9") // Primary Light
        : Color.FromArgb(ColorHex);

    public CategoryViewModel(CategoryResponse response)
    {
        Id = response.Id;
        Name = response.Name;
        Emoji = response.Emoji;
        ColorHex = response.Color;
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-10 | Betrag eingeben | Kontostand-Vorschau aktualisiert |
| TC-M004-11 | Kategorie auswählen | Kategorie markiert |
| TC-M004-12 | Ausgabe speichern | Transaktion erstellt, zurück navigiert |
| TC-M004-13 | Betrag > Kontostand | Warnung angezeigt |
| TC-M004-14 | Ohne Kategorie speichern | Button deaktiviert |

## Story Points
3

## Priorität
Hoch
