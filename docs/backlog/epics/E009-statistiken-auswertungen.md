# Epic E009: Statistiken & Auswertungen

## Status

**ABGESCHLOSSEN (Backend)** - 2026-01-22

## Beschreibung

Eltern und Kinder erhalten visuelle Übersichten und Statistiken zu Kontobewegungen, Ausgabenverhalten und Trends. Die Darstellung ist rollenspezifisch: Kinder sehen einfache, motivierende Visualisierungen; Eltern erhalten detaillierte Analysen über alle Kinder.

## Business Value

Transparenz und Verständnis über Geldflüsse. Kinder lernen ihr Ausgabeverhalten zu reflektieren. Eltern behalten den Überblick und können bei Bedarf eingreifen. Fördert finanzielle Bildung durch Visualisierung.

## Stories

### Kind-Übersichten
- [x] S090 - Ausgaben-Tortendiagramm (nach Kategorie) - API implementiert
- [x] S091 - Kontostand-Verlauf (Liniendiagramm) - API implementiert
- [x] S092 - Monatsvergleich (Balkendiagramm) - API implementiert

### Eltern-Übersichten
- [x] S093 - Familien-Dashboard (alle Kinder) - API implementiert
- [x] S094 - Einnahmen/Ausgaben-Übersicht (pro Kind) - API implementiert
- [x] S095 - Kategorie-Analyse (alle Kinder) - API implementiert

### Gemeinsam
- [x] S096 - Zeitraum-Filter (Woche/Monat/Jahr) - in allen Endpoints verfügbar
- [ ] S097 - Export als PDF (optional) - für späteren Sprint

## Implementierte API-Endpoints

| Endpoint | Methode | Beschreibung | Story |
|----------|---------|--------------|-------|
| `/api/statistics/me/expenses-by-category` | GET | Ausgaben nach Kategorie (Kind-Ansicht) | S090 |
| `/api/statistics/me/balance-history` | GET | Kontostand-Verlauf (Kind-Ansicht) | S091 |
| `/api/statistics/me/month-comparison` | GET | Monatsvergleich (Kind-Ansicht) | S092 |
| `/api/statistics/accounts/{accountId}/expenses-by-category` | GET | Ausgaben nach Kategorie (Eltern-Ansicht) | S090 |
| `/api/statistics/accounts/{accountId}/balance-history` | GET | Kontostand-Verlauf (Eltern-Ansicht) | S091 |
| `/api/statistics/accounts/{accountId}/month-comparison` | GET | Monatsvergleich (Eltern-Ansicht) | S092 |
| `/api/statistics/accounts/{accountId}/income-expenses` | GET | Einnahmen/Ausgaben (Eltern-Ansicht) | S094 |
| `/api/statistics/family/{familyId}/dashboard` | GET | Familien-Dashboard | S093 |
| `/api/statistics/family/{familyId}/expenses-by-category` | GET | Kategorie-Analyse Familie | S095 |

## Abhängigkeiten

- E001 (Benutzerverwaltung)
- E002 (Kontoverwaltung)
- E003 (Transaktionen - Datenbasis)

## Akzeptanzkriterien (Epic-Level)

### Allgemein
- [ ] Übersichten laden schnell (< 2 Sekunden)
- [ ] Zeitraum ist filterbar (Woche, Monat, Quartal, Jahr, Benutzerdefiniert)
- [ ] Visualisierungen sind touch-optimiert für Mobile
- [ ] Farben sind konsistent und barrierefrei

### Kind-Übersichten
- [ ] Kind sieht nur eigene Daten
- [ ] Darstellung ist einfach und kindgerecht
- [ ] Kategorien haben eindeutige Farben

### Eltern-Übersichten
- [ ] Eltern sehen alle Kinder der Familie
- [ ] Drill-Down von Familie → Kind → Detail möglich
- [ ] Vergleich zwischen Kindern möglich

---

## Detail-Spezifikationen

### S090: Ausgaben-Tortendiagramm (Kind)

**User Story**
Als Kind möchte ich sehen, wofür ich mein Geld ausgebe, damit ich mein Ausgabeverhalten verstehe.

**Visualisierung**
```
        Süßigkeiten
           35%
      ┌────────────┐
     ╱              ╲
    │    ████████    │ Spielzeug
    │   ██████████   │   25%
    │  ████████████  │
     ╲  ██████████  ╱
      └────────────┘
    Sonstiges    Kleidung
       20%         20%
```

**Features**
- Tortendiagramm mit Kategorie-Segmenten
- Prozentanzeige pro Kategorie
- Betrag bei Tap auf Segment
- Zeitraum wählbar (Standard: aktueller Monat)

**API**
```
GET /api/statistics/children/{childId}/expenses-by-category
?from=2025-01-01&to=2025-01-31

Response:
{
  "period": { "from": "2025-01-01", "to": "2025-01-31" },
  "total": 45.00,
  "categories": [
    { "name": "Süßigkeiten", "amount": 15.75, "percentage": 35 },
    { "name": "Spielzeug", "amount": 11.25, "percentage": 25 },
    { "name": "Kleidung", "amount": 9.00, "percentage": 20 },
    { "name": "Sonstiges", "amount": 9.00, "percentage": 20 }
  ]
}
```

---

### S091: Kontostand-Verlauf (Kind)

**User Story**
Als Kind möchte ich sehen, wie sich mein Kontostand entwickelt, damit ich verstehe ob ich spare oder ausgebe.

**Visualisierung**
```
EUR
 │
100├──────────────────────●
 │                    ╱
 80├────────────●────╱
 │            ╱
 60├──────●──╱
 │    ╱   ╲
 40├──●     ●
 │
 20├
 │
  └──┬────┬────┬────┬────┬──
    Jan  Feb  Mär  Apr  Mai
```

**Features**
- Liniendiagramm mit Datenpunkten
- Hover/Tap zeigt exakten Betrag + Datum
- Trend-Indikator (steigend/fallend)
- Zeitraum: 3 Monate, 6 Monate, 1 Jahr

**API**
```
GET /api/statistics/children/{childId}/balance-history
?period=6months&granularity=weekly

Response:
{
  "period": "6months",
  "dataPoints": [
    { "date": "2025-01-01", "balance": 45.00 },
    { "date": "2025-01-08", "balance": 52.50 },
    { "date": "2025-01-15", "balance": 48.00 },
    ...
  ],
  "trend": "increasing",
  "changePercent": 12.5
}
```

---

### S092: Monatsvergleich (Kind)

**User Story**
Als Kind möchte ich sehen, ob ich diesen Monat mehr oder weniger ausgegeben habe als letzten Monat.

**Visualisierung**
```
        Ausgaben
EUR
 │
 50├    ┌───┐
    │    │   │  ┌───┐
 40├    │   │  │   │
    │    │   │  │   │
 30├    │   │  │   │
    │    │   │  │   │
 20├    │   │  │   │
    │    │   │  │   │
 10├    │   │  │   │
    │    │   │  │   │
  └────┴───┴──┴───┴────
       Dez     Jan
      2024    2025

    ▼ 15% weniger als letzten Monat 🎉
```

**Features**
- Zwei Balken: aktueller vs. vorheriger Monat
- Prozentuale Veränderung
- Positives Feedback bei Reduzierung
- Optional: Einnahmen daneben

**API**
```
GET /api/statistics/children/{childId}/month-comparison

Response:
{
  "currentMonth": {
    "month": "2025-01",
    "expenses": 35.00,
    "income": 50.00
  },
  "previousMonth": {
    "month": "2024-12",
    "expenses": 42.00,
    "income": 50.00
  },
  "expenseChange": -16.7,
  "incomeChange": 0
}
```

---

### S093: Familien-Dashboard (Eltern)

**User Story**
Als Elternteil möchte ich alle Kinder mit ihren Kontoständen auf einen Blick sehen.

**Visualisierung**
```
┌─────────────────────────────────────────┐
│  Familien-Übersicht          Jan 2025  │
├─────────────────────────────────────────┤
│                                         │
│  ┌─────────────┐  ┌─────────────┐       │
│  │  👧 Lisa    │  │  👦 Max     │       │
│  │             │  │             │       │
│  │  € 125,50   │  │  € 48,00    │       │
│  │  ▲ +12%     │  │  ▼ -5%      │       │
│  └─────────────┘  └─────────────┘       │
│                                         │
│  ┌─────────────┐                        │
│  │  👶 Tim     │                        │
│  │             │                        │
│  │  € 32,00    │                        │
│  │  ━ 0%       │                        │
│  └─────────────┘                        │
│                                         │
│  ─────────────────────────────────────  │
│  Gesamt Familie: € 205,50               │
│  Ausgaben diesen Monat: € 67,30         │
│                                         │
└─────────────────────────────────────────┘
```

**Features**
- Karte pro Kind mit Kontostand
- Trend-Indikator (Veränderung zum Vormonat)
- Tap auf Kind → Detail-Ansicht
- Familien-Summen unten

**API**
```
GET /api/statistics/family/dashboard

Response:
{
  "familyName": "Familie Müller",
  "totalBalance": 205.50,
  "totalExpensesThisMonth": 67.30,
  "children": [
    {
      "childId": "...",
      "name": "Lisa",
      "balance": 125.50,
      "balanceChange": 12.0,
      "expensesThisMonth": 22.00
    },
    ...
  ]
}
```

---

### S094: Einnahmen/Ausgaben-Übersicht (Eltern)

**User Story**
Als Elternteil möchte ich Einnahmen und Ausgaben pro Kind und Monat sehen, um den Geldfluss zu verstehen.

**Visualisierung**
```
┌─────────────────────────────────────────┐
│  Einnahmen / Ausgaben           Lisa   │
├─────────────────────────────────────────┤
│                                         │
│  EUR        Einnahmen  Ausgaben         │
│   │                                     │
│  60├         ┌───┐                      │
│   │          │ E │                      │
│  50├         │   │                      │
│   │          │   │  ┌───┐               │
│  40├         │   │  │ A │               │
│   │          │   │  │   │               │
│  30├  ┌───┐  │   │  │   │  ┌───┐        │
│   │   │ E │  │   │  │   │  │   │        │
│  20├  │   │  │   │  │   │  │   │        │
│   │   │   │  │   │  │   │  │   │        │
│  10├  │   │  │   │  │   │  │   │        │
│   │   └───┘  └───┘  └───┘  └───┘        │
│  └────Nov────Dez────Jan────Feb────      │
│                                         │
│  ─────────────────────────────────────  │
│  Einnahmen:  Taschengeld  │  Geschenke  │
│  Ausgaben:   ████████████████████████   │
│                                         │
└─────────────────────────────────────────┘
```

**Features**
- Gestapelte/gruppierte Balken
- Einnahmen aufgeschlüsselt (Taschengeld, Geschenke, Zinsen)
- Ausgaben als Gesamtbetrag
- Kind-Auswahl (Dropdown oder Tabs)
- Zeitraum wählbar

**API**
```
GET /api/statistics/children/{childId}/income-expenses
?months=6

Response:
{
  "childId": "...",
  "childName": "Lisa",
  "months": [
    {
      "month": "2025-01",
      "income": {
        "total": 55.00,
        "allowance": 40.00,
        "gifts": 15.00,
        "interest": 0.00
      },
      "expenses": {
        "total": 32.00
      },
      "balance": 23.00
    },
    ...
  ]
}
```

---

### S095: Kategorie-Analyse (Eltern)

**User Story**
Als Elternteil möchte ich sehen, wofür meine Kinder ihr Geld ausgeben, um bei Bedarf zu beraten.

**Visualisierung**
```
┌─────────────────────────────────────────┐
│  Ausgaben nach Kategorie     Jan 2025  │
├─────────────────────────────────────────┤
│                                         │
│  Alle Kinder  │ Lisa │ Max │ Tim │      │
│  ─────────────────────────────────────  │
│                                         │
│  Süßigkeiten     ████████████  € 35,00  │
│    Lisa: €15 │ Max: €12 │ Tim: €8       │
│                                         │
│  Spielzeug       ████████      € 28,00  │
│    Lisa: €5  │ Max: €18 │ Tim: €5       │
│                                         │
│  Kleidung        █████         € 18,00  │
│    Lisa: €18 │ Max: €0  │ Tim: €0       │
│                                         │
│  Sonstiges       ███           € 12,00  │
│    Lisa: €4  │ Max: €5  │ Tim: €3       │
│                                         │
│  ─────────────────────────────────────  │
│  Gesamt:                       € 93,00  │
│                                         │
└─────────────────────────────────────────┘
```

**Features**
- Horizontale Balken pro Kategorie
- Filter: Alle Kinder oder einzelnes Kind
- Aufschlüsselung pro Kind unter jeder Kategorie
- Sortierung nach Betrag (höchste zuerst)

**API**
```
GET /api/statistics/family/expenses-by-category
?from=2025-01-01&to=2025-01-31

Response:
{
  "period": { "from": "2025-01-01", "to": "2025-01-31" },
  "totalExpenses": 93.00,
  "categories": [
    {
      "name": "Süßigkeiten",
      "total": 35.00,
      "byChild": [
        { "childId": "...", "name": "Lisa", "amount": 15.00 },
        { "childId": "...", "name": "Max", "amount": 12.00 },
        { "childId": "...", "name": "Tim", "amount": 8.00 }
      ]
    },
    ...
  ]
}
```

---

## Technische Notizen

### Chart-Library (Mobile)
- **Option 1**: LiveCharts2 (MAUI-kompatibel, open source)
- **Option 2**: Microcharts (leichtgewichtig)
- **Option 3**: SkiaSharp Custom Drawing

### Performance
- Statistiken serverseitig berechnen
- Caching für häufige Abfragen (Valkey)
- Pagination für lange Zeiträume

### Offline
- Letzte bekannte Statistiken lokal cachen
- Anzeige mit "Stand: vor X Minuten"

## Priorität

**Mittel** - Erweiterung, nicht MVP-kritisch

## Story Points (geschätzt)

34 (Summe aller Stories)

| Story | Beschreibung | SP |
|-------|--------------|-----|
| S090 | Ausgaben-Torte (Kind) | 5 |
| S091 | Kontostand-Verlauf (Kind) | 5 |
| S092 | Monatsvergleich (Kind) | 5 |
| S093 | Familien-Dashboard (Eltern) | 8 |
| S094 | Einnahmen/Ausgaben (Eltern) | 5 |
| S095 | Kategorie-Analyse (Eltern) | 5 |
| S096 | Zeitraum-Filter | 3 |
| S097 | PDF-Export (optional) | 3 |
