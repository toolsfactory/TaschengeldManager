# Story M003-S03: Kind-Dashboard

## Epic
M003 - Dashboard & Navigation

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **auf meinem Dashboard meinen Kontostand und mein Sparziel sehen**, damit **ich weiß, wie viel Geld ich habe und wie nah ich meinem Ziel bin**.

## Akzeptanzkriterien

- [ ] Gegeben das Kind-Dashboard, wenn es geladen wird, dann wird der aktuelle Kontostand prominent angezeigt
- [ ] Gegeben ein Sparziel, wenn es gesetzt ist, dann wird der Fortschritt visuell dargestellt
- [ ] Gegeben letzte Transaktionen, wenn sie vorhanden sind, dann werden sie kindgerecht angezeigt
- [ ] Gegeben das Dashboard, wenn eine Ausgabe erfasst werden soll, dann ist ein schneller Zugang verfügbar

## UI-Entwurf

```
┌─────────────────────────────┐
│  Hallo, Lisa! 🦁            │
├─────────────────────────────┤
│                             │
│  ┌─────────────────────────┐│
│  │     Dein Kontostand     ││
│  │                         ││
│  │       45,00 €           ││
│  │                         ││
│  │  [Ausgabe hinzufügen]   ││
│  └─────────────────────────┘│
│                             │
│  Dein Sparziel 🎯           │
│  ┌─────────────────────────┐│
│  │  Neue Spielekonsole     ││
│  │  ████████░░░░ 45/200 €  ││
│  │  Noch 155 € zu sparen   ││
│  └─────────────────────────┘│
│                             │
│  Was ist passiert? 📋       │
│  ┌─────────────────────────┐│
│  │ 🍭 Süßigkeiten  -2,50 € ││
│  │    Heute                ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 💰 Taschengeld +10,00 € ││
│  │    Montag               ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 🎁 Von Oma     +20,00 € ││
│  │    Letzte Woche         ││
│  └─────────────────────────┘│
│                             │
│     [Alle anzeigen]         │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### ChildDashboardPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="TaschengeldManager.Mobile.Views.ChildDashboardPage"
             x:DataType="vm:ChildDashboardViewModel"
             Title="Mein Konto"
             BackgroundColor="{StaticResource PrimaryLight}">

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="16" Spacing="16">

                <!-- Header -->
                <HorizontalStackLayout Spacing="8">
                    <Label Text="{Binding Greeting}"
                           Style="{StaticResource HeadlineLabel}" />
                    <Label Text="{Binding AvatarEmoji}"
                           FontSize="32" />
                </HorizontalStackLayout>

                <!-- Kontostand-Karte -->
                <Frame Padding="24"
                       CornerRadius="16"
                       BackgroundColor="{StaticResource Primary}"
                       HasShadow="True">
                    <VerticalStackLayout Spacing="8">
                        <Label Text="Dein Kontostand"
                               FontSize="14"
                               TextColor="White"
                               Opacity="0.8"
                               HorizontalTextAlignment="Center" />

                        <!-- Animierter Kontostand -->
                        <Label x:Name="BalanceLabel"
                               Text="{Binding Balance, StringFormat='{0:C}'}"
                               FontSize="48"
                               FontAttributes="Bold"
                               TextColor="White"
                               HorizontalTextAlignment="Center">
                            <Label.Behaviors>
                                <toolkit:AnimationBehavior EventName="Loaded">
                                    <toolkit:AnimationBehavior.AnimationType>
                                        <toolkit:ScaleAnimation Length="300"
                                                                 From="0.5"
                                                                 To="1"
                                                                 Easing="{x:Static Easing.SpringOut}" />
                                    </toolkit:AnimationBehavior.AnimationType>
                                </toolkit:AnimationBehavior>
                            </Label.Behaviors>
                        </Label>

                        <Button Text="Ausgabe hinzufügen"
                                Command="{Binding AddExpenseCommand}"
                                BackgroundColor="White"
                                TextColor="{StaticResource Primary}"
                                CornerRadius="20"
                                Margin="0,8,0,0" />
                    </VerticalStackLayout>
                </Frame>

                <!-- Sparziel -->
                <Frame Padding="16"
                       CornerRadius="12"
                       IsVisible="{Binding HasSavingsGoal}">
                    <VerticalStackLayout Spacing="8">
                        <HorizontalStackLayout>
                            <Label Text="Dein Sparziel"
                                   FontSize="16"
                                   FontAttributes="Bold" />
                            <Label Text=" 🎯" FontSize="16" />
                        </HorizontalStackLayout>

                        <Label Text="{Binding SavingsGoalName}"
                               FontSize="14" />

                        <Grid ColumnDefinitions="*,Auto">
                            <ProgressBar Progress="{Binding SavingsGoalProgress}"
                                         ProgressColor="{StaticResource Accent}"
                                         HeightRequest="12" />
                            <Label Grid.Column="1"
                                   Text="{Binding SavingsGoalProgressText}"
                                   FontSize="12"
                                   Margin="8,0,0,0" />
                        </Grid>

                        <Label Text="{Binding SavingsGoalRemainingText}"
                               FontSize="12"
                               TextColor="{StaticResource TextSecondaryLight}" />
                    </VerticalStackLayout>
                </Frame>

                <!-- Kein Sparziel -->
                <Frame Padding="16"
                       CornerRadius="12"
                       IsVisible="{Binding HasNoSavingsGoal}">
                    <VerticalStackLayout HorizontalOptions="Center" Spacing="8">
                        <Label Text="🎯"
                               FontSize="48"
                               HorizontalTextAlignment="Center" />
                        <Label Text="Setze dir ein Sparziel!"
                               FontSize="16"
                               HorizontalTextAlignment="Center" />
                        <Button Text="Ziel erstellen"
                                Command="{Binding CreateSavingsGoalCommand}"
                                Style="{StaticResource SecondaryButton}" />
                    </VerticalStackLayout>
                </Frame>

                <!-- Letzte Aktivitäten -->
                <HorizontalStackLayout>
                    <Label Text="Was ist passiert?"
                           FontSize="16"
                           FontAttributes="Bold" />
                    <Label Text=" 📋" FontSize="16" />
                </HorizontalStackLayout>

                <CollectionView ItemsSource="{Binding RecentTransactions}">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:ChildTransactionViewModel">
                            <Frame Padding="12" CornerRadius="8" Margin="0,0,0,8">
                                <Grid ColumnDefinitions="Auto,*,Auto">
                                    <Label Text="{Binding CategoryEmoji}"
                                           FontSize="28"
                                           VerticalOptions="Center" />
                                    <VerticalStackLayout Grid.Column="1" Margin="12,0">
                                        <Label Text="{Binding Description}"
                                               FontSize="14"
                                               FontAttributes="Bold" />
                                        <Label Text="{Binding DateText}"
                                               FontSize="12"
                                               TextColor="{StaticResource TextSecondaryLight}" />
                                    </VerticalStackLayout>
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
                    <CollectionView.EmptyView>
                        <VerticalStackLayout HorizontalOptions="Center" Padding="24">
                            <Label Text="📭"
                                   FontSize="48"
                                   HorizontalTextAlignment="Center" />
                            <Label Text="Noch keine Aktivitäten"
                                   FontSize="14"
                                   HorizontalTextAlignment="Center"
                                   TextColor="{StaticResource TextSecondaryLight}" />
                        </VerticalStackLayout>
                    </CollectionView.EmptyView>
                </CollectionView>

                <!-- Alle anzeigen -->
                <Button Text="Alle anzeigen"
                        Command="{Binding ShowAllTransactionsCommand}"
                        Style="{StaticResource SecondaryButton}"
                        IsVisible="{Binding HasMoreTransactions}" />

            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>

</ContentPage>
```

### ChildDashboardViewModel.cs
```csharp
public partial class ChildDashboardViewModel : ObservableObject
{
    private readonly IAccountApi _accountApi;
    private readonly INavigationService _navigationService;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    private string _greeting = string.Empty;

    [ObservableProperty]
    private string _avatarEmoji = "🦁";

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private bool _hasSavingsGoal;

    [ObservableProperty]
    private string _savingsGoalName = string.Empty;

    [ObservableProperty]
    private double _savingsGoalProgress;

    [ObservableProperty]
    private string _savingsGoalProgressText = string.Empty;

    [ObservableProperty]
    private string _savingsGoalRemainingText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ChildTransactionViewModel> _recentTransactions = new();

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private bool _hasMoreTransactions;

    public bool HasNoSavingsGoal => !HasSavingsGoal;

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
        var response = await _accountApi.GetChildDashboardAsync();
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var data = response.Content;

            Balance = data.Balance;
            AvatarEmoji = data.AvatarEmoji;

            if (data.SavingsGoal != null)
            {
                HasSavingsGoal = true;
                SavingsGoalName = data.SavingsGoal.Name;
                SavingsGoalProgress = (double)(Balance / data.SavingsGoal.TargetAmount);
                SavingsGoalProgressText = $"{Balance:C}/{data.SavingsGoal.TargetAmount:C}";
                SavingsGoalRemainingText = $"Noch {data.SavingsGoal.TargetAmount - Balance:C} zu sparen";
            }
            else
            {
                HasSavingsGoal = false;
            }

            OnPropertyChanged(nameof(HasNoSavingsGoal));

            RecentTransactions = new ObservableCollection<ChildTransactionViewModel>(
                data.RecentTransactions
                    .Take(5)
                    .Select(t => new ChildTransactionViewModel(t)));

            HasMoreTransactions = data.RecentTransactions.Count > 5;
        }
    }

    [RelayCommand]
    private async Task AddExpenseAsync()
    {
        await _navigationService.NavigateToAsync("add-expense");
    }

    [RelayCommand]
    private async Task CreateSavingsGoalAsync()
    {
        await _navigationService.NavigateToAsync("create-savings-goal");
    }

    [RelayCommand]
    private async Task ShowAllTransactionsAsync()
    {
        await _navigationService.NavigateToAsync("//child-main/transactions");
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-09 | Dashboard laden | Kontostand und Ziel angezeigt |
| TC-M003-10 | Kein Sparziel vorhanden | CTA zum Erstellen angezeigt |
| TC-M003-11 | Ausgabe hinzufügen | Navigation zur Erfassungsseite |
| TC-M003-12 | Alle Transaktionen | Navigation zur Transaktionsliste |

## Story Points
3

## Priorität
Hoch
