# Story E004-S037: Automatische Ausführung (Backend-Job)

## Epic
E004 - Automatische Zahlungen

## User Story

Als **System** möchte ich **wiederkehrende Zahlungen automatisch ausführen**, damit **Kinder ihr Taschengeld pünktlich und ohne manuelles Eingreifen erhalten**.

## Akzeptanzkriterien

- [ ] Gegeben eine fällige Zahlung, wenn der Job läuft, dann wird die Transaktion erstellt und der Kontostand aktualisiert
- [ ] Gegeben eine ausgeführte Zahlung, wenn sie abgeschlossen ist, dann wird das nächste Ausführungsdatum berechnet
- [ ] Gegeben einen Fehler bei der Ausführung, wenn er auftritt, dann wird er protokolliert und die Eltern werden benachrichtigt
- [ ] Gegeben mehrere fällige Zahlungen, wenn der Job läuft, dann werden alle verarbeitet
- [ ] Gegeben eine pausierte Zahlung, wenn ihr Datum erreicht wird, dann wird sie übersprungen

## Backend-Job Spezifikation

### Job-Konfiguration
```json
{
  "jobName": "RecurringPaymentProcessor",
  "schedule": "0 6 * * *",
  "timezone": "Europe/Berlin",
  "retryPolicy": {
    "maxRetries": 3,
    "retryDelay": "00:05:00"
  }
}
```

### Verarbeitungslogik
```
1. Alle fälligen Zahlungen abrufen (nextExecutionDate <= heute, status = Active)
2. Für jede Zahlung:
   a. Transaktion erstellen
   b. Kontostand aktualisieren
   c. nextExecutionDate berechnen und speichern
   d. lastExecutedAt aktualisieren
   e. Benachrichtigung an Kind senden
3. Fehler protokollieren und bei kritischen Fehlern Eltern benachrichtigen
4. Job-Statistik speichern
```

### Monitoring-Endpunkt
```
GET /api/admin/jobs/recurring-payments/status
Authorization: Bearer {admin-token}

Response 200:
{
  "lastRun": "datetime",
  "nextScheduledRun": "datetime",
  "lastRunStats": {
    "processed": 150,
    "successful": 148,
    "failed": 2,
    "skipped": 5,
    "duration": "00:00:45"
  },
  "failedPayments": [
    {
      "recurringPaymentId": "guid",
      "childName": "string",
      "error": "string",
      "retryCount": 2
    }
  ]
}
```

## Technische Notizen

- Hosted Service oder Hangfire für Job-Scheduling
- Idempotenz: Doppelte Ausführung verhindern (lastExecutedAt Check)
- Transaktions-Isolation für Kontostand-Updates
- Zeitzone beachten: Europe/Berlin
- Ausführungszeit: 06:00 Uhr morgens
- Bei Serverausfall: Verpasste Zahlungen beim nächsten Lauf nachholen
- Logging: Structured Logging mit Correlation IDs

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-037-01 | Fällige Zahlungen verarbeiten | Transaktionen erstellt |
| TC-E004-037-02 | Nächstes Datum berechnen | Korrekt für Intervall |
| TC-E004-037-03 | Pausierte Zahlung | Wird übersprungen |
| TC-E004-037-04 | Fehler bei Transaktion | Logging + Retry |
| TC-E004-037-05 | Verpasste Zahlung nachholen | Bei nächstem Lauf ausführen |

## Story Points

8

## Priorität

Hoch
