# Story M003-S02: Eltern-Dashboard

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Elternteil** möchte ich **auf meinem Dashboard einen Überblick über alle Kinder und deren Kontostände sehen**, damit **ich die Finanzen meiner Familie im Blick habe**.

## Akzeptanzkriterien

- [ ] Gegeben das Eltern-Dashboard, wenn es geladen wird, dann werden alle Kinder mit Kontoständen angezeigt
- [ ] Gegeben das Dashboard, wenn letzte Transaktionen vorhanden sind, dann werden sie angezeigt
- [ ] Gegeben ein Kind, wenn es angeklickt wird, dann navigiert die App zur Detailansicht
- [ ] Gegeben das Dashboard, wenn schnelle Aktionen verfügbar sind, dann können sie ausgeführt werden

## UI-Entwurf

```
┌─────────────────────────────┐
│  Hallo, Max!           [👤] │
├─────────────────────────────┤
│                             │
│  Familien-Übersicht         │
│  Gesamt: 125,50 €           │
│                             │
│  ┌─────────────────────────┐│
│  │ 🦁 Lisa      45,00 €    ││
│  │    ████████░░░░  (75%)  ││
│  │    Ziel: 60 €           ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 🐰 Tom       80,50 €    ││
│  │    ██████████████ (100%)││
│  │    Ziel: 50 €           ││
│  └─────────────────────────┘│
│                             │
│  Letzte Aktivitäten         │
│  ┌─────────────────────────┐│
│  │ ↓ Taschengeld  +10,00 € ││
│  │   Lisa • Heute          ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ↑ Süßigkeiten  -2,50 €  ││
│  │   Tom • Gestern         ││
│  └─────────────────────────┘│
│                             │
│  Schnellaktionen            │
│  [+ Kind] [💰 Taschengeld]  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### ParentDashboardPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             xmlns:views="clr-namespace:TaschengeldManager.Mobile.Views.Components"
             x:Class="TaschengeldManager.Mobile.Views.ParentDashboardPage"
             x:DataType="vm:ParentDashboardViewModel"
             Title="Dashboard">

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="16" Spacing="16">

                <!-- Header mit Begrüßung -->
                <Grid ColumnDefinitions="*,Auto">
                    <Label Text="{Binding Greeting}"
                           Style="{StaticResource HeadlineLabel}" />
                    <ImageButton Grid.Column="1"
                                 Source="profile.png"
                                 Command="{Binding NavigateToProfileCommand}"
                                 WidthRequest="40"
                                 HeightRequest="40" />
                </Grid>

                <!-- Familien-Übersicht -->
                <Frame Padding="16" CornerRadius="12"
                       BackgroundColor="{StaticResource Primary}">
                    <VerticalStackLayout>
                        <Label Text="Familien-Übersicht"
                               FontSize="14"
                               TextColor="White"
                               Opacity="0.8" />
                        <Label Text="{Binding TotalBalance, StringFormat='{0:C}'}"
                               FontSize="32"
                               FontAttributes="Bold"
                               TextColor="White" />
                    </VerticalStackLayout>
                </Frame>

                <!-- Kinder-Liste -->
                <Label Text="Kinder"
                       FontSize="18"
                       FontAttributes="Bold" />

                <CollectionView ItemsSource="{Binding Children}"
                                SelectionMode="Single"
                                SelectedItem="{Binding SelectedChild}"
                                SelectionChangedCommand="{Binding ChildSelectedCommand}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:ChildSummaryViewModel">
                            <Frame Padding="12" CornerRadius="8" Margin="0,0,0,8">
                                <Grid ColumnDefinitions="Auto,*,Auto"
                                      RowDefinitions="Auto,Auto,Auto">
                                    <!-- Avatar -->
                                    <Label Grid.RowSpan="3"
                                           Text="{Binding AvatarEmoji}"
                                           FontSize="36"
                                           VerticalOptions="Center"
                                           Margin="0,0,12,0" />

                                    <!-- Name und Kontostand -->
                                    <Label Grid.Column="1"
                                           Text="{Binding Name}"
                                           FontSize="16"
                                           FontAttributes="Bold" />
                                    <Label Grid.Column="2"
                                           Text="{Binding Balance, StringFormat='{0:C}'}"
                                           FontSize="16"
                                           FontAttributes="Bold"
                                           TextColor="{StaticResource Primary}" />

                                    <!-- Fortschrittsbalken -->
                                    <ProgressBar Grid.Row="1" Grid.Column="1" Grid.ColumnSpan="2"
                                                 Progress="{Binding GoalProgress}"
                                                 ProgressColor="{StaticResource Primary}"
                                                 Margin="0,4" />

                                    <!-- Ziel -->
                                    <Label Grid.Row="2" Grid.Column="1"
                                           Text="{Binding GoalText}"
                                           FontSize="12"
                                           TextColor="{StaticResource TextSecondaryLight}" />
                                    <Label Grid.Row="2" Grid.Column="2"
                                           Text="{Binding GoalProgress, StringFormat='{0:P0}'}"
                                           FontSize="12"
                                           TextColor="{StaticResource TextSecondaryLight}"
                                           HorizontalTextAlignment="End" />
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>

                <!-- Letzte Aktivitäten -->
                <Label Text="Letzte Aktivitäten"
                       FontSize="18"
                       FontAttributes="Bold" />

                <CollectionView ItemsSource="{Binding RecentTransactions}"
                                HeightRequest="200">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:TransactionSummaryViewModel">
                            <Frame Padding="12" CornerRadius="8" Margin="0,0,0,4">
                                <Grid ColumnDefinitions="Auto,*,Auto">
                                    <Label Text="{Binding Icon}"
                                           FontSize="24"
                                           VerticalOptions="Center" />
                                    <VerticalStackLayout Grid.Column="1" Margin="12,0">
                                        <Label Text="{Binding Description}"
                                               FontSize="14" />
                                        <Label Text="{Binding ChildAndDate}"
                                               FontSize="12"
                                               TextColor="{StaticResource TextSecondaryLight}" />
                                    </VerticalStackLayout>
                                    <Label Grid.Column="2"
                                           Text="{Binding AmountText}"
                                           TextColor="{Binding AmountColor}"
                                           FontSize="14"
                                           FontAttributes="Bold"
                                           VerticalOptions="Center" />
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>

                <!-- Schnellaktionen -->
                <Label Text="Schnellaktionen"
                       FontSize="18"
                       FontAttributes="Bold" />

                <HorizontalStackLayout Spacing="12">
                    <Button Text="+ Kind hinzufügen"
                            Command="{Binding AddChildCommand}"
                            Style="{StaticResource SecondaryButton}" />
                    <Button Text="Taschengeld geben"
                            Command="{Binding GiveAllowanceCommand}"
                            Style="{StaticResource PrimaryButton}" />
                </HorizontalStackLayout>

            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>

</ContentPage>
```

### ParentDashboardViewModel.cs
```csharp
public partial class ParentDashboardViewModel : ObservableObject
{
    private readonly IFamilyApi _familyApi;
    private readonly INavigationService _navigationService;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    private string _greeting = string.Empty;

    [ObservableProperty]
    private decimal _totalBalance;

    [ObservableProperty]
    private ObservableCollection<ChildSummaryViewModel> _children = new();

    [ObservableProperty]
    private ObservableCollection<TransactionSummaryViewModel> _recentTransactions = new();

    [ObservableProperty]
    private ChildSummaryViewModel? _selectedChild;

    [ObservableProperty]
    private bool _isRefreshing;

    public async Task InitializeAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        var hour = DateTime.Now.Hour;
        var timeGreeting = hour switch
        {
            < 12 => "Guten Morgen",
            < 18 => "Guten Tag",
            _ => "Guten Abend"
        };
        Greeting = $"{timeGreeting}, {claims?.FirstName ?? ""}!";

        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadDataAsync();
        IsRefreshing = false;
    }

    private async Task LoadDataAsync()
    {
        var dashboardResponse = await _familyApi.GetParentDashboardAsync();
        if (dashboardResponse.IsSuccessStatusCode && dashboardResponse.Content != null)
        {
            var data = dashboardResponse.Content;

            TotalBalance = data.TotalBalance;

            Children = new ObservableCollection<ChildSummaryViewModel>(
                data.Children.Select(c => new ChildSummaryViewModel(c)));

            RecentTransactions = new ObservableCollection<TransactionSummaryViewModel>(
                data.RecentTransactions.Select(t => new TransactionSummaryViewModel(t)));
        }
    }

    [RelayCommand]
    private async Task ChildSelectedAsync()
    {
        if (SelectedChild != null)
        {
            await _navigationService.NavigateToAsync("child-detail",
                new Dictionary<string, object> { { "ChildId", SelectedChild.Id } });
            SelectedChild = null;
        }
    }

    [RelayCommand]
    private async Task AddChildAsync()
    {
        await _navigationService.NavigateToAsync("add-child");
    }

    [RelayCommand]
    private async Task GiveAllowanceAsync()
    {
        await _navigationService.NavigateToAsync("give-allowance");
    }

    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await _navigationService.NavigateToAsync("profile");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-05 | Dashboard laden | Kinder und Kontostand angezeigt |
| TC-M003-06 | Kind auswählen | Navigation zur Detailansicht |
| TC-M003-07 | Pull-to-Refresh | Daten werden aktualisiert |
| TC-M003-08 | Kind hinzufügen | Navigation zur Anlage-Seite |

## Story Points
5

## Priorität
Hoch
