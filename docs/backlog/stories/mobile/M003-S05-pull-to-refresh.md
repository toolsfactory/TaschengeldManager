# Story M003-S05: Pull-to-Refresh

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **durch Herunterziehen des Bildschirms die Daten aktualisieren können**, damit **ich immer die aktuellsten Informationen sehe**.

## Akzeptanzkriterien

- [ ] Gegeben eine Liste oder ein Dashboard, wenn der Benutzer nach unten zieht, dann wird ein Refresh-Indikator angezeigt
- [ ] Gegeben ein aktiver Refresh, wenn die Daten geladen werden, dann dreht sich der Indikator
- [ ] Gegeben ein abgeschlossener Refresh, wenn die Daten geladen sind, dann verschwindet der Indikator
- [ ] Gegeben ein Fehler beim Refresh, wenn er auftritt, dann wird eine Fehlermeldung angezeigt

## UI-Entwurf

```
┌─────────────────────────────┐
│  ↓ Zum Aktualisieren ziehen │
├─────────────────────────────┤
│                             │
│         [⟳ Loading]         │
│                             │
│  [Content]                  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### BaseViewModel mit Refresh-Support
```csharp
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotRefreshing))]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool IsNotRefreshing => !IsRefreshing;

    [RelayCommand]
    protected virtual async Task RefreshAsync()
    {
        if (IsRefreshing) return;

        try
        {
            IsRefreshing = true;
            HasError = false;
            ErrorMessage = string.Empty;

            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Aktualisierung fehlgeschlagen. Bitte versuche es erneut.";
            Debug.WriteLine($"Refresh error: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    protected abstract Task LoadDataAsync();
}
```

### RefreshView Verwendung
```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TaschengeldManager.Mobile.Views.DashboardPage">

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding RefreshCommand}"
                 RefreshColor="{StaticResource Primary}">

        <ScrollView>
            <VerticalStackLayout>
                <!-- Content -->

                <!-- Fehleranzeige -->
                <Frame IsVisible="{Binding HasError}"
                       BackgroundColor="{StaticResource ErrorLight}"
                       Padding="12"
                       CornerRadius="8"
                       Margin="16">
                    <HorizontalStackLayout Spacing="8">
                        <Label Text="⚠️" FontSize="18" />
                        <Label Text="{Binding ErrorMessage}"
                               VerticalOptions="Center"
                               TextColor="{StaticResource Error}" />
                        <Button Text="Erneut"
                                Command="{Binding RefreshCommand}"
                                Style="{StaticResource SmallPrimaryButton}"
                                HorizontalOptions="EndAndExpand" />
                    </HorizontalStackLayout>
                </Frame>

            </VerticalStackLayout>
        </ScrollView>

    </RefreshView>

</ContentPage>
```

### Implementierung in konkretem ViewModel
```csharp
public partial class TransactionsViewModel : BaseViewModel
{
    private readonly IAccountApi _accountApi;

    [ObservableProperty]
    private ObservableCollection<TransactionViewModel> _transactions = new();

    public TransactionsViewModel(IAccountApi accountApi)
    {
        _accountApi = accountApi;
    }

    protected override async Task LoadDataAsync()
    {
        var response = await _accountApi.GetTransactionsAsync();

        if (response.IsSuccessStatusCode && response.Content != null)
        {
            Transactions = new ObservableCollection<TransactionViewModel>(
                response.Content.Select(t => new TransactionViewModel(t)));
        }
        else
        {
            throw new Exception("Daten konnten nicht geladen werden.");
        }
    }
}
```

### Pull-to-Refresh mit CollectionView
```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}"
             Command="{Binding RefreshCommand}">

    <CollectionView ItemsSource="{Binding Transactions}"
                    RemainingItemsThreshold="5"
                    RemainingItemsThresholdReachedCommand="{Binding LoadMoreCommand}">
        <CollectionView.ItemTemplate>
            <!-- Template -->
        </CollectionView.ItemTemplate>

        <CollectionView.EmptyView>
            <VerticalStackLayout VerticalOptions="Center"
                                 HorizontalOptions="Center"
                                 Padding="24">
                <Label Text="📭" FontSize="48" HorizontalTextAlignment="Center" />
                <Label Text="Keine Einträge vorhanden"
                       HorizontalTextAlignment="Center" />
                <Button Text="Aktualisieren"
                        Command="{Binding RefreshCommand}"
                        Style="{StaticResource SecondaryButton}"
                        Margin="0,16,0,0" />
            </VerticalStackLayout>
        </CollectionView.EmptyView>

        <!-- Loading Footer für Infinite Scroll -->
        <CollectionView.Footer>
            <ActivityIndicator IsRunning="{Binding IsLoadingMore}"
                               IsVisible="{Binding IsLoadingMore}"
                               Margin="0,16" />
        </CollectionView.Footer>
    </CollectionView>

</RefreshView>
```

### Haptic Feedback (optional)
```csharp
public class RefreshHapticBehavior : Behavior<RefreshView>
{
    protected override void OnAttachedTo(RefreshView refreshView)
    {
        base.OnAttachedTo(refreshView);
        refreshView.Refreshing += OnRefreshing;
    }

    protected override void OnDetachingFrom(RefreshView refreshView)
    {
        base.OnDetachingFrom(refreshView);
        refreshView.Refreshing -= OnRefreshing;
    }

    private void OnRefreshing(object? sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-17 | Nach unten ziehen | Refresh-Indikator erscheint |
| TC-M003-18 | Daten werden geladen | Indikator dreht sich |
| TC-M003-19 | Refresh abgeschlossen | Indikator verschwindet |
| TC-M003-20 | Refresh-Fehler | Fehlermeldung angezeigt |

## Story Points
1

## Priorität
Hoch
