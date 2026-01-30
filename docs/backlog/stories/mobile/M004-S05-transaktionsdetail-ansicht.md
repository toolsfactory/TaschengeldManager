# Story M004-S05: Transaktionsdetail-Ansicht

## Epic
M004 - Kontoverwaltung Kind

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **die Details einer Transaktion einsehen können**, damit **ich genau weiß, wann und wofür ich Geld ausgegeben oder bekommen habe**.

## Akzeptanzkriterien

- [ ] Gegeben eine Transaktion, wenn sie ausgewählt wird, dann werden alle Details angezeigt
- [ ] Gegeben die Details, wenn sie angezeigt werden, dann sind Betrag, Datum, Kategorie und Beschreibung sichtbar
- [ ] Gegeben eine Ausgabe, wenn sie vom Kind erstellt wurde, dann kann sie gelöscht werden
- [ ] Gegeben ein Geschenk, wenn es angezeigt wird, dann ist der Absender sichtbar

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück                   │
├─────────────────────────────┤
│                             │
│        [Kategorie-Icon]     │
│             🍭              │
│                             │
│       Süßigkeiten           │
│                             │
│         -2,50 €             │
│                             │
├─────────────────────────────┤
│                             │
│  Datum                      │
│  Dienstag, 19. März 2024    │
│  14:30 Uhr                  │
│                             │
│  ─────────────────────────  │
│                             │
│  Kategorie                  │
│  🍭 Süßigkeiten             │
│                             │
│  ─────────────────────────  │
│                             │
│  Beschreibung               │
│  Gummibärchen beim Kiosk    │
│                             │
│  ─────────────────────────  │
│                             │
│  Kontostand danach          │
│  42,50 €                    │
│                             │
├─────────────────────────────┤
│                             │
│  ┌───────────────────────┐  │
│  │      Löschen          │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### TransactionDetailPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.TransactionDetailPage"
             x:DataType="vm:TransactionDetailViewModel"
             Title="Details">

    <ScrollView>
        <VerticalStackLayout>

            <!-- Header mit Icon und Betrag -->
            <Frame Padding="32"
                   BackgroundColor="{Binding HeaderBackgroundColor}"
                   CornerRadius="0">
                <VerticalStackLayout HorizontalOptions="Center" Spacing="8">
                    <!-- Kategorie Icon -->
                    <Frame WidthRequest="80"
                           HeightRequest="80"
                           CornerRadius="40"
                           Padding="0"
                           BackgroundColor="White"
                           HorizontalOptions="Center">
                        <Label Text="{Binding CategoryEmoji}"
                               FontSize="40"
                               HorizontalOptions="Center"
                               VerticalOptions="Center" />
                    </Frame>

                    <!-- Beschreibung -->
                    <Label Text="{Binding Description}"
                           FontSize="18"
                           TextColor="White"
                           HorizontalTextAlignment="Center" />

                    <!-- Betrag -->
                    <Label Text="{Binding AmountText}"
                           FontSize="36"
                           FontAttributes="Bold"
                           TextColor="White"
                           HorizontalTextAlignment="Center" />

                    <!-- Geschenk-Absender -->
                    <HorizontalStackLayout HorizontalOptions="Center"
                                           Spacing="4"
                                           IsVisible="{Binding IsGift}">
                        <Label Text="von"
                               FontSize="14"
                               TextColor="White"
                               Opacity="0.8" />
                        <Label Text="{Binding GiftSenderName}"
                               FontSize="14"
                               FontAttributes="Bold"
                               TextColor="White" />
                    </HorizontalStackLayout>
                </VerticalStackLayout>
            </Frame>

            <!-- Detail-Informationen -->
            <VerticalStackLayout Padding="16" Spacing="0">

                <!-- Datum -->
                <Frame Padding="16" BackgroundColor="Transparent">
                    <VerticalStackLayout>
                        <Label Text="Datum"
                               FontSize="12"
                               TextColor="{StaticResource TextSecondaryLight}" />
                        <Label Text="{Binding DateText}"
                               FontSize="16" />
                        <Label Text="{Binding TimeText}"
                               FontSize="14"
                               TextColor="{StaticResource TextSecondaryLight}" />
                    </VerticalStackLayout>
                </Frame>
                <BoxView HeightRequest="1"
                         Color="{StaticResource Divider}"
                         Margin="16,0" />

                <!-- Kategorie -->
                <Frame Padding="16" BackgroundColor="Transparent">
                    <VerticalStackLayout>
                        <Label Text="Kategorie"
                               FontSize="12"
                               TextColor="{StaticResource TextSecondaryLight}" />
                        <HorizontalStackLayout Spacing="8">
                            <Label Text="{Binding CategoryEmoji}"
                                   FontSize="20"
                                   VerticalOptions="Center" />
                            <Label Text="{Binding CategoryName}"
                                   FontSize="16"
                                   VerticalOptions="Center" />
                        </HorizontalStackLayout>
                    </VerticalStackLayout>
                </Frame>
                <BoxView HeightRequest="1"
                         Color="{StaticResource Divider}"
                         Margin="16,0" />

                <!-- Notiz/Beschreibung (falls vorhanden) -->
                <Frame Padding="16"
                       BackgroundColor="Transparent"
                       IsVisible="{Binding HasNote}">
                    <VerticalStackLayout>
                        <Label Text="Notiz"
                               FontSize="12"
                               TextColor="{StaticResource TextSecondaryLight}" />
                        <Label Text="{Binding Note}"
                               FontSize="16" />
                    </VerticalStackLayout>
                </Frame>
                <BoxView HeightRequest="1"
                         Color="{StaticResource Divider}"
                         Margin="16,0"
                         IsVisible="{Binding HasNote}" />

                <!-- Kontostand danach -->
                <Frame Padding="16" BackgroundColor="Transparent">
                    <VerticalStackLayout>
                        <Label Text="Kontostand danach"
                               FontSize="12"
                               TextColor="{StaticResource TextSecondaryLight}" />
                        <Label Text="{Binding BalanceAfterText}"
                               FontSize="16"
                               FontAttributes="Bold" />
                    </VerticalStackLayout>
                </Frame>

                <!-- Geschenk-Nachricht -->
                <Frame Padding="16"
                       BackgroundColor="{StaticResource PrimaryLight}"
                       CornerRadius="12"
                       Margin="16"
                       IsVisible="{Binding HasGiftMessage}">
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Nachricht"
                               FontSize="12"
                               TextColor="{StaticResource Primary}" />
                        <Label Text="{Binding GiftMessage}"
                               FontSize="14"
                               FontAttributes="Italic" />
                    </VerticalStackLayout>
                </Frame>

            </VerticalStackLayout>

            <!-- Löschen Button (nur für eigene Ausgaben) -->
            <Button Text="Transaktion löschen"
                    Command="{Binding DeleteCommand}"
                    Style="{StaticResource DangerButton}"
                    Margin="16"
                    IsVisible="{Binding CanDelete}" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### TransactionDetailViewModel.cs
```csharp
public partial class TransactionDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly IAccountApi _accountApi;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Guid _transactionId;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private string _categoryEmoji = string.Empty;

    [ObservableProperty]
    private string _categoryName = string.Empty;

    [ObservableProperty]
    private string? _note;

    [ObservableProperty]
    private decimal _balanceAfter;

    [ObservableProperty]
    private bool _isGift;

    [ObservableProperty]
    private string? _giftSenderName;

    [ObservableProperty]
    private string? _giftMessage;

    [ObservableProperty]
    private bool _canDelete;

    public string AmountText => Amount >= 0
        ? $"+{Amount:C}"
        : Amount.ToString("C");

    public Color HeaderBackgroundColor => Amount >= 0
        ? Color.FromArgb("#4CAF50") // Green
        : Color.FromArgb("#F44336"); // Red

    public string DateText => Date.ToString("dddd, d. MMMM yyyy");
    public string TimeText => Date.ToString("HH:mm") + " Uhr";
    public string BalanceAfterText => BalanceAfter.ToString("C");

    public bool HasNote => !string.IsNullOrWhiteSpace(Note);
    public bool HasGiftMessage => !string.IsNullOrWhiteSpace(GiftMessage);

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("TransactionId", out var id))
        {
            TransactionId = (Guid)id;
            _ = LoadTransactionAsync();
        }
    }

    private async Task LoadTransactionAsync()
    {
        var response = await _accountApi.GetTransactionAsync(TransactionId);
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var transaction = response.Content;

            Description = transaction.Description;
            Amount = transaction.Amount;
            Date = transaction.Date;
            CategoryEmoji = transaction.Category?.Emoji ?? "📦";
            CategoryName = transaction.Category?.Name ?? "Sonstiges";
            Note = transaction.Note;
            BalanceAfter = transaction.BalanceAfter;

            // Geschenk-Informationen
            IsGift = transaction.Type == "Gift";
            GiftSenderName = transaction.GiftSender?.Name;
            GiftMessage = transaction.GiftMessage;

            // Löschbar nur wenn eigene Ausgabe (nicht Taschengeld, Zinsen, Geschenke)
            CanDelete = transaction.Type == "Expense" && transaction.CreatedByChild;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Transaktion löschen",
            "Möchtest du diese Transaktion wirklich löschen?",
            "Löschen",
            "Abbrechen");

        if (!confirm) return;

        var response = await _accountApi.DeleteTransactionAsync(TransactionId);
        if (response.IsSuccessStatusCode)
        {
            WeakReferenceMessenger.Default.Send(new TransactionDeletedMessage(TransactionId));
            await _navigationService.GoBackAsync();
            await Toast.Make("Transaktion gelöscht").Show();
        }
        else
        {
            await Shell.Current.DisplayAlert(
                "Fehler",
                "Die Transaktion konnte nicht gelöscht werden.",
                "OK");
        }
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-19 | Detail-Seite öffnen | Alle Infos angezeigt |
| TC-M004-20 | Ausgabe anzeigen | Roter Header, negativer Betrag |
| TC-M004-21 | Geschenk anzeigen | Absender und Nachricht sichtbar |
| TC-M004-22 | Eigene Ausgabe löschen | Transaktion entfernt |
| TC-M004-23 | Taschengeld anzeigen | Kein Löschen-Button |

## Story Points
2

## Priorität
Mittel
