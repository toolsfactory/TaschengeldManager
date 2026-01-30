# Story E005-S043: Anfragen-Liste anzeigen (Eltern)

## Epic
E005 - Anfragen-System

## User Story

Als **Elternteil** möchte ich **alle Geldanfragen meiner Kinder sehen können**, damit **ich diese prüfen und bearbeiten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es die Anfragen abruft, dann werden alle Anfragen aller Kinder angezeigt
- [ ] Gegeben ausstehende Anfragen, wenn sie angezeigt werden, dann sind sie priorisiert und hervorgehoben
- [ ] Gegeben eine Anfragenliste, wenn sie angezeigt wird, dann kann nach Kind gefiltert werden
- [ ] Gegeben eine Anfrage, wenn sie angezeigt wird, dann ist die vollständige Begründung sichtbar
- [ ] Gegeben dringende Anfragen, wenn sie vorhanden sind, dann werden sie zuerst angezeigt

## API-Endpunkt

```
GET /api/families/{familyId}/requests?status=Pending&childId={childId}&page=1
Authorization: Bearer {token}

Response 200:
{
  "familyId": "guid",
  "requests": [
    {
      "requestId": "guid",
      "childId": "guid",
      "childName": "Max",
      "amount": 15.00,
      "title": "Neues Buch",
      "reason": "Ich möchte gerne...",
      "status": "Pending",
      "urgency": "Normal",
      "createdAt": "datetime",
      "daysSinceCreated": 2
    }
  ],
  "summary": {
    "totalPending": 3,
    "totalPendingAmount": 45.00,
    "byChild": [
      {
        "childId": "guid",
        "childName": "Max",
        "pendingCount": 2,
        "pendingAmount": 30.00
      }
    ]
  },
  "pagination": {...}
}
```

### Einzelne Anfrage Details
```
GET /api/requests/{requestId}
Authorization: Bearer {token}

Response 200:
{
  "requestId": "guid",
  "childId": "guid",
  "childName": "Max",
  "childAccountBalance": 25.50,
  "amount": 15.00,
  "title": "Neues Buch",
  "reason": "Ich möchte gerne das neue Harry Potter Buch...",
  "status": "Pending",
  "urgency": "Normal",
  "createdAt": "datetime",
  "history": [
    {
      "action": "Created",
      "timestamp": "datetime",
      "actor": "Max"
    }
  ]
}
```

## Technische Notizen

- Filter: status, childId, urgency, fromDate, toDate
- Sortierung: Dringende zuerst, dann nach Alter
- daysSinceCreated für visuelle Hervorhebung alter Anfragen
- childAccountBalance für Kontext bei Entscheidung
- Aggregation pro Kind für Übersicht

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-043-01 | Alle ausstehenden Anfragen | 200 mit gefiltert Liste |
| TC-E005-043-02 | Nach Kind filtern | Nur Anfragen des Kindes |
| TC-E005-043-03 | Dringende zuerst | Sortierung korrekt |
| TC-E005-043-04 | Fremde Familie | 403 Forbidden |
| TC-E005-043-05 | Einzelne Anfrage | Details mit Kontostand |

## Story Points

3

## Priorität

Hoch
