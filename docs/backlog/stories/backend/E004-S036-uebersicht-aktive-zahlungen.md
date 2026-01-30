# Story E004-S036: Übersicht aller aktiven Zahlungen

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine Übersicht aller aktiven wiederkehrenden Zahlungen sehen**, damit **ich den Überblick über alle automatischen Taschengeld-Zahlungen behalte**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es die Übersicht abruft, dann werden alle aktiven Zahlungen angezeigt
- [ ] Gegeben eine Zahlungsübersicht, wenn sie angezeigt wird, dann enthält sie Kind, Betrag, Intervall und nächstes Datum
- [ ] Gegeben pausierte Zahlungen, wenn sie vorhanden sind, dann werden sie separat gekennzeichnet
- [ ] Gegeben mehrere Kinder, wenn die Übersicht angezeigt wird, dann sind die Zahlungen nach Kind gruppiert
- [ ] Gegeben eine Übersicht, wenn sie angezeigt wird, dann wird die monatliche Gesamtsumme berechnet

## API-Endpunkt

```
GET /api/families/{familyId}/recurring-payments?status=Active,Paused
Authorization: Bearer {token}

Response 200:
{
  "familyId": "guid",
  "monthlyTotal": 45.00,
  "yearlyTotal": 540.00,
  "payments": [
    {
      "recurringPaymentId": "guid",
      "name": "Taschengeld Max",
      "childId": "guid",
      "childName": "Max",
      "amount": 10.00,
      "interval": "Weekly",
      "intervalDisplay": "Wöchentlich",
      "dayOfWeek": 1,
      "dayDisplay": "Montag",
      "nextExecutionDate": "date",
      "status": "Active",
      "lastExecutedAt": "datetime",
      "totalPaidThisYear": 120.00
    }
  ],
  "byChild": [
    {
      "childId": "guid",
      "childName": "Max",
      "monthlyTotal": 45.00,
      "payments": [...]
    }
  ]
}
```

### Einzelne Zahlung Details
```
GET /api/recurring-payments/{paymentId}
Authorization: Bearer {token}

Response 200:
{
  "recurringPaymentId": "guid",
  "name": "Taschengeld",
  "childId": "guid",
  "childName": "Max",
  "amount": 10.00,
  "interval": "Weekly",
  "dayOfWeek": 1,
  "startDate": "date",
  "endDate": "date (optional)",
  "nextExecutionDate": "date",
  "status": "Active",
  "description": "string",
  "createdAt": "datetime",
  "executionHistory": [
    {
      "transactionId": "guid",
      "amount": 10.00,
      "executedAt": "datetime",
      "status": "Success"
    }
  ]
}
```

## Technische Notizen

- Aggregation für monatliche/jährliche Summen
- Berechnung: Wöchentlich × 4.33, Zweiwöchentlich × 2.17, Monatlich × 1
- Filter: status (Active, Paused, Cancelled, All)
- Sortierung: Nach Kind, dann nach nächstem Datum
- Execution History: Letzte 10 Ausführungen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-036-01 | Alle aktiven Zahlungen | Liste mit Details |
| TC-E004-036-02 | Inklusive pausierte | Pausierte separat markiert |
| TC-E004-036-03 | Monatssumme | Korrekt berechnet |
| TC-E004-036-04 | Keine Zahlungen | Leeres Array |
| TC-E004-036-05 | Fremde Familie | 403 Forbidden |

## Story Points

3

## Priorität

Hoch
