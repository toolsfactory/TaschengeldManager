# Story S094: Einnahmen/Ausgaben-Uebersicht pro Kind (Eltern)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Elternteil** möchte ich **Einnahmen und Ausgaben meiner Kinder pro Monat sehen**, damit **ich den Geldfluss verstehe und bei Bedarf beraten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es die Uebersicht öffnet, dann werden Einnahmen und Ausgaben als Balkendiagramm angezeigt
- [ ] Gegeben das Diagramm, wenn es angezeigt wird, dann sind Einnahmen und Ausgaben farblich unterscheidbar
- [ ] Gegeben die Einnahmen, wenn sie aufgeschluesselt werden, dann werden Taschengeld, Geschenke und Zinsen separat angezeigt
- [ ] Gegeben eine Kind-Auswahl, wenn das Elternteil ein anderes Kind waehlt, dann aktualisiert sich das Diagramm
- [ ] Gegeben ein Zeitraum, wenn das Elternteil zwischen 3/6/12 Monaten waehlt, dann werden entsprechend viele Monate angezeigt
- [ ] Gegeben ein Balken, wenn das Elternteil darauf tippt, dann werden Details zum Monat angezeigt

## UI-Entwurf (ASCII)

### Hauptansicht

```
+---------------------------------------+
|  <- Zurueck   Einnahmen/Ausgaben      |
+---------------------------------------+
|                                       |
|  Kind: [Lisa v]  Zeitraum: [6 Mon. v] |
|                                       |
|         Einnahmen vs. Ausgaben        |
|              Lisa                     |
|                                       |
|  EUR   [===] Einnahmen  [|||] Ausgaben|
|   |                                   |
|  60+         ===                      |
|   |          ===|||                   |
|  50+    ===  ===|||    ===            |
|   |     ===  ===|||    ===|||         |
|  40+    ===  ===|||    ===|||    ===  |
|   |     ===|||===|||    ===|||    ===||
|  30+    ===|||===|||    ===|||    ===||
|   |     ===|||===|||    ===|||    ===||
|  20+    ===|||===|||    ===|||    ===||
|   |     ===|||===|||    ===|||    ===||
|   +-----+---++---++----+---++----+---+|
|        Aug  Sep  Okt  Nov  Dez  Jan   |
|                                       |
|  ------------------------------------ |
|                                       |
|  Einnahmen-Aufschluesselung:          |
|  +----------------------------------+ |
|  |  Taschengeld     EUR 40,00  80%  | |
|  |  ========================        | |
|  |  Geschenke       EUR  8,00  16%  | |
|  |  ====                            | |
|  |  Zinsen          EUR  2,00   4%  | |
|  |  =                               | |
|  +----------------------------------+ |
|                                       |
|  Durchschnitt/Monat:                  |
|  - Einnahmen: EUR 50,00               |
|  - Ausgaben:  EUR 35,00               |
|  - Bilanz:    EUR +15,00              |
|                                       |
+---------------------------------------+
```

### Monats-Detail (bei Tap)

```
+---------------------------------------+
|          September 2024               |
+---------------------------------------+
|                                       |
|  Einnahmen:                           |
|  - Taschengeld:  EUR 40,00            |
|  - Geschenk Oma: EUR 20,00            |
|  - Zinsen:       EUR  0,50            |
|  Gesamt:         EUR 60,50            |
|                                       |
|  Ausgaben:                            |
|  - Suessigkeiten: EUR 12,00           |
|  - Spielzeug:     EUR 25,00           |
|  - Sonstiges:     EUR  8,00           |
|  Gesamt:          EUR 45,00           |
|                                       |
|  Bilanz:          EUR +15,50          |
|                                       |
|  [Schliessen]                         |
+---------------------------------------+
```

### Vergleich mehrerer Kinder

```
+---------------------------------------+
|  <- Zurueck   Einnahmen/Ausgaben      |
+---------------------------------------+
|                                       |
|  Kind: [Alle Kinder v]  [6 Mon. v]    |
|                                       |
|         Ausgaben-Vergleich            |
|                                       |
|  EUR                                  |
|   |                                   |
|  80+              ###                 |
|   |          ###  ###                 |
|  60+     ###  ###  ###      ###       |
|   |      ###  ###  ###  ###  ###      |
|  40+     ###  ###  ###  ###  ###  ### |
|   |      ###  ###  ###  ###  ###  ### |
|  20+     ###  ###  ###  ###  ###  ### |
|   |      ###  ###  ###  ###  ###  ### |
|   +------+----+----+----+----+----+   |
|         Aug  Sep  Okt  Nov  Dez  Jan  |
|                                       |
|  Legende:                             |
|  [#] Lisa  [#] Max  [#] Tim           |
|                                       |
+---------------------------------------+
```

## API-Endpunkt

### Request

```http
GET /api/statistics/children/{childId}/income-expenses
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht | Default |
|-----------|-----|--------------|---------|---------|
| months | int | Anzahl der Monate (3, 6, 12) | Nein | 6 |
| endMonth | string | Endmonat (YYYY-MM) | Nein | Aktueller Monat |

### Beispiel-Request

```http
GET /api/statistics/children/550e8400-e29b-41d4-a716-446655440000/income-expenses?months=6&endMonth=2025-01
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Response 200 OK

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Lisa",
  "currency": "EUR",
  "periodStart": "2024-08",
  "periodEnd": "2025-01",
  "averages": {
    "monthlyIncome": 50.00,
    "monthlyExpenses": 35.00,
    "monthlyBalance": 15.00
  },
  "incomeBreakdown": {
    "allowance": {
      "total": 240.00,
      "percentage": 80.0
    },
    "gifts": {
      "total": 48.00,
      "percentage": 16.0
    },
    "interest": {
      "total": 12.00,
      "percentage": 4.0
    }
  },
  "months": [
    {
      "month": "2024-08",
      "monthName": "August 2024",
      "income": {
        "total": 45.00,
        "allowance": 40.00,
        "gifts": 5.00,
        "interest": 0.00
      },
      "expenses": {
        "total": 32.00,
        "byCategory": [
          { "category": "Suessigkeiten", "amount": 12.00 },
          { "category": "Spielzeug", "amount": 15.00 },
          { "category": "Sonstiges", "amount": 5.00 }
        ]
      },
      "balance": 13.00
    },
    {
      "month": "2024-09",
      "monthName": "September 2024",
      "income": {
        "total": 60.50,
        "allowance": 40.00,
        "gifts": 20.00,
        "interest": 0.50
      },
      "expenses": {
        "total": 45.00,
        "byCategory": [
          { "category": "Suessigkeiten", "amount": 12.00 },
          { "category": "Spielzeug", "amount": 25.00 },
          { "category": "Sonstiges", "amount": 8.00 }
        ]
      },
      "balance": 15.50
    },
    {
      "month": "2024-10",
      "monthName": "Oktober 2024",
      "income": {
        "total": 50.00,
        "allowance": 40.00,
        "gifts": 8.00,
        "interest": 2.00
      },
      "expenses": {
        "total": 38.00,
        "byCategory": [
          { "category": "Suessigkeiten", "amount": 10.00 },
          { "category": "Kleidung", "amount": 20.00 },
          { "category": "Sonstiges", "amount": 8.00 }
        ]
      },
      "balance": 12.00
    }
  ]
}
```

### Response 403 Forbidden

```json
{
  "error": "ACCESS_DENIED",
  "message": "Kein Zugriff auf Kind-Statistiken"
}
```

## API-Endpunkt (Alle Kinder vergleichen)

### Request

```http
GET /api/statistics/family/income-expenses-comparison
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht | Default |
|-----------|-----|--------------|---------|---------|
| months | int | Anzahl der Monate | Nein | 6 |

### Response 200 OK

```json
{
  "familyId": "fam-001",
  "currency": "EUR",
  "periodStart": "2024-08",
  "periodEnd": "2025-01",
  "children": [
    {
      "childId": "child-001",
      "name": "Lisa",
      "color": "#FF6384",
      "totalIncome": 300.00,
      "totalExpenses": 210.00
    },
    {
      "childId": "child-002",
      "name": "Max",
      "color": "#36A2EB",
      "totalIncome": 300.00,
      "totalExpenses": 280.00
    },
    {
      "childId": "child-003",
      "name": "Tim",
      "color": "#FFCE56",
      "totalIncome": 200.00,
      "totalExpenses": 120.00
    }
  ],
  "months": [
    {
      "month": "2024-08",
      "byChild": [
        { "childId": "child-001", "income": 45.00, "expenses": 32.00 },
        { "childId": "child-002", "income": 50.00, "expenses": 48.00 },
        { "childId": "child-003", "income": 30.00, "expenses": 18.00 }
      ]
    }
  ]
}
```

## Technische Notizen

### Backend

- Aggregation nach Transaktionstyp (Einnahme/Ausgabe) und Monat
- Einnahme-Typen: allowance, gift, interest, other_income
- Ausgabe-Kategorisierung aus Transaktions-Metadaten
- Sortierung: Chronologisch nach Monat

### SQL-Beispiel

```sql
-- Monatliche Einnahmen/Ausgaben pro Kind
SELECT
    DATE_TRUNC('month', t.created_at) AS month,
    CASE WHEN t.amount > 0 THEN 'income' ELSE 'expense' END AS type,
    t.transaction_type AS sub_type,
    SUM(ABS(t.amount)) AS total_amount
FROM transactions t
JOIN accounts a ON a.id = t.account_id
WHERE a.child_id = @childId
  AND t.created_at >= @startDate
  AND t.created_at <= @endDate
GROUP BY DATE_TRUNC('month', t.created_at), type, sub_type
ORDER BY month, type;
```

### Chart-Library Empfehlungen (Mobile)

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| LiveCharts2 | Grouped Bar Charts, Legend | Groessere Dependency |
| Microcharts | Einfache Bar Charts | Limitierte Gruppierung |
| SkiaSharp | Volle Kontrolle | Mehr Implementierung |

### Visualisierungs-Empfehlungen

- Farben:
  - Einnahmen: Gruen (#4CAF50)
  - Ausgaben: Orange (#FF9800)
- Gruppierung: Einnahmen und Ausgaben nebeneinander pro Monat
- Y-Achse: Auto-Skalierung basierend auf Max-Wert
- Legende: Immer sichtbar bei mehreren Datenreihen
- Animation: Balken von unten nach oben

### Performance

- Index auf `transactions(account_id, created_at, transaction_type)`
- Caching mit Valkey (TTL: 10 Minuten)
- Bei 12 Monaten: Pre-aggregierte Daten verwenden

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-094-1 | Kind mit 6 Monaten Daten | 6 Balkenpaare angezeigt |
| TC-094-2 | Elternteil waehlt anderes Kind | Diagramm aktualisiert |
| TC-094-3 | Einnahmen aufgeschluesselt | Taschengeld, Geschenke, Zinsen |
| TC-094-4 | Elternteil tippt auf Balken | Monats-Detail erscheint |
| TC-094-5 | Monat ohne Transaktionen | Leere Balken (0 EUR) |
| TC-094-6 | Durchschnitte berechnet | Korrekte Mittelwerte |
| TC-094-7 | Kind versucht Zugriff auf anderes Kind | 403 Forbidden |
| TC-094-8 | Alle Kinder vergleichen | Gestapelte/gruppierte Ansicht |

## Abhaengigkeiten

- E003 - Transaktionen (Datenbasis)
- E002 - Kontoverwaltung
- S096 - Zeitraum-Filter

## Story Points

5

## Prioritaet

Mittel
