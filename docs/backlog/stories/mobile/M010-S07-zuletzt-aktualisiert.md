# Story M010-S07: Zuletzt aktualisiert Anzeige

## Epic

M010 - Offline-Funktionalitaet

## User Story

Als **Benutzer** moechte ich **sehen koennen, wann meine Daten zuletzt aktualisiert wurden**, damit **ich weiss, wie aktuell die angezeigten Informationen sind**.

## Akzeptanzkriterien

- [ ] Gegeben gecachte Daten, wenn sie angezeigt werden, dann ist der Zeitpunkt der letzten Aktualisierung sichtbar
- [ ] Gegeben ein Aktualisierungs-Zeitpunkt, wenn er weniger als 1 Stunde her ist, dann wird "vor X Minuten" angezeigt
- [ ] Gegeben ein Aktualisierungs-Zeitpunkt, wenn er mehr als 1 Stunde her ist, dann wird die Uhrzeit angezeigt
- [ ] Gegeben ein Aktualisierungs-Zeitpunkt von gestern oder aelter, wenn er angezeigt wird, dann wird das Datum mit Uhrzeit angezeigt

## UI-Entwurf

### Verschiedene Zeitformate
```
+------------------------------------+
|  Kontostand                        |
|  Aktualisiert: gerade eben         |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
+------------------------------------+

+------------------------------------+
|  Kontostand                        |
|  Aktualisiert: vor 15 Minuten      |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
+------------------------------------+

+------------------------------------+
|  Kontostand                        |
|  Aktualisiert: vor 2 Stunden       |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
+------------------------------------+

+------------------------------------+
|  Kontostand                        |
|  Aktualisiert: gestern, 18:30      |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
+------------------------------------+

+------------------------------------+
|  Kontostand                        |
|  Aktualisiert: 18.01.2026, 14:30   |
|  +--------------------------------+|
|  |         150,00 EUR             ||
|  +--------------------------------+|
+------------------------------------+
```

### In der Transaktionsliste
```
+------------------------------------+
|  <- Zurueck       Transaktionen    |
+------------------------------------+
|                                    |
|  Stand: vor 30 Minuten             |
|  [Pull-to-Refresh verfuegbar]      |
|                                    |
|  +--------------------------------+|
|  | [Cart] Suessigkeiten           ||
|  |        -2,50 EUR               ||
|  |        20.01.2026, 14:30       ||
|  +--------------------------------+|
|  ...                               |
+------------------------------------+
```

## Technische Notizen

- Helper-Klasse: `TimeAgoFormatter`
- Sprache: Deutsch
- Relative Zeit bis 24 Stunden, danach absolut
- Verwendung in allen gecachten Daten-Ansichten

## Implementierungshinweise

```csharp
public static class TimeAgoFormatter
{
    public static string Format(DateTime dateTime)
    {
        var now = DateTime.Now;
        var diff = now - dateTime;

        if (diff.TotalSeconds < 60)
            return "gerade eben";

        if (diff.TotalMinutes < 60)
        {
            var minutes = (int)diff.TotalMinutes;
            return minutes == 1 ? "vor 1 Minute" : $"vor {minutes} Minuten";
        }

        if (diff.TotalHours < 24)
        {
            var hours = (int)diff.TotalHours;
            return hours == 1 ? "vor 1 Stunde" : $"vor {hours} Stunden";
        }

        if (dateTime.Date == now.Date.AddDays(-1))
            return $"gestern, {dateTime:HH:mm}";

        if (diff.TotalDays < 7)
        {
            var days = (int)diff.TotalDays;
            return days == 1 ? "vor 1 Tag" : $"vor {days} Tagen";
        }

        return dateTime.ToString("dd.MM.yyyy, HH:mm");
    }
}

// Value Converter fuer XAML Binding
public class TimeAgoConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return TimeAgoFormatter.Format(dateTime);
        }
        return "-";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

// XAML Verwendung
// App.xaml
<Application.Resources>
    <converters:TimeAgoConverter x:Key="TimeAgoConverter"/>
</Application.Resources>

// DashboardPage.xaml
<Label Text="{Binding LastUpdated, Converter={StaticResource TimeAgoConverter}, StringFormat='Aktualisiert: {0}'}"
       FontSize="12"
       TextColor="Gray"/>

// ViewModel
public partial class DashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private DateTime? _lastUpdated;

    [ObservableProperty]
    private string _lastUpdatedText = string.Empty;

    partial void OnLastUpdatedChanged(DateTime? value)
    {
        LastUpdatedText = value.HasValue
            ? $"Aktualisiert: {TimeAgoFormatter.Format(value.Value)}"
            : string.Empty;
    }
}

// Automatische Aktualisierung der Anzeige (optional)
public class TimeAgoUpdater : IDisposable
{
    private readonly Timer _timer;
    private readonly Action _updateCallback;

    public TimeAgoUpdater(Action updateCallback)
    {
        _updateCallback = updateCallback;
        _timer = new Timer(60000); // Jede Minute
        _timer.Elapsed += (s, e) => MainThread.BeginInvokeOnMainThread(_updateCallback);
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Vor 30 Sekunden | "gerade eben" |
| TC-002 | Vor 5 Minuten | "vor 5 Minuten" |
| TC-003 | Vor 1 Minute | "vor 1 Minute" (Singular) |
| TC-004 | Vor 2 Stunden | "vor 2 Stunden" |
| TC-005 | Gestern 18:30 | "gestern, 18:30" |
| TC-006 | Vor 3 Tagen | "vor 3 Tagen" |
| TC-007 | Vor 2 Wochen | "06.01.2026, 14:30" (Datum) |
| TC-008 | Kein Wert (null) | Nichts anzeigen oder "-" |

## Story Points

1

## Prioritaet

Niedrig
