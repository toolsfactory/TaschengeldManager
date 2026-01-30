# Epic M005: Kontoverwaltung (Eltern-Perspektive)

**Status:** ⬜ Offen (0/22 SP)

## Beschreibung

Eltern können die Konten ihrer Kinder verwalten: Einzahlungen vornehmen, Ausgaben erfassen, Transaktionen stornieren und Zinsen konfigurieren.

## Business Value

Eltern benötigen vollständige Kontrolle über die Kinderkonten. Zins-Konfiguration ist ein pädagogisches Feature zum Erlernen von Sparen.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M005-S01 | Alle Kinderkonten anzeigen | 2 | ⬜ |
| M005-S02 | Konto-Detail mit Transaktionshistorie | 3 | ⬜ |
| M005-S03 | Einzahlung auf Kind-Konto | 2 | ⬜ |
| M005-S04 | Ausgabe für Kind erfassen | 2 | ⬜ |
| M005-S05 | Transaktion stornieren | 2 | ⬜ |
| M005-S06 | Zinsen konfigurieren (Ein/Aus, Rate, Intervall) | 3 | ⬜ |
| M005-S07 | Zins-Vorschau Rechner | 2 | ⬜ |
| M005-S08 | Transaktions-Export (CSV/PDF) | 3 | ⬜ |
| M005-S09 | Monatsübersicht/Statistiken pro Kind | 3 | ⬜ |

**Gesamt: 22 SP**

## Abhängigkeiten

- M001-M003 (Basis-Setup)
- M004 (Kind-Kontofunktionen als Referenz)

## Akzeptanzkriterien (Epic-Level)

- [ ] Eltern sehen alle Kinderkonten mit Kontostand
- [ ] Eltern können Konto-Details mit Historie aufrufen
- [ ] Einzahlungen können getätigt werden
- [ ] Ausgaben können für Kinder erfasst werden
- [ ] Transaktionen können storniert werden
- [ ] Zinsen können pro Kind aktiviert/deaktiviert werden
- [ ] Zinssatz und Intervall sind konfigurierbar
- [ ] Vorschau zeigt erwartete Zinsen
- [ ] Transaktionen können exportiert werden
- [ ] Monatsstatistiken zeigen Übersicht

## Geplante Pages

- `ParentChildAccountPage` - Konto-Detail eines Kindes
- `ParentDepositPage` - Einzahlung tätigen
- `ParentExpensePage` - Ausgabe für Kind erfassen
- `ParentInterestConfigPage` - Zinsen konfigurieren
- `ParentStatisticsPage` - Monatsübersicht

## Priorität

**Hoch** - Kernfunktion für Eltern

## Story Points

22 SP (0 SP abgeschlossen)
