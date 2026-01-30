# Story S095: Kategorie-Analyse alle Kinder (Eltern)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Elternteil** möchte ich **sehen, wofür meine Kinder ihr Geld ausgeben**, damit **ich bei Bedarf beratend eingreifen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil mit mehreren Kindern, wenn es die Kategorie-Analyse öffnet, dann werden Ausgaben nach Kategorie als horizontale Balken angezeigt
- [ ] Gegeben jede Kategorie, wenn sie angezeigt wird, dann ist die Aufschluesselung pro Kind sichtbar
- [ ] Gegeben ein Filter, wenn das Elternteil zwischen "Alle Kinder" und einzelnem Kind waehlt, dann aktualisiert sich die Ansicht
- [ ] Gegeben die Kategorien, dann sind sie nach Gesamtbetrag sortiert (hoechste zuerst)
- [ ] Gegeben eine Kategorie, wenn das Elternteil darauf tippt, dann werden Detail-Informationen angezeigt
- [ ] Gegeben ein Zeitraum-Filter, wenn ein anderer Zeitraum gewaehlt wird, dann aktualisieren sich die Daten

## UI-Entwurf (ASCII)

### Hauptansicht - Alle Kinder

```
+---------------------------------------+
|  <- Zurueck   Kategorie-Analyse       |
+---------------------------------------+
|                                       |
|  [Alle Kinder v]  [Januar 2025 v]     |
|                                       |
|        Ausgaben nach Kategorie        |
|                                       |
|  Suessigkeiten              EUR 35,00 |
|  ================================     |
|  [Lisa: 15] [Max: 12] [Tim: 8]        |
|                                       |
|  Spielzeug                  EUR 28,00 |
|  ==========================           |
|  [Lisa: 5] [Max: 18] [Tim: 5]         |
|                                       |
|  Kleidung                   EUR 18,00 |
|  ==================                   |
|  [Lisa: 18] [Max: 0] [Tim: 0]         |
|                                       |
|  Buecher                    EUR 15,00 |
|  ===============                      |
|  [Lisa: 10] [Max: 5] [Tim: 0]         |
|                                       |
|  Elektronik                 EUR 12,00 |
|  ============                         |
|  [Lisa: 0] [Max: 12] [Tim: 0]         |
|                                       |
|  Sonstiges                  EUR  9,00 |
|  =========                            |
|  [Lisa: 4] [Max: 2] [Tim: 3]          |
|                                       |
|  ------------------------------------ |
|  Gesamt alle Kategorien:   EUR 117,00 |
|                                       |
+---------------------------------------+
```

### Einzelnes Kind gefiltert

```
+---------------------------------------+
|  <- Zurueck   Kategorie-Analyse       |
+---------------------------------------+
|                                       |
|  [Max v]        [Januar 2025 v]       |
|                                       |
|        Ausgaben nach Kategorie        |
|              Max                      |
|                                       |
|  Spielzeug                  EUR 18,00 |
|  ================================     |
|  43% aller Ausgaben                   |
|                                       |
|  Suessigkeiten              EUR 12,00 |
|  =====================                |
|  29% aller Ausgaben                   |
|                                       |
|  Elektronik                 EUR 12,00 |
|  =====================                |
|  29% aller Ausgaben                   |
|                                       |
|  ------------------------------------ |
|  Gesamt:                    EUR 42,00 |
|                                       |
|  +----------------------------------+ |
|  |  Hinweis: Spielzeug ist die      | |
|  |  groesste Kategorie diesen Monat | |
|  +----------------------------------+ |
|                                       |
+---------------------------------------+
```

### Kategorie-Detail (bei Tap)

```
+---------------------------------------+
|           Suessigkeiten               |
|           Januar 2025                 |
+---------------------------------------+
|                                       |
|  Gesamtausgaben: EUR 35,00            |
|                                       |
|  Aufschluesselung:                    |
|  +----------------------------------+ |
|  |                                  | |
|  |  Lisa                  EUR 15,00 | |
|  |  ==================== 43%        | |
|  |  - 12x Kaugummi                  | |
|  |  - 3x Schokolade                 | |
|  |                                  | |
|  |  Max                   EUR 12,00 | |
|  |  ================ 34%            | |
|  |  - 8x Gummibaerchen              | |
|  |  - 2x Eis                        | |
|  |                                  | |
|  |  Tim                    EUR 8,00 | |
|  |  =========== 23%                 | |
|  |  - 5x Kekse                      | |
|  |                                  | |
|  +----------------------------------+ |
|                                       |
|  Trend vs. Vormonat: /\ +15%          |
|                                       |
|  [Schliessen]                         |
+---------------------------------------+
```

## API-Endpunkt

### Request (Alle Kinder)

```http
GET /api/statistics/family/expenses-by-category
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht | Default |
|-----------|-----|--------------|---------|---------|
| from | date | Startdatum (ISO 8601) | Nein | Erster des aktuellen Monats |
| to | date | Enddatum (ISO 8601) | Nein | Heute |
| childId | guid | Optionaler Filter fuer ein Kind | Nein | Alle Kinder |

### Beispiel-Request

```http
GET /api/statistics/family/expenses-by-category?from=2025-01-01&to=2025-01-31
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Response 200 OK

```json
{
  "familyId": "fam-001",
  "currency": "EUR",
  "period": {
    "from": "2025-01-01",
    "to": "2025-01-31"
  },
  "totalExpenses": 117.00,
  "childFilter": null,
  "categories": [
    {
      "categoryId": "cat-001",
      "name": "Suessigkeiten",
      "color": "#FF6384",
      "total": 35.00,
      "percentage": 29.9,
      "transactionCount": 28,
      "trend": {
        "vsLastPeriod": 15.0,
        "direction": "increasing"
      },
      "byChild": [
        {
          "childId": "child-001",
          "name": "Lisa",
          "amount": 15.00,
          "percentage": 42.9,
          "transactionCount": 15
        },
        {
          "childId": "child-002",
          "name": "Max",
          "amount": 12.00,
          "percentage": 34.3,
          "transactionCount": 10
        },
        {
          "childId": "child-003",
          "name": "Tim",
          "amount": 8.00,
          "percentage": 22.9,
          "transactionCount": 5
        }
      ]
    },
    {
      "categoryId": "cat-002",
      "name": "Spielzeug",
      "color": "#36A2EB",
      "total": 28.00,
      "percentage": 23.9,
      "transactionCount": 5,
      "trend": {
        "vsLastPeriod": -8.0,
        "direction": "decreasing"
      },
      "byChild": [
        {
          "childId": "child-001",
          "name": "Lisa",
          "amount": 5.00,
          "percentage": 17.9,
          "transactionCount": 1
        },
        {
          "childId": "child-002",
          "name": "Max",
          "amount": 18.00,
          "percentage": 64.3,
          "transactionCount": 3
        },
        {
          "childId": "child-003",
          "name": "Tim",
          "amount": 5.00,
          "percentage": 17.9,
          "transactionCount": 1
        }
      ]
    },
    {
      "categoryId": "cat-003",
      "name": "Kleidung",
      "color": "#FFCE56",
      "total": 18.00,
      "percentage": 15.4,
      "transactionCount": 1,
      "trend": {
        "vsLastPeriod": 0.0,
        "direction": "unchanged"
      },
      "byChild": [
        {
          "childId": "child-001",
          "name": "Lisa",
          "amount": 18.00,
          "percentage": 100.0,
          "transactionCount": 1
        }
      ]
    }
  ],
  "insights": [
    {
      "type": "top_category",
      "message": "Suessigkeiten ist die groesste Ausgabenkategorie.",
      "relatedCategory": "cat-001"
    },
    {
      "type": "biggest_spender",
      "message": "Max hat am meisten fuer Spielzeug ausgegeben (EUR 18,00).",
      "relatedChild": "child-002",
      "relatedCategory": "cat-002"
    }
  ]
}
```

### Response 200 OK (Ein Kind gefiltert)

```json
{
  "familyId": "fam-001",
  "currency": "EUR",
  "period": {
    "from": "2025-01-01",
    "to": "2025-01-31"
  },
  "totalExpenses": 42.00,
  "childFilter": {
    "childId": "child-002",
    "name": "Max"
  },
  "categories": [
    {
      "categoryId": "cat-002",
      "name": "Spielzeug",
      "color": "#36A2EB",
      "total": 18.00,
      "percentage": 42.9,
      "transactionCount": 3,
      "trend": {
        "vsLastPeriod": 20.0,
        "direction": "increasing"
      }
    },
    {
      "categoryId": "cat-001",
      "name": "Suessigkeiten",
      "color": "#FF6384",
      "total": 12.00,
      "percentage": 28.6,
      "transactionCount": 10,
      "trend": {
        "vsLastPeriod": 0.0,
        "direction": "unchanged"
      }
    },
    {
      "categoryId": "cat-005",
      "name": "Elektronik",
      "color": "#9966FF",
      "total": 12.00,
      "percentage": 28.6,
      "transactionCount": 1,
      "trend": {
        "vsLastPeriod": 100.0,
        "direction": "increasing"
      }
    }
  ],
  "insights": [
    {
      "type": "top_category",
      "message": "Spielzeug ist Max' groesste Ausgabenkategorie diesen Monat."
    }
  ]
}
```

### Response 403 Forbidden

```json
{
  "error": "ACCESS_DENIED",
  "message": "Nur Eltern koennen die Familien-Kategorie-Analyse sehen"
}
```

## Technische Notizen

### Backend

- Aggregation nach Kategorie mit Gruppierung pro Kind
- Sortierung nach Gesamtbetrag (absteigend)
- Trend-Berechnung: Vergleich mit gleichem Zeitraum des Vormonats
- Insights: Automatisch generierte Hinweise basierend auf Daten

### SQL-Beispiel

```sql
-- Kategorie-Analyse fuer alle Kinder
SELECT
    c.name AS category_name,
    c.id AS category_id,
    c.color,
    ch.id AS child_id,
    ch.name AS child_name,
    SUM(ABS(t.amount)) AS total_amount,
    COUNT(t.id) AS transaction_count
FROM transactions t
JOIN accounts a ON a.id = t.account_id
JOIN children ch ON ch.id = a.child_id
JOIN categories c ON c.id = t.category_id
WHERE ch.family_id = @familyId
  AND t.amount < 0  -- Nur Ausgaben
  AND t.created_at >= @fromDate
  AND t.created_at <= @toDate
GROUP BY c.id, c.name, c.color, ch.id, ch.name
ORDER BY SUM(ABS(t.amount)) DESC;
```

### Insights-Generator

```csharp
public List<Insight> GenerateInsights(CategoryAnalysis analysis)
{
    var insights = new List<Insight>();

    // Top-Kategorie identifizieren
    var topCategory = analysis.Categories.FirstOrDefault();
    if (topCategory != null)
    {
        insights.Add(new Insight
        {
            Type = "top_category",
            Message = $"{topCategory.Name} ist die groesste Ausgabenkategorie.",
            RelatedCategory = topCategory.CategoryId
        });
    }

    // Groesster Spender pro Kategorie
    foreach (var category in analysis.Categories.Take(3))
    {
        var biggestSpender = category.ByChild.OrderByDescending(c => c.Amount).FirstOrDefault();
        if (biggestSpender != null && biggestSpender.Percentage > 50)
        {
            insights.Add(new Insight
            {
                Type = "biggest_spender",
                Message = $"{biggestSpender.Name} hat am meisten fuer {category.Name} ausgegeben.",
                RelatedChild = biggestSpender.ChildId,
                RelatedCategory = category.CategoryId
            });
        }
    }

    return insights;
}
```

### Chart-Library Empfehlungen (Mobile)

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| LiveCharts2 | Horizontale Balken, Stacked | Groessere Dependency |
| Microcharts | Einfache Bar Charts | Keine Stacked Bars |
| SkiaSharp | Volle Kontrolle | Mehr Code |

### Visualisierungs-Empfehlungen

- Horizontale Balken: Besser lesbar fuer Kategorienamen
- Stacked Bars: Anteil pro Kind visuell dargestellt
- Farben: Konsistente Kind-Farben ueber alle Kategorien
- Sortierung: Automatisch nach Betrag (hoechste oben)

### Performance

- Index auf `transactions(category_id, created_at)`
- Caching mit Valkey (TTL: 10 Minuten)
- Maximal 10-15 Kategorien anzeigen, Rest unter "Sonstiges" zusammenfassen

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-095-1 | Familie mit 3 Kindern, 5 Kategorien | 5 horizontale Balken |
| TC-095-2 | Filter auf ein Kind | Nur Ausgaben dieses Kindes |
| TC-095-3 | Sortierung | Hoechste Kategorie oben |
| TC-095-4 | Kategorie antippen | Detail-Popup erscheint |
| TC-095-5 | Trend steigend | Roter Pfeil nach oben |
| TC-095-6 | Keine Ausgaben in Kategorie | Kategorie nicht angezeigt |
| TC-095-7 | Kind versucht Zugriff | 403 Forbidden |
| TC-095-8 | Insights generiert | Mindestens 1 Insight |

## Abhaengigkeiten

- E003 - Transaktionen (Datenbasis)
- E001 - Benutzerverwaltung (Familienstruktur)
- S096 - Zeitraum-Filter

## Story Points

5

## Prioritaet

Mittel
