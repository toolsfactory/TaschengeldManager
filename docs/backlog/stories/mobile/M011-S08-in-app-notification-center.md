# Story M011-S08: In-App Benachrichtigungs-Center

## Epic

M011 - Push-Benachrichtigungen

## User Story

Als **Benutzer** moechte ich **alle meine Benachrichtigungen in der App einsehen koennen**, damit **ich auch aeltere Benachrichtigungen nachlesen kann und nichts verpasse**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er das Benachrichtigungs-Center oeffnet, dann sieht er alle seine Benachrichtigungen
- [ ] Gegeben ungelesene Benachrichtigungen, wenn der Benutzer das Icon sieht, dann wird die Anzahl als Badge angezeigt
- [ ] Gegeben eine Benachrichtigung, wenn der Benutzer darauf tippt, dann wird er zur entsprechenden Seite geleitet
- [ ] Gegeben eine Benachrichtigung, wenn der Benutzer sie oeffnet, dann wird sie als gelesen markiert
- [ ] Gegeben viele Benachrichtigungen, wenn der Benutzer scrollt, dann werden aeltere Benachrichtigungen nachgeladen

## UI-Entwurf

### Icon mit Badge
```
+------------------------------------+
|  TaschengeldManager      [Bell 3]  |
+------------------------------------+
```

### Benachrichtigungs-Center
```
+------------------------------------+
|  <- Zurueck    Benachrichtigungen  |
+------------------------------------+
|                                    |
|  Heute                             |
|  +--------------------------------+|
|  | [NEW] Anfrage genehmigt!      ||
|  |       Deine Anfrage fuer      ||
|  |       "Videospiel" wurde...   ||
|  |       vor 2 Stunden           ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [NEW] Geschenk von Oma Maria  ||
|  |       Du hast 20,00 EUR       ||
|  |       erhalten...             ||
|  |       vor 5 Stunden           ||
|  +--------------------------------+|
|                                    |
|  Gestern                           |
|  +--------------------------------+|
|  | Taschengeld eingegangen       ||
|  |       Dein woechentliches     ||
|  |       Taschengeld von...      ||
|  |       gestern, 08:00          ||
|  +--------------------------------+|
|                                    |
|  Aeltere                           |
|  +--------------------------------+|
|  | Anfrage abgelehnt             ||
|  |       Deine Anfrage fuer      ||
|  |       "Schuhe" wurde...       ||
|  |       15.01.2026              ||
|  +--------------------------------+|
|                                    |
|  [Alle als gelesen markieren]      |
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Benachrichtigungen abrufen
```
GET /api/notifications?page=1&pageSize=20
Authorization: Bearer {token}

Response 200:
{
  "notifications": [
    {
      "id": "guid",
      "type": "money_request_approved",
      "title": "Anfrage genehmigt!",
      "body": "Deine Anfrage fuer \"Videospiel\" wurde genehmigt.",
      "data": {
        "requestId": "guid"
      },
      "isRead": false,
      "createdAt": "2026-01-20T14:30:00Z"
    }
  ],
  "totalCount": 45,
  "unreadCount": 3,
  "page": 1,
  "pageSize": 20
}
```

### Ungelesene Anzahl
```
GET /api/notifications/unread-count
Authorization: Bearer {token}

Response 200:
{
  "count": 3
}
```

### Als gelesen markieren
```
PUT /api/notifications/{id}/read
Authorization: Bearer {token}

Response 200:
{
  "success": true
}
```

### Alle als gelesen markieren
```
PUT /api/notifications/read-all
Authorization: Bearer {token}

Response 200:
{
  "markedCount": 3
}
```

## Technische Notizen

- Benachrichtigungen werden im Backend gespeichert (nicht nur Push)
- Badge-Count wird bei App-Start und nach Aktionen aktualisiert
- Gruppierung nach Datum (Heute, Gestern, Aeltere)
- Pagination fuer Performance

## Implementierungshinweise

```csharp
// ViewModel
public partial class NotificationCenterViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private int _currentPage = 1;
    private bool _hasMoreItems = true;

    public ObservableCollection<NotificationGroup> NotificationGroups { get; } = new();

    [ObservableProperty]
    private int _unreadCount;

    [RelayCommand]
    private async Task LoadNotificationsAsync()
    {
        var result = await _notificationService.GetNotificationsAsync(_currentPage, 20);

        var grouped = result.Notifications
            .GroupBy(n => GetDateGroup(n.CreatedAt))
            .Select(g => new NotificationGroup(g.Key, g.ToList()));

        foreach (var group in grouped)
        {
            var existing = NotificationGroups.FirstOrDefault(g => g.Title == group.Title);
            if (existing != null)
            {
                foreach (var item in group)
                {
                    existing.Add(item);
                }
            }
            else
            {
                NotificationGroups.Add(group);
            }
        }

        UnreadCount = result.UnreadCount;
        _hasMoreItems = NotificationGroups.Sum(g => g.Count) < result.TotalCount;
        _currentPage++;
    }

    private string GetDateGroup(DateTime date)
    {
        if (date.Date == DateTime.Today) return "Heute";
        if (date.Date == DateTime.Today.AddDays(-1)) return "Gestern";
        return "Aeltere";
    }

    [RelayCommand]
    private async Task OpenNotificationAsync(NotificationItem notification)
    {
        // Als gelesen markieren
        if (!notification.IsRead)
        {
            await _notificationService.MarkAsReadAsync(notification.Id);
            notification.IsRead = true;
            UnreadCount = Math.Max(0, UnreadCount - 1);
        }

        // Navigation basierend auf Typ
        await NavigateToNotificationTarget(notification);
    }

    [RelayCommand]
    private async Task MarkAllAsReadAsync()
    {
        await _notificationService.MarkAllAsReadAsync();

        foreach (var group in NotificationGroups)
        {
            foreach (var notification in group)
            {
                notification.IsRead = true;
            }
        }

        UnreadCount = 0;
        await _toastService.ShowSuccessAsync("Alle als gelesen markiert");
    }

    private async Task NavigateToNotificationTarget(NotificationItem notification)
    {
        var route = notification.Type switch
        {
            "money_request_approved" or "money_request_rejected" =>
                $"//requests/detail?id={notification.Data["requestId"]}",
            "money_request_new" =>
                $"//requests/detail?id={notification.Data["requestId"]}",
            "gift_received" =>
                $"//gifts/detail?id={notification.Data["giftId"]}",
            "allowance_received" or "interest_credited" =>
                "//dashboard",
            _ => "//dashboard"
        };

        await Shell.Current.GoToAsync(route);
    }
}

// Badge in Shell
public partial class AppShellViewModel : ObservableObject
{
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private int _notificationBadge;

    public async Task RefreshBadgeAsync()
    {
        NotificationBadge = await _notificationService.GetUnreadCountAsync();
    }
}

// XAML Badge
<Shell.FlyoutHeader>
    <Grid>
        <ImageButton Source="bell.png"
                     Command="{Binding OpenNotificationsCommand}"/>
        <Border BackgroundColor="Red"
                WidthRequest="20" HeightRequest="20"
                IsVisible="{Binding NotificationBadge, Converter={StaticResource GreaterThanZero}}">
            <Label Text="{Binding NotificationBadge}"
                   TextColor="White"
                   FontSize="12"
                   HorizontalOptions="Center"
                   VerticalOptions="Center"/>
        </Border>
    </Grid>
</Shell.FlyoutHeader>
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Benachrichtigungen laden | Liste wird angezeigt |
| TC-002 | 3 ungelesene | Badge zeigt "3" |
| TC-003 | Auf Benachrichtigung tippen | Als gelesen markiert + Navigation |
| TC-004 | Alle als gelesen | Badge verschwindet |
| TC-005 | Scrollen fuer mehr | Aeltere werden nachgeladen |
| TC-006 | Gruppierung | Heute, Gestern, Aeltere korrekt |

## Story Points

3

## Prioritaet

Mittel
