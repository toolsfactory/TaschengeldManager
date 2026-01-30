# Story S092: Monatsvergleich Balkendiagramm (Kind)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Kind** möchte ich **sehen, ob ich diesen Monat mehr oder weniger ausgegeben habe als letzten Monat**, damit **ich mein Sparverhalten vergleichen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Transaktionen, wenn es die Vergleich-Ansicht öffnet, dann werden zwei Balken (aktueller und vorheriger Monat) angezeigt
- [ ] Gegeben die Balken, wenn sie angezeigt werden, dann ist die prozentuale Veraenderung deutlich sichtbar
- [ ] Gegeben eine Reduzierung der Ausgaben, wenn der Vergleich angezeigt wird, dann erscheint positives Feedback
- [ ] Gegeben eine Erhoehung der Ausgaben, wenn der Vergleich angezeigt wird, dann erscheint ein neutraler Hinweis
- [ ] Gegeben die Option, wenn das Kind auf einen Balken tippt, dann werden Details zum Monat angezeigt
- [ ] Gegeben die Einnahmen-Option, wenn aktiviert, dann werden Einnahmen-Balken zusaetzlich angezeigt

## UI-Entwurf (ASCII)

### Hauptansicht - Ausgaben gesunken

```
+---------------------------------------+
|  <- Zurueck    Monatsvergleich        |
+---------------------------------------+
|                                       |
|  Vergleich: [Ausgaben v]              |
|                                       |
|            Meine Ausgaben             |
|                                       |
|  EUR                                  |
|   |                                   |
|  50+     +-------+                    |
|   |      |       |                    |
|  40+     |       |   +-------+        |
|   |      |       |   |       |        |
|  30+     |       |   |       |        |
|   |      |       |   |       |        |
|  20+     |       |   |       |        |
|   |      |       |   |       |        |
|  10+     |       |   |       |        |
|   |      |       |   |       |        |
|   +------+-------+---+-------+---     |
|          Dez 24       Jan 25          |
|                                       |
|  +----------------------------------+ |
|  |                                  | |
|  |     \/ 16,7% weniger ausgegeben  | |
|  |                                  | |
|  |        Super gemacht!            | |
|  |                                  | |
|  +----------------------------------+ |
|                                       |
|  Dezember: EUR 42,00                  |
|  Januar:   EUR 35,00                  |
|  Ersparnis: EUR 7,00                  |
|                                       |
+---------------------------------------+
```

### Ausgaben gestiegen

```
+---------------------------------------+
|  <- Zurueck    Monatsvergleich        |
+---------------------------------------+
|                                       |
|  Vergleich: [Ausgaben v]              |
|                                       |
|            Meine Ausgaben             |
|                                       |
|  EUR                                  |
|   |                                   |
|  60+                 +-------+        |
|   |                  |       |        |
|  50+                 |       |        |
|   |      +-------+   |       |        |
|  40+     |       |   |       |        |
|   |      |       |   |       |        |
|  30+     |       |   |       |        |
|   |      |       |   |       |        |
|  20+     |       |   |       |        |
|   |      |       |   |       |        |
|   +------+-------+---+-------+---     |
|          Dez 24       Jan 25          |
|                                       |
|  +----------------------------------+ |
|  |                                  | |
|  |     /\ 20% mehr ausgegeben       | |
|  |                                  | |
|  |  Vielleicht naechsten Monat      | |
|  |  etwas weniger ausgeben?         | |
|  |                                  | |
|  +----------------------------------+ |
|                                       |
+---------------------------------------+
```

### Einnahmen und Ausgaben kombiniert

```
+---------------------------------------+
|  <- Zurueck    Monatsvergleich        |
+---------------------------------------+
|                                       |
|  Vergleich: [Beides v]                |
|                                       |
|        Einnahmen vs Ausgaben          |
|                                       |
|  EUR   [E]=Einnahmen  [A]=Ausgaben    |
|   |                                   |
|  60+     +---+                        |
|   |      |[E]|           +---+        |
|  50+     +---+   +---+   |[E]|        |
|   |      |[A]|   |[E]|   +---+        |
|  40+     +---+   +---+   |[A]|        |
|   |              |[A]|   +---+        |
|  30+             +---+                |
|   |                                   |
|   +------+-------+---+-------+---     |
|          Dez 24       Jan 25          |
|                                       |
|  Dezember:                            |
|    Einnahmen: EUR 50,00               |
|    Ausgaben:  EUR 42,00               |
|    Bilanz:    EUR +8,00               |
|                                       |
|  Januar:                              |
|    Einnahmen: EUR 50,00               |
|    Ausgaben:  EUR 35,00               |
|    Bilanz:    EUR +15,00              |
|                                       |
+---------------------------------------+
```

## API-Endpunkt

### Request

```http
GET /api/statistics/children/{childId}/month-comparison
Authorization: Bearer {token}
```

### Query-Parameter

| Parameter | Typ | Beschreibung | Pflicht | Default |
|-----------|-----|--------------|---------|---------|
| currentMonth | string | Aktueller Monat (YYYY-MM) | Nein | Aktueller Monat |
| includeIncome | boolean | Einnahmen mit einbeziehen | Nein | false |

### Beispiel-Request

```http
GET /api/statistics/children/550e8400-e29b-41d4-a716-446655440000/month-comparison?currentMonth=2025-01&includeIncome=true
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Response 200 OK

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Max",
  "currency": "EUR",
  "currentMonth": {
    "month": "2025-01",
    "monthName": "Januar 2025",
    "expenses": 35.00,
    "income": 50.00,
    "balance": 15.00,
    "transactionCount": 12
  },
  "previousMonth": {
    "month": "2024-12",
    "monthName": "Dezember 2024",
    "expenses": 42.00,
    "income": 50.00,
    "balance": 8.00,
    "transactionCount": 15
  },
  "comparison": {
    "expenseChange": {
      "amount": -7.00,
      "percent": -16.7,
      "direction": "decreased"
    },
    "incomeChange": {
      "amount": 0.00,
      "percent": 0.0,
      "direction": "unchanged"
    },
    "balanceChange": {
      "amount": 7.00,
      "percent": 87.5,
      "direction": "increased"
    }
  },
  "feedback": {
    "type": "positive",
    "message": "Super gemacht! Du hast diesen Monat weniger ausgegeben.",
    "emoji": "star"
  }
}
```

### Response 200 OK (Erster Monat)

```json
{
  "childId": "550e8400-e29b-41d4-a716-446655440000",
  "childName": "Max",
  "currency": "EUR",
  "currentMonth": {
    "month": "2025-01",
    "monthName": "Januar 2025",
    "expenses": 35.00,
    "income": 50.00,
    "balance": 15.00,
    "transactionCount": 12
  },
  "previousMonth": null,
  "comparison": null,
  "feedback": {
    "type": "info",
    "message": "Naechsten Monat kannst du vergleichen!",
    "emoji": "clock"
  }
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

- Aggregation der Transaktionen pro Monat
- Einnahmen = positive Transaktionen (Taschengeld, Geschenke, Zinsen)
- Ausgaben = negative Transaktionen
- Feedback-Logik basierend auf Veraenderung:
  - < -10%: "Super gemacht!" (positive)
  - -10% bis +10%: "Gut gehalten!" (neutral)
  - > +10%: "Vielleicht naechsten Monat weniger?" (hint)

### Feedback-Kategorien

```json
{
  "feedbackTypes": {
    "positive": {
      "threshold": -10,
      "messages": [
        "Super gemacht!",
        "Toll gespart!",
        "Weiter so!"
      ]
    },
    "neutral": {
      "threshold": 10,
      "messages": [
        "Gut gehalten!",
        "Stabil geblieben!",
        "Gleichmaessig!"
      ]
    },
    "hint": {
      "threshold": null,
      "messages": [
        "Vielleicht naechsten Monat etwas weniger?",
        "Schau mal, wo du sparen kannst!",
        "Naechsten Monat wird's besser!"
      ]
    }
  }
}
```

### Chart-Library Empfehlungen (Mobile)

| Library | Vorteile | Nachteile |
|---------|----------|-----------|
| LiveCharts2 | Animierte Balken, Touch | Groessere Dependency |
| Microcharts | Bar Chart Support, einfach | Weniger Anpassung |
| SkiaSharp | Volle Kontrolle | Mehr Code |

### Visualisierungs-Empfehlungen

- Farben:
  - Einnahmen: Gruen (#4CAF50)
  - Ausgaben: Orange (#FF9800) oder Rot (#F44336)
- Animation: Balken von unten nach oben wachsen lassen
- Beschriftung: Betrag ueber jedem Balken
- Legende: Falls Einnahmen/Ausgaben kombiniert

### Performance

- Einfache Aggregation, kein Caching noetig
- Query auf `transactions(child_id, created_at)`

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-092-1 | Ausgaben gesunken (>10%) | Positives Feedback |
| TC-092-2 | Ausgaben gestiegen (>10%) | Hint-Feedback |
| TC-092-3 | Ausgaben stabil (+-10%) | Neutrales Feedback |
| TC-092-4 | Erster Monat ohne Vergleich | Info-Meldung |
| TC-092-5 | Einnahmen und Ausgaben kombiniert | Beide Balken pro Monat |
| TC-092-6 | Kind tippt auf Balken | Detail-Popup erscheint |
| TC-092-7 | Keine Transaktionen im Monat | 0 EUR angezeigt |

## Abhaengigkeiten

- E003 - Transaktionen (Datenbasis)
- S096 - Zeitraum-Filter (optional)

## Story Points

5

## Prioritaet

Mittel
