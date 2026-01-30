# Story W002-S04: Kassenbuch-Einträge mit Kategorien

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Kind** möchte ich **bei jedem Eintrag die Kategorie mit einem Icon sehen**, damit **ich schnell erkennen kann, wofür ich Geld ausgegeben oder bekommen habe**.

## Akzeptanzkriterien

- [x] Gegeben ein Kassenbuch-Eintrag, wenn er eine Kategorie hat, dann wird ein passendes Emoji/Icon angezeigt
- [x] Gegeben verschiedene Transaktionstypen, wenn sie angezeigt werden, dann haben sie unterschiedliche Icons (Taschengeld, Geschenk, Zinsen, Ausgabe)
- [x] Gegeben eine Ausgabe mit Kategorie, wenn sie angezeigt wird, dann steht die Kategorie in der Beschreibung mit Icon
- [x] Gegeben die Summenzeile am Ende, wenn sie angezeigt wird, dann zeigt sie die Gesamtsummen für Einnahmen und Ausgaben
- [x] Gegeben die Tabelle, wenn sie gerendert wird, dann sind die Zeilen abwechselnd eingefärbt (Zebrastreifen)

## Technische Hinweise

### Icon-Mapping für Transaktionstypen

```typescript
const transactionTypeIcons: Record<TransactionType, string> = {
  [TransactionType.Deposit]: '💵',      // Einzahlung
  [TransactionType.Withdrawal]: '💸',   // Ausgabe
  [TransactionType.Gift]: '🎁',         // Geschenk
  [TransactionType.Interest]: '📈',     // Zinsen
  [TransactionType.Allowance]: '🔄',    // Taschengeld
  [TransactionType.Correction]: '✏️',   // Korrektur
};
```

### Icon-Mapping für Kategorien (Ausgaben)

```typescript
const categoryIcons: Record<string, string> = {
  'Süßigkeiten': '🍬',
  'Spielzeug': '🎮',
  'Kleidung': '👕',
  'Bücher': '📚',
  'Essen': '🍔',
  'Freizeit': '🎪',
  'Sparen': '🏦',
  'Sonstiges': '📦',
};

function getCategoryIcon(category: string | undefined): string {
  if (!category) return '📦';
  return categoryIcons[category] || '📦';
}
```

### Komponente: src/components/CashbookRow.tsx

```tsx
interface CashbookRowProps {
  entry: CashbookEntry;
  isEven: boolean;
}

export function CashbookRow({ entry, isEven }: CashbookRowProps) {
  const icon = entry.type === TransactionType.Withdrawal
    ? getCategoryIcon(entry.category)
    : transactionTypeIcons[entry.type];

  const description = entry.category
    ? `${icon} ${entry.category}${entry.description ? ` - ${entry.description}` : ''}`
    : `${icon} ${entry.description || getTransactionTypeLabel(entry.type)}`;

  return (
    <tr className={`${isEven ? 'bg-gray-50 dark:bg-gray-800/50' : ''} hover:bg-gray-100 dark:hover:bg-gray-700`}>
      {/* Datum */}
      <td className="px-4 py-3 text-sm text-gray-600 dark:text-gray-400 whitespace-nowrap">
        {formatDate(entry.date)}
      </td>

      {/* Beschreibung */}
      <td className="px-4 py-3 text-sm text-gray-900 dark:text-gray-100">
        {description}
      </td>

      {/* Einnahmen */}
      <td className="px-4 py-3 text-sm text-right text-green-600 dark:text-green-400 font-medium">
        {entry.income ? `+${entry.income.toFixed(2)}` : ''}
      </td>

      {/* Ausgaben */}
      <td className="px-4 py-3 text-sm text-right text-red-600 dark:text-red-400 font-medium">
        {entry.expense ? `-${entry.expense.toFixed(2)}` : ''}
      </td>

      {/* Saldo */}
      <td className={`px-4 py-3 text-sm text-right font-medium ${
        entry.runningBalance >= 0
          ? 'text-gray-900 dark:text-gray-100'
          : 'text-red-600 dark:text-red-400'
      }`}>
        {entry.runningBalance.toFixed(2)} €
      </td>
    </tr>
  );
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('de-DE', {
    day: '2-digit',
    month: '2-digit',
  }).format(date);
}

function getTransactionTypeLabel(type: TransactionType): string {
  const labels: Record<TransactionType, string> = {
    [TransactionType.Deposit]: 'Einzahlung',
    [TransactionType.Withdrawal]: 'Ausgabe',
    [TransactionType.Gift]: 'Geschenk',
    [TransactionType.Interest]: 'Zinsen',
    [TransactionType.Allowance]: 'Taschengeld',
    [TransactionType.Correction]: 'Korrektur',
  };
  return labels[type] || 'Transaktion';
}
```

### Summenzeile am Ende

```tsx
// In CashbookTable.tsx - nach den Einträgen
<tfoot>
  <tr className="bg-gray-100 dark:bg-gray-700 font-semibold">
    <td className="px-4 py-3 text-sm"></td>
    <td className="px-4 py-3 text-sm text-gray-900 dark:text-gray-100">
      Summen
    </td>
    <td className="px-4 py-3 text-sm text-right text-green-600 dark:text-green-400">
      +{totalIncome.toFixed(2)} €
    </td>
    <td className="px-4 py-3 text-sm text-right text-red-600 dark:text-red-400">
      -{totalExpenses.toFixed(2)} €
    </td>
    <td className="px-4 py-3 text-sm text-right"></td>
  </tr>
</tfoot>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-30 | Taschengeld-Transaktion | Icon 🔄 und "Taschengeld" angezeigt |
| TC-W002-31 | Geschenk-Transaktion | Icon 🎁 und Beschreibung angezeigt |
| TC-W002-32 | Ausgabe mit Kategorie "Süßigkeiten" | Icon 🍬 in Beschreibung |
| TC-W002-33 | Ausgabe mit Kategorie "Spielzeug" | Icon 🎮 in Beschreibung |
| TC-W002-34 | Ausgabe ohne Kategorie | Standard-Icon 📦 |
| TC-W002-35 | Zebrastreifen | Jede zweite Zeile hat andere Hintergrundfarbe |
| TC-W002-36 | Summenzeile | Zeigt korrekte Summen für Einnahmen und Ausgaben |
| TC-W002-37 | Hover auf Zeile | Hintergrundfarbe ändert sich |

## Story Points

3

## Priorität

Mittel
