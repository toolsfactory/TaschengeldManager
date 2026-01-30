# Story W002-S02: Monatsnavigation und -auswahl

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Kind** möchte ich **zwischen verschiedenen Monaten navigieren können**, damit **ich auch vergangene Monate in meinem Kassenbuch ansehen kann**.

## Akzeptanzkriterien

- [x] Gegeben die Kassenbuch-Ansicht, wenn sie geladen wird, dann sind Pfeile für "Vorheriger Monat" und "Nächster Monat" sichtbar
- [x] Gegeben der aktuelle Monat ist angezeigt, wenn ich auf "Nächster Monat" klicke, dann ist der Button deaktiviert (kann nicht in die Zukunft navigieren)
- [x] Gegeben ein vergangener Monat, wenn ich auf "Vorheriger Monat" klicke, dann wird der vorherige Monat geladen
- [x] Gegeben die Monatsnavigation, wenn ein Monat/Jahr-Dropdown existiert, dann kann ich direkt einen Monat auswählen
- [x] Gegeben die Navigation, wenn der Monat gewechselt wird, dann werden die Transaktionen für den neuen Monat geladen

## Technische Hinweise

### Komponente: src/components/MonthSelector.tsx

```tsx
interface MonthSelectorProps {
  selectedMonth: string; // Format: "2025-12"
  onMonthChange: (month: string) => void;
  minMonth?: string;     // Ältester verfügbarer Monat
}

export function MonthSelector({ selectedMonth, onMonthChange, minMonth }: MonthSelectorProps) {
  const [year, month] = selectedMonth.split('-').map(Number);

  const currentDate = new Date();
  const currentMonth = `${currentDate.getFullYear()}-${String(currentDate.getMonth() + 1).padStart(2, '0')}`;

  const canGoNext = selectedMonth < currentMonth;
  const canGoPrev = !minMonth || selectedMonth > minMonth;

  const goToPrevious = () => {
    const prevDate = new Date(year, month - 2, 1);
    onMonthChange(`${prevDate.getFullYear()}-${String(prevDate.getMonth() + 1).padStart(2, '0')}`);
  };

  const goToNext = () => {
    const nextDate = new Date(year, month, 1);
    onMonthChange(`${nextDate.getFullYear()}-${String(nextDate.getMonth() + 1).padStart(2, '0')}`);
  };

  const monthNames = [
    'Januar', 'Februar', 'März', 'April', 'Mai', 'Juni',
    'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'
  ];

  return (
    <div className="flex items-center justify-between mb-6">
      <button
        onClick={goToPrevious}
        disabled={!canGoPrev}
        className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
        aria-label="Vorheriger Monat"
      >
        <ChevronLeftIcon className="w-6 h-6" />
      </button>

      <div className="flex items-center space-x-2">
        <span className="text-xl font-semibold text-gray-900 dark:text-gray-100">
          {monthNames[month - 1]} {year}
        </span>

        {/* Optional: Dropdown für Direktauswahl */}
        <select
          value={selectedMonth}
          onChange={(e) => onMonthChange(e.target.value)}
          className="input w-auto"
        >
          {generateMonthOptions(minMonth, currentMonth).map((m) => (
            <option key={m.value} value={m.value}>
              {m.label}
            </option>
          ))}
        </select>
      </div>

      <button
        onClick={goToNext}
        disabled={!canGoNext}
        className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
        aria-label="Nächster Monat"
      >
        <ChevronRightIcon className="w-6 h-6" />
      </button>
    </div>
  );
}

function generateMonthOptions(minMonth: string | undefined, maxMonth: string) {
  const options: { value: string; label: string }[] = [];
  const monthNames = [
    'Januar', 'Februar', 'März', 'April', 'Mai', 'Juni',
    'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'
  ];

  // Generate last 12 months
  const [maxYear, maxMonthNum] = maxMonth.split('-').map(Number);

  for (let i = 0; i < 12; i++) {
    const date = new Date(maxYear, maxMonthNum - 1 - i, 1);
    const value = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;

    if (minMonth && value < minMonth) break;

    options.push({
      value,
      label: `${monthNames[date.getMonth()]} ${date.getFullYear()}`
    });
  }

  return options;
}
```

### URL-Parameter für Deep-Linking (optional)

```tsx
// In Cashbook.tsx
import { useSearchParams } from 'react-router-dom';

const [searchParams, setSearchParams] = useSearchParams();
const monthParam = searchParams.get('month');

useEffect(() => {
  if (monthParam && /^\d{4}-\d{2}$/.test(monthParam)) {
    setSelectedMonth(monthParam);
  }
}, [monthParam]);

const handleMonthChange = (month: string) => {
  setSelectedMonth(month);
  setSearchParams({ month });
};
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-10 | Kassenbuch öffnen | Aktueller Monat ist ausgewählt |
| TC-W002-11 | Klick auf "Vorheriger Monat" | November wird angezeigt (wenn Dezember aktuell) |
| TC-W002-12 | Klick auf "Nächster Monat" bei aktuellem Monat | Button ist deaktiviert |
| TC-W002-13 | Monat über Dropdown wählen | Gewählter Monat wird geladen |
| TC-W002-14 | URL mit month-Parameter | Entsprechender Monat wird angezeigt |
| TC-W002-15 | Tastaturnavigation | Pfeile per Tab erreichbar, Enter aktiviert |

## Story Points

3

## Priorität

Hoch
