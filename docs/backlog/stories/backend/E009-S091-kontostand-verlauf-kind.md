# Story S091: Kontostand-Verlauf Liniendiagramm (Kind)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Kind** möchte ich **den Verlauf meines Kontostands als Liniendiagramm sehen**, damit **ich verstehe, ob ich spare oder ausgebe**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Kontohistorie, wenn es die Verlauf-Ansicht öffnet, dann wird ein Liniendiagramm des Kontostands angezeigt
- [ ] Gegeben ein Liniendiagramm, wenn das Kind auf einen Datenpunkt tippt, dann werden Datum und genauer Kontostand angezeigt
- [ ] Gegeben verschiedene Zeitraeume, wenn das Kind zwischen 3/6/12 Monaten waehlt, dann aendert sich die Darstellungsgranularitaet
- [ ] Gegeben ein steigender Trend, wenn der Kontostand waechst, dann wird ein positiver Trend-Indikator angezeigt
- [ ] Gegeben ein fallender Trend, wenn der Kontostand sinkt, dann wird ein negativer Trend-Indikator angezeigt
- [ ] Gegeben die Diagramm-Linie, dann ist sie visuell klar und touch-optimiert (dicke Linie, grosse Datenpunkte)

## UI-Entwurf (ASCII)

### Hauptansicht

```
+---------------------------------------+
|  <- Zurueck    Mein Kontostand        |
+---------------------------------------+
|                                       |
|  Zeitraum: [6 Monate v]               |
|                                       |
|  Aktueller Stand: EUR 85,50           |
|  Trend: /\ +12,5% (steigend)          |
|                                       |
|  EUR                                  |
|   |                                   |
| 100+                          *       |
|   |                        *'         |
|  80+                    *-'           |
|   |                  *'               |
|  60+              *-'                 |
|   |           *--'                    |
|  40+       *-'                        |
|   |    *--'                           |
|  20+                                  |
|   |                                   |
|   +--+----+----+----+----+----+       |
|     Aug  Sep  Okt  Nov  Dez  Jan      |
|                                       |
|  +----------------------------------+ |
|  |  Hoechststand: EUR 92,00         | |
|  |  am 15.12.2024                   | |
|  |                                  | |
|  |  Tiefststand: EUR 35,00          | |
|  |  am 28.08.2024                   | |
|  +----------------------------------+ |
|                                       |
+---------------------------------------+
```

### Datenpunkt-Detail (bei Tap)

```
+---------------------------------------+
|                                       |
|          +------------------+         |
|          |  15. Dez 2024    |         |
|          |  EUR 92,00       |         |
|          |  Hoechststand    |         |
|          +------------------+         |
|                    |                  |
|                    v                  |
|  EUR                                  |
|   |                                   |
| 100+                    [*]           |
|   |                   *'              |
|  80+                *-'               |
|                                       |
+---------------------------------------+
```

### Leerer Zustand

```
+---------------------------------------+
|  <- Zurueck    Mein Kontostand        |
+---------------------------------------+
|                                       |
|                                       |
|        +----------------------+       |
|        |                      |       |
|        |   (Linien-Icon)      |       |
|        |                      |       |
|        |  Noch nicht genug    |       |
|        |  Daten vorhanden     |       |
|        |                      |       |
|        +----------------------+       |
|                                       |
|  Komm in ein paar Wochen wieder!      |
|                                       |
+---------------------------------------+
```

## API-Endpunkt

### Request

```http
GET /api/statistics/children/{childId}/balance-history
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht | Default |
|-----------|-----|--------------|---------|---------|
| period | string | Zeitraum (3months, 6months, 1year) | Nein | 6months |
| granularity | string | Datenpunkt-Intervall (daily, weekly, monthly) | Nein | weekly |

### Beispiel-Request

```http
GET /api/statistics/children/550e8400-e29b-41d4-a716-446655440000/balance-history?period=6months&granularity=weekly
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Response 200 OK

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Max",
  "period": "6months",
  "granularity": "weekly",
  "currency": "EUR",
  "currentBalance": 85.50,
  "trend": {
    "direction": "increasing",
    "changeAmount": 9.50,
    "changePercent": 12.5
  },
  "statistics": {
    "highestBalance": {
      "amount": 92.00,
      "date": "2024-12-15"
    },
    "lowestBalance": {
      "amount": 35.00,
      "date": "2024-08-28"
    },
    "averageBalance": 62.75
  },
  "dataPoints": [
    {
      "date": "2024-08-01",
      "balance": 45.00,
      "isHighest": false,
      "isLowest": false
    },
    {
      "date": "2024-08-08",
      "balance": 52.50,
      "isHighest": false,
      "isLowest": false
    },
    {
      "date": "2024-08-15",
      "balance": 48.00,
      "isHighest": false,
      "isLowest": false
    },
    {
      "date": "2024-08-22",
      "balance": 38.00,
      "isHighest": false,
      "isLowest": false
    },
    {
      "date": "2024-08-28",
      "balance": 35.00,
      "isHighest": false,
      "isLowest": true
    },
    {
      "date": "2024-12-15",
      "balance": 92.00,
      "isHighest": true,
      "isLowest": false
    },
    {
      "date": "2025-01-01",
      "balance": 85.50,
      "isHighest": false,
      "isLowest": false
    }
  ]
}
```

### Response 200 OK (Nicht genuegend Daten)

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Max",
  "period": "6months",
  "granularity": "weekly",
  "currency": "EUR",
  "currentBalance": 20.00,
  "trend": null,
  "statistics": null,
  "dataPoints": [],
  "message": "Nicht genuegend Daten fuer den gewaehlten Zeitraum"
}
```

### Response 403 Forbidden

```json
{
  "error": "ACCESS_DENIED",
  "message": "Kein Zugriff auf diese Statistik"
}
```

## Technische Notizen

### Backend

- Kontostand-Snapshots in eigener Tabelle oder berechnet aus Transaktionen
- Bei grossem Datenvolumen: Materialized View fuer Performance
- Trend-Berechnung: Vergleich Anfang vs. Ende des Zeitraums
- Granularitaet anpassen basierend auf Zeitraum:
  - 3 Monate -> taeglich oder woechentlich
  - 6 Monate -> woechentlich
  - 1 Jahr -> monatlich

### Datenpunkt-Aggregation

```sql
-- Beispiel: Woechentliche Snapshots
SELECT
    DATE_TRUNC('week', snapshot_date) as week_start,
    AVG(balance) as balance
FROM balance_snapshots
WHERE child_id = @childId
  AND snapshot_date >= @fromDate
GROUP BY DATE_TRUNC('week', snapshot_date)
ORDER BY week_start;
```

### Chart-Library Empfehlungen (Mobile)

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| LiveCharts2 | Animationen, Touch-Events | Groessere Dependency |
| Microcharts | Einfach, Line Chart Support | Limitierte Interaktivitaet |
| OxyPlot | Wissenschaftliche Charts | Komplexere API |

### Visualisierungs-Empfehlungen

- Linienstärke: mindestens 3px fuer Touch-Geraete
- Datenpunkte: 8-10px Durchmesser
- Y-Achse: Immer bei 0 starten fuer ehrliche Darstellung
- Farben: Gruen fuer positiven Trend, Rot fuer negativen

### Performance

- Index auf `balance_snapshots(child_id, snapshot_date)`
- Caching mit Valkey (TTL: 15 Minuten)
- Maximal 52 Datenpunkte (1 Jahr woechentlich)

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-091-1 | Kind mit 6 Monaten Historie | Liniendiagramm mit ~26 Datenpunkten |
| TC-091-2 | Kind mit steigendem Trend | Gruener Pfeil, positiver Prozentsatz |
| TC-091-3 | Kind mit fallendem Trend | Roter Pfeil, negativer Prozentsatz |
| TC-091-4 | Kind tippt auf Datenpunkt | Popup mit Datum und Betrag |
| TC-091-5 | Kind wechselt zu 1 Jahr | Monatliche Granularitaet |
| TC-091-6 | Neues Kind ohne Historie | Leerer Zustand angezeigt |
| TC-091-7 | Hoechst-/Tiefststand markiert | Spezielle Marker im Diagramm |

## Abhaengigkeiten

- E003 - Transaktionen (Datenbasis)
- E002 - Kontoverwaltung (Kontostand)
- S096 - Zeitraum-Filter

## Story Points

5

## Prioritaet

Mittel
