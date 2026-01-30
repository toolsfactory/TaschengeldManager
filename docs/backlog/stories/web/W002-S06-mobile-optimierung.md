# Story W002-S06: Mobile-optimierte Darstellung

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Kind mit Smartphone** möchte ich **das Kassenbuch auch auf meinem Handy gut lesen können**, damit **ich unterwegs meine Finanzen überprüfen kann**.

## Akzeptanzkriterien

- [x] Gegeben ein Smartphone, wenn das Kassenbuch geöffnet wird, dann ist die Tabelle horizontal scrollbar
- [x] Gegeben ein kleiner Bildschirm, wenn die Zusammenfassung angezeigt wird, dann werden die Kacheln vertikal gestapelt (2x2 Grid)
- [x] Gegeben die mobile Ansicht, wenn sie genutzt wird, dann ist die Schrift ausreichend groß (mindestens 14px)
- [x] Gegeben die Tabelle auf Mobile, wenn sie angezeigt wird, dann sind Datum und Saldo immer sichtbar (sticky)
- [x] Gegeben Touch-Eingabe, wenn auf eine Zeile getippt wird, dann ist sie als angeklickt erkennbar

## Technische Hinweise

### Responsive Tabelle

```tsx
// CashbookTable.tsx
<div className="overflow-x-auto -mx-4 sm:mx-0">
  <div className="inline-block min-w-full align-middle">
    <table className="min-w-full">
      {/* ... */}
    </table>
  </div>
</div>
```

### Sticky Columns für Mobile

```css
/* In index.css oder als Tailwind Plugin */
@media (max-width: 640px) {
  .cashbook-table th:first-child,
  .cashbook-table td:first-child {
    position: sticky;
    left: 0;
    z-index: 10;
    background: inherit;
  }

  .cashbook-table th:last-child,
  .cashbook-table td:last-child {
    position: sticky;
    right: 0;
    z-index: 10;
    background: inherit;
  }
}
```

### Alternative: Card-Ansicht für Mobile

```tsx
// CashbookMobileCard.tsx - Alternative zur Tabelle auf kleinen Bildschirmen
interface CashbookMobileCardProps {
  entry: CashbookEntry;
}

export function CashbookMobileCard({ entry }: CashbookMobileCardProps) {
  const isIncome = entry.income !== undefined;

  return (
    <div className="p-4 border-b border-gray-200 dark:border-gray-700">
      <div className="flex justify-between items-start">
        <div className="flex-1">
          <p className="text-sm text-gray-500 dark:text-gray-400">
            {formatDate(entry.date)}
          </p>
          <p className="font-medium text-gray-900 dark:text-gray-100">
            {entry.description}
          </p>
        </div>
        <div className="text-right">
          <p className={`text-lg font-bold ${
            isIncome
              ? 'text-green-600 dark:text-green-400'
              : 'text-red-600 dark:text-red-400'
          }`}>
            {isIncome ? '+' : '-'}{(entry.income || entry.expense || 0).toFixed(2)} €
          </p>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Saldo: {entry.runningBalance.toFixed(2)} €
          </p>
        </div>
      </div>
    </div>
  );
}
```

### Responsive Layout mit Breakpoint-Switch

```tsx
// Cashbook.tsx
import { useMediaQuery } from '../hooks/useMediaQuery';

export function Cashbook() {
  const isMobile = useMediaQuery('(max-width: 640px)');

  return (
    <div>
      {/* Header immer gleich */}
      <CashbookHeader {...summaryData} />

      {/* Tabelle oder Cards je nach Bildschirmgröße */}
      {isMobile ? (
        <div className="card p-0">
          {entries.map((entry) => (
            <CashbookMobileCard key={entry.id} entry={entry} />
          ))}
        </div>
      ) : (
        <CashbookTable entries={entries} openingBalance={openingBalance} />
      )}
    </div>
  );
}
```

### useMediaQuery Hook

```tsx
// src/hooks/useMediaQuery.ts
import { useState, useEffect } from 'react';

export function useMediaQuery(query: string): boolean {
  const [matches, setMatches] = useState(
    () => window.matchMedia(query).matches
  );

  useEffect(() => {
    const mediaQuery = window.matchMedia(query);
    const handler = (e: MediaQueryListEvent) => setMatches(e.matches);

    mediaQuery.addEventListener('change', handler);
    return () => mediaQuery.removeEventListener('change', handler);
  }, [query]);

  return matches;
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-50 | Kassenbuch auf iPhone SE (375px) | Ansicht ist lesbar und bedienbar |
| TC-W002-51 | Tabelle horizontal scrollen | Datum und Saldo bleiben sichtbar |
| TC-W002-52 | Mobile Card-Ansicht | Einträge als Karten dargestellt |
| TC-W002-53 | Zusammenfassung auf Mobile | 2x2 Grid oder vertikal gestapelt |
| TC-W002-54 | Touch auf Eintrag | Visuelles Feedback (Ripple oder Highlight) |
| TC-W002-55 | Monatsnavigation auf Mobile | Pfeile und Dropdown sind touch-freundlich |
| TC-W002-56 | Desktop-Ansicht (1920px) | Volle Tabelle ohne horizontales Scrollen |

## Design-Hinweise

### Breakpoints
- **Mobile**: < 640px (Card-Ansicht)
- **Tablet**: 640px - 1024px (Kompakte Tabelle)
- **Desktop**: > 1024px (Volle Tabelle)

### Touch-Targets
Mindestgröße für Touch-Elemente: 44x44px

## Story Points

3

## Priorität

Mittel
