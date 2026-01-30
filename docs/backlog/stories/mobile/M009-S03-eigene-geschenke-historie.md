# Story M009-S03: Eigene Geschenke-Historie

## Epic

M009 - Geschenke (Verwandten-Rolle)

## User Story

Als **Verwandter** moechte ich **eine Uebersicht aller meiner gesendeten Geschenke sehen koennen**, damit **ich nachvollziehen kann, wem ich wann wie viel geschenkt habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Verwandter, wenn er die Geschenke-Historie oeffnet, dann sieht er alle seine gesendeten Geschenke
- [ ] Gegeben mehrere Geschenke, wenn die Liste angezeigt wird, dann sind sie nach Datum sortiert (neueste zuerst)
- [ ] Gegeben ein Geschenk in der Liste, wenn der Verwandte es sieht, dann wird Empfaenger, Betrag, Datum und Nachricht angezeigt
- [ ] Gegeben keine Geschenke, wenn der Verwandte die Historie oeffnet, dann wird eine entsprechende Meldung angezeigt
- [ ] Gegeben viele Geschenke, wenn der Verwandte scrollt, dann werden weitere Eintraege nachgeladen (Pagination)

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck      Meine Geschenke   |
+------------------------------------+
|                                    |
|  +--------------------------------+|
|  | [Gift] Max          20,00 EUR ||
|  |        "Zum Geburtstag!"       ||
|  |        15.01.2026              ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Gift] Lisa         15,00 EUR ||
|  |        "Fuer gute Noten"       ||
|  |        10.01.2026              ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Gift] Max          50,00 EUR ||
|  |        "Weihnachten"           ||
|  |        24.12.2025              ||
|  +--------------------------------+|
|                                    |
|  Gesamt 2026: 35,00 EUR            |
|  Gesamt alle: 85,00 EUR            |
|                                    |
+------------------------------------+
```

## API-Endpunkt

```
GET /api/gifts/history?page=1&pageSize=20
Authorization: Bearer {token}

Response 200:
{
  "gifts": [
    {
      "id": "guid",
      "childId": "guid",
      "childName": "Max",
      "childAvatarUrl": "string | null",
      "amount": 20.00,
      "message": "Zum Geburtstag!",
      "createdAt": "2026-01-15T14:30:00Z"
    }
  ],
  "totalCount": 15,
  "page": 1,
  "pageSize": 20,
  "summary": {
    "currentYear": 35.00,
    "allTime": 85.00
  }
}
```

## Technische Notizen

- ViewModel: `GiftHistoryViewModel` mit ObservableCollection<GiftHistoryItem>
- Service: `IGiftService.GetHistoryAsync(page, pageSize)`
- Infinite Scroll mit IncrementalLoadingCollection
- Zusammenfassung am Ende der Liste (Jahressumme, Gesamtsumme)

## Implementierungshinweise

```csharp
public partial class GiftHistoryViewModel : BaseViewModel
{
    private readonly IGiftService _giftService;
    private int _currentPage = 1;
    private bool _hasMoreItems = true;

    public ObservableCollection<GiftHistoryItem> Gifts { get; } = new();

    [ObservableProperty]
    private decimal _currentYearTotal;

    [ObservableProperty]
    private decimal _allTimeTotal;

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (!_hasMoreItems || IsBusy) return;

        IsBusy = true;
        var result = await _giftService.GetHistoryAsync(_currentPage, 20);

        foreach (var gift in result.Gifts)
        {
            Gifts.Add(gift);
        }

        CurrentYearTotal = result.Summary.CurrentYear;
        AllTimeTotal = result.Summary.AllTime;

        _hasMoreItems = Gifts.Count < result.TotalCount;
        _currentPage++;
        IsBusy = false;
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Verwandter mit 5 Geschenken | Liste zeigt alle 5 chronologisch |
| TC-002 | Verwandter ohne Geschenke | Hinweismeldung wird angezeigt |
| TC-003 | Mehr als 20 Geschenke | Pagination laedt weitere nach |
| TC-004 | Summen-Anzeige | Jahres- und Gesamtsumme korrekt |

## Story Points

2

## Prioritaet

Mittel
