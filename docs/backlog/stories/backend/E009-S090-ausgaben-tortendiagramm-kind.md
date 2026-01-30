# Story S090: Ausgaben-Tortendiagramm (Kind)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Kind** möchte ich **ein Tortendiagramm meiner Ausgaben nach Kategorie sehen**, damit **ich verstehe, wofür ich mein Geld ausgebe**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Transaktionen, wenn es die Statistik-Seite öffnet, dann wird ein Tortendiagramm der Ausgaben angezeigt
- [ ] Gegeben mehrere Ausgabe-Kategorien, wenn das Diagramm angezeigt wird, dann hat jede Kategorie ein farblich unterschiedliches Segment
- [ ] Gegeben ein Tortendiagramm, wenn das Kind auf ein Segment tippt, dann wird der genaue Betrag und Prozentsatz angezeigt
- [ ] Gegeben ein Zeitraum-Filter, wenn das Kind einen anderen Zeitraum wählt, dann aktualisiert sich das Diagramm entsprechend
- [ ] Gegeben keine Ausgaben im gewählten Zeitraum, wenn das Diagramm geladen wird, dann wird ein leerer Zustand mit Hinweis angezeigt
- [ ] Gegeben die API-Antwort, wenn sie geladen wird, dann werden die Daten in weniger als 2 Sekunden dargestellt

## UI-Entwurf (ASCII)

### Hauptansicht

```
+---------------------------------------+
|  <- Zurueck    Meine Ausgaben         |
+---------------------------------------+
|                                       |
|  Zeitraum: [Januar 2025 v]            |
|                                       |
|             Meine Ausgaben            |
|              Januar 2025              |
|                                       |
|          .-------------------.        |
|        /   Suessigkeiten      \       |
|       /        35%             \      |
|      |  .----------------.      |     |
|      | /    Spielzeug     \     |     |
|      ||       25%          |    |     |
|      | \                  /     |     |
|      |  '----------------'      |     |
|       \    Kleidung  20%       /      |
|        \   Sonstiges 20%      /       |
|          '-------------------'        |
|                                       |
|  +----------------------------------+ |
|  | Suessigkeiten    EUR 15,75  35% | |
|  | Spielzeug        EUR 11,25  25% | |
|  | Kleidung         EUR  9,00  20% | |
|  | Sonstiges        EUR  9,00  20% | |
|  +----------------------------------+ |
|                                       |
|  Gesamt: EUR 45,00                    |
|                                       |
+---------------------------------------+
```

### Leerer Zustand

```
+---------------------------------------+
|  <- Zurueck    Meine Ausgaben         |
+---------------------------------------+
|                                       |
|  Zeitraum: [Januar 2025 v]            |
|                                       |
|                                       |
|        +----------------------+       |
|        |                      |       |
|        |    (Kreis-Icon)      |       |
|        |                      |       |
|        |  Keine Ausgaben      |       |
|        |  in diesem Zeitraum  |       |
|        |                      |       |
|        +----------------------+       |
|                                       |
|  Waehle einen anderen Zeitraum        |
|  oder gib zuerst etwas aus!           |
|                                       |
+---------------------------------------+
```

## API-Endpunkt

### Request

```http
GET /api/statistics/children/{childId}/expenses-by-category
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht |
|-----------|-----|--------------|---------|
| from | date | Startdatum (ISO 8601) | Ja |
| to | date | Enddatum (ISO 8601) | Ja |

### Beispiel-Request

```http
GET /api/statistics/children/550e8400-e29b-41d4-a716-446655440000/expenses-by-category?from=2025-01-01&to=2025-01-31
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Response 200 OK

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Max",
  "period": {
    "from": "2025-01-01",
    "to": "2025-01-31"
  },
  "totalExpenses": 45.00,
  "currency": "EUR",
  "categories": [
    {
      "categoryId": "cat-001",
      "name": "Suessigkeiten",
      "color": "#FF6384",
      "amount": 15.75,
      "percentage": 35.0,
      "transactionCount": 8
    },
    {
      "categoryId": "cat-002",
      "name": "Spielzeug",
      "color": "#36A2EB",
      "amount": 11.25,
      "percentage": 25.0,
      "transactionCount": 2
    },
    {
      "categoryId": "cat-003",
      "name": "Kleidung",
      "color": "#FFCE56",
      "amount": 9.00,
      "percentage": 20.0,
      "transactionCount": 1
    },
    {
      "categoryId": "cat-004",
      "name": "Sonstiges",
      "color": "#4BC0C0",
      "amount": 9.00,
      "percentage": 20.0,
      "transactionCount": 5
    }
  ]
}
```

### Response 200 OK (Keine Ausgaben)

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Max",
  "period": {
    "from": "2025-01-01",
    "to": "2025-01-31"
  },
  "totalExpenses": 0.00,
  "currency": "EUR",
  "categories": []
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

- Aggregation der Transaktionen nach Kategorie mit GROUP BY
- Nur Ausgaben (negative Transaktionen) berucksichtigen
- Prozentberechnung serverseitig durchfuehren
- Caching mit Valkey (TTL: 5 Minuten) fuer haeufige Zeitraeume
- Autorisierung: Kind darf nur eigene Daten sehen, Eltern duerfen Kinder-Daten sehen

### Chart-Library Empfehlungen (Mobile)

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| LiveCharts2 | MAUI-kompatibel, viele Features | Groessere Dependency |
| Microcharts | Leichtgewichtig, einfach | Weniger Anpassungsoptionen |
| SkiaSharp | Volle Kontrolle, performant | Mehr Implementierungsaufwand |

### Farb-Palette (barrierefrei)

```
#FF6384 - Rot/Pink (Suessigkeiten)
#36A2EB - Blau (Spielzeug)
#FFCE56 - Gelb (Kleidung)
#4BC0C0 - Tuerkis (Sonstiges)
#9966FF - Violett (Buecher)
#FF9F40 - Orange (Elektronik)
#C9CBCF - Grau (Andere)
```

### Performance

- Index auf `transactions(child_id, created_at, category_id)`
- Bei grossen Zeitraeumen (> 1 Jahr): Warnung anzeigen
- Pagination nicht noetig (begrenzte Kategorien)

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-090-1 | Kind mit Ausgaben in 3 Kategorien | 3 Segmente im Diagramm |
| TC-090-2 | Kind ohne Ausgaben im Zeitraum | Leerer Zustand angezeigt |
| TC-090-3 | Kind waehlt anderen Zeitraum | Diagramm aktualisiert sich |
| TC-090-4 | Kind tippt auf Segment | Detail-Popup erscheint |
| TC-090-5 | Anderes Kind versucht Zugriff | 403 Forbidden |
| TC-090-6 | Elternteil ruft Kind-Statistik ab | 200 OK |
| TC-090-7 | Prozentsumme | Immer 100% (gerundet) |

## Abhaengigkeiten

- E003 - Transaktionen (Datenbasis)
- E001 - Benutzerverwaltung (Authentifizierung)
- S096 - Zeitraum-Filter

## Story Points

5

## Prioritaet

Mittel
