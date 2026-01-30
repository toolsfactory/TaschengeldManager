# Story M009-S02: Geschenk senden

## Epic

M009 - Geschenke (Verwandten-Rolle)

## User Story

Als **Verwandter** moechte ich **einem Kind einen Geldbetrag mit einer persoenlichen Nachricht schenken koennen**, damit **das Kind sich ueber mein Geschenk freut und es direkt auf seinem Konto hat**.

## Akzeptanzkriterien

- [ ] Gegeben ein ausgewaehltes Kind, wenn der Verwandte das Geschenk-Formular sieht, dann kann er einen Betrag eingeben
- [ ] Gegeben ein Betrag von 0 oder negativ, wenn der Verwandte absenden will, dann wird ein Validierungsfehler angezeigt
- [ ] Gegeben ein gueltiger Betrag, wenn der Verwandte eine Nachricht hinzufuegt, dann wird diese mit dem Geschenk gespeichert
- [ ] Gegeben alle Daten sind gueltig, wenn der Verwandte auf "Senden" tippt, dann wird das Geschenk verarbeitet und eine Erfolgsmeldung angezeigt
- [ ] Gegeben ein erfolgreiches Geschenk, wenn der Prozess abgeschlossen ist, dann wird das Kind per Push benachrichtigt

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck    Geschenk an Max     |
+------------------------------------+
|                                    |
|              [Gift-Icon]           |
|                                    |
|  Wie viel moechtest du schenken?   |
|  +--------------------------------+|
|  |          20,00 EUR             ||
|  +--------------------------------+|
|                                    |
|  Persoenliche Nachricht (optional) |
|  +--------------------------------+|
|  |                                ||
|  | Alles Gute zum Geburtstag,     ||
|  | mein Schatz!                   ||
|  |                                ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |      [Gift] Geschenk senden    ||
|  +--------------------------------+|
|                                    |
|  Max freut sich bestimmt!          |
|                                    |
+------------------------------------+
```

## API-Endpunkt

```
POST /api/gifts
Authorization: Bearer {token}
Content-Type: application/json

{
  "childId": "guid",
  "amount": 20.00,
  "message": "Alles Gute zum Geburtstag!"
}

Response 201:
{
  "giftId": "guid",
  "childId": "guid",
  "amount": 20.00,
  "message": "Alles Gute zum Geburtstag!",
  "createdAt": "2026-01-20T10:30:00Z"
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss groesser als 0 sein"],
    "message": ["Nachricht darf maximal 500 Zeichen haben"]
  }
}

Response 403:
{
  "error": "Kein Zugang zu diesem Kind"
}
```

## Technische Notizen

- ViewModel: `SendGiftViewModel` mit Betrag und Nachricht
- Service: `IGiftService.SendGiftAsync(childId, amount, message)`
- Waehrungs-Eingabe mit Decimal-Validierung
- Nachricht optional, max. 500 Zeichen
- Toast-Erfolg: "Geschenk wurde an {Name} gesendet!"

## Implementierungshinweise

```csharp
public partial class SendGiftViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IGiftService _giftService;
    private Guid _childId;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string _childName = string.Empty;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("childId", out var id))
        {
            _childId = Guid.Parse(id.ToString());
        }
    }

    [RelayCommand]
    private async Task SendGiftAsync()
    {
        if (Amount <= 0)
        {
            await _toastService.ShowErrorAsync("Bitte gib einen gueltigen Betrag ein");
            return;
        }

        var result = await _giftService.SendGiftAsync(_childId, Amount, Message);
        if (result.IsSuccess)
        {
            await _toastService.ShowSuccessAsync($"Geschenk wurde an {ChildName} gesendet!");
            await Shell.Current.GoToAsync("..");
        }
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Betrag 20 EUR mit Nachricht | Geschenk erfolgreich gesendet |
| TC-002 | Betrag 0 EUR | Validierungsfehler |
| TC-003 | Betrag ohne Nachricht | Geschenk erfolgreich (Nachricht optional) |
| TC-004 | Nachricht > 500 Zeichen | Validierungsfehler |
| TC-005 | Kind nicht zugaenglich | 403 Forbidden |

## Story Points

2

## Prioritaet

Mittel
