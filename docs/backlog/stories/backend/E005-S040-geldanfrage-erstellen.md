# Story E005-S040: Geldanfrage erstellen (Kind)

## Epic
E005 - Anfragen-System

## User Story

Als **Kind** möchte ich **eine Geldanfrage an meine Eltern stellen können**, damit **ich um zusätzliches Geld für besondere Wünsche bitten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Konto, wenn es eine Anfrage erstellt, dann wird diese an die Eltern gesendet
- [ ] Gegeben eine Anfrage, wenn sie erstellt wird, dann muss ein Betrag angegeben werden
- [ ] Gegeben eine Anfrage, wenn sie erstellt wird, dann kann optional ein Grund/Wunsch angegeben werden
- [ ] Gegeben eine erstellte Anfrage, wenn sie gespeichert wird, dann hat sie den Status "Ausstehend"
- [ ] Gegeben eine neue Anfrage, wenn sie erstellt wird, dann werden die Eltern benachrichtigt

## API-Endpunkt

```
POST /api/me/requests
Authorization: Bearer {token}
Content-Type: application/json

{
  "amount": 15.00,
  "title": "Neues Buch",
  "reason": "Ich möchte gerne das neue Harry Potter Buch kaufen",
  "urgency": "Normal|Urgent"
}

Response 201:
{
  "requestId": "guid",
  "amount": 15.00,
  "title": "Neues Buch",
  "reason": "Ich möchte gerne das neue Harry Potter Buch kaufen",
  "urgency": "Normal",
  "status": "Pending",
  "createdAt": "datetime"
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss positiv sein"],
    "title": ["Titel ist erforderlich"]
  }
}

Response 403:
{
  "message": "Nur Kinder können Geldanfragen erstellen"
}

Response 404:
{
  "message": "Kein Konto vorhanden"
}
```

## Technische Notizen

- MoneyRequest Entity mit Status-Tracking
- Status-Enum: Pending, Approved, Declined, Withdrawn
- Push-Benachrichtigung an alle Eltern der Familie
- Maximaler Betrag: konfigurierbar pro Familie (optional)
- Urgency für Priorisierung in der Eltern-Ansicht

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-040-01 | Valide Anfrage | 201 + Status Pending |
| TC-E005-040-02 | Mit Begründung | 201 + Begründung gespeichert |
| TC-E005-040-03 | Ohne Betrag | 400 Validierungsfehler |
| TC-E005-040-04 | Elternteil erstellt Anfrage | 403 Forbidden |
| TC-E005-040-05 | Benachrichtigung | Push an Eltern |

## Story Points

3

## Priorität

Hoch
