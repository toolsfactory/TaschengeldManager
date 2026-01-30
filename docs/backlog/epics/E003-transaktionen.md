# Epic E003: Transaktionen (Einnahmen/Ausgaben)

## Beschreibung

Einnahmen und Ausgaben können auf Taschengeld-Konten gebucht werden. Eltern können Einzahlungen vornehmen, Kinder können ihre Ausgaben erfassen.

## Business Value

Die Kernfunktion der App - ohne Transaktionen kein Taschengeld-Management. Ermöglicht das Tracking von Geldflüssen und lehrt Kinder den Umgang mit Geld.

## Stories

- [x] S020 - Einzahlung auf Kind-Konto (Eltern) ✅
- [x] S021 - Ausgabe erfassen (Kind) ✅
- [x] S022 - Ausgabe erfassen (Eltern für Kind) ✅
- [x] S023 - Transaktionshistorie anzeigen ✅
- [x] S024 - Transaktion mit Kategorie versehen ✅
- [x] S025 - Transaktion mit Notiz versehen ✅
- [x] S026 - Transaktion stornieren (nur Eltern) ✅

## Abhängigkeiten

- E001 (Benutzerverwaltung)
- E002 (Kontoverwaltung)

## Akzeptanzkriterien (Epic-Level)

- [x] Eltern können beliebige Beträge einzahlen ✅
- [x] Kinder können Ausgaben bis zum verfügbaren Kontostand erfassen ✅
- [x] Jede Transaktion hat: Betrag, Typ, Datum, optional Kategorie/Notiz ✅
- [x] Transaktionshistorie ist chronologisch sortiert ✅
- [x] Kontostand wird automatisch aktualisiert ✅
- [x] Negative Kontostände sind nicht erlaubt ✅

## Transaktionstypen

| Typ | Beschreibung | Wer kann |
|-----|--------------|----------|
| `Deposit` | Einzahlung | Eltern |
| `Withdrawal` | Auszahlung/Ausgabe | Eltern, Kinder |
| `Allowance` | Taschengeld (automatisch) | System |
| `Reversal` | Storno | Eltern |

## Kategorien (Vorschläge)

- Süßigkeiten
- Spielzeug
- Kleidung
- Sparen
- Geschenke
- Sonstiges

## Priorität

**Hoch** - MVP-Blocker

## Story Points (geschätzt)

21 (Summe aller Stories)
