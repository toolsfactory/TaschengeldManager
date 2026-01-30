# Story S093: Familien-Dashboard (Eltern)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Elternteil** möchte ich **alle Kinder mit ihren Kontostaenden auf einen Blick sehen**, damit **ich den finanziellen Ueberblick ueber meine Familie behalte**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil mit mehreren Kindern, wenn es das Dashboard öffnet, dann werden alle Kinder mit Kontostand als Karten angezeigt
- [ ] Gegeben jede Kind-Karte, wenn sie angezeigt wird, dann enthaelt sie Name, Kontostand und Trend-Indikator
- [ ] Gegeben die Familien-Uebersicht, wenn sie angezeigt wird, dann werden Gesamtsumme und Monatsausgaben angezeigt
- [ ] Gegeben eine Kind-Karte, wenn das Elternteil darauf tippt, dann wird zur Detail-Ansicht des Kindes navigiert
- [ ] Gegeben die Trend-Indikatoren, dann zeigen sie die Veraenderung zum Vormonat an (steigend/fallend/stabil)
- [ ] Gegeben die Daten, dann werden sie in weniger als 2 Sekunden geladen

## UI-Entwurf (ASCII)

### Hauptansicht

```
+---------------------------------------+
|        Familien-Uebersicht            |
|            Januar 2025                |
+---------------------------------------+
|                                       |
|  +---------------+  +---------------+ |
|  |               |  |               | |
|  |  (Avatar)     |  |  (Avatar)     | |
|  |    Lisa       |  |    Max        | |
|  |               |  |               | |
|  |  EUR 125,50   |  |  EUR 48,00    | |
|  |  /\ +12%      |  |  \/ -5%       | |
|  |               |  |               | |
|  +---------------+  +---------------+ |
|                                       |
|  +---------------+                    |
|  |               |                    |
|  |  (Avatar)     |                    |
|  |    Tim        |                    |
|  |               |                    |
|  |  EUR 32,00    |                    |
|  |  -- 0%        |                    |
|  |               |                    |
|  +---------------+                    |
|                                       |
|  ------------------------------------ |
|                                       |
|  +----------------------------------+ |
|  |  Familien-Zusammenfassung        | |
|  |                                  | |
|  |  Gesamt:        EUR 205,50       | |
|  |  Ausgaben Jan:  EUR  67,30       | |
|  |  Einnahmen Jan: EUR 150,00       | |
|  |                                  | |
|  |  Bilanz Jan:    EUR +82,70       | |
|  +----------------------------------+ |
|                                       |
|  [Zeitraum: Januar 2025 v]            |
|                                       |
+---------------------------------------+
```

### Kind-Karte Detail

```
+-------------------+
|                   |
|      (Avatar)     |
|       Lisa        |
|                   |
|   EUR 125,50      |
|   /\ +12%         |
|                   |
|   Ausgaben: 22,00 |
|   Einnahmen: 50,00|
|                   |
|      [Details >]  |
+-------------------+
```

### Leerer Zustand (keine Kinder)

```
+---------------------------------------+
|        Familien-Uebersicht            |
+---------------------------------------+
|                                       |
|                                       |
|        +----------------------+       |
|        |                      |       |
|        |   (Familien-Icon)    |       |
|        |                      |       |
|        |  Noch keine Kinder   |       |
|        |  hinzugefuegt        |       |
|        |                      |       |
|        +----------------------+       |
|                                       |
|        [+ Kind hinzufuegen]           |
|                                       |
+---------------------------------------+
```

## API-Endpunkt

### Request

```http
GET /api/statistics/family/dashboard
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht | Default |
|-----------|-----|--------------|---------|---------|
| month | string | Monat fuer Statistiken (YYYY-MM) | Nein | Aktueller Monat |

### Beispiel-Request

```http
GET /api/statistics/family/dashboard?month=2025-01
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Response 200 OK

```json
{
  "familyId": "fam-001",
  "familyName": "Familie Mueller",
  "currency": "EUR",
  "month": "2025-01",
  "monthName": "Januar 2025",
  "summary": {
    "totalBalance": 205.50,
    "totalExpensesThisMonth": 67.30,
    "totalIncomeThisMonth": 150.00,
    "balanceThisMonth": 82.70,
    "childCount": 3
  },
  "children": [
    {
      "childId": "child-001",
      "name": "Lisa",
      "avatarUrl": "/api/users/child-001/avatar",
      "balance": 125.50,
      "trend": {
        "direction": "increasing",
        "changeAmount": 13.50,
        "changePercent": 12.0
      },
      "thisMonth": {
        "expenses": 22.00,
        "income": 50.00,
        "balance": 28.00
      }
    },
    {
      "childId": "child-002",
      "name": "Max",
      "avatarUrl": "/api/users/child-002/avatar",
      "balance": 48.00,
      "trend": {
        "direction": "decreasing",
        "changeAmount": -2.50,
        "changePercent": -5.0
      },
      "thisMonth": {
        "expenses": 35.00,
        "income": 50.00,
        "balance": 15.00
      }
    },
    {
      "childId": "child-003",
      "name": "Tim",
      "avatarUrl": "/api/users/child-003/avatar",
      "balance": 32.00,
      "trend": {
        "direction": "unchanged",
        "changeAmount": 0.00,
        "changePercent": 0.0
      },
      "thisMonth": {
        "expenses": 10.30,
        "income": 50.00,
        "balance": 39.70
      }
    }
  ]
}
```

### Response 200 OK (Keine Kinder)

```json
{
  "familyId": "fam-001",
  "familyName": "Familie Mueller",
  "currency": "EUR",
  "month": "2025-01",
  "monthName": "Januar 2025",
  "summary": {
    "totalBalance": 0.00,
    "totalExpensesThisMonth": 0.00,
    "totalIncomeThisMonth": 0.00,
    "balanceThisMonth": 0.00,
    "childCount": 0
  },
  "children": []
}
```

### Response 403 Forbidden

```json
{
  "error": "ACCESS_DENIED",
  "message": "Nur Eltern koennen das Familien-Dashboard sehen"
}
```

## Technische Notizen

### Backend

- Aggregation ueber alle Kinder der Familie
- Trend-Berechnung: Vergleich aktueller Monat vs. Vormonat
- Sortierung: Nach Kontostand (hoechster zuerst) oder alphabetisch

### SQL-Beispiel

```sql
-- Kinder mit Kontostaenden und Trends
SELECT
    c.id AS child_id,
    c.name,
    c.avatar_url,
    a.balance AS current_balance,
    SUM(CASE WHEN t.amount < 0 AND t.created_at >= @monthStart THEN ABS(t.amount) ELSE 0 END) AS expenses_this_month,
    SUM(CASE WHEN t.amount > 0 AND t.created_at >= @monthStart THEN t.amount ELSE 0 END) AS income_this_month,
    (SELECT balance FROM balance_snapshots WHERE child_id = c.id AND snapshot_date = @lastMonthEnd) AS last_month_balance
FROM children c
JOIN accounts a ON a.child_id = c.id
LEFT JOIN transactions t ON t.account_id = a.id AND t.created_at >= @monthStart
WHERE c.family_id = @familyId
GROUP BY c.id, c.name, c.avatar_url, a.balance;
```

### Trend-Berechnung

```csharp
public string CalculateTrendDirection(decimal currentBalance, decimal lastMonthBalance)
{
    if (lastMonthBalance == 0) return "unchanged";

    var changePercent = ((currentBalance - lastMonthBalance) / lastMonthBalance) * 100;

    return changePercent switch
    {
        > 2 => "increasing",
        < -2 => "decreasing",
        _ => "unchanged"
    };
}
```

### Chart-Library Empfehlungen (Mobile)

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| LiveCharts2 | Karten-Layout, Animationen | Groessere Dependency |
| Custom MAUI | Volle Kontrolle, FlexLayout | Mehr Implementierung |
| CollectionView | Native MAUI, performant | Styling aufwaendiger |

### Visualisierungs-Empfehlungen

- Karten-Layout: 2 Spalten auf Tablets, 1-2 auf Phones
- Trend-Farben:
  - Steigend: Gruen (#4CAF50)
  - Fallend: Rot (#F44336)
  - Stabil: Grau (#9E9E9E)
- Avatar: Rund mit Initialen als Fallback
- Pull-to-Refresh fuer Aktualisierung

### Performance

- Index auf `children(family_id)`, `accounts(child_id)`, `transactions(account_id, created_at)`
- Caching mit Valkey (TTL: 5 Minuten)
- Parallele Abfragen pro Kind bei grossen Familien

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-093-1 | Familie mit 3 Kindern | 3 Kind-Karten angezeigt |
| TC-093-2 | Elternteil tippt auf Kind | Navigation zur Detail-Ansicht |
| TC-093-3 | Kind mit steigendem Trend | Gruener Pfeil nach oben |
| TC-093-4 | Kind mit fallendem Trend | Roter Pfeil nach unten |
| TC-093-5 | Kind mit stabilem Kontostand | Graues Minus-Zeichen |
| TC-093-6 | Familie ohne Kinder | Leerer Zustand mit Hinweis |
| TC-093-7 | Gesamt-Summe korrekt | Summe aller Kind-Kontstaende |
| TC-093-8 | Kind versucht Zugriff | 403 Forbidden |

## Abhaengigkeiten

- E001 - Benutzerverwaltung (Familienstruktur)
- E002 - Kontoverwaltung (Kontostaende)
- E003 - Transaktionen (Monatsausgaben)

## Story Points

8

## Prioritaet

Mittel
