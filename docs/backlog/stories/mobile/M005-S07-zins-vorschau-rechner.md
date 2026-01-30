# Story M005-S07: Zins-Vorschau Rechner

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **eine Vorschau der zu erwartenden Zinsen sehen**, damit **ich meinem Kind erklären kann, wie viel es durch Sparen verdienen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Zinseinstellungen, wenn ich den Vorschau-Rechner öffne, dann sehe ich die berechneten Zinsen für verschiedene Zeiträume
- [ ] Gegeben der Zins-Rechner, wenn ich einen hypothetischen Kontostand eingebe, dann werden die Zinsen dafür berechnet
- [ ] Gegeben der Zins-Rechner, wenn ich den Zeitraum ändere (1/3/6/12 Monate), dann wird die Vorschau aktualisiert
- [ ] Gegeben die Zinsvorschau, wenn sie angezeigt wird, dann sehe ich sowohl die Zinsen als auch den Gesamtbetrag
- [ ] Gegeben der Zins-Rechner, wenn Zinsen aktiviert sind, dann wird auch der Zinseszins-Effekt visualisiert

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Zins-Rechner   │
├─────────────────────────────┤
│                             │
│  Aktueller Zinssatz: 3,5%   │
│  (monatliche Auszahlung)    │
│                             │
│  Startguthaben:             │
│  ┌───────────────────────┐  │
│  │      45,00 €          │  │
│  └───────────────────────┘  │
│  [Aktuell verwenden]        │
│                             │
│  Zeitraum:                  │
│  [1 Mo] [3 Mo] [6 Mo] [12Mo]│
│                             │
├─────────────────────────────┤
│                             │
│  📈 Zinsvorschau (12 Monate)│
│                             │
│  Monat │ Guthaben │ Zinsen  │
│  ──────┼──────────┼─────────│
│  Jan   │  45,00 € │  0,13 € │
│  Feb   │  45,13 € │  0,13 € │
│  Mär   │  45,26 € │  0,13 € │
│  ...   │  ...     │  ...    │
│  Dez   │  46,58 € │  0,14 € │
│                             │
├─────────────────────────────┤
│                             │
│  Zusammenfassung:           │
│  Startguthaben:    45,00 €  │
│  Zinsen gesamt:   + 1,58 €  │
│  ─────────────────────────  │
│  Endguthaben:      46,58 €  │
│                             │
│  💡 Durch Sparen verdient   │
│  Emma 1,58 € in einem Jahr! │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `InterestCalculatorPage.xaml`
- **ViewModel**: `InterestCalculatorViewModel.cs`
- **Service**: `IInterestCalculationService.cs`

## API-Endpunkt

```
POST /api/children/{childId}/account/interest-preview
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "startBalance": 45.00,
  "months": 12
}

Response 200:
{
  "startBalance": 45.00,
  "endBalance": 46.58,
  "totalInterest": 1.58,
  "ratePercent": 3.5,
  "monthlyBreakdown": [
    {
      "month": "2024-01",
      "startBalance": 45.00,
      "interest": 0.13,
      "endBalance": 45.13
    },
    {
      "month": "2024-02",
      "startBalance": 45.13,
      "interest": 0.13,
      "endBalance": 45.26
    }
  ]
}
```

## Technische Notizen

- Berechnung lokal im ViewModel für schnelle Reaktion
- API-Call für Validierung/exakte Berechnung
- Zinseszins-Formel: A = P(1 + r/n)^(nt)
- Kindgerechte Darstellung der Ergebnisse
- Optional: Grafische Darstellung als Chart

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-07-01 | 100€ bei 5% für 12 Monate | Korrekte Berechnung |
| TC-M005-07-02 | Zeitraum wechseln | Vorschau aktualisiert |
| TC-M005-07-03 | Hypothetischen Betrag eingeben | Neuberechnung |
| TC-M005-07-04 | Aktuelles Guthaben verwenden | Betrag wird übernommen |
| TC-M005-07-05 | Zinseszins-Effekt | Korrekt berechnet |

## Story Points

2

## Priorität

Niedrig

## Status

⬜ Offen
