# Story M004-S01: Kontostand-Anzeige mit Animation

## Epic
M004 - Kontoverwaltung Kind

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **meinen Kontostand prominent und ansprechend animiert sehen**, damit **ich sofort weiß, wie viel Geld ich habe und Freude an der App habe**.

## Akzeptanzkriterien

- [ ] Gegeben das Dashboard, wenn es geladen wird, dann wird der Kontostand mit einer Zähl-Animation angezeigt
- [ ] Gegeben der Kontostand, wenn er sich ändert, dann wird die Änderung animiert
- [ ] Gegeben ein positiver Kontostand, wenn er angezeigt wird, dann ist er grün gefärbt
- [ ] Gegeben ein niedriger Kontostand, wenn er unter einem Schwellenwert liegt, dann wird ein Hinweis angezeigt

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│  ┌─────────────────────────┐│
│  │                         ││
│  │   Dein Kontostand       ││
│  │                         ││
│  │      45,00 €            ││
│  │        ↑                ││
│  │   (Zähl-Animation)      ││
│  │                         ││
│  │   +10,00 € diese Woche  ││
│  │                         ││
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### AnimatedBalanceView Component
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TaschengeldManager.Mobile.Views.Components.AnimatedBalanceView"
             x:Name="BalanceView">

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

            <!-- Animierter Betrag -->
            <Label x:Name="BalanceLabel"
                   Text="{Binding Source={x:Reference BalanceView}, Path=DisplayBalance, StringFormat='{0:C}'}"
                   FontSize="48"
                   FontAttributes="Bold"
                   TextColor="White"
                   HorizontalTextAlignment="Center" />

            <!-- Trend-Anzeige -->
            <HorizontalStackLayout HorizontalOptions="Center"
                                   Spacing="4"
                                   IsVisible="{Binding Source={x:Reference BalanceView}, Path=HasWeeklyChange}">
                <Label Text="{Binding Source={x:Reference BalanceView}, Path=TrendIcon}"
                       FontSize="14"
                       TextColor="{Binding Source={x:Reference BalanceView}, Path=TrendColor}" />
                <Label Text="{Binding Source={x:Reference BalanceView}, Path=WeeklyChangeText}"
                       FontSize="12"
                       TextColor="White"
                       Opacity="0.9" />
            </HorizontalStackLayout>

        </VerticalStackLayout>
    </Frame>

</ContentView>
```

### AnimatedBalanceView.xaml.cs
```csharp
public partial class AnimatedBalanceView : ContentView
{
    public static readonly BindableProperty BalanceProperty =
        BindableProperty.Create(nameof(Balance), typeof(decimal), typeof(AnimatedBalanceView),
            0m, propertyChanged: OnBalanceChanged);

    public static readonly BindableProperty WeeklyChangeProperty =
        BindableProperty.Create(nameof(WeeklyChange), typeof(decimal), typeof(AnimatedBalanceView), 0m);

    private decimal _displayBalance;
    private CancellationTokenSource? _animationCts;

    public decimal Balance
    {
        get => (decimal)GetValue(BalanceProperty);
        set => SetValue(BalanceProperty, value);
    }

    public decimal WeeklyChange
    {
        get => (decimal)GetValue(WeeklyChangeProperty);
        set => SetValue(WeeklyChangeProperty, value);
    }

    public decimal DisplayBalance
    {
        get => _displayBalance;
        private set
        {
            _displayBalance = value;
            OnPropertyChanged();
        }
    }

    public bool HasWeeklyChange => WeeklyChange != 0;

    public string TrendIcon => WeeklyChange >= 0 ? "↑" : "↓";

    public Color TrendColor => WeeklyChange >= 0
        ? Colors.LightGreen
        : Colors.LightCoral;

    public string WeeklyChangeText => WeeklyChange >= 0
        ? $"+{WeeklyChange:C} diese Woche"
        : $"{WeeklyChange:C} diese Woche";

    private static void OnBalanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AnimatedBalanceView view)
        {
            var oldBalance = (decimal)oldValue;
            var newBalance = (decimal)newValue;
            view.AnimateBalance(oldBalance, newBalance);
        }
    }

    private async void AnimateBalance(decimal from, decimal to)
    {
        // Vorherige Animation abbrechen
        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();
        var token = _animationCts.Token;

        const int durationMs = 1000;
        const int frameCount = 60;
        var frameDelay = durationMs / frameCount;
        var increment = (to - from) / frameCount;

        try
        {
            for (int i = 0; i <= frameCount; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                // Easing (EaseOut)
                var progress = (double)i / frameCount;
                var easedProgress = 1 - Math.Pow(1 - progress, 3);
                DisplayBalance = from + (to - from) * (decimal)easedProgress;

                await Task.Delay(frameDelay, token);
            }

            DisplayBalance = to;
        }
        catch (TaskCanceledException)
        {
            // Animation wurde abgebrochen
        }
    }

    public AnimatedBalanceView()
    {
        InitializeComponent();
    }
}
```

### Verwendung im ChildDashboardPage
```xml
<views:AnimatedBalanceView Balance="{Binding Balance}"
                            WeeklyChange="{Binding WeeklyChange}" />
```

### Bounce-Animation bei Änderung
```csharp
private async void AnimateBalanceWithBounce()
{
    var label = BalanceLabel;

    // Scale-Animation (Bounce)
    await label.ScaleTo(1.1, 150, Easing.CubicOut);
    await label.ScaleTo(0.95, 100, Easing.CubicIn);
    await label.ScaleTo(1.0, 100, Easing.CubicOut);
}
```

### Confetti-Animation bei großem Eingang (optional)
```csharp
public async Task ShowConfettiAnimation()
{
    // Für Geschenk-Eingänge oder Taschengeld
    var confettiView = new ConfettiView();
    // Animation abspielen...
}
```

### Low Balance Warning
```csharp
public partial class ChildDashboardViewModel : ObservableObject
{
    private const decimal LowBalanceThreshold = 5.00m;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private bool _showLowBalanceWarning;

    partial void OnBalanceChanged(decimal value)
    {
        ShowLowBalanceWarning = value < LowBalanceThreshold && value >= 0;
    }
}
```

```xml
<!-- Low Balance Warning -->
<Frame IsVisible="{Binding ShowLowBalanceWarning}"
       BackgroundColor="{StaticResource WarningLight}"
       Padding="12"
       CornerRadius="8"
       Margin="16,8">
    <HorizontalStackLayout Spacing="8">
        <Label Text="💡" FontSize="18" />
        <Label Text="Dein Kontostand ist niedrig. Vielleicht sparst du ein bisschen?"
               FontSize="12"
               VerticalOptions="Center" />
    </HorizontalStackLayout>
</Frame>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-01 | Dashboard öffnen | Kontostand zählt hoch |
| TC-M004-02 | Kontostand ändert sich | Animation spielt ab |
| TC-M004-03 | Niedriger Kontostand | Warnung wird angezeigt |
| TC-M004-04 | Wöchentliche Änderung positiv | Grüner Pfeil nach oben |

## Story Points
2

## Priorität
Hoch
