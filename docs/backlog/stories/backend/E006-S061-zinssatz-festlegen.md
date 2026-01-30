# Story E006-S061: Zinssatz pro Kind festlegen

## Epic
E006 - Zinsen

## User Story

Als **Elternteil** möchte ich **für jedes Kind einen individuellen Zinssatz festlegen können**, damit **ich unterschiedliche Spar-Anreize setzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit aktivierten Zinsen, wenn der Zinssatz geändert wird, dann gilt er ab der nächsten Berechnung
- [ ] Gegeben ein Zinssatz, wenn er festgelegt wird, dann muss er zwischen 0% und 100% liegen
- [ ] Gegeben eine Zinssatzänderung, wenn sie erfolgt, dann wird sie protokolliert
- [ ] Gegeben verschiedene Kinder, wenn Zinssätze festgelegt werden, dann können sie unterschiedlich sein
- [ ] Gegeben ein Zinssatz, wenn er angezeigt wird, dann wird er als Jahres-Zinssatz dargestellt

## API-Endpunkt

```
PATCH /api/families/{familyId}/children/{childId}/interest/rate
Authorization: Bearer {token}
Content-Type: application/json

{
  "interestRate": 5.0
}

Response 200:
{
  "childId": "guid",
  "previousRate": 3.0,
  "newRate": 5.0,
  "effectiveFrom": "date",
  "updatedAt": "datetime"
}

Response 400:
{
  "errors": {
    "interestRate": ["Zinssatz muss zwischen 0 und 100 liegen"]
  }
}

Response 400:
{
  "message": "Zinsen sind für dieses Kind nicht aktiviert"
}
```

### Zinssatz-Historie abrufen
```
GET /api/families/{familyId}/children/{childId}/interest/rate-history
Authorization: Bearer {token}

Response 200:
{
  "childId": "guid",
  "currentRate": 5.0,
  "history": [
    {
      "rate": 3.0,
      "effectiveFrom": "date",
      "effectiveTo": "date",
      "changedBy": "string"
    },
    {
      "rate": 5.0,
      "effectiveFrom": "date",
      "effectiveTo": null,
      "changedBy": "string"
    }
  ]
}
```

## Technische Notizen

- Zinssatz als Dezimalzahl (5.0 = 5%)
- Änderung gilt ab nächster Berechnung
- Historie für Audit-Zwecke speichern
- Umrechnung: Jahreszins / 12 für monatliche Berechnung
- Keine rückwirkende Änderung von Zinsen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E006-061-01 | Zinssatz ändern | 200 + neuer Satz |
| TC-E006-061-02 | Zinssatz 0% | 200 (keine Zinsen aber aktiv) |
| TC-E006-061-03 | Zinssatz > 100% | 400 Validierungsfehler |
| TC-E006-061-04 | Negativer Zinssatz | 400 Validierungsfehler |
| TC-E006-061-05 | Historie abrufen | Alle Änderungen |

## Story Points

2

## Priorität

Mittel
