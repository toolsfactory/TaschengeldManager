# Story E006-S063: Zinsen automatisch berechnen (Backend-Job)

## Epic
E006 - Zinsen

## User Story

Als **System** möchte ich **Zinsen automatisch berechnen und gutschreiben**, damit **Kinder ihr verdientes Geld ohne manuelles Eingreifen erhalten**.

## Akzeptanzkriterien

- [ ] Gegeben fällige Zinsberechnungen, wenn der Job läuft, dann werden Zinsen berechnet und gutgeschrieben
- [ ] Gegeben eine Zinsgutschrift, wenn sie erfolgt, dann wird eine Transaktion vom Typ "Interest" erstellt
- [ ] Gegeben einen Fehler, wenn er auftritt, dann wird er protokolliert und die Eltern werden benachrichtigt
- [ ] Gegeben unterschiedliche Intervalle, wenn der Job läuft, dann werden nur fällige Konten verarbeitet
- [ ] Gegeben eine Zinsgutschrift, wenn sie erfolgt, dann wird das Kind benachrichtigt

## Backend-Job Spezifikation

### Job-Konfiguration
```json
{
  "jobName": "InterestCalculationProcessor",
  "schedule": "0 0 * * *",
  "timezone": "Europe/Berlin",
  "retryPolicy": {
    "maxRetries": 3,
    "retryDelay": "00:05:00"
  }
}
```

### Verarbeitungslogik
```
1. Alle fälligen Zinsberechnungen abrufen (nextCalculationDate <= heute)
2. Für jedes Konto:
   a. Kontostand ermitteln (je nach Berechnungsmethode)
   b. Zinsen berechnen: (Kontostand * Jahreszins / 100) / Divisor
   c. Auf 2 Dezimalstellen runden
   d. Falls Zinsen > 0: Transaktion erstellen
   e. Kontostand aktualisieren
   f. nextCalculationDate berechnen
   g. Zins-Eintrag in Historie speichern
3. Benachrichtigungen senden
4. Job-Statistik speichern
```

### Zinsberechnung (EndOfPeriod)
```
Zinsen = Kontostand * (Jahreszins / 100) / Divisor
Beispiel: 100 € * (3.0 / 100) / 12 = 0.25 €
```

### Zinsberechnung (AverageBalance)
```
Durchschnitt = Summe(Tagesendstände) / Anzahl Tage
Zinsen = Durchschnitt * (Jahreszins / 100) / Divisor
```

### Monitoring-Endpunkt
```
GET /api/admin/jobs/interest-calculation/status
Authorization: Bearer {admin-token}

Response 200:
{
  "lastRun": "datetime",
  "nextScheduledRun": "datetime",
  "lastRunStats": {
    "processed": 50,
    "withInterest": 45,
    "skipped": 5,
    "totalInterestPaid": 12.75,
    "duration": "00:00:15"
  }
}
```

## Technische Notizen

- Job täglich um Mitternacht
- Nur Konten mit interestEnabled = true
- Mindestzins: 0.01 € (darunter keine Buchung)
- Rundung: Kaufmännisch auf 2 Dezimalstellen
- Idempotenz durch lastCalculationDate Check
- Transaktions-Typ: "Interest"

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E006-063-01 | Fällige Zinsen berechnen | Transaktion erstellt |
| TC-E006-063-02 | Zinsen < 0.01 € | Keine Buchung |
| TC-E006-063-03 | AverageBalance Methode | Durchschnitt korrekt |
| TC-E006-063-04 | Deaktivierte Zinsen | Wird übersprungen |
| TC-E006-063-05 | Nächstes Datum | Korrekt für Intervall |

## Story Points

8

## Priorität

Hoch
