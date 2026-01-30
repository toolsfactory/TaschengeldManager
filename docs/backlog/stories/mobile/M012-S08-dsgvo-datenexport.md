# Story M012-S08: DSGVO-Datenexport

## Epic

M012 - Profil & Account-Verwaltung

## User Story

Als **Benutzer** moechte ich **alle meine gespeicherten Daten exportieren koennen**, damit **ich DSGVO-konform Auskunft ueber meine Daten erhalte**.

## Akzeptanzkriterien

- [ ] Gegeben ein angemeldeter Benutzer, wenn er einen Datenexport anfordert, dann wird der Export vorbereitet
- [ ] Gegeben ein angeforderter Export, wenn er fertig ist, dann erhaelt der Benutzer eine Benachrichtigung
- [ ] Gegeben ein fertiger Export, wenn der Benutzer ihn herunterlaed, dann enthaelt er alle personenbezogenen Daten
- [ ] Gegeben ein Export, wenn er erstellt wird, dann ist er in einem lesbaren Format (JSON oder PDF)
- [ ] Gegeben ein angeforderter Export, wenn 24 Stunden vergangen sind, dann wird er automatisch geloescht

## UI-Entwurf

### Datenexport anfordern
```
+------------------------------------+
|  <- Zurueck       Datenexport      |
+------------------------------------+
|                                    |
|  Deine Daten exportieren           |
|                                    |
|  Fordere eine Kopie aller deiner   |
|  gespeicherten Daten an. Der       |
|  Export wird als ZIP-Datei         |
|  bereitgestellt.                   |
|                                    |
|  Enthalten sind:                   |
|  - Persoenliche Daten              |
|  - Transaktionshistorie            |
|  - Geldanfragen                    |
|  - Einstellungen                   |
|                                    |
|  Format                            |
|  +--------------------------------+|
|  | ( ) JSON (maschinenlesbar)     ||
|  | (x) PDF (lesbar)               ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |  [Download] Export anfordern   ||
|  +--------------------------------+|
|                                    |
|  Der Export kann einige Minuten    |
|  dauern. Du wirst benachrichtigt,  |
|  wenn er bereit ist.               |
|                                    |
+------------------------------------+
```

### Export bereit
```
+------------------------------------+
|  <- Zurueck       Datenexport      |
+------------------------------------+
|                                    |
|  [Check] Export bereit!            |
|                                    |
|  Dein Datenexport vom 20.01.2026   |
|  ist bereit zum Download.          |
|                                    |
|  Datei: export_20260120.zip        |
|  Groesse: 1,2 MB                   |
|                                    |
|  +--------------------------------+|
|  |     [Download] Herunterladen   ||
|  +--------------------------------+|
|                                    |
|  Hinweis: Der Download ist 24      |
|  Stunden verfuegbar.               |
|                                    |
|  Verfuegbar bis: 21.01.2026, 10:00 |
|                                    |
+------------------------------------+
```

### Export wird erstellt
```
+------------------------------------+
|  <- Zurueck       Datenexport      |
+------------------------------------+
|                                    |
|  [Loading] Export wird erstellt... |
|                                    |
|  Dein Datenexport wird gerade      |
|  vorbereitet. Du wirst             |
|  benachrichtigt, sobald er         |
|  bereit ist.                       |
|                                    |
|  Angefordert: 20.01.2026, 10:00    |
|                                    |
+------------------------------------+
```

## API-Endpunkte

### Export anfordern
```
POST /api/users/me/export
Authorization: Bearer {token}
Content-Type: application/json

{
  "format": "pdf" | "json"
}

Response 202:
{
  "exportId": "guid",
  "status": "processing",
  "requestedAt": "2026-01-20T10:00:00Z",
  "estimatedCompletionTime": "2026-01-20T10:05:00Z"
}
```

### Export-Status pruefen
```
GET /api/users/me/export/status
Authorization: Bearer {token}

Response 200 (kein Export):
{
  "hasExport": false
}

Response 200 (in Bearbeitung):
{
  "hasExport": true,
  "status": "processing",
  "requestedAt": "2026-01-20T10:00:00Z"
}

Response 200 (bereit):
{
  "hasExport": true,
  "status": "ready",
  "exportId": "guid",
  "fileName": "export_20260120.zip",
  "fileSize": 1258000,
  "createdAt": "2026-01-20T10:05:00Z",
  "expiresAt": "2026-01-21T10:05:00Z",
  "downloadUrl": "/api/users/me/export/download"
}
```

### Export herunterladen
```
GET /api/users/me/export/download
Authorization: Bearer {token}

Response 200:
Content-Type: application/zip
Content-Disposition: attachment; filename="export_20260120.zip"

(Binary ZIP data)
```

## Technische Notizen

- Backend-Job verarbeitet Export asynchron
- ZIP-Datei enthaelt: JSON-Dateien + optional PDF-Zusammenfassung
- Verschluesselte Speicherung bis zum Download
- Automatische Loeschung nach 24 Stunden
- Push-Benachrichtigung wenn fertig

## Export-Inhalt

### JSON-Struktur
```json
{
  "exportDate": "2026-01-20T10:05:00Z",
  "user": {
    "id": "guid",
    "email": "max@example.com",
    "firstName": "Max",
    "lastName": "Mustermann",
    "role": "Parent",
    "createdAt": "2025-01-15T10:00:00Z"
  },
  "transactions": [
    {
      "id": "guid",
      "amount": -2.50,
      "description": "Suessigkeiten",
      "category": "Essen",
      "date": "2026-01-20T14:30:00Z"
    }
  ],
  "requests": [...],
  "settings": {...},
  "notifications": [...]
}
```

## Implementierungshinweise

```csharp
public partial class DataExportViewModel : BaseViewModel
{
    private readonly IDataExportService _exportService;

    [ObservableProperty]
    private ExportFormat _selectedFormat = ExportFormat.Pdf;

    [ObservableProperty]
    private ExportStatus _status = ExportStatus.None;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private long _fileSize;

    [ObservableProperty]
    private DateTime? _expiresAt;

    [RelayCommand]
    private async Task LoadStatusAsync()
    {
        var export = await _exportService.GetExportStatusAsync();

        if (!export.HasExport)
        {
            Status = ExportStatus.None;
            return;
        }

        Status = export.Status == "processing"
            ? ExportStatus.Processing
            : ExportStatus.Ready;

        if (Status == ExportStatus.Ready)
        {
            FileName = export.FileName;
            FileSize = export.FileSize;
            ExpiresAt = export.ExpiresAt;
        }
    }

    [RelayCommand]
    private async Task RequestExportAsync()
    {
        IsBusy = true;

        try
        {
            await _exportService.RequestExportAsync(SelectedFormat);
            Status = ExportStatus.Processing;
            await _toastService.ShowSuccessAsync("Export wird vorbereitet");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DownloadExportAsync()
    {
        IsBusy = true;

        try
        {
            var filePath = await _exportService.DownloadExportAsync();

            // Share/Open the file
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Datenexport",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await _toastService.ShowErrorAsync("Download fehlgeschlagen");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public enum ExportStatus
{
    None,
    Processing,
    Ready
}

public enum ExportFormat
{
    Json,
    Pdf
}

// Backend: Export Job
public class DataExportJob : IJob
{
    public async Task<string> CreateExportAsync(Guid userId, ExportFormat format)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        var requests = await _requestRepository.GetByUserIdAsync(userId);
        var settings = await _settingsRepository.GetByUserIdAsync(userId);

        var exportData = new ExportData
        {
            ExportDate = DateTime.UtcNow,
            User = MapUser(user),
            Transactions = transactions.Select(MapTransaction).ToList(),
            Requests = requests.Select(MapRequest).ToList(),
            Settings = MapSettings(settings)
        };

        var zipPath = await CreateZipFileAsync(exportData, format);
        return zipPath;
    }

    private async Task<string> CreateZipFileAsync(ExportData data, ExportFormat format)
    {
        var tempPath = Path.GetTempFileName();

        using var zip = ZipFile.Open(tempPath, ZipArchiveMode.Create);

        // JSON hinzufuegen
        var jsonEntry = zip.CreateEntry("data.json");
        using (var stream = jsonEntry.Open())
        {
            await JsonSerializer.SerializeAsync(stream, data);
        }

        // Optional: PDF hinzufuegen
        if (format == ExportFormat.Pdf)
        {
            var pdfBytes = await GeneratePdfAsync(data);
            var pdfEntry = zip.CreateEntry("datenexport.pdf");
            using var pdfStream = pdfEntry.Open();
            await pdfStream.WriteAsync(pdfBytes);
        }

        return tempPath;
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Export anfordern | Status wechselt zu "Processing" |
| TC-002 | Export fertig | Push-Benachrichtigung wird gesendet |
| TC-003 | Export herunterladen | ZIP-Datei wird gespeichert |
| TC-004 | JSON-Format | Nur JSON in ZIP |
| TC-005 | PDF-Format | JSON + PDF in ZIP |
| TC-006 | Nach 24 Stunden | Export nicht mehr verfuegbar |
| TC-007 | Export-Inhalt | Alle Benutzerdaten enthalten |

## Story Points

3

## Prioritaet

Mittel
