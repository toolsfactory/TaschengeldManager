# Story E004-S030: Wiederkehrende Zahlung einrichten

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine wiederkehrende Taschengeld-Zahlung einrichten können**, damit **mein Kind automatisch und regelmäßig sein Taschengeld erhält**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es eine wiederkehrende Zahlung einrichtet, dann wird diese für das gewählte Kind aktiviert
- [ ] Gegeben eine neue Zahlung, wenn sie eingerichtet wird, dann müssen Betrag und Intervall angegeben werden
- [ ] Gegeben eine eingerichtete Zahlung, wenn sie gespeichert wird, dann wird das nächste Ausführungsdatum berechnet
- [ ] Gegeben eine Zahlung, wenn sie eingerichtet wird, dann kann optional ein Enddatum angegeben werden
- [ ] Gegeben eine aktive Zahlung, wenn sie eingerichtet wird, dann wird sie in der Übersicht angezeigt

## API-Endpunkt

```
POST /api/families/{familyId}/children/{childId}/recurring-payments
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Taschengeld",
  "amount": 10.00,
  "interval": "Weekly|Biweekly|Monthly",
  "dayOfWeek": 1,
  "dayOfMonth": 1,
  "startDate": "date",
  "endDate": "date (optional)",
  "description": "string (optional)"
}

Response 201:
{
  "recurringPaymentId": "guid",
  "name": "Taschengeld",
  "childId": "guid",
  "childName": "string",
  "amount": 10.00,
  "interval": "Weekly",
  "dayOfWeek": 1,
  "nextExecutionDate": "date",
  "status": "Active",
  "createdAt": "datetime"
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss positiv sein"],
    "interval": ["Intervall ist erforderlich"]
  }
}
```

## Technische Notizen

- RecurringPayment Entity mit eigenem Status
- nextExecutionDate automatisch berechnen
- Intervalle: Wöchentlich, Zweiwöchentlich, Monatlich
- Für Monatlich: dayOfMonth (1-28 empfohlen, 29-31 wird angepasst)
- Für Wöchentlich: dayOfWeek (0=Sonntag, 1=Montag, etc.)
- Status: Active, Paused, Completed, Cancelled

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-030-01 | Wöchentliche Zahlung einrichten | 201 + nächstes Datum korrekt |
| TC-E004-030-02 | Monatliche Zahlung einrichten | 201 + Tag des Monats |
| TC-E004-030-03 | Mit Enddatum | 201 + Enddatum gespeichert |
| TC-E004-030-04 | Negativer Betrag | 400 Validierungsfehler |
| TC-E004-030-05 | Startdatum in Vergangenheit | 400 Validierungsfehler |

## Story Points

5

## Priorität

Hoch
