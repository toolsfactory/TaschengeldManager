# Story M004-S02: Transaktionsliste mit Filterung

## Epic
M004 - Kontoverwaltung Kind

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **alle meine Transaktionen sehen und filtern können**, damit **ich nachvollziehen kann, wofür ich mein Geld ausgegeben habe und woher es kam**.

## Akzeptanzkriterien

- [ ] Gegeben die Transaktionsliste, wenn sie geladen wird, dann werden alle Transaktionen chronologisch angezeigt
- [ ] Gegeben die Filter, wenn nach Kategorie gefiltert wird, dann werden nur entsprechende Transaktionen angezeigt
- [ ] Gegeben die Filter, wenn nach Zeitraum gefiltert wird, dann werden nur Transaktionen im Zeitraum angezeigt
- [ ] Gegeben eine Transaktion, wenn sie angeklickt wird, dann wird die Detailansicht geöffnet

## UI-Entwurf

```
┌─────────────────────────────┐
│  Transaktionen      [Filter]│
├─────────────────────────────┤
│  [Alle] [Einnahmen] [Ausgab]│
├─────────────────────────────┤
│                             │
│  Heute                      │
│  ┌─────────────────────────┐│
│  │ 🍭 Süßigkeiten  -2,50 € ││
│  │    14:30 Uhr            ││
│  └─────────────────────────┘│
│                             │
│  Gestern                    │
│  ┌─────────────────────────┐│
│  │ 💰 Taschengeld +10,00 € ││
│  │    08:00 Uhr            ││
│  └─────────────────────────┘│
│                             │
│  Montag, 18.03.             │
│  ┌─────────────────────────┐│
│  │ 🎁 Von Oma     +20,00 € ││
│  │    12:15 Uhr            ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 🎮 Spiel       -15,00 € ││
│  │    16:45 Uhr            ││
│  └─────────────────────────┘│
│                             │
│  [Mehr laden...]            │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### TransactionsPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="TaschengeldManager.Mobile.Views.TransactionsPage"
             x:DataType="vm:TransactionsViewModel"
             Title="Transaktionen">

    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Filter"
                     Command="{Binding ShowFilterCommand}"
                     IconImageSource="filter.png" />
    </ContentPage.ToolbarItems>

    <Grid RowDefinitions="Auto,*">

        <!-- Schnellfilter -->
        <HorizontalStackLayout Grid.Row="0"
                               Spacing="8"
                               Padding="16,8">
            <Button Text="Alle"
                    Command="{Binding SetFilterCommand}"
                    CommandParameter="All"
                    Style="{Binding AllFilterStyle}" />
            <Button Text="Einnahmen"
                    Command="{Binding SetFilterCommand}"
                    CommandParameter="Income"
                    Style="{Binding IncomeFilterStyle}" />
            <Button Text="Ausgaben"
                    Command="{Binding SetFilterCommand}"
                    CommandParameter="Expense"
                    Style="{Binding ExpenseFilterStyle}" />
        </HorizontalStackLayout>

        <!-- Transaktionsliste -->
        <RefreshView Grid.Row="1"
                     IsRefreshing="{Binding IsRefreshing}"
                     Command="{Binding RefreshCommand}">
            <CollectionView ItemsSource="{Binding GroupedTransactions}"
                            IsGrouped="True"
                            RemainingItemsThreshold="5"
                            RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}"
                            SelectionMode="Single"
                            SelectedItem="{Binding SelectedTransaction}"
                            SelectionChangedCommand="{Binding TransactionSelectedCommand}">

                <!-- Gruppen-Header (Datum) -->
                <CollectionView.GroupHeaderTemplate>
                    <DataTemplate x:DataType="vm:TransactionGroup">
                        <Label Text="{Binding DateHeader}"
                               FontSize="14"
                               FontAttributes="Bold"
                               TextColor="{StaticResource TextSecondaryLight}"
                               Padding="16,16,16,8" />
                    </DataTemplate>
                </CollectionView.GroupHeaderTemplate>

                <!-- Transaktions-Item -->
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="vm:TransactionItemViewModel">
                        <Frame Padding="12"
                               Margin="16,0,16,8"
                               CornerRadius="8">
                            <Grid ColumnDefinitions="Auto,*,Auto">
                                <!-- Kategorie-Icon -->
                                <Frame WidthRequest="44"
                                       HeightRequest="44"
                                       CornerRadius="22"
                                       Padding="0"
                                       BackgroundColor="{Binding CategoryBackgroundColor}">
                                    <Label Text="{Binding CategoryEmoji}"
                                           FontSize="20"
                                           HorizontalOptions="Center"
                                           VerticalOptions="Center" />
                                </Frame>

                                <!-- Beschreibung und Zeit -->
                                <VerticalStackLayout Grid.Column="1"
                                                     Margin="12,0"
                                                     VerticalOptions="Center">
                                    <Label Text="{Binding Description}"
                                           FontSize="14"
                                           FontAttributes="Bold"
                                           LineBreakMode="TailTruncation" />
                                    <Label Text="{Binding TimeText}"
                                           FontSize="12"
                                           TextColor="{StaticResource TextSecondaryLight}" />
                                </VerticalStackLayout>

                                <!-- Betrag -->
                                <Label Grid.Column="2"
                                       Text="{Binding AmountText}"
                                       TextColor="{Binding AmountColor}"
                                       FontSize="16"
                                       FontAttributes="Bold"
                                       VerticalOptions="Center" />
                            </Grid>
                        </Frame>
                    </DataTemplate>
                </CollectionView.ItemTemplate>

                <!-- Empty View -->
                <CollectionView.EmptyView>
                    <views:EmptyStateView Emoji="📭"
                                           Title="Keine Transaktionen"
                                           Description="Hier werden deine Ein- und Ausgaben angezeigt." />
                </CollectionView.EmptyView>

                <!-- Footer für Loading -->
                <CollectionView.Footer>
                    <ActivityIndicator IsRunning="{Binding IsLoadingMore}"
                                       IsVisible="{Binding IsLoadingMore}"
                                       Margin="0,16" />
                </CollectionView.Footer>

            </CollectionView>
        </RefreshView>

    </Grid>

</ContentPage>
```

### TransactionsViewModel.cs
```csharp
public partial class TransactionsViewModel : BaseViewModel
{
    private readonly IAccountApi _accountApi;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<TransactionGroup> _groupedTransactions = new();

    [ObservableProperty]
    private TransactionItemViewModel? _selectedTransaction;

    [ObservableProperty]
    private string _currentFilter = "All";

    [ObservableProperty]
    private bool _isLoadingMore;

    private int _currentPage = 1;
    private bool _hasMoreItems = true;

    public Style AllFilterStyle => GetFilterStyle("All");
    public Style IncomeFilterStyle => GetFilterStyle("Income");
    public Style ExpenseFilterStyle => GetFilterStyle("Expense");

    private Style GetFilterStyle(string filter) =>
        CurrentFilter == filter
            ? (Style)Application.Current!.Resources["FilterButtonActiveStyle"]
            : (Style)Application.Current!.Resources["FilterButtonStyle"];

    partial void OnCurrentFilterChanged(string value)
    {
        OnPropertyChanged(nameof(AllFilterStyle));
        OnPropertyChanged(nameof(IncomeFilterStyle));
        OnPropertyChanged(nameof(ExpenseFilterStyle));

        // Neu laden mit Filter
        _currentPage = 1;
        _hasMoreItems = true;
        _ = LoadDataAsync();
    }

    protected override async Task LoadDataAsync()
    {
        var response = await _accountApi.GetTransactionsAsync(
            page: _currentPage,
            pageSize: 20,
            filter: CurrentFilter == "All" ? null : CurrentFilter);

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var transactions = response.Content.Items
                .Select(t => new TransactionItemViewModel(t))
                .ToList();

            // Nach Datum gruppieren
            var grouped = transactions
                .GroupBy(t => t.Date.Date)
                .Select(g => new TransactionGroup(g.Key, g.ToList()))
                .ToList();

            if (_currentPage == 1)
            {
                GroupedTransactions = new ObservableCollection<TransactionGroup>(grouped);
            }
            else
            {
                foreach (var group in grouped)
                {
                    var existingGroup = GroupedTransactions
                        .FirstOrDefault(g => g.Date == group.Date);

                    if (existingGroup != null)
                    {
                        foreach (var item in group)
                        {
                            existingGroup.Add(item);
                        }
                    }
                    else
                    {
                        GroupedTransactions.Add(group);
                    }
                }
            }

            _hasMoreItems = response.Content.HasMore;
        }
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsLoadingMore || !_hasMoreItems)
            return;

        IsLoadingMore = true;
        _currentPage++;
        await LoadDataAsync();
        IsLoadingMore = false;
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        CurrentFilter = filter;
    }

    [RelayCommand]
    private async Task TransactionSelectedAsync()
    {
        if (SelectedTransaction != null)
        {
            await _navigationService.NavigateToAsync("transaction-detail",
                new Dictionary<string, object> { { "TransactionId", SelectedTransaction.Id } });
            SelectedTransaction = null;
        }
    }

    [RelayCommand]
    private async Task ShowFilterAsync()
    {
        // Filter-Popup öffnen
        var popup = new TransactionFilterPopup(new TransactionFilterViewModel());
        await Shell.Current.ShowPopupAsync(popup);
    }
}

public class TransactionGroup : ObservableCollection<TransactionItemViewModel>
{
    public DateTime Date { get; }

    public string DateHeader
    {
        get
        {
            if (Date == DateTime.Today)
                return "Heute";
            if (Date == DateTime.Today.AddDays(-1))
                return "Gestern";
            if (Date > DateTime.Today.AddDays(-7))
                return Date.ToString("dddd, dd.MM.");
            return Date.ToString("dd. MMMM yyyy");
        }
    }

    public TransactionGroup(DateTime date, IEnumerable<TransactionItemViewModel> items)
        : base(items)
    {
        Date = date;
    }
}
```

### TransactionItemViewModel.cs
```csharp
public class TransactionItemViewModel
{
    public Guid Id { get; }
    public string Description { get; }
    public decimal Amount { get; }
    public DateTime Date { get; }
    public string CategoryEmoji { get; }
    public Color CategoryBackgroundColor { get; }

    public string AmountText => Amount >= 0
        ? $"+{Amount:C}"
        : Amount.ToString("C");

    public Color AmountColor => Amount >= 0
        ? Colors.Green
        : Colors.Red;

    public string TimeText => Date.ToString("HH:mm") + " Uhr";

    public TransactionItemViewModel(TransactionResponse response)
    {
        Id = response.Id;
        Description = response.Description;
        Amount = response.Amount;
        Date = response.Date;
        CategoryEmoji = response.Category?.Emoji ?? "💰";
        CategoryBackgroundColor = Color.FromArgb(response.Category?.Color ?? "#E0E0E0");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-05 | Liste laden | Transaktionen gruppiert nach Datum |
| TC-M004-06 | Nach Einnahmen filtern | Nur Einnahmen angezeigt |
| TC-M004-07 | Nach Ausgaben filtern | Nur Ausgaben angezeigt |
| TC-M004-08 | Transaktion auswählen | Navigation zur Detailansicht |
| TC-M004-09 | Mehr laden | Weitere Transaktionen laden |

## Story Points
3

## Priorität
Hoch
