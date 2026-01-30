# Story E005-S042: Anfragen-Liste anzeigen (Kind)

## Epic
E005 - Anfragen-System

## User Story

Als **Kind** möchte ich **meine Geldanfragen und deren Status sehen können**, damit **ich weiß, welche Anfragen noch offen sind und welche bereits bearbeitet wurden**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind, wenn es seine Anfragen abruft, dann werden alle eigenen Anfragen angezeigt
- [ ] Gegeben eine Anfragenliste, wenn sie angezeigt wird, dann enthält sie Status, Betrag und Erstellungsdatum
- [ ] Gegeben ausstehende Anfragen, wenn sie angezeigt werden, dann sind sie hervorgehoben
- [ ] Gegeben abgelehnte Anfragen, wenn sie angezeigt werden, dann ist der Ablehnungsgrund sichtbar
- [ ] Gegeben eine lange Liste, wenn sie abgerufen wird, dann wird Pagination verwendet

## API-Endpunkt

```
GET /api/me/requests?status=All&page=1&pageSize=20
Authorization: Bearer {token}

Response 200:
{
  "requests": [
    {
      "requestId": "guid",
      "amount": 15.00,
      "title": "Neues Buch",
      "reason": "string",
      "status": "Pending|Approved|Declined|Withdrawn",
      "urgency": "Normal",
      "createdAt": "datetime",
      "decidedAt": "datetime (optional)",
      "decidedBy": "string (optional)",
      "decisionReason": "string (optional, bei Ablehnung)"
    }
  ],
  "summary": {
    "pending": 2,
    "approved": 5,
    "declined": 1,
    "withdrawn": 0,
    "totalApprovedAmount": 75.00
  },
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 8,
    "totalPages": 1
  }
}
```

## Technische Notizen

- Filter: status (Pending, Approved, Declined, Withdrawn, All)
- Sortierung: neueste zuerst (Standard)
- Summary für schnellen Überblick
- Nur eigene Anfragen anzeigen (childId aus Token)
- Kindgerechte Statusbezeichnungen im Frontend

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-042-01 | Alle Anfragen abrufen | 200 mit Liste |
| TC-E005-042-02 | Nach Status filtern | Nur passende Anfragen |
| TC-E005-042-03 | Leere Liste | 200 mit leerem Array |
| TC-E005-042-04 | Mit Ablehnungsgrund | Grund wird angezeigt |
| TC-E005-042-05 | Pagination | Korrekte Aufteilung |

## Story Points

2

## Priorität

Hoch
