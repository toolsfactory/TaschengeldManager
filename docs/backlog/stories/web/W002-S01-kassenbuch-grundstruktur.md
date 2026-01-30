# Story W002-S01: Kassenbuch-Monatsansicht Grundstruktur

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Kind** möchte ich **meine Kontobewegungen in einer Kassenbuch-Tabelle sehen**, damit **ich einen klaren Überblick über meine Einnahmen und Ausgaben habe**.

## Akzeptanzkriterien

- [x] Gegeben die Kassenbuch-Seite, wenn sie geladen wird, dann wird der aktuelle Monat angezeigt
- [x] Gegeben Transaktionen im Monat, wenn sie angezeigt werden, dann haben sie Spalten für: Datum, Beschreibung, Einnahmen, Ausgaben, Saldo
- [x] Gegeben eine Einnahme, wenn sie angezeigt wird, dann steht der Betrag in der Einnahmen-Spalte (grün)
- [x] Gegeben eine Ausgabe, wenn sie angezeigt wird, dann steht der Betrag in der Ausgaben-Spalte (rot)
- [x] Gegeben die Saldo-Spalte, wenn Transaktionen angezeigt werden, dann zeigt sie den laufenden Kontostand nach jeder Transaktion
- [x] Gegeben die erste Zeile, wenn der Monat nicht der erste Monat ist, dann zeigt sie den Übertrag vom Vormonat

## Technische Hinweise

### Neue Seite: src/pages/Cashbook.tsx

```tsx
import { useState, useEffect } from 'react';
import { accountApi } from '../api';
import { useAuth } from '../contexts/AuthContext';
import { TransactionType } from '../types';

interface CashbookEntry {
  id: string;
  date: Date;
  description: string;
  category?: string;
  type: TransactionType;
  income?: number;
  expense?: number;
  runningBalance: number;
}

export function Cashbook() {
  const { user } = useAuth();
  const [entries, setEntries] = useState<CashbookEntry[]>([]);
  const [selectedMonth, setSelectedMonth] = useState(() => {
    const now = new Date();
    return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
  });
  const [isLoading, setIsLoading] = useState(true);

  // ... Implementation
}
```

### Komponente: src/components/CashbookTable.tsx

```tsx
interface CashbookTableProps {
  entries: CashbookEntry[];
  openingBalance: number;
}

export function CashbookTable({ entries, openingBalance }: CashbookTableProps) {
  return (
    <div className="overflow-x-auto">
      <table className="min-w-full">
        <thead className="bg-gray-50 dark:bg-gray-800">
          <tr>
            <th className="px-4 py-3 text-left text-sm font-semibold">Datum</th>
            <th className="px-4 py-3 text-left text-sm font-semibold">Beschreibung</th>
            <th className="px-4 py-3 text-right text-sm font-semibold text-green-600">Einnahmen</th>
            <th className="px-4 py-3 text-right text-sm font-semibold text-red-600">Ausgaben</th>
            <th className="px-4 py-3 text-right text-sm font-semibold">Saldo</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
          {/* Übertrag-Zeile */}
          <tr className="bg-gray-100 dark:bg-gray-700">
            <td className="px-4 py-2 text-sm">01.{month}</td>
            <td className="px-4 py-2 text-sm font-medium">Übertrag</td>
            <td></td>
            <td></td>
            <td className="px-4 py-2 text-sm text-right font-medium">
              {openingBalance.toFixed(2)} €
            </td>
          </tr>
          {/* Transaktionen */}
          {entries.map((entry, index) => (
            <CashbookRow key={entry.id} entry={entry} isEven={index % 2 === 0} />
          ))}
        </tbody>
      </table>
    </div>
  );
}
```

### Routing hinzufügen (App.tsx)

```tsx
<Route path="/cashbook" element={<ProtectedRoute><Cashbook /></ProtectedRoute>} />
```

### Navigation hinzufügen (Layout.tsx für Kind-Rolle)

```tsx
{ to: '/cashbook', label: '📖 Kassenbuch', roles: [UserRole.Child] }
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-01 | Kind öffnet Kassenbuch | Aktueller Monat wird angezeigt |
| TC-W002-02 | Monat hat Transaktionen | Alle Transaktionen werden tabellarisch angezeigt |
| TC-W002-03 | Einnahme-Transaktion | Betrag in grüner Einnahmen-Spalte |
| TC-W002-04 | Ausgabe-Transaktion | Betrag in roter Ausgaben-Spalte |
| TC-W002-05 | Saldo-Berechnung | Jede Zeile zeigt korrekten laufenden Saldo |
| TC-W002-06 | Übertrag-Zeile | Erste Zeile zeigt Anfangssaldo des Monats |
| TC-W002-07 | Leerer Monat | Meldung "Keine Transaktionen in diesem Monat" |

## Design-Hinweise

### Farben
- Einnahmen: `text-green-600 dark:text-green-400`
- Ausgaben: `text-red-600 dark:text-red-400`
- Saldo positiv: `text-gray-900 dark:text-gray-100`
- Saldo negativ: `text-red-600 dark:text-red-400`

### Zebrastreifen
```css
tr:nth-child(even) {
  @apply bg-gray-50 dark:bg-gray-800/50;
}
```

## Story Points

5

## Priorität

Hoch
