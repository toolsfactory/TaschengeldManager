# Story E005-S044: Anfrage genehmigen (Eltern)

## Epic
E005 - Anfragen-System

## User Story

Als **Elternteil** möchte ich **eine Geldanfrage meines Kindes genehmigen können**, damit **das Geld auf sein Konto überwiesen wird**.

## Akzeptanzkriterien

- [ ] Gegeben eine ausstehende Anfrage, wenn sie genehmigt wird, dann wird der Betrag dem Kinderkonto gutgeschrieben
- [ ] Gegeben eine Genehmigung, wenn sie erfolgt, dann wird die Anfrage als "Genehmigt" markiert
- [ ] Gegeben eine Genehmigung, wenn sie erfolgt, dann wird das Kind benachrichtigt
- [ ] Gegeben eine Genehmigung, wenn sie erfolgt, dann kann optional ein anderer Betrag überwiesen werden
- [ ] Gegeben eine bereits bearbeitete Anfrage, wenn sie erneut genehmigt werden soll, dann wird dies verhindert

## API-Endpunkt

```
POST /api/requests/{requestId}/approve
Authorization: Bearer {token}
Content-Type: application/json

{
  "amount": 15.00,
  "note": "string (optional)"
}

Response 200:
{
  "requestId": "guid",
  "status": "Approved",
  "requestedAmount": 15.00,
  "approvedAmount": 15.00,
  "transactionId": "guid",
  "newBalance": 40.50,
  "approvedBy": "string",
  "approvedAt": "datetime"
}

Response 400:
{
  "message": "Anfrage wurde bereits bearbeitet"
}

Response 403:
{
  "message": "Keine Berechtigung für diese Anfrage"
}

Response 404:
{
  "message": "Anfrage nicht gefunden"
}
```

## Technische Notizen

- Status auf "Approved" setzen
- Transaktion vom Typ "RequestApproval" erstellen
- approvedAmount kann von requestedAmount abweichen
- Push-Benachrichtigung an Kind
- decidedBy und decidedAt speichern
- Atomare Operation: Status + Transaktion + Kontostand

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-044-01 | Anfrage genehmigen | 200 + Geld gutgeschrieben |
| TC-E005-044-02 | Mit anderem Betrag | approvedAmount abweichend |
| TC-E005-044-03 | Bereits genehmigt | 400 Fehler |
| TC-E005-044-04 | Bereits abgelehnt | 400 Fehler |
| TC-E005-044-05 | Kind wird benachrichtigt | Push-Notification |

## Story Points

3

## Priorität

Hoch
