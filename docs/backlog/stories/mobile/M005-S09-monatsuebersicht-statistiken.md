# Story M005-S09: Monatsübersicht/Statistiken

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **eine monatliche Übersicht und Statistiken zum Kinderkonto sehen**, damit **ich das Ausgabeverhalten meines Kindes besser verstehen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Konto-Detailansicht, wenn ich auf "Statistiken" tippe, dann sehe ich eine Monatsübersicht
- [ ] Gegeben die Statistikansicht, wenn sie geladen wird, dann sehe ich Einnahmen und Ausgaben des aktuellen Monats
- [ ] Gegeben die Statistikansicht, wenn ich einen anderen Monat auswähle, dann werden die Daten für diesen Monat angezeigt
- [ ] Gegeben die Ausgabenstatistik, wenn sie angezeigt wird, dann sehe ich eine Aufschlüsselung nach Kategorien
- [ ] Gegeben die Statistikansicht, wenn sie geladen wird, dann sehe ich einen Vergleich zum Vormonat
- [ ] Gegeben genügend Daten, wenn ich die Statistik öffne, dann sehe ich einen Trend-Graphen über die letzten Monate

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück   📊 Statistiken  │
├─────────────────────────────┤
│                             │
│  [◀ Jan 2024 ▶]             │
│                             │
│  ┌─────────────────────────┐│
│  │     Monatsübersicht     ││
│  │                         ││
│  │ Einnahmen:    +25,00 €  ││
│  │ Ausgaben:     -15,00 €  ││
│  │ ───────────────────────  ││
│  │ Saldo:        +10,00 €  ││
│  └─────────────────────────┘│
│                             │
│  Ausgaben nach Kategorie:   │
│  ┌─────────────────────────┐│
│  │ 🍬 Süßigkeiten   40%    ││
│  │ ████████░░░░░  6,00 €   ││
│  │                         ││
│  │ 🎮 Spielzeug    33%     ││
│  │ ██████░░░░░░   5,00 €   ││
│  │                         ││
│  │ 📱 Sonstiges    27%     ││
│  │ █████░░░░░░░   4,00 €   ││
│  └─────────────────────────┘│
│                             │
│  Entwicklung (6 Monate):    │
│  ┌─────────────────────────┐│
│  │    📈                   ││
│  │      ╱╲    ╱            ││
│  │  ╱╲ ╱  ╲  ╱             ││
│  │ ╱  ╳    ╲╱              ││
│  │ A S O  N  D  J          ││
│  └─────────────────────────┘│
│                             │
│  vs. Vormonat:              │
│  Ausgaben: +5,00 € (↑ 50%)  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `AccountStatisticsPage.xaml`
- **ViewModel**: `AccountStatisticsViewModel.cs`
- **Models**: `MonthlyStatistics.cs`, `CategoryBreakdown.cs`
- **Components**: `BarChartView.xaml`, `TrendChartView.xaml`

## API-Endpunkt

```
GET /api/children/{childId}/account/statistics?month=2024-01
Authorization: Bearer {parent-token}

Response 200:
{
  "month": "2024-01",
  "totalIncome": 25.00,
  "totalExpenses": 15.00,
  "netChange": 10.00,
  "startBalance": 35.00,
  "endBalance": 45.00,
  "categoryBreakdown": [
    {
      "category": "sweets",
      "categoryName": "Süßigkeiten",
      "amount": 6.00,
      "percentage": 40.0,
      "transactionCount": 3
    }
  ],
  "previousMonthComparison": {
    "incomeChange": 5.00,
    "incomeChangePercent": 25.0,
    "expenseChange": 5.00,
    "expenseChangePercent": 50.0
  },
  "trend": [
    {"month": "2023-08", "balance": 30.00},
    {"month": "2023-09", "balance": 35.00},
    {"month": "2023-10", "balance": 32.00},
    {"month": "2023-11", "balance": 38.00},
    {"month": "2023-12", "balance": 35.00},
    {"month": "2024-01", "balance": 45.00}
  ]
}
```

## Technische Notizen

- Charts mit SkiaSharp oder Microcharts rendern
- Kategorien mit farbigen Icons visualisieren
- Daten cachen um Ladezeiten zu reduzieren
- Prozentuale Veränderungen farblich hervorheben (grün/rot)
- Leerer Zustand für Monate ohne Transaktionen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-09-01 | Monat mit Transaktionen | Korrekte Summen |
| TC-M005-09-02 | Monat ohne Transaktionen | Leerer Zustand |
| TC-M005-09-03 | Monat wechseln | Daten aktualisieren |
| TC-M005-09-04 | Kategorien-Aufschlüsselung | Prozente = 100% |
| TC-M005-09-05 | Vormonatsvergleich | Korrekter Unterschied |

## Story Points

3

## Priorität

Niedrig

## Status

⬜ Offen
