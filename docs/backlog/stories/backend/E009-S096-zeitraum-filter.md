# Story S096: Zeitraum-Filter (Woche/Monat/Jahr)

## Epic

E009 - Statistiken & Auswertungen

## User Story

Als **Benutzer (Kind oder Elternteil)** möchte ich **den Zeitraum fuer Statistiken flexibel waehlen koennen**, damit **ich relevante Zeitabschnitte analysieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine Statistik-Ansicht, wenn der Benutzer den Zeitraum-Filter sieht, dann kann er zwischen vordefinierten Zeitraeumen waehlen
- [ ] Gegeben die vordefinierten Optionen, dann sind Woche, Monat, Quartal und Jahr verfuegbar
- [ ] Gegeben ein benutzerdefinierter Zeitraum, wenn der Benutzer Start- und Enddatum waehlt, dann werden genau diese Daten verwendet
- [ ] Gegeben eine Zeitraum-Auswahl, wenn der Benutzer bestaetigt, dann aktualisieren sich alle Statistiken auf der Seite
- [ ] Gegeben ein sehr langer Zeitraum (> 1 Jahr), wenn er gewaehlt wird, dann erscheint ein Performance-Hinweis
- [ ] Gegeben der letzte gewaehlte Zeitraum, dann wird er beim naechsten Oeffnen wiederhergestellt

## UI-Entwurf (ASCII)

### Zeitraum-Auswahl (Dropdown)

```
+---------------------------------------+
|  Zeitraum:                            |
|  +-------------------------------+    |
|  | Diese Woche                 v |    |
|  +-------------------------------+    |
|                                       |
|  +-------------------------------+    |
|  | o Diese Woche                 |    |
|  | o Letzter Monat               |    |
|  | o Dieses Quartal              |    |
|  | o Dieses Jahr                 |    |
|  | o Benutzerdefiniert...        |    |
|  +-------------------------------+    |
|                                       |
+---------------------------------------+
```

### Benutzerdefinierter Zeitraum (Modal)

```
+---------------------------------------+
|        Zeitraum waehlen               |
+---------------------------------------+
|                                       |
|  Von:                                 |
|  +-------------------------------+    |
|  | 01.01.2025                    |[x] |
|  +-------------------------------+    |
|                                       |
|  Bis:                                 |
|  +-------------------------------+    |
|  | 31.01.2025                    |[x] |
|  +-------------------------------+    |
|                                       |
|  Schnellauswahl:                      |
|  [Letzte 7 Tage]  [Letzte 30 Tage]    |
|  [Dieses Jahr]    [Letztes Jahr]      |
|                                       |
|  ------------------------------------ |
|                                       |
|  | Hinweis: Lange Zeitraeume         ||
|  | koennen laenger laden.            ||
|                                       |
|  [Abbrechen]         [Uebernehmen]    |
|                                       |
+---------------------------------------+
```

### Monats-/Wochen-Navigator

```
+---------------------------------------+
|                                       |
|  < Dezember 2024  |  Januar 2025 >    |
|                                       |
|  oder                                 |
|                                       |
|  <  KW 51  |  KW 52  |  KW 1  |  KW 2 >
|                                       |
+---------------------------------------+
```

### Kalender-Ansicht (fuer benutzerdefiniert)

```
+---------------------------------------+
|          Januar 2025                  |
|  Mo  Di  Mi  Do  Fr  Sa  So           |
+---------------------------------------+
|       30  31  [1] [2] [3]  4   5      |
|   6   7   8   9  10  11  12           |
|  13  14  15  16  17  18  19           |
|  20  21  22  23  24  25  26           |
|  27  28  29  30  [31]                 |
+---------------------------------------+
|  Ausgewaehlt: 01.01. - 31.01.2025     |
+---------------------------------------+
```

## API-Spezifikation

### Query-Parameter (einheitlich fuer alle Statistik-Endpoints)

| Parameter | Typ | Beschreibung | Beispiele |
|-----------|-----|--------------|-----------|
| from | date | Startdatum (ISO 8601) | 2025-01-01 |
| to | date | Enddatum (ISO 8601) | 2025-01-31 |
| period | string | Vordefinierter Zeitraum | this_week, last_month, this_quarter, this_year |

### Vordefinierte Zeitraeume

| Wert | Beschreibung | Beispiel (Heute: 15.01.2025) |
|------|--------------|------------------------------|
| this_week | Aktuelle Kalenderwoche | 13.01.2025 - 19.01.2025 |
| last_week | Letzte Kalenderwoche | 06.01.2025 - 12.01.2025 |
| this_month | Aktueller Monat | 01.01.2025 - 31.01.2025 |
| last_month | Letzter Monat | 01.12.2024 - 31.12.2024 |
| this_quarter | Aktuelles Quartal | 01.01.2025 - 31.03.2025 |
| last_quarter | Letztes Quartal | 01.10.2024 - 31.12.2024 |
| this_year | Aktuelles Jahr | 01.01.2025 - 31.12.2025 |
| last_year | Letztes Jahr | 01.01.2024 - 31.12.2024 |
| last_7_days | Letzte 7 Tage | 09.01.2025 - 15.01.2025 |
| last_30_days | Letzte 30 Tage | 16.12.2024 - 15.01.2025 |
| last_90_days | Letzte 90 Tage | 17.10.2024 - 15.01.2025 |

### Beispiel-Requests

```http
# Vordefinierter Zeitraum
GET /api/statistics/children/{childId}/expenses-by-category?period=this_month

# Benutzerdefiniert
GET /api/statistics/children/{childId}/expenses-by-category?from=2025-01-01&to=2025-01-31

# Period hat Vorrang, wenn beides angegeben
GET /api/statistics/children/{childId}/expenses-by-category?period=this_month&from=2025-01-01&to=2025-01-15
# -> Verwendet this_month (01.01. - 31.01.)
```

### Verfuegbare Zeitraeume Endpoint

```http
GET /api/statistics/periods
```

### Response 200 OK

```json
{
  "today": "2025-01-15",
  "periods": [
    {
      "key": "this_week",
      "label": "Diese Woche",
      "from": "2025-01-13",
      "to": "2025-01-19"
    },
    {
      "key": "last_week",
      "label": "Letzte Woche",
      "from": "2025-01-06",
      "to": "2025-01-12"
    },
    {
      "key": "this_month",
      "label": "Dieser Monat",
      "from": "2025-01-01",
      "to": "2025-01-31"
    },
    {
      "key": "last_month",
      "label": "Letzter Monat",
      "from": "2024-12-01",
      "to": "2024-12-31"
    },
    {
      "key": "this_quarter",
      "label": "Dieses Quartal",
      "from": "2025-01-01",
      "to": "2025-03-31"
    },
    {
      "key": "this_year",
      "label": "Dieses Jahr",
      "from": "2025-01-01",
      "to": "2025-12-31"
    },
    {
      "key": "custom",
      "label": "Benutzerdefiniert",
      "from": null,
      "to": null
    }
  ],
  "constraints": {
    "maxDays": 365,
    "minDate": "2020-01-01",
    "maxDate": "2025-01-15"
  }
}
```

### Validierung Response 400

```json
{
  "error": "INVALID_PERIOD",
  "message": "Enddatum darf nicht vor Startdatum liegen",
  "details": {
    "from": "2025-01-31",
    "to": "2025-01-01"
  }
}
```

```json
{
  "error": "PERIOD_TOO_LONG",
  "message": "Der Zeitraum darf maximal 365 Tage betragen",
  "details": {
    "requestedDays": 400,
    "maxDays": 365
  }
}
```

## Technische Notizen

### Backend

- Zeitraum-Berechnung serverseitig basierend auf UTC
- Kalenderwochen nach ISO 8601 (Woche beginnt Montag)
- Validierung: Startdatum <= Enddatum, max. 365 Tage
- Performance-Warnung bei Zeitraum > 90 Tage

### Zeitraum-Berechnung (C#)

```csharp
public record DateRange(DateTime From, DateTime To);

public DateRange GetPeriodRange(string period, DateTime today)
{
    return period switch
    {
        "this_week" => GetWeekRange(today, 0),
        "last_week" => GetWeekRange(today, -1),
        "this_month" => GetMonthRange(today, 0),
        "last_month" => GetMonthRange(today, -1),
        "this_quarter" => GetQuarterRange(today, 0),
        "last_quarter" => GetQuarterRange(today, -1),
        "this_year" => GetYearRange(today, 0),
        "last_year" => GetYearRange(today, -1),
        "last_7_days" => new DateRange(today.AddDays(-6), today),
        "last_30_days" => new DateRange(today.AddDays(-29), today),
        "last_90_days" => new DateRange(today.AddDays(-89), today),
        _ => throw new ArgumentException($"Unknown period: {period}")
    };
}

private DateRange GetWeekRange(DateTime date, int weekOffset)
{
    var startOfWeek = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
    startOfWeek = startOfWeek.AddDays(weekOffset * 7);
    return new DateRange(startOfWeek, startOfWeek.AddDays(6));
}

private DateRange GetMonthRange(DateTime date, int monthOffset)
{
    var start = new DateTime(date.Year, date.Month, 1).AddMonths(monthOffset);
    var end = start.AddMonths(1).AddDays(-1);
    return new DateRange(start, end);
}
```

### Mobile (MAUI)

- DatePicker fuer Datumseingabe
- Picker/Dropdown fuer vordefinierte Zeitraeume
- Speichern der letzten Auswahl in Preferences/SecureStorage

### UI-Komponente (Wiederverwendbar)

```csharp
public partial class PeriodSelector : ContentView
{
    public static readonly BindableProperty SelectedPeriodProperty =
        BindableProperty.Create(nameof(SelectedPeriod), typeof(string), typeof(PeriodSelector), "this_month");

    public static readonly BindableProperty FromDateProperty =
        BindableProperty.Create(nameof(FromDate), typeof(DateTime?), typeof(PeriodSelector));

    public static readonly BindableProperty ToDateProperty =
        BindableProperty.Create(nameof(ToDate), typeof(DateTime?), typeof(PeriodSelector));

    public event EventHandler<PeriodChangedEventArgs> PeriodChanged;
}
```

### Caching-Strategie

- Vordefinierte Zeitraeume: Laengeres TTL (15 Minuten)
- Benutzerdefinierte Zeitraeume: Kuerzeres TTL (5 Minuten)
- Cache-Key: `stats:{endpoint}:{userId}:{period}:{from}:{to}`

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-096-1 | Vordefinierter Zeitraum "this_month" | Korrektes Monats-Intervall |
| TC-096-2 | Benutzerdefiniert 01.01. - 15.01. | Genau diese Daten verwendet |
| TC-096-3 | Enddatum vor Startdatum | 400 Bad Request |
| TC-096-4 | Zeitraum > 365 Tage | 400 Bad Request |
| TC-096-5 | Wechsel von Monat zu Woche | Statistiken aktualisieren |
| TC-096-6 | Zeitraum-Auswahl speichern | Bei erneutem Oeffnen wiederhergestellt |
| TC-096-7 | Kalenderwochen-Navigation | Korrekte KW-Grenzen |
| TC-096-8 | Zeitraum in der Zukunft | Bis heute begrenzt |

## Abhaengigkeiten

- Keine direkten Story-Abhaengigkeiten
- Wird von allen Statistik-Stories verwendet (S090-S095)

## Story Points

3

## Prioritaet

Hoch (Grundlage fuer alle Statistiken)
