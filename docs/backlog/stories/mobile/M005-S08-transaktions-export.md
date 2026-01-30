# Story M005-S08: Transaktions-Export (CSV/PDF)

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **die Transaktionshistorie als CSV oder PDF exportieren können**, damit **ich die Daten archivieren oder für andere Zwecke verwenden kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Transaktionshistorie, wenn ich auf "Exportieren" tippe, dann kann ich zwischen CSV und PDF wählen
- [ ] Gegeben der Export-Dialog, wenn ich einen Zeitraum auswähle, dann werden nur Transaktionen aus diesem Zeitraum exportiert
- [ ] Gegeben ein CSV-Export, wenn er generiert wird, dann enthält er alle relevanten Felder (Datum, Betrag, Beschreibung, Kategorie, Typ)
- [ ] Gegeben ein PDF-Export, wenn er generiert wird, dann enthält er einen formatierten Kontoauszug mit Kopfzeile
- [ ] Gegeben ein fertiger Export, wenn er abgeschlossen ist, dann kann ich die Datei teilen oder speichern
- [ ] Gegeben der Export, wenn er generiert wird, dann sehe ich einen Fortschrittsindikator

## UI-Entwurf

```
┌─────────────────────────────┐
│  × Transaktionen exportieren│
├─────────────────────────────┤
│                             │
│  Konto: Emma                │
│                             │
│  Zeitraum:                  │
│  Von: [01.01.2024 📅]       │
│  Bis: [31.01.2024 📅]       │
│                             │
│  Schnellauswahl:            │
│  [Diesen Monat] [Letzter Mo]│
│  [Dieses Jahr] [Alles]      │
│                             │
│  Format:                    │
│  ┌─────────────────────────┐│
│  │ ○ CSV (Excel-kompatibel)││
│  │ ○ PDF (Kontoauszug)     ││
│  └─────────────────────────┘│
│                             │
│  Inhalt:                    │
│  [✓] Transaktionen          │
│  [✓] Kontostand             │
│  [ ] Zinsinformationen      │
│                             │
│  ┌───────────────────────┐  │
│  │     Exportieren       │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘

Nach Export:
┌─────────────────────────────┐
│         ✓ Fertig!           │
│                             │
│  emma_kontoauszug_2024.pdf  │
│                             │
│  [📤 Teilen] [💾 Speichern] │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ExportTransactionsPage.xaml` (als Modal)
- **ViewModel**: `ExportTransactionsViewModel.cs`
- **Services**: `IExportService.cs`, `IPdfGeneratorService.cs`

## API-Endpunkt

```
POST /api/children/{childId}/transactions/export
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "fromDate": "2024-01-01",
  "toDate": "2024-01-31",
  "format": "pdf",
  "includeBalance": true,
  "includeInterestInfo": false
}

Response 200:
Content-Type: application/pdf (oder text/csv)
Content-Disposition: attachment; filename="emma_kontoauszug_2024-01.pdf"

[Binary PDF/CSV data]
```

## PDF-Vorlage

```
┌─────────────────────────────────────────┐
│  TaschengeldManager                     │
│  Kontoauszug                            │
│─────────────────────────────────────────│
│  Name: Emma Mustermann                  │
│  Zeitraum: 01.01.2024 - 31.01.2024      │
│  Erstellt am: 15.01.2024                │
│─────────────────────────────────────────│
│  Anfangsbestand: 35,00 €                │
│  Einzahlungen:  +25,00 €                │
│  Ausgaben:      -15,00 €                │
│  Endbestand:    45,00 €                 │
│─────────────────────────────────────────│
│  Datum      Beschreibung       Betrag   │
│  01.01.24   Taschengeld       +5,00 €   │
│  05.01.24   Süßigkeiten       -2,50 €   │
│  ...                                    │
└─────────────────────────────────────────┘
```

## Technische Notizen

- PDF-Generierung evtl. serverseitig für Konsistenz
- CSV mit UTF-8 BOM für Excel-Kompatibilität
- Share-Sheet für natives Teilen verwenden
- Export-Dateien temporär speichern und nach Teilen löschen
- Bei großen Datenmengen: Streaming/Pagination

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-08-01 | CSV-Export eines Monats | Valide CSV-Datei |
| TC-M005-08-02 | PDF-Export eines Monats | Formatiertes PDF |
| TC-M005-08-03 | Export ohne Transaktionen | Hinweis, leerer Export |
| TC-M005-08-04 | Datei teilen | Share-Sheet öffnet sich |
| TC-M005-08-05 | Großer Zeitraum | Export erfolgreich |

## Story Points

3

## Priorität

Niedrig

## Status

⬜ Offen
