# Story M002-S11: Session-Verwaltung

## Epic
M002 - Authentifizierung

## Status
Offen

## User Story

Als **sicherheitsbewusster Benutzer** möchte ich **meine aktiven Sessions einsehen und verwalten können**, damit **ich unbefugten Zugriff erkennen und beenden kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Einstellungen, wenn der Benutzer "Aktive Sessions" öffnet, dann werden alle Sessions angezeigt
- [ ] Gegeben eine Session-Liste, wenn eine Session angezeigt wird, dann sind Gerät, Ort und letzte Aktivität sichtbar
- [ ] Gegeben eine fremde Session, wenn der Benutzer sie beendet, dann wird sie serverseitig invalidiert
- [ ] Gegeben die aktuelle Session, wenn sie markiert ist, dann kann sie nicht beendet werden

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Aktive Sessions  │
├─────────────────────────────┤
│                             │
│  Aktuelle Session           │
│  ┌───────────────────────┐  │
│  │ 📱 iPhone 14 Pro      │  │
│  │ TaschengeldManager    │  │
│  │ Berlin, Deutschland   │  │
│  │ Aktiv: Jetzt          │  │
│  │ ● Dieses Gerät        │  │
│  └───────────────────────┘  │
│                             │
│  Andere Sessions            │
│  ┌───────────────────────┐  │
│  │ 📱 Samsung Galaxy S23 │  │
│  │ TaschengeldManager    │  │
│  │ München, Deutschland  │  │
│  │ Aktiv: Vor 2 Stunden  │  │
│  │           [Beenden]   │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │ 💻 Chrome Browser     │  │
│  │ Web                   │  │
│  │ Hamburg, Deutschland  │  │
│  │ Aktiv: Vor 3 Tagen    │  │
│  │           [Beenden]   │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │ Alle anderen Sessions │  │
│  │     beenden           │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### SessionsPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.SessionsPage"
             x:DataType="vm:SessionsViewModel"
             Title="Aktive Sessions">

    <RefreshView IsRefreshing="{Binding IsRefreshing}"
                 Command="{Binding RefreshCommand}">
        <ScrollView>
            <VerticalStackLayout Padding="16" Spacing="16">

                <!-- Aktuelle Session -->
                <Label Text="Aktuelle Session"
                       FontSize="14"
                       FontAttributes="Bold"
                       TextColor="{StaticResource TextSecondaryLight}" />

                <Frame Padding="16"
                       CornerRadius="8"
                       BorderColor="{StaticResource Primary}"
                       IsVisible="{Binding CurrentSession, Converter={StaticResource IsNotNullConverter}}">
                    <Grid RowDefinitions="Auto,Auto,Auto,Auto" ColumnDefinitions="Auto,*">
                        <Label Grid.Row="0" Grid.ColumnSpan="2"
                               Text="{Binding CurrentSession.DeviceName}"
                               FontSize="16"
                               FontAttributes="Bold" />
                        <Label Grid.Row="1" Grid.ColumnSpan="2"
                               Text="{Binding CurrentSession.AppName}"
                               FontSize="12" />
                        <Label Grid.Row="2" Grid.ColumnSpan="2"
                               Text="{Binding CurrentSession.Location}"
                               FontSize="12"
                               TextColor="{StaticResource TextSecondaryLight}" />
                        <HorizontalStackLayout Grid.Row="3" Grid.ColumnSpan="2" Spacing="8">
                            <Ellipse WidthRequest="8" HeightRequest="8"
                                     Fill="Green"
                                     VerticalOptions="Center" />
                            <Label Text="Dieses Gerät"
                                   FontSize="12"
                                   TextColor="Green" />
                        </HorizontalStackLayout>
                    </Grid>
                </Frame>

                <!-- Andere Sessions -->
                <Label Text="Andere Sessions"
                       FontSize="14"
                       FontAttributes="Bold"
                       TextColor="{StaticResource TextSecondaryLight}"
                       IsVisible="{Binding HasOtherSessions}" />

                <CollectionView ItemsSource="{Binding OtherSessions}"
                                SelectionMode="None">
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="vm:SessionItemViewModel">
                            <Frame Padding="16" CornerRadius="8" Margin="0,0,0,8">
                                <Grid RowDefinitions="Auto,Auto,Auto,Auto"
                                      ColumnDefinitions="*,Auto">
                                    <Label Grid.Row="0"
                                           Text="{Binding DeviceName}"
                                           FontSize="16"
                                           FontAttributes="Bold" />
                                    <Label Grid.Row="1"
                                           Text="{Binding AppName}"
                                           FontSize="12" />
                                    <Label Grid.Row="2"
                                           Text="{Binding Location}"
                                           FontSize="12"
                                           TextColor="{StaticResource TextSecondaryLight}" />
                                    <Label Grid.Row="3"
                                           Text="{Binding LastActiveText}"
                                           FontSize="12"
                                           TextColor="{StaticResource TextSecondaryLight}" />
                                    <Button Grid.RowSpan="4" Grid.Column="1"
                                            Text="Beenden"
                                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:SessionsViewModel}}, Path=RevokeSessionCommand}"
                                            CommandParameter="{Binding SessionId}"
                                            Style="{StaticResource SmallDangerButton}"
                                            VerticalOptions="Center" />
                                </Grid>
                            </Frame>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>

                <!-- Keine anderen Sessions -->
                <Label Text="Keine anderen aktiven Sessions"
                       HorizontalTextAlignment="Center"
                       TextColor="{StaticResource TextSecondaryLight}"
                       IsVisible="{Binding HasNoOtherSessions}" />

                <!-- Alle beenden Button -->
                <Button Text="Alle anderen Sessions beenden"
                        Command="{Binding RevokeAllOtherSessionsCommand}"
                        Style="{StaticResource DangerButton}"
                        IsVisible="{Binding HasOtherSessions}"
                        Margin="0,16,0,0" />

            </VerticalStackLayout>
        </ScrollView>
    </RefreshView>

</ContentPage>
```

### SessionsViewModel.cs
```csharp
public partial class SessionsViewModel : ObservableObject
{
    private readonly ISessionApi _sessionApi;
    private readonly ITokenService _tokenService;

    [ObservableProperty]
    private SessionItemViewModel? _currentSession;

    [ObservableProperty]
    private ObservableCollection<SessionItemViewModel> _otherSessions = new();

    [ObservableProperty]
    private bool _isRefreshing;

    public bool HasOtherSessions => OtherSessions.Any();
    public bool HasNoOtherSessions => !OtherSessions.Any();

    [RelayCommand]
    private async Task RefreshAsync()
    {
        try
        {
            IsRefreshing = true;

            var response = await _sessionApi.GetSessionsAsync();
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var currentSessionId = await GetCurrentSessionIdAsync();

                CurrentSession = response.Content
                    .Where(s => s.SessionId == currentSessionId)
                    .Select(s => new SessionItemViewModel(s, true))
                    .FirstOrDefault();

                OtherSessions = new ObservableCollection<SessionItemViewModel>(
                    response.Content
                        .Where(s => s.SessionId != currentSessionId)
                        .Select(s => new SessionItemViewModel(s, false)));

                OnPropertyChanged(nameof(HasOtherSessions));
                OnPropertyChanged(nameof(HasNoOtherSessions));
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RevokeSessionAsync(string sessionId)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Session beenden",
            "Möchtest du diese Session wirklich beenden?",
            "Beenden",
            "Abbrechen");

        if (!confirm) return;

        var response = await _sessionApi.RevokeSessionAsync(sessionId);
        if (response.IsSuccessStatusCode)
        {
            var session = OtherSessions.FirstOrDefault(s => s.SessionId == sessionId);
            if (session != null)
            {
                OtherSessions.Remove(session);
                OnPropertyChanged(nameof(HasOtherSessions));
                OnPropertyChanged(nameof(HasNoOtherSessions));
            }
        }
    }

    [RelayCommand]
    private async Task RevokeAllOtherSessionsAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Alle Sessions beenden",
            "Möchtest du wirklich alle anderen Sessions beenden?",
            "Alle beenden",
            "Abbrechen");

        if (!confirm) return;

        var response = await _sessionApi.RevokeAllOtherSessionsAsync();
        if (response.IsSuccessStatusCode)
        {
            OtherSessions.Clear();
            OnPropertyChanged(nameof(HasOtherSessions));
            OnPropertyChanged(nameof(HasNoOtherSessions));
        }
    }

    private async Task<string> GetCurrentSessionIdAsync()
    {
        var claims = await _tokenService.GetUserClaimsAsync();
        return claims?.SessionId ?? string.Empty;
    }
}
```

### API-Interface
```csharp
public interface ISessionApi
{
    [Get("/api/sessions")]
    Task<ApiResponse<List<SessionResponse>>> GetSessionsAsync();

    [Delete("/api/sessions/{sessionId}")]
    Task<ApiResponse<EmptyResponse>> RevokeSessionAsync(string sessionId);

    [Delete("/api/sessions/others")]
    Task<ApiResponse<EmptyResponse>> RevokeAllOtherSessionsAsync();
}

public record SessionResponse(
    string SessionId,
    string DeviceName,
    string AppName,
    string Location,
    DateTime LastActiveAt,
    bool IsCurrent);
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M002-45 | Sessions laden | Alle Sessions angezeigt |
| TC-M002-46 | Einzelne Session beenden | Session entfernt |
| TC-M002-47 | Alle anderen beenden | Nur aktuelle bleibt |
| TC-M002-48 | Aktuelle Session | Kein Beenden-Button |

## Story Points
2

## Priorität
Niedrig
