# Story E005-S045: Anfrage ablehnen mit Begründung

## Epic
E005 - Anfragen-System

## User Story

Als **Elternteil** möchte ich **eine Geldanfrage meines Kindes ablehnen und einen Grund angeben können**, damit **mein Kind versteht, warum die Anfrage nicht genehmigt wurde**.

## Akzeptanzkriterien

- [ ] Gegeben eine ausstehende Anfrage, wenn sie abgelehnt wird, dann wird sie als "Abgelehnt" markiert
- [ ] Gegeben eine Ablehnung, wenn sie erfolgt, dann muss ein Ablehnungsgrund angegeben werden
- [ ] Gegeben eine Ablehnung, wenn sie erfolgt, dann wird das Kind benachrichtigt
- [ ] Gegeben eine abgelehnte Anfrage, wenn sie angezeigt wird, dann ist der Ablehnungsgrund für das Kind sichtbar
- [ ] Gegeben eine bereits bearbeitete Anfrage, wenn sie abgelehnt werden soll, dann wird dies verhindert

## API-Endpunkt

```
POST /api/requests/{requestId}/decline
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Das ist zu teuer. Vielleicht können wir es zum Geburtstag besprechen."
}

Response 200:
{
  "requestId": "guid",
  "status": "Declined",
  "requestedAmount": 50.00,
  "declineReason": "Das ist zu teuer. Vielleicht können wir es zum Geburtstag besprechen.",
  "declinedBy": "string",
  "declinedAt": "datetime"
}

Response 400:
{
  "errors": {
    "reason": ["Ablehnungsgrund ist erforderlich"]
  }
}

Response 400:
{
  "message": "Anfrage wurde bereits bearbeitet"
}

Response 403:
{
  "message": "Keine Berechtigung für diese Anfrage"
}
```

## Technische Notizen

- Status auf "Declined" setzen
- Ablehnungsgrund ist Pflichtfeld
- Keine Transaktion erstellen (kein Geldfluss)
- Push-Benachrichtigung an Kind mit Hinweis
- decidedBy und decidedAt speichern
- Kind kann Grund in seiner Anfragenliste sehen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-045-01 | Anfrage ablehnen | 200 + Status Declined |
| TC-E005-045-02 | Ohne Begründung | 400 Validierungsfehler |
| TC-E005-045-03 | Bereits genehmigt | 400 Fehler |
| TC-E005-045-04 | Bereits abgelehnt | 400 Fehler |
| TC-E005-045-05 | Kind wird benachrichtigt | Push mit Grund-Hinweis |

## Story Points

2

## Priorität

Hoch
