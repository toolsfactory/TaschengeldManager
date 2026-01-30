import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { accountApi } from '../api';
import { useAuth } from '../contexts/AuthContext';
import { UserRole, TransactionType, type AccountDto, type TransactionDto } from '../types';

export function AccountDetails() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const [account, setAccount] = useState<AccountDto | null>(null);
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showDepositModal, setShowDepositModal] = useState(false);
  const [showGiftModal, setShowGiftModal] = useState(false);
  const [showInterestModal, setShowInterestModal] = useState(false);

  useEffect(() => {
    const loadData = async () => {
      if (!id) return;

      try {
        const accountData = await accountApi.getAccount(id);
        setAccount(accountData);

        const txData = await accountApi.getTransactions(id, 1, 10);
        setTransactions(txData);
      } catch (error) {
        console.error('Failed to load account:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, [id]);

  const refreshData = async () => {
    if (!id) return;
    const accountData = await accountApi.getAccount(id);
    setAccount(accountData);
    const txData = await accountApi.getTransactions(id, 1, 10);
    setTransactions(txData);
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
      </div>
    );
  }

  if (!account) {
    return (
      <div className="card text-center py-12">
        <span className="text-6xl">❌</span>
        <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Konto nicht gefunden</h2>
        <Link to="/accounts" className="btn-primary mt-4 inline-block">
          Zurück zur Übersicht
        </Link>
      </div>
    );
  }

  const isParent = user?.role === UserRole.Parent;
  const isRelative = user?.role === UserRole.Relative;

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <Link to="/accounts" className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 text-sm">
            ← Zurück zur Übersicht
          </Link>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">{account.ownerName}</h1>
        </div>
      </div>

      {/* Balance Card */}
      <div className="card bg-gradient-to-r from-blue-500 to-blue-600 dark:from-blue-600 dark:to-blue-700 text-white">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-blue-100">Aktuelles Guthaben</p>
            <p className="text-4xl font-bold mt-2">{account.balance.toFixed(2)} €</p>
            {account.interestEnabled && (
              <p className="text-blue-100 text-sm mt-1">
                📈 {account.interestRate}% Zinsen aktiv
              </p>
            )}
          </div>
          <div className="text-6xl opacity-50">💰</div>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {isParent && (
          <>
            <button
              onClick={() => setShowDepositModal(true)}
              className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow text-center"
            >
              <span className="text-3xl">💵</span>
              <p className="font-medium text-gray-900 dark:text-gray-100 mt-2">Einzahlen</p>
            </button>
            <button
              onClick={() => setShowInterestModal(true)}
              className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow text-center"
            >
              <span className="text-3xl">📈</span>
              <p className="font-medium text-gray-900 dark:text-gray-100 mt-2">Zinsen</p>
            </button>
          </>
        )}
        {(isParent || isRelative) && (
          <button
            onClick={() => setShowGiftModal(true)}
            className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow text-center"
          >
            <span className="text-3xl">🎁</span>
            <p className="font-medium text-gray-900 dark:text-gray-100 mt-2">Geschenk</p>
          </button>
        )}
        <Link
          to={`/accounts/${id}/history`}
          className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow text-center"
        >
          <span className="text-3xl">📋</span>
          <p className="font-medium text-gray-900 dark:text-gray-100 mt-2">Verlauf</p>
        </Link>
        <Link
          to={`/accounts/${id}/cashbook`}
          className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow text-center"
        >
          <span className="text-3xl">📖</span>
          <p className="font-medium text-gray-900 dark:text-gray-100 mt-2">Kassenbuch</p>
        </Link>
      </div>

      {/* Recent Transactions */}
      <div className="card">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">Letzte Transaktionen</h2>
          <Link
            to={`/accounts/${id}/history`}
            className="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
          >
            Alle anzeigen →
          </Link>
        </div>
        {transactions.length === 0 ? (
          <p className="text-gray-500 dark:text-gray-400 text-center py-4">Noch keine Transaktionen.</p>
        ) : (
          <div className="space-y-3">
            {transactions.map((tx) => (
              <TransactionItem key={tx.id} transaction={tx} />
            ))}
          </div>
        )}
      </div>

      {/* Modals */}
      {showDepositModal && (
        <DepositModal
          accountId={account.id}
          onClose={() => setShowDepositModal(false)}
          onSuccess={refreshData}
        />
      )}
      {showGiftModal && (
        <GiftModal
          accountId={account.id}
          childName={account.ownerName}
          onClose={() => setShowGiftModal(false)}
          onSuccess={refreshData}
        />
      )}
      {showInterestModal && (
        <InterestModal
          account={account}
          onClose={() => setShowInterestModal(false)}
          onSuccess={refreshData}
        />
      )}
    </div>
  );
}

function TransactionItem({ transaction }: { transaction: TransactionDto }) {
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

  const isPositive =
    transaction.type === TransactionType.Deposit ||
    transaction.type === TransactionType.Gift ||
    transaction.type === TransactionType.Interest ||
    transaction.type === TransactionType.Allowance;

  return (
    <div className="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
      <div className="flex items-center space-x-3">
        <span className="text-2xl">{getIcon(transaction.type)}</span>
        <div>
          <p className="font-medium text-gray-900 dark:text-gray-100">{getLabel(transaction.type)}</p>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            {new Date(transaction.createdAt).toLocaleDateString('de-DE', {
              day: '2-digit',
              month: '2-digit',
              year: 'numeric',
              hour: '2-digit',
              minute: '2-digit',
            })}
          </p>
          {transaction.description && (
            <p className="text-sm text-gray-500 dark:text-gray-400">{transaction.description}</p>
          )}
        </div>
      </div>
      <span
        className={`font-semibold ${isPositive ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'}`}
      >
        {isPositive ? '+' : ''}{Math.abs(transaction.amount).toFixed(2)} €
      </span>
    </div>
  );
}

function DepositModal({
  accountId,
  onClose,
  onSuccess,
}: {
  accountId: string;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await accountApi.deposit(accountId, {
        amount: parseFloat(amount),
        description: description || undefined,
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError('Einzahlung fehlgeschlagen');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal title="Einzahlung" onClose={onClose}>
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && <ErrorMessage message={error} />}
        <div>
          <label className="label">Betrag (€)</label>
          <input
            type="number"
            step="0.01"
            min="0.01"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            className="input"
            placeholder="10.00"
            required
          />
        </div>
        <div>
          <label className="label">Beschreibung (optional)</label>
          <input
            type="text"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="input"
            placeholder="z.B. Taschengeld"
          />
        </div>
        <div className="flex space-x-3">
          <button type="button" onClick={onClose} className="btn-secondary flex-1">
            Abbrechen
          </button>
          <button type="submit" disabled={isLoading} className="btn-primary flex-1">
            {isLoading ? 'Wird eingezahlt...' : 'Einzahlen'}
          </button>
        </div>
      </form>
    </Modal>
  );
}

function GiftModal({
  accountId,
  childName,
  onClose,
  onSuccess,
}: {
  accountId: string;
  childName: string;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [amount, setAmount] = useState('');
  const [message, setMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await accountApi.gift(accountId, {
        amount: parseFloat(amount),
        message: message || undefined,
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError('Geschenk fehlgeschlagen');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal title={`Geschenk für ${childName}`} onClose={onClose}>
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && <ErrorMessage message={error} />}
        <div>
          <label className="label">Betrag (€)</label>
          <input
            type="number"
            step="0.01"
            min="0.01"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            className="input"
            placeholder="20.00"
            required
          />
        </div>
        <div>
          <label className="label">Nachricht (optional)</label>
          <input
            type="text"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            className="input"
            placeholder="z.B. Zum Geburtstag!"
          />
        </div>
        <div className="flex space-x-3">
          <button type="button" onClick={onClose} className="btn-secondary flex-1">
            Abbrechen
          </button>
          <button type="submit" disabled={isLoading} className="btn-success flex-1">
            {isLoading ? 'Wird geschenkt...' : '🎁 Schenken'}
          </button>
        </div>
      </form>
    </Modal>
  );
}

function InterestModal({
  account,
  onClose,
  onSuccess,
}: {
  account: AccountDto;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [interestRate, setInterestRate] = useState(account.interestRate?.toString() || '');
  const [interestEnabled, setInterestEnabled] = useState(account.interestEnabled);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await accountApi.setInterest(account.id, {
        interestRate: parseFloat(interestRate) || 0,
        interestEnabled,
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError('Zinsen konnten nicht aktualisiert werden');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Modal title="Zinsen verwalten" onClose={onClose}>
      <form onSubmit={handleSubmit} className="space-y-4">
        {error && <ErrorMessage message={error} />}
        <div className="flex items-center space-x-3">
          <input
            type="checkbox"
            id="interestEnabled"
            checked={interestEnabled}
            onChange={(e) => setInterestEnabled(e.target.checked)}
            className="w-5 h-5 rounded border-gray-300 dark:border-gray-600 text-blue-600 focus:ring-blue-500 dark:bg-gray-700"
          />
          <label htmlFor="interestEnabled" className="font-medium text-gray-900 dark:text-gray-100">
            Zinsen aktivieren
          </label>
        </div>
        {interestEnabled && (
          <div>
            <label className="label">Zinssatz (%)</label>
            <input
              type="number"
              step="0.1"
              min="0"
              max="100"
              value={interestRate}
              onChange={(e) => setInterestRate(e.target.value)}
              className="input"
              placeholder="2.5"
              required
            />
          </div>
        )}
        <div className="flex space-x-3">
          <button type="button" onClick={onClose} className="btn-secondary flex-1">
            Abbrechen
          </button>
          <button type="submit" disabled={isLoading} className="btn-primary flex-1">
            {isLoading ? 'Speichere...' : 'Speichern'}
          </button>
        </div>
      </form>
    </Modal>
  );
}

function Modal({
  title,
  children,
  onClose,
}: {
  title: string;
  children: React.ReactNode;
  onClose: () => void;
}) {
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 dark:bg-opacity-70 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-md w-full p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">{title}</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 text-2xl leading-none"
          >
            ×
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}

function ErrorMessage({ message }: { message: string }) {
  return (
    <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg">
      {message}
    </div>
  );
}
