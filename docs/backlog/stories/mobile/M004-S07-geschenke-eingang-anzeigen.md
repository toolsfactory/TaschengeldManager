# Story M004-S07: Geschenke-Eingang anzeigen

## Epic
M004 - Kontoverwaltung Kind

## Status
Offen

## User Story

Als **Kind** möchte ich **sehen, wenn ich ein Geldgeschenk von Verwandten bekomme**, damit **ich mich freuen und mich bedanken kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Geschenk-Eingang, wenn er erfolgt, dann wird er prominent angezeigt
- [ ] Gegeben die Transaktionsliste, wenn ein Geschenk angezeigt wird, dann ist der Absender sichtbar
- [ ] Gegeben die Detail-Ansicht, wenn sie geöffnet wird, dann ist die persönliche Nachricht sichtbar
- [ ] Gegeben ein neues Geschenk, wenn die App geöffnet wird, dann wird eine Benachrichtigung angezeigt

## UI-Entwurf

```
Dashboard mit neuem Geschenk:
┌─────────────────────────────┐
│                             │
│  ┌─────────────────────────┐│
│  │  🎁 Neues Geschenk!     ││
│  │                         ││
│  │  Oma Helga hat dir      ││
│  │  20,00 € geschenkt!     ││
│  │                         ││
│  │  "Zum Geburtstag,       ││
│  │   mein Schatz!"         ││
│  │                         ││
│  │  [Ansehen] [Danke sagen]││
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘

Transaktions-Detail:
┌─────────────────────────────┐
│        🎁                   │
│                             │
│    Geschenk von Oma         │
│                             │
│       +20,00 €              │
├─────────────────────────────┤
│                             │
│  Von                        │
│  👵 Oma Helga               │
│                             │
│  ─────────────────────────  │
│                             │
│  Nachricht                  │
│  "Zum Geburtstag, mein      │
│   Schatz! Kauf dir was      │
│   Schönes davon!"           │
│                             │
│  ─────────────────────────  │
│                             │
│  Datum                      │
│  15. März 2024, 10:30 Uhr   │
│                             │
├─────────────────────────────┤
│                             │
│  ┌───────────────────────┐  │
│  │   💌 Danke sagen      │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### GiftNotificationBanner Component
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TaschengeldManager.Mobile.Views.Components.GiftNotificationBanner"
             x:Name="GiftBanner">

    <Frame Padding="16"
           CornerRadius="16"
           BackgroundColor="{StaticResource Primary}"
           Margin="16"
           HasShadow="True">

        <!-- Schließen Button -->
        <Frame.GestureRecognizers>
            <SwipeGestureRecognizer Direction="Up"
                                    Command="{Binding Source={x:Reference GiftBanner}, Path=DismissCommand}" />
        </Frame.GestureRecognizers>

        <VerticalStackLayout Spacing="12">

            <!-- Header -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="8">
                <Label Text="🎁" FontSize="28" />
                <Label Text="Neues Geschenk!"
                       FontSize="20"
                       FontAttributes="Bold"
                       TextColor="White"
                       VerticalOptions="Center" />
            </HorizontalStackLayout>

            <!-- Absender und Betrag -->
            <Label HorizontalTextAlignment="Center" TextColor="White">
                <Label.FormattedText>
                    <FormattedString>
                        <Span Text="{Binding Source={x:Reference GiftBanner}, Path=SenderName}" />
                        <Span Text=" hat dir " />
                        <Span Text="{Binding Source={x:Reference GiftBanner}, Path=Amount, StringFormat='{0:C}'}"
                              FontAttributes="Bold" />
                        <Span Text=" geschenkt!" />
                    </FormattedString>
                </Label.FormattedText>
            </Label>

            <!-- Nachricht -->
            <Frame BackgroundColor="White"
                   Opacity="0.9"
                   Padding="12"
                   CornerRadius="8"
                   IsVisible="{Binding Source={x:Reference GiftBanner}, Path=HasMessage}">
                <Label Text="{Binding Source={x:Reference GiftBanner}, Path=Message}"
                       FontSize="14"
                       FontAttributes="Italic"
                       HorizontalTextAlignment="Center"
                       TextColor="{StaticResource TextPrimaryLight}" />
            </Frame>

            <!-- Aktionen -->
            <HorizontalStackLayout HorizontalOptions="Center" Spacing="12">
                <Button Text="Ansehen"
                        Command="{Binding Source={x:Reference GiftBanner}, Path=ViewCommand}"
                        BackgroundColor="White"
                        TextColor="{StaticResource Primary}" />
                <Button Text="💌 Danke sagen"
                        Command="{Binding Source={x:Reference GiftBanner}, Path=ThankCommand}"
                        BackgroundColor="{StaticResource Accent}"
                        TextColor="White" />
            </HorizontalStackLayout>

        </VerticalStackLayout>
    </Frame>

</ContentView>
```

### GiftTransactionViewModel
```csharp
public class GiftTransactionViewModel : TransactionItemViewModel
{
    public string SenderName { get; }
    public string SenderEmoji { get; }
    public string? Message { get; }
    public bool HasMessage => !string.IsNullOrWhiteSpace(Message);
    public bool IsThanked { get; set; }

    public override string Description =>
        $"Geschenk von {SenderName}";

    public override string CategoryEmoji => "🎁";

    public override Color CategoryBackgroundColor =>
        Color.FromArgb("#F8BBD0"); // Pink Light

    public GiftTransactionViewModel(TransactionResponse response)
        : base(response)
    {
        SenderName = response.GiftSender?.Name ?? "Unbekannt";
        SenderEmoji = response.GiftSender?.RelationType switch
        {
            "Grandmother" => "👵",
            "Grandfather" => "👴",
            "Aunt" => "👩",
            "Uncle" => "👨",
            _ => "👤"
        };
        Message = response.GiftMessage;
        IsThanked = response.GiftThanked ?? false;
    }
}
```

### ChildDashboardViewModel - Geschenk-Check
```csharp
public partial class ChildDashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private GiftTransactionViewModel? _newGift;

    [ObservableProperty]
    private bool _hasNewGift;

    private async Task CheckForNewGiftsAsync()
    {
        var response = await _accountApi.GetUnseenGiftsAsync();
        if (response.IsSuccessStatusCode && response.Content?.Any() == true)
        {
            var latestGift = response.Content.First();
            NewGift = new GiftTransactionViewModel(latestGift);
            HasNewGift = true;
        }
    }

    [RelayCommand]
    private async Task DismissGiftBannerAsync()
    {
        if (NewGift != null)
        {
            await _accountApi.MarkGiftAsSeenAsync(NewGift.Id);
            HasNewGift = false;
            NewGift = null;
        }
    }

    [RelayCommand]
    private async Task ViewGiftAsync()
    {
        if (NewGift != null)
        {
            await _navigationService.NavigateToAsync("transaction-detail",
                new Dictionary<string, object> { { "TransactionId", NewGift.Id } });
            await DismissGiftBannerAsync();
        }
    }

    [RelayCommand]
    private async Task SendThankYouAsync()
    {
        if (NewGift != null)
        {
            await _navigationService.NavigateToAsync("send-thank-you",
                new Dictionary<string, object>
                {
                    { "GiftId", NewGift.Id },
                    { "SenderName", NewGift.SenderName }
                });
        }
    }
}
```

### Push-Notification für Geschenke
```csharp
public class GiftNotificationService
{
    public async Task SendGiftNotificationAsync(
        string childName,
        string senderName,
        decimal amount,
        string? message)
    {
        var body = $"{senderName} hat dir {amount:C} geschenkt!";
        if (!string.IsNullOrWhiteSpace(message))
        {
            body += $"\n\"{message}\"";
        }

        var notification = new LocalNotification
        {
            Title = $"🎁 Neues Geschenk für {childName}!",
            Body = body,
            CategoryType = NotificationCategoryType.Event,
            Android = new AndroidOptions
            {
                ChannelId = "gifts",
                Priority = NotificationPriority.High
            }
        };

        await LocalNotificationCenter.Current.Show(notification);
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-28 | Neues Geschenk erhalten | Banner auf Dashboard |
| TC-M004-29 | Geschenk in Liste | Absender angezeigt |
| TC-M004-30 | Geschenk-Detail öffnen | Nachricht sichtbar |
| TC-M004-31 | "Danke sagen" klicken | Navigation zur Danke-Seite |
| TC-M004-32 | Push-Notification | Benachrichtigung erscheint |

## Story Points
1

## Priorität
Mittel
