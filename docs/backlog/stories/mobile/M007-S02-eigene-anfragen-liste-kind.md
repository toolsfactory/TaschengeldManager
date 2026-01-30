# Story M007-S02: Eigene Anfragen-Liste (Kind)

## Epic
M007 - Geldanfragen

## User Story

Als **Kind** möchte ich **meine Geldanfragen und deren Status sehen können**, damit **ich weiß, ob meine Anfrage genehmigt oder abgelehnt wurde**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggtes Kind, wenn ich auf "Meine Anfragen" tippe, dann sehe ich eine Liste aller meiner Anfragen
- [ ] Gegeben die Anfragenliste, wenn eine Anfrage angezeigt wird, dann sehe ich Betrag, Grund und Status
- [ ] Gegeben eine genehmigte Anfrage, wenn sie angezeigt wird, dann ist sie grün markiert mit Häkchen
- [ ] Gegeben eine abgelehnte Anfrage, wenn sie angezeigt wird, dann ist sie rot markiert mit Grund
- [ ] Gegeben eine ausstehende Anfrage, wenn sie angezeigt wird, dann sehe ich "Wartet auf Antwort"

## UI-Entwurf

```
┌─────────────────────────────┐
│  ☰ Meine Anfragen       🔔   │
├─────────────────────────────┤
│                             │
│  Offene Anfragen            │
│  ┌─────────────────────────┐│
│  │ 🕐 Lego-Set     15,00 € ││
│  │    Wartet auf Antwort   ││
│  │    Vor 2 Stunden        ││
│  └─────────────────────────┘│
│                             │
│  Letzte Anfragen            │
│  ┌─────────────────────────┐│
│  │ ✅ Süßigkeiten  5,00 €  ││
│  │    Genehmigt!           ││
│  │    15. Jan 2024         ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ❌ Spielkonsole 50,00 € ││
│  │    Abgelehnt            ││
│  │    "Zu teuer, spar      ││
│  │     dafür bitte"        ││
│  │    10. Jan 2024         ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ✅ Comic-Heft   8,00 €  ││
│  │    Genehmigt!           ││
│  │    5. Jan 2024          ││
│  └─────────────────────────┘│
│                             │
│  ┌───────────────────────┐  │
│  │  + Neue Anfrage       │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `MyRequestsPage.xaml`
- **ViewModel**: `MyRequestsViewModel.cs`
- **Model**: `MoneyRequest.cs`

## API-Endpunkt

```
GET /api/children/{childId}/requests?page=1&pageSize=20
Authorization: Bearer {child-token}

Response 200:
{
  "requests": [
    {
      "requestId": "guid",
      "amount": 15.00,
      "reason": "Lego-Set",
      "status": "pending",
      "urgency": "normal",
      "createdAt": "2024-01-20T14:00:00Z",
      "imageUrl": null,
      "response": null
    },
    {
      "requestId": "guid",
      "amount": 5.00,
      "reason": "Süßigkeiten",
      "status": "approved",
      "createdAt": "2024-01-15T10:00:00Z",
      "response": {
        "respondedAt": "2024-01-15T12:00:00Z",
        "respondedBy": "Mama"
      }
    },
    {
      "requestId": "guid",
      "amount": 50.00,
      "reason": "Spielkonsole",
      "status": "rejected",
      "createdAt": "2024-01-10T10:00:00Z",
      "response": {
        "respondedAt": "2024-01-10T18:00:00Z",
        "respondedBy": "Papa",
        "rejectionReason": "Zu teuer, spar dafür bitte"
      }
    }
  ],
  "totalCount": 10,
  "page": 1,
  "pageSize": 20
}
```

## Technische Notizen

- Anfragen nach Status gruppieren (offen, beantwortet)
- Farbcodierung: Grün=genehmigt, Rot=abgelehnt, Grau=ausstehend
- Kindgerechte Statusanzeige mit Emojis
- Pull-to-Refresh für Aktualisierung
- Real-time Update wenn Eltern antworten (SignalR)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-02-01 | Liste mit Anfragen | Alle werden angezeigt |
| TC-M007-02-02 | Genehmigte Anfrage | Grün mit Häkchen |
| TC-M007-02-03 | Abgelehnte Anfrage | Rot mit Grund |
| TC-M007-02-04 | Ausstehende Anfrage | Grau, "Wartet" |
| TC-M007-02-05 | Keine Anfragen | Leerer Zustand |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
