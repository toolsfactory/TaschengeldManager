# Story E004-S031: Zahlungsintervall wählen

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **verschiedene Zahlungsintervalle auswählen können**, damit **ich die Frequenz an unsere Familienbedürfnisse anpassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es ein Intervall wählt, dann stehen Wöchentlich, Zweiwöchentlich und Monatlich zur Verfügung
- [ ] Gegeben ein gewähltes Intervall, wenn es gespeichert wird, dann wird das nächste Ausführungsdatum korrekt berechnet
- [ ] Gegeben eine bestehende Zahlung, wenn das Intervall geändert wird, dann wird das nächste Datum neu berechnet
- [ ] Gegeben Monatszahlung am 31., wenn der Monat kürzer ist, dann wird auf den letzten Tag des Monats angepasst
- [ ] Gegeben eine Intervalländerung, wenn sie gespeichert wird, dann werden vergangene Zahlungen nicht beeinflusst

## API-Endpunkt

```
GET /api/recurring-payments/intervals
Authorization: Bearer {token}

Response 200:
{
  "intervals": [
    {
      "value": "Weekly",
      "displayName": "Wöchentlich",
      "description": "Jeden gewählten Wochentag"
    },
    {
      "value": "Biweekly",
      "displayName": "Zweiwöchentlich",
      "description": "Alle zwei Wochen am gewählten Tag"
    },
    {
      "value": "Monthly",
      "displayName": "Monatlich",
      "description": "Einmal im Monat am gewählten Tag"
    }
  ]
}
```

### Intervall ändern
```
PATCH /api/recurring-payments/{paymentId}/interval
Authorization: Bearer {token}
Content-Type: application/json

{
  "interval": "Monthly",
  "dayOfMonth": 15
}

Response 200:
{
  "recurringPaymentId": "guid",
  "interval": "Monthly",
  "dayOfMonth": 15,
  "previousNextExecution": "date",
  "newNextExecution": "date"
}
```

## Technische Notizen

- Enum für Intervalle: Weekly, Biweekly, Monthly
- Berechnungslogik für nächstes Datum:
  - Weekly: Nächster Wochentag ab heute
  - Biweekly: Nächster Wochentag + 14 Tage falls gerade ausgeführt
  - Monthly: Nächster Tag X des Monats
- Edge Cases: Monatswechsel, Jahreswechsel, Schaltjahr
- Keine rückwirkende Änderung von Zahlungen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-031-01 | Wöchentlich wählen | Korrekte Berechnung |
| TC-E004-031-02 | Zweiwöchentlich wählen | Korrekte Berechnung |
| TC-E004-031-03 | Monatlich am 31. | Anpassung für kurze Monate |
| TC-E004-031-04 | Intervall ändern | Neues Datum berechnet |
| TC-E004-031-05 | Ungültiges Intervall | 400 Validierungsfehler |

## Story Points

3

## Priorität

Hoch
