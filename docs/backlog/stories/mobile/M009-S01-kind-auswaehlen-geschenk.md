# Story M009-S01: Kind auswaehlen fuer Geschenk

## Epic

M009 - Geschenke (Verwandten-Rolle)

## User Story

Als **Verwandter** moechte ich **aus einer Liste der verbundenen Kinder eines auswaehlen koennen**, damit **ich diesem Kind ein Geschenk senden kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Verwandter, wenn er das Geschenke-Feature oeffnet, dann sieht er alle Kinder der Familie, zu der er Zugang hat
- [ ] Gegeben mehrere Kinder, wenn der Verwandte die Liste sieht, dann werden Name und Avatar jedes Kindes angezeigt
- [ ] Gegeben ein Kind in der Liste, wenn der Verwandte darauf tippt, dann wird er zur Geschenk-senden-Seite weitergeleitet
- [ ] Gegeben keine verbundenen Kinder, wenn der Verwandte das Feature oeffnet, dann wird eine entsprechende Meldung angezeigt

## UI-Entwurf

```
+------------------------------------+
|  <- Zurueck      Geschenk senden   |
+------------------------------------+
|                                    |
|  Waehle ein Kind aus:              |
|                                    |
|  +--------------------------------+|
|  | [Avatar] Max                   ||
|  |          12 Jahre              ||
|  |                            >   ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Avatar] Lisa                  ||
|  |          9 Jahre               ||
|  |                            >   ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  | [Avatar] Tim                   ||
|  |          6 Jahre               ||
|  |                            >   ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkt

```
GET /api/relatives/children
Authorization: Bearer {token}

Response 200:
{
  "children": [
    {
      "id": "guid",
      "firstName": "Max",
      "lastName": "Mustermann",
      "avatarUrl": "string | null",
      "birthDate": "2013-03-15"
    }
  ]
}

Response 401:
{
  "error": "Unauthorized"
}
```

## Technische Notizen

- ViewModel: `SelectChildViewModel` mit ObservableCollection<ChildDto>
- Service: `IRelativeService.GetAccessibleChildrenAsync()`
- Caching der Kinderliste fuer Offline-Nutzung (siehe M010)
- Navigation: `await Shell.Current.GoToAsync($"sendGift?childId={selectedChild.Id}")`

## Implementierungshinweise

```csharp
public class SelectChildViewModel : BaseViewModel
{
    private readonly IRelativeService _relativeService;

    public ObservableCollection<ChildDto> Children { get; } = new();

    [RelayCommand]
    private async Task LoadChildrenAsync()
    {
        var children = await _relativeService.GetAccessibleChildrenAsync();
        Children.Clear();
        foreach (var child in children)
        {
            Children.Add(child);
        }
    }

    [RelayCommand]
    private async Task SelectChildAsync(ChildDto child)
    {
        await Shell.Current.GoToAsync($"sendGift?childId={child.Id}");
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Verwandter mit 3 verbundenen Kindern | Liste zeigt alle 3 Kinder |
| TC-002 | Verwandter ohne verbundene Kinder | Hinweismeldung wird angezeigt |
| TC-003 | Kind antippen | Navigation zur Geschenk-senden-Seite |
| TC-004 | Nicht authentifiziert | Redirect zum Login |

## Story Points

2

## Prioritaet

Mittel
