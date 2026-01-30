# Story M007-S03: Anfrage zurückziehen

## Epic
M007 - Geldanfragen

## User Story

Als **Kind** möchte ich **eine noch nicht beantwortete Anfrage zurückziehen können**, damit **ich meine Meinung ändern kann, bevor meine Eltern antworten**.

## Akzeptanzkriterien

- [ ] Gegeben eine ausstehende Anfrage, wenn ich darauf tippe, dann sehe ich die Option "Zurückziehen"
- [ ] Gegeben die Zurückziehen-Option, wenn ich darauf tippe, dann werde ich um Bestätigung gebeten
- [ ] Gegeben die Bestätigung, wenn ich bestätige, dann wird die Anfrage als "zurückgezogen" markiert
- [ ] Gegeben eine zurückgezogene Anfrage, wenn sie abgeschlossen ist, dann verschwindet sie aus der offenen Liste
- [ ] Gegeben eine bereits beantwortete Anfrage, wenn ich sie ansehe, dann gibt es keine Zurückziehen-Option

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Anfrage        │
├─────────────────────────────┤
│                             │
│  Deine Anfrage              │
│                             │
│  ┌─────────────────────────┐│
│  │ Betrag: 15,00 €         ││
│  │                         ││
│  │ Wofür: Lego-Set         ││
│  │                         ││
│  │ Status: 🕐 Wartet       ││
│  │                         ││
│  │ Erstellt: Vor 2 Stunden ││
│  └─────────────────────────┘│
│                             │
│  ┌───────────────────────┐  │
│  │   Anfrage zurückziehen│  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘

Bestätigungs-Dialog:
┌─────────────────────────────┐
│  Anfrage zurückziehen?      │
├─────────────────────────────┤
│                             │
│  Möchtest du diese Anfrage  │
│  wirklich zurückziehen?     │
│                             │
│  Betrag: 15,00 €            │
│  Wofür: Lego-Set            │
│                             │
│  [Nein, behalten]           │
│  [Ja, zurückziehen]         │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `RequestDetailPage.xaml`
- **ViewModel**: `RequestDetailViewModel.cs`
- **Service**: `IRequestService.cs`

## API-Endpunkt

```
DELETE /api/requests/{requestId}
Authorization: Bearer {child-token}

Response 200:
{
  "message": "Anfrage zurückgezogen",
  "requestId": "guid",
  "withdrawnAt": "2024-01-20T16:00:00Z"
}

Response 400:
{
  "error": "already_responded",
  "message": "Diese Anfrage wurde bereits beantwortet"
}

Response 404:
{
  "error": "not_found",
  "message": "Anfrage nicht gefunden"
}
```

## Technische Notizen

- Nur ausstehende Anfragen können zurückgezogen werden
- Soft-Delete: Status auf "withdrawn" setzen
- Benachrichtigung an Eltern, dass Anfrage zurückgezogen wurde
- Kindgerechte Bestätigungstexte
- Zurückgezogene Anfragen werden nach 30 Tagen gelöscht

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-03-01 | Ausstehende Anfrage zurückziehen | Erfolgreich |
| TC-M007-03-02 | Genehmigte Anfrage | Option nicht verfügbar |
| TC-M007-03-03 | Abgelehnte Anfrage | Option nicht verfügbar |
| TC-M007-03-04 | Bestätigung abbrechen | Anfrage bleibt bestehen |
| TC-M007-03-05 | Nach Zurückziehen | Nicht mehr in offener Liste |

## Story Points

1

## Priorität

Mittel

## Status

⬜ Offen
