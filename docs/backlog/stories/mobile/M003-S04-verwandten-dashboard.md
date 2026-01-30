# Story M003-S04: Verwandten-Dashboard

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Verwandter** möchte ich **auf meinem Dashboard die mir zugeordneten Kinder sehen**, damit **ich ihnen einfach Geldgeschenke senden kann**.

## Akzeptanzkriterien

- [ ] Gegeben das Verwandten-Dashboard, wenn es geladen wird, dann werden die zugeordneten Kinder angezeigt
- [ ] Gegeben ein Kind, wenn ein Geschenk gesendet werden soll, dann ist ein schneller Zugang verfügbar
- [ ] Gegeben gesendete Geschenke, wenn sie vorhanden sind, dann wird eine Historie angezeigt
- [ ] Gegeben das Dashboard, wenn keine Kinder zugeordnet sind, dann wird ein entsprechender Hinweis angezeigt

## UI-Entwurf

```
┌─────────────────────────────┐
│  Hallo, Oma Helga!          │
├─────────────────────────────┤
│                             │
│  Deine Enkel               │
│  ┌─────────────────────────┐│
│  │ 🦁 Lisa                 ││
│  │ Kontostand: 45,00 €     ││
│  │       [🎁 Schenken]     ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 🐰 Tom                  ││
│  │ Kontostand: 80,50 €     ││
│  │       [🎁 Schenken]     ││
│  └─────────────────────────┘│
│                             │
│  Deine Geschenke 🎁         │
│  ┌─────────────────────────┐│
│  │ An Lisa      20,00 €    ││
│  │ "Zum Geburtstag"        ││
│  │ 15.03.2024              ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ An Tom       15,00 €    ││
│  │ "Für gute Noten"        ││
│  │ 10.03.2024              ││
│  └─────────────────────────┘│
│                             │
│  Dieses Jahr: 185,00 €      │
│  verschenkt                 │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### RelativeDashboardPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.RelativeDashboardPage"
             x:DataType="vm:RelativeDashboardViewModel"
             Title="Dashboard">

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="16" Spacing="16">

                <!-- Header -->
                <Label Text="{Binding Greeting}"
                       Style="{StaticResource HeadlineLabel}" />

                <!-- Kinder-Sektion -->
                <Label Text="Deine Enkel"
                       FontSize="18"
                       FontAttributes="Bold"
                       IsVisible="{Binding HasChildren}" />

                <CollectionView ItemsSource="{Binding Children}"
                                SelectionMode="None"
                                IsVisible="{Binding HasChildren}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:RelativeChildViewModel">
                            <Frame Padding="16" CornerRadius="12" Margin="0,0,0,8">
                                <Grid ColumnDefinitions="Auto,*,Auto"
                                      RowDefinitions="Auto,Auto">
                                    <!-- Avatar -->
                                    <Label Grid.RowSpan="2"
                                           Text="{Binding AvatarEmoji}"
                                           FontSize="40"
                                           VerticalOptions="Center"
                                           Margin="0,0,12,0" />

                                    <!-- Name -->
                                    <Label Grid.Column="1"
                                           Text="{Binding Name}"
                                           FontSize="18"
                                           FontAttributes="Bold" />

                                    <!-- Kontostand -->
                                    <Label Grid.Row="1" Grid.Column="1"
                                           Text="{Binding Balance, StringFormat='Kontostand: {0:C}'}"
                                           FontSize="14"
                                           TextColor="{StaticResource TextSecondaryLight}" />

                                    <!-- Schenken Button -->
                                    <Button Grid.RowSpan="2" Grid.Column="2"
                                            Text="🎁 Schenken"
                                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:RelativeDashboardViewModel}}, Path=SendGiftCommand}"
                                            CommandParameter="{Binding Id}"
                                            Style="{StaticResource PrimaryButton}"
                                            VerticalOptions="Center" />
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>

                <!-- Keine Kinder zugeordnet -->
                <Frame Padding="24"
                       CornerRadius="12"
                       IsVisible="{Binding HasNoChildren}">
                    <VerticalStackLayout HorizontalOptions="Center" Spacing="12">
                        <Label Text="👨‍👩‍👧‍👦"
                               FontSize="48"
                               HorizontalTextAlignment="Center" />
                        <Label Text="Noch keine Kinder zugeordnet"
                               FontSize="16"
                               FontAttributes="Bold"
                               HorizontalTextAlignment="Center" />
                        <Label Text="Bitte die Eltern, dich einzuladen."
                               FontSize="14"
                               TextColor="{StaticResource TextSecondaryLight}"
                               HorizontalTextAlignment="Center" />
                    </VerticalStackLayout>
                </Frame>

                <!-- Geschenke-Historie -->
                <HorizontalStackLayout IsVisible="{Binding HasGifts}">
                    <Label Text="Deine Geschenke"
                           FontSize="18"
                           FontAttributes="Bold" />
                    <Label Text=" 🎁" FontSize="18" />
                </HorizontalStackLayout>

                <CollectionView ItemsSource="{Binding RecentGifts}"
                                SelectionMode="None"
                                IsVisible="{Binding HasGifts}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:GiftHistoryItemViewModel">
                            <Frame Padding="12" CornerRadius="8" Margin="0,0,0,4">
                                <Grid ColumnDefinitions="*,Auto">
                                    <VerticalStackLayout>
                                        <Label FontSize="14">
                                            <Label.FormattedText>
                                                <FormattedString>
                                                    <Span Text="An " />
                                                    <Span Text="{Binding ChildName}"
                                                          FontAttributes="Bold" />
                                                </FormattedString>
                                            </Label.FormattedText>
                                        </Label>
                                        <Label Text="{Binding Message}"
                                               FontSize="12"
                                               FontAttributes="Italic"
                                               TextColor="{StaticResource TextSecondaryLight}"
                                               IsVisible="{Binding HasMessage}" />
                                        <Label Text="{Binding DateText}"
                                               FontSize="12"
                                               TextColor="{StaticResource TextSecondaryLight}" />
                                    </VerticalStackLayout>
                                    <Label Grid.Column="1"
                                           Text="{Binding Amount, StringFormat='{0:C}'}"
                                           FontSize="16"
                                           FontAttributes="Bold"
                                           TextColor="{StaticResource Primary}"
                                           VerticalOptions="Center" />
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>

                <!-- Jahres-Statistik -->
                <Frame Padding="16"
                       CornerRadius="12"
                       BackgroundColor="{StaticResource PrimaryLight}"
                       IsVisible="{Binding HasGifts}">
                    <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                        <Label Text="Dieses Jahr verschenkt:"
                               FontSize="14"
                               VerticalOptions="Center" />
                        <Label Text="{Binding YearlyTotal, StringFormat='{0:C}'}"
                               FontSize="18"
                               FontAttributes="Bold"
                               TextColor="{StaticResource Primary}"
                               VerticalOptions="Center" />
                    </HorizontalStackLayout>
                </Frame>

                <!-- Alle Geschenke anzeigen -->
                <Button Text="Alle Geschenke anzeigen"
                        Command="{Binding ShowAllGiftsCommand}"
                        Style="{StaticResource SecondaryButton}"
                        IsVisible="{Binding HasMoreGifts}" />

            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>

</ContentPage>
```

### RelativeDashboardViewModel.cs
```csharp
public partial class RelativeDashboardViewModel : ObservableObject
{
    private readonly IRelativeApi _relativeApi;
    private readonly INavigationService _navigationService;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    private string _greeting = string.Empty;

    [ObservableProperty]
    private ObservableCollection<RelativeChildViewModel> _children = new();

    [ObservableProperty]
    private ObservableCollection<GiftHistoryItemViewModel> _recentGifts = new();

    [ObservableProperty]
    private decimal _yearlyTotal;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _hasMoreGifts;

    public bool HasChildren => Children.Any();
    public bool HasNoChildren => !HasChildren;
    public bool HasGifts => RecentGifts.Any();

    public async Task InitializeAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        Greeting = $"Hallo, {claims?.FirstName ?? ""}!";

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
        var response = await _relativeApi.GetRelativeDashboardAsync();
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var data = response.Content;

            Children = new ObservableCollection<RelativeChildViewModel>(
                data.Children.Select(c => new RelativeChildViewModel(c)));

            RecentGifts = new ObservableCollection<GiftHistoryItemViewModel>(
                data.RecentGifts
                    .Take(5)
                    .Select(g => new GiftHistoryItemViewModel(g)));

            YearlyTotal = data.YearlyTotal;
            HasMoreGifts = data.RecentGifts.Count > 5;

            OnPropertyChanged(nameof(HasChildren));
            OnPropertyChanged(nameof(HasNoChildren));
            OnPropertyChanged(nameof(HasGifts));
        }
    }

    [RelayCommand]
    private async Task SendGiftAsync(Guid childId)
    {
        await _navigationService.NavigateToAsync("send-gift",
            new Dictionary<string, object> { { "ChildId", childId } });
    }

    [RelayCommand]
    private async Task ShowAllGiftsAsync()
    {
        await _navigationService.NavigateToAsync("//relative-main/gifts");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-13 | Dashboard laden | Kinder angezeigt |
| TC-M003-14 | Keine Kinder zugeordnet | Hinweis angezeigt |
| TC-M003-15 | Geschenk senden | Navigation zur Geschenk-Seite |
| TC-M003-16 | Geschenk-Historie | Letzte Geschenke angezeigt |

## Story Points
2

## Priorität
Mittel
