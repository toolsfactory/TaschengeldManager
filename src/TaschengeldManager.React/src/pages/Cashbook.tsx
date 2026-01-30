import { useEffect, useState, useMemo, useCallback } from 'react';
import { useSearchParams, useParams, Link } from 'react-router-dom';
import { accountApi, familyApi } from '../api';
import { useAuth } from '../contexts/AuthContext';
import { useMediaQuery } from '../hooks';
import {
  TransactionType,
  UserRole,
  type TransactionDto,
  type AccountDto,
  type CashbookEntry,
  type CashbookSummary,
} from '../types';

// Category icon mapping for expense categories
const categoryIcons: Record<string, string> = {
  'Süßigkeiten': '🍬',
  'Spielzeug': '🎮',
  'Kino': '🎬',
  'Bücher': '📚',
  'Kleidung': '👕',
  'Spiele': '🎲',
  'Essen & Trinken': '🍔',
  'Geschenke': '🎀',
  'Freizeit': '🎪',
  'Sparen': '🏦',
  'Sonstiges': '📦',
};

function getCategoryIcon(category: string | undefined): string {
  if (!category) return '💸';
  return categoryIcons[category] || '📦';
}

// Helper functions
function getTransactionTypeIcon(type: TransactionType): string {
  switch (type) {
    case TransactionType.Deposit:
      return '💵';
    case TransactionType.Withdrawal:
      return '💸';
    case TransactionType.Gift:
      return '🎁';
    case TransactionType.Interest:
      return '📈';
    case TransactionType.Allowance:
      return '🔄';
    case TransactionType.Correction:
      return '✏️';
    default:
      return '💰';
  }
}

function getTransactionTypeLabel(type: TransactionType): string {
  switch (type) {
    case TransactionType.Deposit:
      return 'Einzahlung';
    case TransactionType.Withdrawal:
      return 'Ausgabe';
    case TransactionType.Gift:
      return 'Geschenk';
    case TransactionType.Interest:
      return 'Zinsen';
    case TransactionType.Allowance:
      return 'Taschengeld';
    case TransactionType.Correction:
      return 'Korrektur';
    default:
      return 'Transaktion';
  }
}

function isIncomeTransaction(type: TransactionType, amount: number): boolean {
  // For Correction type, use the sign of the amount to determine income/expense
  if (type === TransactionType.Correction) {
    return amount > 0;
  }
  // For other types, use the type itself
  return (
    type === TransactionType.Deposit ||
    type === TransactionType.Gift ||
    type === TransactionType.Interest ||
    type === TransactionType.Allowance
  );
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('de-DE', {
    day: '2-digit',
    month: '2-digit',
  }).format(date);
}

const monthNames = [
  'Januar', 'Februar', 'März', 'April', 'Mai', 'Juni',
  'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember',
];

// Cashbook Header Component
function CashbookHeader({
  summary,
  childName,
}: {
  summary: CashbookSummary;
  childName?: string;
}) {
  const [year, monthNum] = summary.month.split('-').map(Number);
  const monthName = monthNames[monthNum - 1];

  return (
    <div className="card mb-6">
      <div className="text-center mb-4">
        <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">
          Kassenbuch {childName ? `- ${childName} - ` : ''}{monthName} {year}
        </h2>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {/* Anfangssaldo */}
        <div className="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
          <p className="text-xs text-gray-500 dark:text-gray-400 uppercase tracking-wide">
            Anfangssaldo
          </p>
          <p className="text-xl font-bold text-gray-900 dark:text-gray-100">
            {summary.openingBalance.toFixed(2)} €
          </p>
        </div>

        {/* Einnahmen */}
        <div className="text-center p-3 bg-green-50 dark:bg-green-900/30 rounded-lg">
          <p className="text-xs text-green-600 dark:text-green-400 uppercase tracking-wide">
            Einnahmen
          </p>
          <p className="text-xl font-bold text-green-600 dark:text-green-400">
            +{summary.totalIncome.toFixed(2)} €
          </p>
        </div>

        {/* Ausgaben */}
        <div className="text-center p-3 bg-red-50 dark:bg-red-900/30 rounded-lg">
          <p className="text-xs text-red-600 dark:text-red-400 uppercase tracking-wide">
            Ausgaben
          </p>
          <p className="text-xl font-bold text-red-600 dark:text-red-400">
            -{summary.totalExpenses.toFixed(2)} €
          </p>
        </div>

        {/* Endsaldo */}
        <div
          className={`text-center p-3 rounded-lg ${
            summary.closingBalance >= 0
              ? 'bg-blue-50 dark:bg-blue-900/30'
              : 'bg-red-50 dark:bg-red-900/30'
          }`}
        >
          <p
            className={`text-xs uppercase tracking-wide ${
              summary.closingBalance >= 0
                ? 'text-blue-600 dark:text-blue-400'
                : 'text-red-600 dark:text-red-400'
            }`}
          >
            Endsaldo
          </p>
          <p
            className={`text-xl font-bold ${
              summary.closingBalance >= 0
                ? 'text-blue-600 dark:text-blue-400'
                : 'text-red-600 dark:text-red-400'
            }`}
          >
            {summary.closingBalance.toFixed(2)} €
          </p>
        </div>
      </div>

      {/* Saldo-Differenz */}
      <div className="mt-4 text-center">
        <p className="text-sm text-gray-500 dark:text-gray-400">
          {summary.closingBalance > summary.openingBalance ? (
            <span className="text-green-600 dark:text-green-400">
              📈 {summary.closingBalance - summary.openingBalance > 0 ? `${(summary.closingBalance - summary.openingBalance).toFixed(2)} € mehr` : 'Gleich'} als am Monatsanfang!
            </span>
          ) : summary.closingBalance < summary.openingBalance ? (
            <span className="text-yellow-600 dark:text-yellow-400">
              📉 {(summary.openingBalance - summary.closingBalance).toFixed(2)} € weniger als am Monatsanfang.
            </span>
          ) : (
            <span>➡️ Kontostand ist gleich geblieben.</span>
          )}
        </p>
      </div>
    </div>
  );
}

// Mobile Card Component for small screens
function CashbookMobileCard({ entry }: { entry: CashbookEntry }) {
  const isIncome = entry.income !== undefined && entry.income > 0;

  // Use category icon for withdrawals, transaction type icon for others
  const icon =
    entry.type === TransactionType.Withdrawal
      ? getCategoryIcon(entry.category)
      : getTransactionTypeIcon(entry.type);

  const description = entry.category
    ? `${entry.category}${entry.description ? ` - ${entry.description}` : ''}`
    : entry.description || getTransactionTypeLabel(entry.type);

  return (
    <div className="p-4 border-b border-gray-200 dark:border-gray-700 active:bg-gray-100 dark:active:bg-gray-700 transition-colors">
      <div className="flex justify-between items-start gap-4">
        <div className="flex-1 min-w-0">
          <p className="text-sm text-gray-500 dark:text-gray-400">
            {formatDate(entry.date)}
          </p>
          <p className="font-medium text-gray-900 dark:text-gray-100 flex items-center gap-2">
            <span>{icon}</span>
            <span className="truncate">{description}</span>
          </p>
        </div>
        <div className="text-right flex-shrink-0">
          <p
            className={`text-lg font-bold ${
              isIncome
                ? 'text-green-600 dark:text-green-400'
                : 'text-red-600 dark:text-red-400'
            }`}
          >
            {isIncome ? '+' : '-'}
            {(entry.income || entry.expense || 0).toFixed(2)} €
          </p>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Saldo: {entry.runningBalance.toFixed(2)} €
          </p>
        </div>
      </div>
    </div>
  );
}

// Mobile Card List Component
function CashbookMobileList({
  entries,
  openingBalance,
  month,
  totalIncome,
  totalExpenses,
}: {
  entries: CashbookEntry[];
  openingBalance: number;
  month: string;
  totalIncome: number;
  totalExpenses: number;
}) {
  const [, monthNum] = month.split('-').map(Number);
  const monthStr = String(monthNum).padStart(2, '0');

  return (
    <div className="card p-0 overflow-hidden">
      {/* Übertrag */}
      <div className="p-4 bg-gray-100 dark:bg-gray-700 border-b border-gray-200 dark:border-gray-600">
        <div className="flex justify-between items-center">
          <div>
            <p className="text-sm text-gray-500 dark:text-gray-400">01.{monthStr}</p>
            <p className="font-medium text-gray-900 dark:text-gray-100">Übertrag</p>
          </div>
          <p className="text-lg font-bold text-gray-900 dark:text-gray-100">
            {openingBalance.toFixed(2)} €
          </p>
        </div>
      </div>

      {/* Entries */}
      {entries.map((entry) => (
        <CashbookMobileCard key={entry.id} entry={entry} />
      ))}

      {/* Summen */}
      <div className="p-4 bg-gray-100 dark:bg-gray-700 border-t border-gray-200 dark:border-gray-600">
        <div className="flex justify-between items-center">
          <p className="font-semibold text-gray-900 dark:text-gray-100">Summen</p>
          <div className="text-right">
            <p className="text-green-600 dark:text-green-400 font-semibold">
              +{totalIncome.toFixed(2)} €
            </p>
            <p className="text-red-600 dark:text-red-400 font-semibold">
              -{totalExpenses.toFixed(2)} €
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

// Cashbook Row Component
function CashbookRow({ entry, isEven }: { entry: CashbookEntry; isEven: boolean }) {
  // Use category icon for withdrawals, transaction type icon for others
  const icon =
    entry.type === TransactionType.Withdrawal
      ? getCategoryIcon(entry.category)
      : getTransactionTypeIcon(entry.type);

  const description = entry.category
    ? `${icon} ${entry.category}${entry.description ? ` - ${entry.description}` : ''}`
    : `${icon} ${entry.description || getTransactionTypeLabel(entry.type)}`;

  return (
    <tr
      className={`${
        isEven ? 'bg-gray-50 dark:bg-gray-800/50' : ''
      } hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors`}
    >
      {/* Datum */}
      <td className="px-4 py-3 text-sm text-gray-600 dark:text-gray-400 whitespace-nowrap">
        {formatDate(entry.date)}
      </td>

      {/* Beschreibung */}
      <td className="px-4 py-3 text-sm text-gray-900 dark:text-gray-100">{description}</td>

      {/* Einnahmen */}
      <td className="px-4 py-3 text-sm text-right text-green-600 dark:text-green-400 font-medium whitespace-nowrap">
        {entry.income ? `+${entry.income.toFixed(2)}` : ''}
      </td>

      {/* Ausgaben */}
      <td className="px-4 py-3 text-sm text-right text-red-600 dark:text-red-400 font-medium whitespace-nowrap">
        {entry.expense ? `-${entry.expense.toFixed(2)}` : ''}
      </td>

      {/* Saldo */}
      <td
        className={`px-4 py-3 text-sm text-right font-medium whitespace-nowrap ${
          entry.runningBalance >= 0
            ? 'text-gray-900 dark:text-gray-100'
            : 'text-red-600 dark:text-red-400'
        }`}
      >
        {entry.runningBalance.toFixed(2)} €
      </td>
    </tr>
  );
}

// Cashbook Table Component
function CashbookTable({
  entries,
  openingBalance,
  month,
  totalIncome,
  totalExpenses,
}: {
  entries: CashbookEntry[];
  openingBalance: number;
  month: string;
  totalIncome: number;
  totalExpenses: number;
}) {
  const [, monthNum] = month.split('-').map(Number);
  const monthStr = String(monthNum).padStart(2, '0');

  return (
    <div className="card overflow-hidden">
      <div className="overflow-x-auto -mx-4 sm:mx-0">
        <div className="inline-block min-w-full align-middle">
          <table className="min-w-full cashbook-table">
            <thead className="bg-gray-50 dark:bg-gray-800">
              <tr>
                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-gray-100 whitespace-nowrap">
                  Datum
                </th>
                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900 dark:text-gray-100">
                  Beschreibung
                </th>
                <th className="px-4 py-3 text-right text-sm font-semibold text-green-600 dark:text-green-400 whitespace-nowrap">
                  Einnahmen
                </th>
                <th className="px-4 py-3 text-right text-sm font-semibold text-red-600 dark:text-red-400 whitespace-nowrap">
                  Ausgaben
                </th>
                <th className="px-4 py-3 text-right text-sm font-semibold text-gray-900 dark:text-gray-100 whitespace-nowrap">
                  Saldo
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
              {/* Übertrag-Zeile */}
              <tr className="bg-gray-100 dark:bg-gray-700 uebertrag-row">
                <td className="px-4 py-2 text-sm text-gray-600 dark:text-gray-400 whitespace-nowrap">
                  01.{monthStr}
                </td>
                <td className="px-4 py-2 text-sm font-medium text-gray-900 dark:text-gray-100">
                  Übertrag
                </td>
                <td></td>
                <td></td>
                <td className="px-4 py-2 text-sm text-right font-medium text-gray-900 dark:text-gray-100 whitespace-nowrap">
                  {openingBalance.toFixed(2)} €
                </td>
              </tr>
              {/* Transaktionen */}
              {entries.map((entry, index) => (
                <CashbookRow key={entry.id} entry={entry} isEven={index % 2 === 1} />
              ))}
            </tbody>
            <tfoot>
              <tr className="bg-gray-100 dark:bg-gray-700 font-semibold">
                <td className="px-4 py-3 text-sm"></td>
                <td className="px-4 py-3 text-sm text-gray-900 dark:text-gray-100">Summen</td>
                <td className="px-4 py-3 text-sm text-right text-green-600 dark:text-green-400">
                  +{totalIncome.toFixed(2)} €
                </td>
                <td className="px-4 py-3 text-sm text-right text-red-600 dark:text-red-400">
                  -{totalExpenses.toFixed(2)} €
                </td>
                <td className="px-4 py-3 text-sm text-right"></td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>
    </div>
  );
}

// Month Selector Component
function MonthSelector({
  selectedMonth,
  onMonthChange,
  minMonth,
}: {
  selectedMonth: string;
  onMonthChange: (month: string) => void;
  minMonth?: string;
}) {
  const [year, month] = selectedMonth.split('-').map(Number);

  const currentDate = new Date();
  const currentMonth = `${currentDate.getFullYear()}-${String(currentDate.getMonth() + 1).padStart(2, '0')}`;

  const canGoNext = selectedMonth < currentMonth;
  const canGoPrev = !minMonth || selectedMonth > minMonth;

  const goToPrevious = () => {
    const prevDate = new Date(year, month - 2, 1);
    onMonthChange(
      `${prevDate.getFullYear()}-${String(prevDate.getMonth() + 1).padStart(2, '0')}`
    );
  };

  const goToNext = () => {
    const nextDate = new Date(year, month, 1);
    onMonthChange(
      `${nextDate.getFullYear()}-${String(nextDate.getMonth() + 1).padStart(2, '0')}`
    );
  };

  // Generate month options (last 12 months)
  const generateMonthOptions = () => {
    const options: { value: string; label: string }[] = [];
    const [maxYear, maxMonthNum] = currentMonth.split('-').map(Number);

    for (let i = 0; i < 12; i++) {
      const date = new Date(maxYear, maxMonthNum - 1 - i, 1);
      const value = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;

      if (minMonth && value < minMonth) break;

      options.push({
        value,
        label: `${monthNames[date.getMonth()]} ${date.getFullYear()}`,
      });
    }

    return options;
  };

  return (
    <div className="flex items-center justify-between mb-6">
      <button
        onClick={goToPrevious}
        disabled={!canGoPrev}
        className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        aria-label="Vorheriger Monat"
      >
        <svg
          className="w-6 h-6 text-gray-600 dark:text-gray-400"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
        </svg>
      </button>

      <div className="flex items-center space-x-2">
        <span className="text-xl font-semibold text-gray-900 dark:text-gray-100">
          {monthNames[month - 1]} {year}
        </span>

        <select
          value={selectedMonth}
          onChange={(e) => onMonthChange(e.target.value)}
          className="input w-auto py-1 text-sm"
        >
          {generateMonthOptions().map((m) => (
            <option key={m.value} value={m.value}>
              {m.label}
            </option>
          ))}
        </select>
      </div>

      <button
        onClick={goToNext}
        disabled={!canGoNext}
        className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        aria-label="Nächster Monat"
      >
        <svg
          className="w-6 h-6 text-gray-600 dark:text-gray-400"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
        </svg>
      </button>
    </div>
  );
}

// Cashbook Content Component (reusable for both child and parent views)
function CashbookContent({
  accountId,
  childName,
  isParentView,
}: {
  accountId?: string;
  childName?: string;
  isParentView?: boolean;
}) {
  const { user } = useAuth();
  const [searchParams, setSearchParams] = useSearchParams();
  const [allTransactions, setAllTransactions] = useState<TransactionDto[]>([]);
  const [account, setAccount] = useState<AccountDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const isMobile = useMediaQuery('(max-width: 640px)');

  // Get current month as default
  const getCurrentMonth = () => {
    const now = new Date();
    return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
  };

  // Initialize selected month from URL parameter or current month
  const [selectedMonth, setSelectedMonth] = useState(() => {
    const monthParam = searchParams.get('month');
    if (monthParam && /^\d{4}-\d{2}$/.test(monthParam)) {
      return monthParam;
    }
    return getCurrentMonth();
  });

  // Handle month change and update URL
  const handleMonthChange = useCallback(
    (month: string) => {
      setSelectedMonth(month);
      setSearchParams({ month });
    },
    [setSearchParams]
  );

  // Sync URL parameter changes to state
  useEffect(() => {
    const monthParam = searchParams.get('month');
    if (monthParam && /^\d{4}-\d{2}$/.test(monthParam) && monthParam !== selectedMonth) {
      setSelectedMonth(monthParam);
    }
  }, [searchParams, selectedMonth]);

  // Load transactions
  useEffect(() => {
    const loadData = async () => {
      setIsLoading(true);
      try {
        let accountData: AccountDto;
        let txData: TransactionDto[];

        if (accountId) {
          // Parent viewing a child's account
          accountData = await accountApi.getAccount(accountId);
          txData = await accountApi.getTransactions(accountId, 1, 500);
        } else {
          // Child viewing their own account
          accountData = await accountApi.getMyAccount();
          txData = await accountApi.getMyTransactions(1, 500);
        }

        setAccount(accountData);
        setAllTransactions(txData);
      } catch (error) {
        console.error('Failed to load cashbook data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, [accountId]);

  // Filter and process transactions for selected month
  const { entries, summary } = useMemo(() => {
    const [year, month] = selectedMonth.split('-').map(Number);
    const monthStart = new Date(year, month - 1, 1);
    const monthEnd = new Date(year, month, 0, 23, 59, 59, 999);

    // Filter transactions for the selected month
    const monthTransactions = allTransactions.filter((tx) => {
      const txDate = new Date(tx.createdAt);
      return txDate >= monthStart && txDate <= monthEnd;
    });

    // Sort by date ascending (oldest first) - use localeCompare for reliable string comparison
    monthTransactions.sort((a, b) => {
      const dateA = new Date(a.createdAt).getTime();
      const dateB = new Date(b.createdAt).getTime();
      if (dateA !== dateB) return dateA - dateB;
      // If same timestamp, sort by ID for consistency
      return a.id.localeCompare(b.id);
    });

    // Calculate opening balance from transactions BEFORE this month
    let openingBalance = 0;
    const transactionsBeforeMonth = allTransactions.filter((tx) => {
      const txDate = new Date(tx.createdAt);
      return txDate < monthStart;
    });

    if (transactionsBeforeMonth.length > 0) {
      // Sort to find the most recent transaction before this month
      transactionsBeforeMonth.sort((a, b) => {
        const dateA = new Date(a.createdAt).getTime();
        const dateB = new Date(b.createdAt).getTime();
        return dateB - dateA; // Descending to get newest first
      });
      // The most recent transaction before this month gives us the opening balance
      openingBalance = transactionsBeforeMonth[0].balanceAfter;
    } else if (monthTransactions.length > 0) {
      // No transactions before this month - calculate from first transaction of month
      const firstTx = monthTransactions[0];
      openingBalance = firstTx.balanceAfter - firstTx.amount;
    } else {
      // No transactions at all - use current account balance or 0
      openingBalance = account?.balance ?? 0;
    }

    // Convert to CashbookEntry format and calculate running balance
    let runningBalance = openingBalance;
    const cashbookEntries: CashbookEntry[] = monthTransactions.map((tx) => {
      const isIncome = isIncomeTransaction(tx.type, tx.amount);
      const amount = Math.abs(tx.amount);

      // Calculate running balance: add for income, subtract for expense
      if (isIncome) {
        runningBalance += amount;
      } else {
        runningBalance -= amount;
      }

      return {
        id: tx.id,
        date: new Date(tx.createdAt),
        description: tx.description || '',
        category: tx.category || undefined,
        type: tx.type,
        income: isIncome ? amount : undefined,
        expense: !isIncome ? amount : undefined,
        runningBalance: runningBalance,
      };
    });

    // Calculate totals
    const totalIncome = cashbookEntries.reduce((sum, e) => sum + (e.income || 0), 0);
    const totalExpenses = cashbookEntries.reduce((sum, e) => sum + (e.expense || 0), 0);
    const closingBalance = openingBalance + totalIncome - totalExpenses;

    const cashbookSummary: CashbookSummary = {
      month: selectedMonth,
      openingBalance,
      totalIncome,
      totalExpenses,
      closingBalance,
    };

    return { entries: cashbookEntries, summary: cashbookSummary };
  }, [allTransactions, selectedMonth, account]);

  // Calculate min month (oldest transaction)
  const minMonth = useMemo(() => {
    if (allTransactions.length === 0) return undefined;

    const oldestDate = allTransactions.reduce((oldest, tx) => {
      const txDate = new Date(tx.createdAt);
      return txDate < oldest ? txDate : oldest;
    }, new Date());

    return `${oldestDate.getFullYear()}-${String(oldestDate.getMonth() + 1).padStart(2, '0')}`;
  }, [allTransactions]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
      </div>
    );
  }

  return (
    <>
      {/* Header with different text for parent/child */}
      {!isParentView && (
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Mein Kassenbuch</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            Hallo {user?.nickname}! Hier siehst du deine Einnahmen und Ausgaben.
          </p>
        </div>
      )}

      {/* Month Navigation */}
      <MonthSelector
        selectedMonth={selectedMonth}
        onMonthChange={handleMonthChange}
        minMonth={minMonth}
      />

      {/* Summary Header */}
      <CashbookHeader summary={summary} childName={childName} />

      {/* Cashbook Table or Mobile Cards */}
      {entries.length > 0 ? (
        isMobile ? (
          <CashbookMobileList
            entries={entries}
            openingBalance={summary.openingBalance}
            month={selectedMonth}
            totalIncome={summary.totalIncome}
            totalExpenses={summary.totalExpenses}
          />
        ) : (
          <CashbookTable
            entries={entries}
            openingBalance={summary.openingBalance}
            month={selectedMonth}
            totalIncome={summary.totalIncome}
            totalExpenses={summary.totalExpenses}
          />
        )
      ) : (
        <div className="card text-center py-12">
          <span className="text-6xl">📋</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">
            Keine Transaktionen
          </h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            In diesem Monat gibt es noch keine Einnahmen oder Ausgaben.
          </p>
        </div>
      )}
    </>
  );
}

// Main Cashbook Component
export function Cashbook() {
  const { accountId } = useParams<{ accountId?: string }>();
  const { user } = useAuth();
  const [children, setChildren] = useState<AccountDto[]>([]);
  const [selectedChildId, setSelectedChildId] = useState<string | null>(accountId || null);
  const [isLoadingChildren, setIsLoadingChildren] = useState(false);

  const isParent = user?.role === UserRole.Parent;
  const isParentView = isParent && !accountId;

  // Load children accounts for parent
  useEffect(() => {
    const loadFamilyAccounts = async () => {
      if (!isParent) return;

      setIsLoadingChildren(true);
      try {
        const families = await familyApi.getMyFamilies();
        if (families.length > 0) {
          const accounts = await accountApi.getFamilyAccounts(families[0].id);
          setChildren(accounts);
          if (!selectedChildId && accounts.length > 0) {
            setSelectedChildId(accounts[0].id);
          }
        }
      } catch (error) {
        console.error('Failed to load family accounts:', error);
      } finally {
        setIsLoadingChildren(false);
      }
    };

    loadFamilyAccounts();
  }, [isParent, selectedChildId]);

  // If parent is viewing via direct URL with accountId
  if (accountId && isParent) {
    const childAccount = children.find((c) => c.id === accountId);
    return (
      <div className="space-y-6">
        <div>
          <Link
            to="/cashbooks"
            className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 text-sm"
          >
            ← Zurück zur Übersicht
          </Link>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">
            Kassenbuch - {childAccount?.ownerName || 'Kind'}
          </h1>
        </div>
        <CashbookContent
          accountId={accountId}
          childName={childAccount?.ownerName}
          isParentView={true}
        />
      </div>
    );
  }

  // Parent view with child selector
  if (isParentView) {
    if (isLoadingChildren) {
      return (
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
        </div>
      );
    }

    if (children.length === 0) {
      return (
        <div className="space-y-6">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Kassenbücher</h1>
          <div className="card text-center py-12">
            <span className="text-6xl">👶</span>
            <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">
              Keine Kinderkonten vorhanden
            </h2>
            <p className="text-gray-600 dark:text-gray-400 mt-2">
              Fügen Sie zuerst ein Kind zu Ihrer Familie hinzu.
            </p>
            <Link
              to="/family/children/add"
              className="btn-primary mt-4 inline-block"
            >
              Kind hinzufügen
            </Link>
          </div>
        </div>
      );
    }

    const selectedChild = children.find((c) => c.id === selectedChildId);

    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Kassenbücher</h1>

        {/* Child Selector */}
        <div className="card">
          <label className="label">Kind auswählen</label>
          <select
            value={selectedChildId || ''}
            onChange={(e) => setSelectedChildId(e.target.value)}
            className="input"
          >
            {children.map((child) => (
              <option key={child.id} value={child.id}>
                {child.ownerName} ({child.balance.toFixed(2)} €)
              </option>
            ))}
          </select>
        </div>

        {/* Cashbook Content */}
        {selectedChildId && (
          <CashbookContent
            accountId={selectedChildId}
            childName={selectedChild?.ownerName}
            isParentView={true}
          />
        )}
      </div>
    );
  }

  // Child view (default)
  return (
    <div className="space-y-6">
      <CashbookContent />
    </div>
  );
}
