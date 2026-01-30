# Story M007-S04: Anfragen-Liste für Eltern

## Epic
M007 - Geldanfragen

## User Story

Als **Elternteil** möchte ich **alle Geldanfragen meiner Kinder in einer Übersicht sehen**, damit **ich schnell auf offene Anfragen reagieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggtes Elternteil, wenn ich auf "Anfragen" tippe, dann sehe ich alle Anfragen aller Kinder
- [ ] Gegeben die Anfragenliste, wenn sie angezeigt wird, dann sind offene Anfragen oben sortiert
- [ ] Gegeben mehrere offene Anfragen, wenn sie angezeigt werden, dann sehe ich welches Kind die Anfrage gestellt hat
- [ ] Gegeben eine Anfrage mit Bild, wenn sie angezeigt wird, dann sehe ich eine Vorschau des Bildes
- [ ] Gegeben die Anfragenliste, wenn ich nach Kind filtere, dann sehe ich nur Anfragen dieses Kindes

## UI-Entwurf

```
┌─────────────────────────────┐
│  ☰ Geldanfragen        🔔   │
├─────────────────────────────┤
│  Filter: [Alle Kinder ▼]    │
├─────────────────────────────┤
│                             │
│  Offene Anfragen (2)        │
│  ┌─────────────────────────┐│
│  │ 👧 Emma        15,00 € ││
│  │ Lego-Set               ││
│  │ 🔴 Dringend             ││
│  │ Vor 2 Stunden          ││
│  │ [Ansehen →]            ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 👦 Max Jr.     5,00 €  ││
│  │ Eis kaufen             ││
│  │ Vor 1 Tag              ││
│  │ [Ansehen →]            ││
│  └─────────────────────────┘│
│                             │
│  Bearbeitete Anfragen       │
│  ┌─────────────────────────┐│
│  │ 👧 Emma  ✅   8,00 €   ││
│  │ Comic-Heft             ││
│  │ Genehmigt am 15.01.    ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 👦 Max Jr. ❌  50,00 € ││
│  │ Spielkonsole           ││
│  │ Abgelehnt am 10.01.    ││
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ParentRequestsPage.xaml`
- **ViewModel**: `ParentRequestsViewModel.cs`
- **Model**: `MoneyRequest.cs`

## API-Endpunkt

```
GET /api/families/{familyId}/requests?status=all&childId=optional&page=1&pageSize=20
Authorization: Bearer {parent-token}

Response 200:
{
  "pending": [
    {
      "requestId": "guid",
      "childId": "guid",
      "childName": "Emma",
      "childAvatarUrl": "string",
      "amount": 15.00,
      "reason": "Lego-Set",
      "urgency": "urgent",
      "imageUrl": "string",
      "createdAt": "2024-01-20T14:00:00Z"
    }
  ],
  "responded": [
    {
      "requestId": "guid",
      "childId": "guid",
      "childName": "Emma",
      "amount": 8.00,
      "reason": "Comic-Heft",
      "status": "approved",
      "respondedAt": "2024-01-15T12:00:00Z",
      "respondedBy": "Mama"
    }
  ],
  "pendingCount": 2,
  "totalCount": 15
}
```

## Technische Notizen

- Offene Anfragen prominent oben anzeigen
- Badge-Counter für offene Anfragen in Navigation
- Sortierung: Dringend > Normal, dann nach Datum
- Pull-to-Refresh für Aktualisierung
- Real-time Updates bei neuen Anfragen (SignalR)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-04-01 | Mehrere offene Anfragen | Oben sortiert |
| TC-M007-04-02 | Nach Kind filtern | Nur dessen Anfragen |
| TC-M007-04-03 | Anfrage mit Bild | Vorschau sichtbar |
| TC-M007-04-04 | Dringende Anfrage | Rot markiert |
| TC-M007-04-05 | Keine Anfragen | Leerer Zustand |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
