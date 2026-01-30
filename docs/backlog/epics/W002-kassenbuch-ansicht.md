# Epic W002: Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## Beschreibung

Kinder erhalten eine klassische Kassenbuch-Ansicht ihrer Kontobewegungen als Monatsübersicht. Die Darstellung orientiert sich an einem traditionellen Kassenbuch mit übersichtlichen Spalten für Datum, Beschreibung, Einnahmen, Ausgaben und laufendem Saldo. Dies fördert das Verständnis für Buchführung und Finanzen.

## Business Value

- **Finanzielle Bildung**: Kinder lernen das Prinzip eines Kassenbuchs kennen
- **Transparenz**: Klare Übersicht über alle Geldbewegungen im Monat
- **Nachvollziehbarkeit**: Jede Transaktion ist mit Datum, Betrag und Beschreibung dokumentiert
- **Lerneffekt**: Verständnis für Einnahmen vs. Ausgaben und laufenden Kontostand

## Betroffene Benutzerrollen

- **Primär**: Kinder (Hauptnutzer der Ansicht)
- **Sekundär**: Eltern (können dieselbe Ansicht für Kinderkonten sehen)

## User Stories

| Story ID | Titel | Status | Story Points |
|----------|-------|--------|--------------|
| W002-S01 | Kassenbuch-Monatsansicht Grundstruktur | FERTIG | 5 |
| W002-S02 | Monatsnavigation und -auswahl | FERTIG | 3 |
| W002-S03 | Monatszusammenfassung (Kopfbereich) | FERTIG | 3 |
| W002-S04 | Kassenbuch-Einträge mit Kategorien | FERTIG | 3 |
| W002-S05 | Eltern-Ansicht für Kinderkonten | FERTIG | 2 |
| W002-S06 | Mobile-optimierte Darstellung | FERTIG | 3 |
| W002-S07 | Dark Mode Support | FERTIG | 2 |

## Akzeptanzkriterien (Epic-Level)

### Funktional
- [x] Monatsweise Darstellung aller Transaktionen
- [x] Klassisches Kassenbuch-Layout mit Spalten: Datum | Beschreibung | Einnahmen | Ausgaben | Saldo
- [x] Navigation zwischen Monaten (vor/zurück)
- [x] Direktauswahl eines Monats über Dropdown oder Kalender
- [x] Monatszusammenfassung mit Anfangssaldo, Summe Einnahmen, Summe Ausgaben, Endsaldo

### Visuell
- [x] Einnahmen in Grün dargestellt
- [x] Ausgaben in Rot dargestellt
- [x] Saldo-Spalte zeigt laufenden Kontostand
- [x] Zebrastreifen für bessere Lesbarkeit
- [x] Responsive Design für Desktop und Mobile

### Technisch
- [x] Nutzung der bestehenden Transaktions-API
- [ ] Lazy Loading für große Datenmengen (optional)
- [x] Dark Mode kompatibel

## Wireframe

```
┌─────────────────────────────────────────────────────────────────────┐
│                         📖 Mein Kassenbuch                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│   ◀ Dezember 2025                    [Monat wählen ▼]      ▶        │
│                                                                     │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │  Anfangssaldo: € 45,00                                        │  │
│  │  ─────────────────────────────────────────────────────────────│  │
│  │  + Einnahmen:   € 55,00    │    - Ausgaben:    € 32,50       │  │
│  │  ─────────────────────────────────────────────────────────────│  │
│  │  Endsaldo:      € 67,50                                       │  │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                     │
│  ┌───────────────────────────────────────────────────────────────┐  │
│  │ Datum     │ Beschreibung          │ Einnahmen │ Ausgaben │Saldo│ │
│  ├───────────┼───────────────────────┼───────────┼──────────┼─────┤ │
│  │ 01.12.    │ Übertrag              │           │          │45,00│ │
│  │ 03.12.    │ Taschengeld           │   +10,00  │          │55,00│ │
│  │ 05.12.    │ 🍬 Süßigkeiten        │           │   -3,50  │51,50│ │
│  │ 08.12.    │ 🎁 Geschenk von Oma   │   +20,00  │          │71,50│ │
│  │ 10.12.    │ 🎮 Spielzeug          │           │  -15,00  │56,50│ │
│  │ 15.12.    │ 📈 Zinsen             │    +0,50  │          │57,00│ │
│  │ 18.12.    │ 🍬 Süßigkeiten        │           │   -4,00  │53,00│ │
│  │ 22.12.    │ Taschengeld           │   +10,00  │          │63,00│ │
│  │ 24.12.    │ 🎁 Weihnachtsgeld     │   +15,00  │          │78,00│ │
│  │ 27.12.    │ 📚 Buch               │           │  -10,00  │68,00│ │
│  │ 31.12.    │ 📈 Zinsen             │    +0,50  │          │68,50│ │
│  ├───────────┼───────────────────────┼───────────┼──────────┼─────┤ │
│  │           │ Summen                │   +56,00  │  -32,50  │     │ │
│  └───────────────────────────────────────────────────────────────┘  │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

## Technische Notizen

### Benötigte API-Daten
- Transaktionen für den gewählten Monat (bereits vorhanden: `GET /api/account/{id}/transactions`)
- Kontostand zu Monatsbeginn (neu berechnen oder aus Transaktionshistorie ableiten)

### Komponenten-Struktur (React)
```
src/pages/
  Cashbook.tsx              # Hauptkomponente
src/components/
  CashbookHeader.tsx        # Monatszusammenfassung
  CashbookTable.tsx         # Tabellenkomponente
  CashbookRow.tsx           # Einzelne Zeile
  MonthSelector.tsx         # Monatsnavigation
```

### Datentransformation
```typescript
interface CashbookEntry {
  date: Date;
  description: string;
  category?: string;
  categoryIcon?: string;
  income?: number;       // Einnahmen (positiv)
  expense?: number;      // Ausgaben (positiv, ohne Minus)
  runningBalance: number; // Laufender Saldo
}

interface CashbookMonth {
  month: string;         // "2025-12"
  openingBalance: number;
  closingBalance: number;
  totalIncome: number;
  totalExpenses: number;
  entries: CashbookEntry[];
}
```

## Abhängigkeiten

- E003 (Transaktionen) - Basis-Daten
- W001 (Dark Mode) - Styling-Konsistenz
- Bestehende Transaktions-API

## Priorität

**Mittel** - Erweiterte Funktion für bessere User Experience

## Geschätzter Gesamtaufwand

21 Story Points

## Risiken

- Performance bei vielen Transaktionen pro Monat
- Berechnung des Anfangssaldos erfordert ggf. zusätzliche API-Logik

## Offene Fragen

1. Soll die Ansicht druckbar sein (Print-CSS)?
2. Soll Export als PDF/CSV möglich sein?
3. Sollen Eltern dieselbe Ansicht für ihre Kinder haben?
