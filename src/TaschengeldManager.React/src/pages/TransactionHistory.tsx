import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { accountApi } from '../api';
import { useAuth } from '../contexts/AuthContext';
import { UserRole, TransactionType, type TransactionDto, type AccountDto } from '../types';

export function TransactionHistory() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [account, setAccount] = useState<AccountDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [filterType, setFilterType] = useState<TransactionType | 'all'>('all');
  const pageSize = 20;

  const isChildView = user?.role === UserRole.Child && !id;

  useEffect(() => {
    const loadData = async () => {
      setIsLoading(true);
      try {
        let txData: TransactionDto[];

        if (isChildView) {
          // Child viewing their own transactions
          txData = await accountApi.getMyTransactions(1, pageSize);
          const accountData = await accountApi.getMyAccount();
          setAccount(accountData);
        } else if (id) {
          // Parent/Relative viewing specific account
          txData = await accountApi.getTransactions(id, 1, pageSize);
          const accountData = await accountApi.getAccount(id);
          setAccount(accountData);
        } else {
          txData = [];
        }

        setTransactions(txData);
        setHasMore(txData.length === pageSize);
        setPage(1);
      } catch (error) {
        console.error('Failed to load transactions:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, [id, isChildView]);

  const loadMore = async () => {
    const nextPage = page + 1;
    try {
      let txData: TransactionDto[];

      if (isChildView) {
        txData = await accountApi.getMyTransactions(nextPage, pageSize);
      } else if (id) {
        txData = await accountApi.getTransactions(id, nextPage, pageSize);
      } else {
        return;
      }

      setTransactions((prev) => [...prev, ...txData]);
      setHasMore(txData.length === pageSize);
      setPage(nextPage);
    } catch (error) {
      console.error('Failed to load more transactions:', error);
    }
  };

  const filteredTransactions =
    filterType === 'all'
      ? transactions
      : transactions.filter((tx) => tx.type === filterType);

  const getIcon = (type: TransactionType) => {
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
  };

  const getLabel = (type: TransactionType) => {
    switch (type) {
      case TransactionType.Deposit:
        return 'Einzahlung';
      case TransactionType.Withdrawal:
        return 'Abhebung';
      case TransactionType.Gift:
        return 'Geschenk';
      case TransactionType.Interest:
        return 'Zinsen';
      case TransactionType.Allowance:
        return 'Taschengeld';
      case TransactionType.Correction:
        return 'Korrektur';
      default:
        return type;
    }
  };

  const isPositive = (type: TransactionType) =>
    type === TransactionType.Deposit ||
    type === TransactionType.Gift ||
    type === TransactionType.Interest ||
    type === TransactionType.Allowance;

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          {!isChildView && id && (
            <Link to={`/accounts/${id}`} className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 text-sm">
              ← Zurück zum Konto
            </Link>
          )}
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">
            {isChildView ? 'Meine Transaktionen' : `Transaktionen - ${account?.ownerName}`}
          </h1>
        </div>
      </div>

      {/* Current Balance */}
      {account && (
        <div className="card bg-gradient-to-r from-blue-500 to-blue-600 dark:from-blue-600 dark:to-blue-700 text-white">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-blue-100">Aktuelles Guthaben</p>
              <p className="text-3xl font-bold mt-1">{account.balance.toFixed(2)} €</p>
            </div>
            <div className="text-5xl opacity-50">💰</div>
          </div>
        </div>
      )}

      {/* Filter */}
      <div className="card">
        <div className="flex flex-wrap gap-2">
          <button
            onClick={() => setFilterType('all')}
            className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
              filterType === 'all'
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
            }`}
          >
            Alle
          </button>
          {Object.values(TransactionType)
            .filter((v): v is TransactionType => typeof v === 'number')
            .map((type) => (
            <button
              key={type}
              onClick={() => setFilterType(type)}
              className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
                filterType === type
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
              }`}
            >
              {getIcon(type)} {getLabel(type)}
            </button>
          ))}
        </div>
      </div>

      {/* Transactions List */}
      <div className="card">
        {filteredTransactions.length === 0 ? (
          <p className="text-gray-500 dark:text-gray-400 text-center py-8">
            {filterType === 'all'
              ? 'Noch keine Transaktionen vorhanden.'
              : `Keine ${getLabel(filterType as TransactionType)} vorhanden.`}
          </p>
        ) : (
          <div className="space-y-3">
            {filteredTransactions.map((tx) => (
              <div
                key={tx.id}
                className="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg"
              >
                <div className="flex items-center space-x-4">
                  <div
                    className={`w-12 h-12 rounded-full flex items-center justify-center ${
                      isPositive(tx.type) ? 'bg-green-100 dark:bg-green-900/30' : 'bg-red-100 dark:bg-red-900/30'
                    }`}
                  >
                    <span className="text-2xl">{getIcon(tx.type)}</span>
                  </div>
                  <div>
                    <p className="font-medium text-gray-900 dark:text-gray-100">{getLabel(tx.type)}</p>
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                      {new Date(tx.createdAt).toLocaleDateString('de-DE', {
                        weekday: 'short',
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric',
                        hour: '2-digit',
                        minute: '2-digit',
                      })}
                    </p>
                    {tx.description && (
                      <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">{tx.description}</p>
                    )}
                    {tx.createdByUserName && (
                      <p className="text-xs text-gray-400 dark:text-gray-500">von {tx.createdByUserName}</p>
                    )}
                  </div>
                </div>
                <div className="text-right">
                  <p
                    className={`text-lg font-semibold ${
                      isPositive(tx.type) ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'
                    }`}
                  >
                    {isPositive(tx.type) ? '+' : ''}
                    {Math.abs(tx.amount).toFixed(2)} €
                  </p>
                  <p className="text-xs text-gray-400 dark:text-gray-500">
                    Saldo: {tx.balanceAfter.toFixed(2)} €
                  </p>
                </div>
              </div>
            ))}
          </div>
        )}

        {hasMore && filteredTransactions.length > 0 && (
          <button
            onClick={loadMore}
            className="w-full mt-4 py-3 text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium"
          >
            Mehr laden...
          </button>
        )}
      </div>
    </div>
  );
}
