# Story E004-S032: Zahlungstag festlegen

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **den genauen Zahlungstag festlegen können**, damit **das Taschengeld immer am gewünschten Tag ausgezahlt wird**.

## Akzeptanzkriterien

- [ ] Gegeben eine wöchentliche Zahlung, wenn der Tag gewählt wird, dann kann ein Wochentag (Mo-So) ausgewählt werden
- [ ] Gegeben eine monatliche Zahlung, wenn der Tag gewählt wird, dann kann ein Tag des Monats (1-28) ausgewählt werden
- [ ] Gegeben ein Tag > 28, wenn er gewählt wird, dann wird bei kürzeren Monaten auf den letzten Tag gewechselt
- [ ] Gegeben eine bestehende Zahlung, wenn der Tag geändert wird, dann wird das nächste Ausführungsdatum aktualisiert
- [ ] Gegeben ein geänderter Tag, wenn er vor dem heutigen Datum liegt, dann wird der nächste passende Tag gewählt

## API-Endpunkt

```
PATCH /api/recurring-payments/{paymentId}/schedule
Authorization: Bearer {token}
Content-Type: application/json

{
  "dayOfWeek": 1,
  "dayOfMonth": 15
}

Response 200:
{
  "recurringPaymentId": "guid",
  "interval": "Monthly",
  "dayOfWeek": null,
  "dayOfMonth": 15,
  "nextExecutionDate": "date",
  "updatedAt": "datetime"
}

Response 400:
{
  "errors": {
    "dayOfMonth": ["Tag muss zwischen 1 und 31 liegen"]
  }
}
```

### Wochentage Referenz
```
GET /api/recurring-payments/weekdays
Authorization: Bearer {token}

Response 200:
{
  "weekdays": [
    { "value": 0, "name": "Sonntag" },
    { "value": 1, "name": "Montag" },
    { "value": 2, "name": "Dienstag" },
    { "value": 3, "name": "Mittwoch" },
    { "value": 4, "name": "Donnerstag" },
    { "value": 5, "name": "Freitag" },
    { "value": 6, "name": "Samstag" }
  ]
}
```

## Technische Notizen

- dayOfWeek: 0-6 (Sonntag-Samstag, .NET Standard)
- dayOfMonth: 1-31 (28+ mit Fallback-Logik)
- Bei Monthly: dayOfWeek ignorieren
- Bei Weekly/Biweekly: dayOfMonth ignorieren
- Berechnung nächster Tag: Wenn heute der Tag ist -> heute, sonst nächster

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-032-01 | Montag für wöchentlich | Nächster Montag berechnet |
| TC-E004-032-02 | 15. für monatlich | Nächster 15. berechnet |
| TC-E004-032-03 | 31. im Februar | Auf 28./29. angepasst |
| TC-E004-032-04 | Heutiger Tag | Heute als nächster Termin |
| TC-E004-032-05 | Ungültiger Tag | 400 Validierungsfehler |

## Story Points

3

## Priorität

Hoch
