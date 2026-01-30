# Story S097: Export als PDF

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Elternteil** möchte ich **Statistiken und Uebersichten als PDF exportieren koennen**, damit **ich sie archivieren, drucken oder teilen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine Statistik-Ansicht, wenn das Elternteil auf "Als PDF exportieren" tippt, dann wird ein PDF generiert
- [ ] Gegeben das generierte PDF, dann enthaelt es alle sichtbaren Diagramme und Daten
- [ ] Gegeben das PDF, dann ist es im A4-Format und fuer Druck optimiert
- [ ] Gegeben der Export, dann wird ein Download-Link oder Share-Dialog angeboten
- [ ] Gegeben mehrere Kinder, wenn ein Familien-Bericht exportiert wird, dann sind alle Kinder enthalten
- [ ] Gegeben das PDF, dann enthaelt es Kopfzeile mit Datum und Zeitraum

## UI-Entwurf (ASCII)

### Export-Button in Statistik-Ansicht

```
+---------------------------------------+
|  <- Zurueck   Familien-Uebersicht     |
+---------------------------------------+
|                                       |
|  [Zeitraum: Januar 2025 v]  [PDF]     |
|                                       |
|       ... Statistik-Inhalt ...        |
|                                       |
+---------------------------------------+
```

### Export-Dialog

```
+---------------------------------------+
|          PDF Exportieren              |
+---------------------------------------+
|                                       |
|  Was moechten Sie exportieren?        |
|                                       |
|  [ ] Familien-Dashboard               |
|  [x] Einnahmen/Ausgaben (alle Kinder) |
|  [x] Kategorie-Analyse                |
|  [ ] Nur Lisa                         |
|  [ ] Nur Max                          |
|  [ ] Nur Tim                          |
|                                       |
|  Zeitraum: Januar 2025                |
|                                       |
|  Format:                              |
|  (o) Zusammenfassung                  |
|  ( ) Detailliert (mit Transaktionen)  |
|                                       |
|  [Abbrechen]    [PDF erstellen]       |
|                                       |
+---------------------------------------+
```

### Generierung (Ladeansicht)

```
+---------------------------------------+
|                                       |
|                                       |
|          (Ladeanimation)              |
|                                       |
|       PDF wird erstellt...            |
|                                       |
|       Bitte warten                    |
|                                       |
|                                       |
+---------------------------------------+
```

### Fertig-Dialog

```
+---------------------------------------+
|          PDF erstellt                 |
+---------------------------------------+
|                                       |
|       (Haekchen-Icon)                 |
|                                       |
|  Ihr Bericht ist fertig!              |
|                                       |
|  Taschengeld-Bericht_Jan2025.pdf      |
|  2,3 MB                               |
|                                       |
|  [Oeffnen]  [Teilen]  [Speichern]     |
|                                       |
+---------------------------------------+
```

### PDF-Layout (Seitenvorschau)

```
+---------------------------------------+
|  TaschengeldManager                   |
|  Familien-Bericht                     |
|  Januar 2025                          |
|  Erstellt am: 15.01.2025              |
+---------------------------------------+
|                                       |
|  FAMILIEN-UEBERSICHT                  |
|  ___________________________________  |
|                                       |
|  Gesamtkontostand: EUR 205,50         |
|  Ausgaben Januar:  EUR  67,30         |
|                                       |
|  +-------------+  +-------------+     |
|  |    Lisa     |  |     Max     |     |
|  | EUR 125,50  |  |  EUR 48,00  |     |
|  +-------------+  +-------------+     |
|                                       |
|  AUSGABEN NACH KATEGORIE              |
|  ___________________________________  |
|                                       |
|  [Tortendiagramm]                     |
|                                       |
|  Suessigkeiten: EUR 35,00 (30%)       |
|  Spielzeug:     EUR 28,00 (24%)       |
|  ...                                  |
|                                       |
|---------------------------------------+
|  Seite 1 von 3                        |
+---------------------------------------+
```

## API-Endpunkt

### Request

```http
POST /api/statistics/export/pdf
Authorization: Bearer {token}
Content-Type: application/json
```

### Request Body

```json
{
  "reportType": "family_summary",
  "period": {
    "from": "2025-01-01",
    "to": "2025-01-31"
  },
  "sections": [
    "family_dashboard",
    "income_expenses",
    "category_analysis"
  ],
  "childFilter": null,
  "format": "summary",
  "includeTransactions": false
}
```

### Request Body (Einzelnes Kind, detailliert)

```json
{
  "reportType": "child_detail",
  "period": {
    "from": "2025-01-01",
    "to": "2025-01-31"
  },
  "sections": [
    "balance_history",
    "expenses_pie_chart",
    "month_comparison",
    "transactions"
  ],
  "childFilter": "child-001",
  "format": "detailed",
  "includeTransactions": true
}
```

### Response 202 Accepted (Asynchrone Generierung)

```json
{
  "exportId": "exp-001",
  "status": "processing",
  "estimatedTimeSeconds": 15,
  "statusUrl": "/api/statistics/export/pdf/exp-001/status"
}
```

### Status Endpoint

```http
GET /api/statistics/export/pdf/{exportId}/status
```

### Response 200 OK (In Bearbeitung)

```json
{
  "exportId": "exp-001",
  "status": "processing",
  "progress": 60,
  "currentStep": "Generiere Diagramme..."
}
```

### Response 200 OK (Fertig)

```json
{
  "exportId": "exp-001",
  "status": "completed",
  "progress": 100,
  "file": {
    "name": "Taschengeld-Bericht_Jan2025.pdf",
    "sizeBytes": 2412345,
    "downloadUrl": "/api/statistics/export/pdf/exp-001/download",
    "expiresAt": "2025-01-15T12:00:00Z"
  }
}
```

### Download Endpoint

```http
GET /api/statistics/export/pdf/{exportId}/download
```

### Response 200 OK

```http
Content-Type: application/pdf
Content-Disposition: attachment; filename="Taschengeld-Bericht_Jan2025.pdf"

[Binary PDF Data]
```

### Response 404 Not Found (Abgelaufen)

```json
{
  "error": "EXPORT_EXPIRED",
  "message": "Der Export ist abgelaufen. Bitte erstellen Sie einen neuen."
}
```

### Response 403 Forbidden

```json
{
  "error": "ACCESS_DENIED",
  "message": "Nur Eltern koennen PDF-Berichte exportieren"
}
```

## Technische Notizen

### Backend

- PDF-Generierung asynchron (Background Job)
- Chart-Rendering serverseitig (kein Client-Dependencies)
- Temporaere Speicherung der PDFs (TTL: 1 Stunde)
- Maximale Berichtgroesse: 10 MB

### PDF-Library Empfehlungen

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| QuestPDF | Fluent API, .NET native, kostenlos | Weniger bekannt |
| iText7 | Industrie-Standard, viele Features | Lizenzkosten (AGPL) |
| PdfSharp | Open Source, einfach | Keine Chart-Integration |
| Puppeteer/Playwright | HTML zu PDF, einfaches Layout | Node.js Dependency |

### Empfehlung: QuestPDF

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class FamilyReportDocument : IDocument
{
    private readonly FamilyReportData _data;

    public FamilyReportDocument(FamilyReportData data)
    {
        _data = data;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("TaschengeldManager")
                    .FontSize(20).Bold();
                column.Item().Text("Familien-Bericht")
                    .FontSize(14);
                column.Item().Text($"{_data.Period.From:MMMM yyyy}")
                    .FontSize(12).FontColor(Colors.Grey.Medium);
            });

            row.ConstantItem(100).Image("logo.png");
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            // Familien-Dashboard
            column.Item().Element(ComposeFamilyDashboard);

            // Kategorie-Analyse
            column.Item().Element(ComposeCategoryAnalysis);

            // Charts als Bilder einbetten
            column.Item().Image(_chartService.RenderPieChart(_data.Categories));
        });
    }
}
```

### Chart-Rendering (Server-seitig)

```csharp
public interface IChartRenderService
{
    byte[] RenderPieChart(List<CategoryData> data);
    byte[] RenderLineChart(List<DataPoint> data);
    byte[] RenderBarChart(List<BarData> data);
}

// Implementierung mit SkiaSharp
public class SkiaChartRenderService : IChartRenderService
{
    public byte[] RenderPieChart(List<CategoryData> data)
    {
        using var surface = SKSurface.Create(new SKImageInfo(400, 400));
        var canvas = surface.Canvas;

        // Tortendiagramm zeichnen...

        using var image = surface.Snapshot();
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }
}
```

### Mobile (MAUI)

- Share-API fuer native Teilen-Dialog
- FileSystem-API fuer lokales Speichern
- Polling fuer Status-Updates waehrend Generierung

```csharp
public async Task ExportAndSharePdf()
{
    // 1. Export starten
    var exportResponse = await _statisticsApi.StartExportAsync(request);

    // 2. Auf Fertigstellung warten
    while (exportResponse.Status != "completed")
    {
        await Task.Delay(2000);
        exportResponse = await _statisticsApi.GetExportStatusAsync(exportResponse.ExportId);
        OnProgressChanged(exportResponse.Progress);
    }

    // 3. Download
    var pdfBytes = await _statisticsApi.DownloadExportAsync(exportResponse.ExportId);

    // 4. Temporaer speichern
    var filePath = Path.Combine(FileSystem.CacheDirectory, exportResponse.File.Name);
    await File.WriteAllBytesAsync(filePath, pdfBytes);

    // 5. Share-Dialog anzeigen
    await Share.RequestAsync(new ShareFileRequest
    {
        Title = "Bericht teilen",
        File = new ShareFile(filePath)
    });
}
```

### Performance

- Asynchrone Generierung (Queue-basiert)
- Chart-Caching fuer wiederkehrende Anfragen
- Parallel-Generierung mehrerer Sektionen
- Timeout: 60 Sekunden

### Sicherheit

- Nur Eltern duerfen exportieren
- Export-IDs sind nicht erratbar (UUID)
- PDFs nach 1 Stunde automatisch loeschen
- Rate-Limiting: Max 5 Exports pro Stunde

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-097-1 | Familien-Bericht exportieren | PDF mit allen Kindern |
| TC-097-2 | Einzelnes Kind exportieren | PDF nur mit Kind-Daten |
| TC-097-3 | PDF herunterladen | Gueltiges PDF-File |
| TC-097-4 | PDF oeffnen | Lesbares Dokument |
| TC-097-5 | PDF teilen (iOS/Android) | Native Share-Dialog |
| TC-097-6 | Export abgelaufen | 404 mit Hinweis |
| TC-097-7 | Kind versucht Export | 403 Forbidden |
| TC-097-8 | Langer Zeitraum (1 Jahr) | PDF innerhalb 60s generiert |
| TC-097-9 | Detailliert mit Transaktionen | Transaktionsliste im PDF |

## Abhaengigkeiten

- S093 - Familien-Dashboard (Datenquelle)
- S094 - Einnahmen/Ausgaben (Datenquelle)
- S095 - Kategorie-Analyse (Datenquelle)
- S096 - Zeitraum-Filter

## Story Points

3

## Prioritaet

Niedrig (Optional, Nice-to-have)
