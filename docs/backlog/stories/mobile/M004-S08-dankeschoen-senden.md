# Story M004-S08: Dankeschön an Verwandten senden

## Epic
M004 - Kontoverwaltung Kind

## Status
Offen

## User Story

Als **Kind** möchte ich **einem Verwandten für ein Geschenk danken können**, damit **ich meine Dankbarkeit zeigen kann und der Verwandte weiß, dass das Geschenk angekommen ist**.

## Akzeptanzkriterien

- [ ] Gegeben ein Geschenk, wenn "Danke sagen" gewählt wird, dann kann eine Nachricht verfasst werden
- [ ] Gegeben die Danke-Seite, wenn sie geöffnet wird, dann werden Vorschläge für Nachrichten angezeigt
- [ ] Gegeben eine gesendete Danke-Nachricht, wenn sie verschickt wird, dann erhält der Verwandte eine Benachrichtigung
- [ ] Gegeben ein bedanktes Geschenk, wenn es in der Liste angezeigt wird, dann ist es als "bedankt" markiert

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Danke sagen      │
├─────────────────────────────┤
│                             │
│        💌                   │
│                             │
│  Danke an Oma Helga         │
│  für 20,00 €                │
│                             │
│  ┌─────────────────────────┐│
│  │ Deine Nachricht:        ││
│  │                         ││
│  │ Vielen Dank für das     ││
│  │ tolle Geschenk! Ich     ││
│  │ freue mich sehr! 💕     ││
│  │                         ││
│  │                         ││
│  │              123/200    ││
│  └─────────────────────────┘│
│                             │
│  Oder wähle eine Vorlage:   │
│  ┌─────────────────────────┐│
│  │ Danke, das war super    ││
│  │ lieb von dir! 😊        ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ Vielen Dank für das     ││
│  │ Geschenk! Ich hab mich  ││
│  │ riesig gefreut! 🎉      ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ Dankeschön! Du bist die ││
│  │ beste Oma/Opa! ❤️       ││
│  └─────────────────────────┘│
│                             │
│  ┌───────────────────────┐  │
│  │  💌 Nachricht senden  │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### SendThankYouPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.SendThankYouPage"
             x:DataType="vm:SendThankYouViewModel"
             Title="Danke sagen">

    <ScrollView>
        <VerticalStackLayout Padding="24" Spacing="16">

            <!-- Icon -->
            <Label Text="💌"
                   FontSize="64"
                   HorizontalTextAlignment="Center" />

            <!-- Header -->
            <Label HorizontalTextAlignment="Center" FontSize="16">
                <Label.FormattedText>
                    <FormattedString>
                        <Span Text="Danke an " />
                        <Span Text="{Binding SenderName}"
                              FontAttributes="Bold" />
                        <Span Text="{Binding Amount, StringFormat=' für {0:C}'}" />
                    </FormattedString>
                </Label.FormattedText>
            </Label>

            <!-- Nachricht-Eingabe -->
            <Frame Padding="16" CornerRadius="12">
                <VerticalStackLayout Spacing="8">
                    <Label Text="Deine Nachricht:"
                           FontSize="14"
                           TextColor="{StaticResource TextSecondaryLight}" />
                    <Editor Text="{Binding Message}"
                            Placeholder="Schreibe hier deine Nachricht..."
                            MaxLength="200"
                            HeightRequest="120"
                            AutoSize="TextChanges" />
                    <Label Text="{Binding CharacterCount, StringFormat='{0}/200'}"
                           FontSize="12"
                           TextColor="{StaticResource TextSecondaryLight}"
                           HorizontalTextAlignment="End" />
                </VerticalStackLayout>
            </Frame>

            <!-- Vorlagen -->
            <Label Text="Oder wähle eine Vorlage:"
                   FontSize="14"
                   TextColor="{StaticResource TextSecondaryLight}" />

            <VerticalStackLayout BindableLayout.ItemsSource="{Binding Templates}"
                                 Spacing="8">
                <BindableLayout.ItemTemplate>
                    <DataTemplate x:DataType="vm:ThankYouTemplateViewModel">
                        <Frame Padding="12"
                               CornerRadius="8"
                               BorderColor="{Binding BorderColor}">
                            <Frame.GestureRecognizers>
                                <TapGestureRecognizer
                                    Command="{Binding Source={RelativeSource AncestorType={x:Type vm:SendThankYouViewModel}}, Path=SelectTemplateCommand}"
                                    CommandParameter="{Binding}" />
                            </Frame.GestureRecognizers>
                            <Label Text="{Binding Text}"
                                   FontSize="14" />
                        </Frame>
                    </DataTemplate>
                </BindableLayout.ItemTemplate>
            </VerticalStackLayout>

            <!-- Emoji-Auswahl -->
            <Label Text="Füge ein Emoji hinzu:"
                   FontSize="14"
                   TextColor="{StaticResource TextSecondaryLight}" />

            <HorizontalStackLayout HorizontalOptions="Center" Spacing="16">
                <Label Text="❤️" FontSize="32">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer
                            Command="{Binding AddEmojiCommand}"
                            CommandParameter="❤️" />
                    </Label.GestureRecognizers>
                </Label>
                <Label Text="😊" FontSize="32">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer
                            Command="{Binding AddEmojiCommand}"
                            CommandParameter="😊" />
                    </Label.GestureRecognizers>
                </Label>
                <Label Text="🎉" FontSize="32">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer
                            Command="{Binding AddEmojiCommand}"
                            CommandParameter="🎉" />
                    </Label.GestureRecognizers>
                </Label>
                <Label Text="💕" FontSize="32">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer
                            Command="{Binding AddEmojiCommand}"
                            CommandParameter="💕" />
                    </Label.GestureRecognizers>
                </Label>
                <Label Text="🤗" FontSize="32">
                    <Label.GestureRecognizers>
                        <TapGestureRecognizer
                            Command="{Binding AddEmojiCommand}"
                            CommandParameter="🤗" />
                    </Label.GestureRecognizers>
                </Label>
            </HorizontalStackLayout>

            <!-- Senden Button -->
            <Button Text="💌 Nachricht senden"
                    Command="{Binding SendCommand}"
                    Style="{StaticResource PrimaryButton}"
                    IsEnabled="{Binding CanSend}"
                    Margin="0,16,0,0" />

            <ActivityIndicator IsRunning="{Binding IsBusy}"
                               IsVisible="{Binding IsBusy}" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### SendThankYouViewModel.cs
```csharp
public partial class SendThankYouViewModel : ObservableObject, IQueryAttributable
{
    private readonly IGiftApi _giftApi;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Guid _giftId;

    [ObservableProperty]
    private string _senderName = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CharacterCount))]
    [NotifyPropertyChangedFor(nameof(CanSend))]
    private string _message = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ThankYouTemplateViewModel> _templates;

    [ObservableProperty]
    private bool _isBusy;

    public int CharacterCount => Message.Length;
    public bool CanSend => !string.IsNullOrWhiteSpace(Message) && !IsBusy;

    public SendThankYouViewModel(IGiftApi giftApi, INavigationService navigationService)
    {
        _giftApi = giftApi;
        _navigationService = navigationService;

        Templates = new ObservableCollection<ThankYouTemplateViewModel>
        {
            new("Danke, das war super lieb von dir! 😊"),
            new("Vielen Dank für das Geschenk! Ich hab mich riesig gefreut! 🎉"),
            new("Dankeschön! Du bist die/der Beste! ❤️"),
            new("Wow, danke! Das ist ja toll! 🤗"),
            new("Tausend Dank! Ich freu mich so! 💕")
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("GiftId", out var giftId))
            GiftId = (Guid)giftId;

        if (query.TryGetValue("SenderName", out var senderName))
            SenderName = senderName.ToString() ?? "";

        if (query.TryGetValue("Amount", out var amount))
            Amount = (decimal)amount;
    }

    [RelayCommand]
    private void SelectTemplate(ThankYouTemplateViewModel template)
    {
        // Alle deselektieren
        foreach (var t in Templates)
        {
            t.IsSelected = t == template;
        }

        Message = template.Text;
    }

    [RelayCommand]
    private void AddEmoji(string emoji)
    {
        if (Message.Length < 198) // 200 - emoji length
        {
            Message += emoji;
        }
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        if (!CanSend) return;

        try
        {
            IsBusy = true;

            var response = await _giftApi.SendThankYouAsync(GiftId, new SendThankYouRequest
            {
                Message = Message
            });

            if (response.IsSuccessStatusCode)
            {
                // Messenger für Liste-Update
                WeakReferenceMessenger.Default.Send(new GiftThankedMessage(GiftId));

                await _navigationService.GoBackAsync();

                // Erfolgs-Animation/Toast
                await Toast.Make("Deine Nachricht wurde gesendet! 💌", ToastDuration.Long).Show();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Fehler",
                    "Die Nachricht konnte nicht gesendet werden. Bitte versuche es erneut.",
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public partial class ThankYouTemplateViewModel : ObservableObject
{
    public string Text { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BorderColor))]
    private bool _isSelected;

    public Color BorderColor => IsSelected
        ? Color.FromArgb("#4CAF50")
        : Colors.Transparent;

    public ThankYouTemplateViewModel(string text)
    {
        Text = text;
    }
}
```

### API-Interface
```csharp
public interface IGiftApi
{
    [Post("/api/gifts/{giftId}/thank")]
    Task<ApiResponse<EmptyResponse>> SendThankYouAsync(
        Guid giftId,
        [Body] SendThankYouRequest request);
}

public record SendThankYouRequest(string Message);
```

### Markierung in der Transaktionsliste
```xml
<!-- In TransactionItemTemplate -->
<Label Text="✓ Bedankt"
       FontSize="10"
       TextColor="{StaticResource Primary}"
       IsVisible="{Binding IsThanked}" />
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-33 | Danke-Seite öffnen | Absender und Vorlagen angezeigt |
| TC-M004-34 | Vorlage auswählen | Text wird übernommen |
| TC-M004-35 | Emoji hinzufügen | Emoji erscheint im Text |
| TC-M004-36 | Nachricht senden | Erfolgsmeldung, zurück navigiert |
| TC-M004-37 | Geschenk bedankt | "Bedankt"-Markierung in Liste |

## Story Points
2

## Priorität
Mittel
