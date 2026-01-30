# Story W002-S03: Monatszusammenfassung (Kopfbereich)

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Kind** möchte ich **eine Zusammenfassung meines Monats sehen**, damit **ich auf einen Blick erkenne, wie viel ich eingenommen und ausgegeben habe**.

## Akzeptanzkriterien

- [x] Gegeben die Kassenbuch-Ansicht, wenn sie geladen wird, dann wird ein Zusammenfassungs-Bereich oberhalb der Tabelle angezeigt
- [x] Gegeben die Zusammenfassung, wenn sie angezeigt wird, dann enthält sie: Anfangssaldo, Summe Einnahmen, Summe Ausgaben, Endsaldo
- [x] Gegeben positive Einnahmen, wenn sie angezeigt werden, dann sind sie grün dargestellt mit Plus-Zeichen
- [x] Gegeben Ausgaben, wenn sie angezeigt werden, dann sind sie rot dargestellt mit Minus-Zeichen
- [x] Gegeben der Endsaldo, wenn er positiv ist, dann ist er in Standardfarbe; wenn negativ, dann in Rot

## Technische Hinweise

### Komponente: src/components/CashbookHeader.tsx

```tsx
interface CashbookHeaderProps {
  month: string;
  openingBalance: number;
  totalIncome: number;
  totalExpenses: number;
  closingBalance: number;
}

export function CashbookHeader({
  month,
  openingBalance,
  totalIncome,
  totalExpenses,
  closingBalance
}: CashbookHeaderProps) {
  const monthNames = [
    'Januar', 'Februar', 'März', 'April', 'Mai', 'Juni',
    'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'
  ];

  const [year, monthNum] = month.split('-').map(Number);
  const monthName = monthNames[monthNum - 1];

  return (
    <div className="card mb-6">
      <div className="text-center mb-4">
        <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
          Kassenbuch {monthName} {year}
        </h2>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {/* Anfangssaldo */}
        <div className="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
          <p className="text-xs text-gray-500 dark:text-gray-400 uppercase tracking-wide">
            Anfangssaldo
          </p>
          <p className="text-xl font-bold text-gray-900 dark:text-gray-100">
            {openingBalance.toFixed(2)} €
          </p>
        </div>

        {/* Einnahmen */}
        <div className="text-center p-3 bg-green-50 dark:bg-green-900/30 rounded-lg">
          <p className="text-xs text-green-600 dark:text-green-400 uppercase tracking-wide">
            Einnahmen
          </p>
          <p className="text-xl font-bold text-green-600 dark:text-green-400">
            +{totalIncome.toFixed(2)} €
          </p>
        </div>

        {/* Ausgaben */}
        <div className="text-center p-3 bg-red-50 dark:bg-red-900/30 rounded-lg">
          <p className="text-xs text-red-600 dark:text-red-400 uppercase tracking-wide">
            Ausgaben
          </p>
          <p className="text-xl font-bold text-red-600 dark:text-red-400">
            -{totalExpenses.toFixed(2)} €
          </p>
        </div>

        {/* Endsaldo */}
        <div className={`text-center p-3 rounded-lg ${
          closingBalance >= 0
            ? 'bg-blue-50 dark:bg-blue-900/30'
            : 'bg-red-50 dark:bg-red-900/30'
        }`}>
          <p className={`text-xs uppercase tracking-wide ${
            closingBalance >= 0
              ? 'text-blue-600 dark:text-blue-400'
              : 'text-red-600 dark:text-red-400'
          }`}>
            Endsaldo
          </p>
          <p className={`text-xl font-bold ${
            closingBalance >= 0
              ? 'text-blue-600 dark:text-blue-400'
              : 'text-red-600 dark:text-red-400'
          }`}>
            {closingBalance.toFixed(2)} €
          </p>
        </div>
      </div>

      {/* Saldo-Differenz */}
      <div className="mt-4 text-center">
        <p className="text-sm text-gray-500 dark:text-gray-400">
          {closingBalance > openingBalance ? (
            <span className="text-green-600 dark:text-green-400">
              📈 Du hast {(closingBalance - openingBalance).toFixed(2)} € mehr als am Monatsanfang!
            </span>
          ) : closingBalance < openingBalance ? (
            <span className="text-yellow-600 dark:text-yellow-400">
              📉 Du hast {(openingBalance - closingBalance).toFixed(2)} € weniger als am Monatsanfang.
            </span>
          ) : (
            <span>
              ➡️ Dein Kontostand ist gleich geblieben.
            </span>
          )}
        </p>
      </div>
    </div>
  );
}
```

### Berechnung der Werte

```typescript
function calculateMonthSummary(
  transactions: TransactionDto[],
  previousMonthEndBalance: number
): CashbookHeaderProps {
  const totalIncome = transactions
    .filter(t => t.amount > 0)
    .reduce((sum, t) => sum + t.amount, 0);

  const totalExpenses = Math.abs(
    transactions
      .filter(t => t.amount < 0)
      .reduce((sum, t) => sum + t.amount, 0)
  );

  const closingBalance = previousMonthEndBalance + totalIncome - totalExpenses;

  return {
    openingBalance: previousMonthEndBalance,
    totalIncome,
    totalExpenses,
    closingBalance
  };
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-20 | Monat mit Transaktionen | Alle vier Werte werden korrekt angezeigt |
| TC-W002-21 | Anfangssaldo 45€, Einnahmen 55€, Ausgaben 32€ | Endsaldo zeigt 68€ |
| TC-W002-22 | Mehr Einnahmen als Ausgaben | Positive Nachricht wird angezeigt |
| TC-W002-23 | Mehr Ausgaben als Einnahmen | Gelbe Warnung wird angezeigt |
| TC-W002-24 | Negativer Endsaldo | Endsaldo in Rot dargestellt |
| TC-W002-25 | Leerer Monat | Einnahmen und Ausgaben zeigen 0,00€ |

## Story Points

3

## Priorität

Hoch
