# Epic M004: Kontoverwaltung (Kind-Perspektive)

**Status:** 🔶 Teilweise abgeschlossen (13/16 SP)

## Beschreibung

Alle Funktionen für Kinder zur Verwaltung ihres Taschengeldes: Kontostand, Transaktionen, Ausgaben erfassen.

## Business Value

Kinder lernen den Umgang mit Geld durch Transparenz über eigene Finanzen und aktive Beteiligung.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M004-S01 | Kontostand-Anzeige mit Animation | 2 | ✅ |
| M004-S02 | Transaktionsliste mit Filterung | 3 | ✅ |
| M004-S03 | Ausgabe erfassen (Betrag, Kategorie, Notiz) | 3 | ✅ |
| M004-S04 | Kategorie-Auswahl mit Icons | 2 | ✅ |
| M004-S05 | Transaktionsdetail-Ansicht | 2 | ✅ |
| M004-S06 | Zins-Gutschriften anzeigen | 1 | ✅ |
| M004-S07 | Geschenke-Eingang anzeigen (von Verwandten) | 1 | ⬜ |
| M004-S08 | Dankeschön an Verwandten senden | 2 | ⬜ |

**Gesamt: 16 SP** (13 SP abgeschlossen, 3 SP offen)

## Abhängigkeiten

- M001-M003 (Basis-Setup)
- M009 (für Geschenke-Integration)

## Akzeptanzkriterien (Epic-Level)

- [x] Kind sieht aktuellen Kontostand
- [x] Kontostand animiert bei Änderungen
- [x] Transaktionen können gefiltert werden (Typ, Datum, Suche)
- [x] Ausgaben können erfasst werden
- [x] Kategorien haben Icons
- [x] Zins-Gutschriften sind erkennbar (📈)
- [ ] Geschenke von Verwandten sind markiert
- [ ] Dankeschön kann gesendet werden

## Implementierte Pages

- `ChildDashboardPage` - Übersicht mit Kontostand und letzten Transaktionen
- `ChildHistoryPage` - Vollständige Transaktionsliste mit Filterung
- `ChildAddExpensePage` - Ausgabe erfassen
- `ChildTransactionDetailPage` - Details einer Transaktion

## Implementierte ViewModels

- `ChildDashboardViewModel` - Dashboard-Logik mit Animation
- `ChildHistoryViewModel` - Filterung und Suche
- `ChildAddExpenseViewModel` - Ausgabe-Erfassung
- `ChildTransactionDetailViewModel` - Transaktionsdetails

## Kategorien

| Icon | Name | Farbe |
|------|------|-------|
| 🍬 | Süßigkeiten | Pink |
| 🎮 | Spielzeug | Purple |
| 👕 | Kleidung | Blue |
| 📚 | Bücher | Brown |
| 🍕 | Essen | Orange |
| ⚽ | Hobby | Green |
| 🎁 | Geschenke | Red |
| 📦 | Sonstiges | Gray |

## Transaktions-Typen

| Icon | Typ | Farbe |
|------|-----|-------|
| 💰 | Einnahme | Green |
| 💸 | Ausgabe | Red |
| 📈 | Zinsen | Teal |

## Filter-Funktionen

- **Typ-Filter:** Alle / Einnahmen / Ausgaben
- **Datumsbereich:** Von-Bis mit DatePicker
- **Textsuche:** Beschreibung und Kategorie
- **Summen-Anzeige:** Anzahl und Gesamtbetrag

## Priorität

**Hoch** - Kernfunktion für Kinder

## Story Points

16 SP (13 SP abgeschlossen, 3 SP offen)
