# Story M004-S06: Zins-Gutschriften anzeigen

## Epic
M004 - Kontoverwaltung Kind

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **sehen, wenn ich Zinsen auf mein Erspartes bekomme**, damit **ich verstehe, dass Sparen sich lohnt und mein Geld wächst**.

## Akzeptanzkriterien

- [ ] Gegeben eine Zins-Gutschrift, wenn sie erfolgt, dann wird sie als Transaktion angezeigt
- [ ] Gegeben die Transaktionsliste, wenn Zinsen angezeigt werden, dann haben sie ein spezielles Icon
- [ ] Gegeben das Dashboard, wenn Zinsen gutgeschrieben werden, dann wird eine kindgerechte Erklärung angezeigt
- [ ] Gegeben die Details, wenn sie geöffnet werden, dann ist der Zinssatz sichtbar

## UI-Entwurf

```
┌─────────────────────────────┐
│                             │
│  Transaktionsliste          │
│  ┌─────────────────────────┐│
│  │ 📈 Zinsen       +0,45 € ││
│  │    1. März              ││
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘

Detail-Ansicht:
┌─────────────────────────────┐
│        📈                   │
│                             │
│    Zinsen für Februar       │
│                             │
│       +0,45 €               │
├─────────────────────────────┤
│                             │
│  Super! Du bekommst Zinsen, │
│  weil du gespart hast! 🎉   │
│                             │
│  ─────────────────────────  │
│                             │
│  Zinssatz                   │
│  3% pro Jahr                │
│                             │
│  Dein Guthaben              │
│  180,00 €                   │
│                             │
│  Berechneter Zins           │
│  180,00 € × 3% ÷ 12 = 0,45 €│
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### Zins-Transaktion in der Liste
```csharp
public class TransactionItemViewModel
{
    public bool IsInterest { get; }

    public string CategoryEmoji => IsInterest ? "📈" : Category?.Emoji ?? "💰";

    public string Description => IsInterest
        ? $"Zinsen für {InterestMonth}"
        : _description;

    public Color CategoryBackgroundColor => IsInterest
        ? Color.FromArgb("#B2DFDB") // Teal Light
        : Color.FromArgb(Category?.Color ?? "#E0E0E0");

    public TransactionItemViewModel(TransactionResponse response)
    {
        IsInterest = response.Type == "Interest";
        // ... weitere Initialisierung
    }
}
```

### InterestDetailSection in TransactionDetailPage
```xml
<!-- Zins-spezifische Details -->
<VerticalStackLayout IsVisible="{Binding IsInterest}"
                     Padding="16"
                     Spacing="16">

    <!-- Motivierende Nachricht -->
    <Frame BackgroundColor="{StaticResource PrimaryLight}"
           Padding="16"
           CornerRadius="12">
        <HorizontalStackLayout Spacing="8">
            <Label Text="🎉" FontSize="24" />
            <Label Text="Super! Du bekommst Zinsen, weil du gespart hast!"
                   FontSize="14"
                   VerticalOptions="Center" />
        </HorizontalStackLayout>
    </Frame>

    <!-- Zinssatz -->
    <Frame Padding="16" BackgroundColor="Transparent">
        <Grid ColumnDefinitions="*,Auto">
            <Label Text="Zinssatz"
                   FontSize="14"
                   TextColor="{StaticResource TextSecondaryLight}" />
            <Label Grid.Column="1"
                   Text="{Binding InterestRate, StringFormat='{0:P1} pro Jahr'}"
                   FontSize="14"
                   FontAttributes="Bold" />
        </Grid>
    </Frame>

    <!-- Guthaben -->
    <Frame Padding="16" BackgroundColor="Transparent">
        <Grid ColumnDefinitions="*,Auto">
            <Label Text="Dein Guthaben"
                   FontSize="14"
                   TextColor="{StaticResource TextSecondaryLight}" />
            <Label Grid.Column="1"
                   Text="{Binding InterestBase, StringFormat='{0:C}'}"
                   FontSize="14"
                   FontAttributes="Bold" />
        </Grid>
    </Frame>

    <!-- Berechnung -->
    <Frame Padding="16" BackgroundColor="{StaticResource SurfaceLight}" CornerRadius="8">
        <VerticalStackLayout Spacing="4">
            <Label Text="So haben wir gerechnet:"
                   FontSize="12"
                   TextColor="{StaticResource TextSecondaryLight}" />
            <Label Text="{Binding InterestCalculation}"
                   FontSize="14"
                   FontFamily="Monospace" />
        </VerticalStackLayout>
    </Frame>

</VerticalStackLayout>
```

### TransactionDetailViewModel - Zins-Eigenschaften
```csharp
public partial class TransactionDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isInterest;

    [ObservableProperty]
    private decimal _interestRate;

    [ObservableProperty]
    private decimal _interestBase;

    public string InterestMonth => Date.AddMonths(-1).ToString("MMMM");

    public string InterestCalculation =>
        $"{InterestBase:C} × {InterestRate:P1} ÷ 12 = {Amount:C}";

    private async Task LoadTransactionAsync()
    {
        var response = await _accountApi.GetTransactionAsync(TransactionId);
        if (response.IsSuccessStatusCode && response.Content != null)
        {
            var transaction = response.Content;

            IsInterest = transaction.Type == "Interest";

            if (IsInterest)
            {
                InterestRate = transaction.InterestDetails?.Rate ?? 0.03m;
                InterestBase = transaction.InterestDetails?.BaseAmount ?? 0;
            }

            // ... weitere Initialisierung
        }
    }
}
```

### Zins-Notification (Push)
```csharp
public class InterestNotificationService
{
    public async Task SendInterestNotificationAsync(
        string childName,
        decimal interestAmount,
        decimal newBalance)
    {
        var notification = new LocalNotification
        {
            Title = "Zinsen gutgeschrieben! 📈",
            Body = $"Hey {childName}! Du hast {interestAmount:C} Zinsen bekommen. " +
                   $"Dein neuer Kontostand: {newBalance:C}",
            CategoryType = NotificationCategoryType.Status
        };

        await LocalNotificationCenter.Current.Show(notification);
    }
}
```

### Dashboard-Hinweis bei neuen Zinsen
```xml
<!-- Im Kind-Dashboard, wenn neue Zinsen da sind -->
<Frame IsVisible="{Binding HasNewInterest}"
       BackgroundColor="{StaticResource PrimaryLight}"
       Padding="12"
       CornerRadius="12"
       Margin="16,0">
    <Grid ColumnDefinitions="Auto,*,Auto">
        <Label Text="📈" FontSize="24" VerticalOptions="Center" />
        <VerticalStackLayout Grid.Column="1" Margin="12,0">
            <Label Text="Neue Zinsen!"
                   FontSize="14"
                   FontAttributes="Bold" />
            <Label Text="{Binding NewInterestAmount, StringFormat='+{0:C} gutgeschrieben'}"
                   FontSize="12"
                   TextColor="{StaticResource Primary}" />
        </VerticalStackLayout>
        <Button Grid.Column="2"
                Text="Ansehen"
                Command="{Binding ViewInterestCommand}"
                Style="{StaticResource SmallPrimaryButton}" />
    </Grid>
</Frame>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-24 | Zins-Transaktion in Liste | Spezielles Icon und Text |
| TC-M004-25 | Zins-Detail öffnen | Berechnung angezeigt |
| TC-M004-26 | Motivation anzeigen | Kindgerechte Nachricht |
| TC-M004-27 | Neue Zinsen | Hinweis auf Dashboard |

## Story Points
1

## Priorität
Niedrig
