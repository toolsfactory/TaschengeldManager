import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { accountApi, familyApi, moneyRequestApi } from '../api';
import { UserRole, MoneyRequestStatus, ExpenseCategories, type AccountDto, type FamilyDto, type MoneyRequestDto } from '../types';

export function Dashboard() {
  const { user } = useAuth();

  if (!user) return null;

  switch (user.role) {
    case UserRole.Parent:
      return <ParentDashboard />;
    case UserRole.Child:
      return <ChildDashboard />;
    case UserRole.Relative:
      return <RelativeDashboard />;
    default:
      return <div>Unbekannte Rolle</div>;
  }
}

function ParentDashboard() {
  const { user } = useAuth();
  const [families, setFamilies] = useState<FamilyDto[]>([]);
  const [accounts, setAccounts] = useState<AccountDto[]>([]);
  const [pendingRequests, setPendingRequests] = useState<MoneyRequestDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      try {
        const familiesData = await familyApi.getMyFamilies();
        setFamilies(familiesData);

        if (familiesData.length > 0) {
          const accountsData = await accountApi.getFamilyAccounts(familiesData[0].id);
          setAccounts(accountsData);

          const requestsData = await moneyRequestApi.getAll();
          setPendingRequests(requestsData.filter((r) => r.status === MoneyRequestStatus.Pending));
        }
      } catch (error) {
        console.error('Failed to load dashboard data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, []);

  if (isLoading) {
    return <LoadingSpinner />;
  }

  const totalBalance = accounts.reduce((sum, acc) => sum + acc.balance, 0);

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
          Willkommen, {user?.nickname}!
        </h1>
      </div>

      {families.length === 0 ? (
        <div className="card text-center py-12">
          <span className="text-6xl">👨‍👩‍👧‍👦</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Keine Familie gefunden</h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">Erstelle eine Familie, um loszulegen.</p>
          <Link to="/family/create" className="btn-primary mt-4 inline-block">
            Familie erstellen
          </Link>
        </div>
      ) : (
        <>
          {/* Stats Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="card">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Gesamtguthaben</p>
                  <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">
                    {totalBalance.toFixed(2)} €
                  </p>
                </div>
                <div className="w-12 h-12 bg-green-100 dark:bg-green-900/30 rounded-full flex items-center justify-center">
                  <span className="text-2xl">💰</span>
                </div>
              </div>
            </div>

            <div className="card">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Kinder</p>
                  <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">{accounts.length}</p>
                </div>
                <div className="w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-full flex items-center justify-center">
                  <span className="text-2xl">👶</span>
                </div>
              </div>
            </div>

            <div className="card">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Offene Anfragen</p>
                  <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">{pendingRequests.length}</p>
                </div>
                <div className="w-12 h-12 bg-yellow-100 dark:bg-yellow-900/30 rounded-full flex items-center justify-center">
                  <span className="text-2xl">📝</span>
                </div>
              </div>
              {pendingRequests.length > 0 && (
                <Link
                  to="/money-requests"
                  className="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 mt-2 block"
                >
                  Anfragen anzeigen →
                </Link>
              )}
            </div>
          </div>

          {/* Children's Accounts */}
          <div className="card">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">Kinderkonten</h2>
              <Link to="/accounts" className="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300">
                Alle anzeigen →
              </Link>
            </div>
            {accounts.length === 0 ? (
              <p className="text-gray-500 dark:text-gray-400">Noch keine Kinderkonten vorhanden.</p>
            ) : (
              <div className="space-y-3">
                {accounts.slice(0, 5).map((account) => (
                  <Link
                    key={account.id}
                    to={`/accounts/${account.id}`}
                    className="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
                  >
                    <div className="flex items-center space-x-3">
                      <div className="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                        <span className="text-blue-600 dark:text-blue-400 font-medium">
                          {account.ownerName.charAt(0).toUpperCase()}
                        </span>
                      </div>
                      <span className="font-medium text-gray-900 dark:text-gray-100">{account.ownerName}</span>
                    </div>
                    <span className="font-semibold text-gray-900 dark:text-gray-100">{account.balance.toFixed(2)} €</span>
                  </Link>
                ))}
              </div>
            )}
          </div>

          {/* Quick Actions */}
          <div className="card">
            <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">Schnellaktionen</h2>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <Link
                to="/family/children/add"
                className="flex flex-col items-center p-4 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
              >
                <span className="text-2xl mb-2">➕</span>
                <span className="text-sm text-gray-700 dark:text-gray-300">Kind hinzufügen</span>
              </Link>
              <Link
                to="/recurring-payments"
                className="flex flex-col items-center p-4 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
              >
                <span className="text-2xl mb-2">🔄</span>
                <span className="text-sm text-gray-700 dark:text-gray-300">Auto. Zahlungen</span>
              </Link>
              <Link
                to="/family/invite"
                className="flex flex-col items-center p-4 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
              >
                <span className="text-2xl mb-2">📧</span>
                <span className="text-sm text-gray-700 dark:text-gray-300">Einladen</span>
              </Link>
              <Link
                to="/money-requests"
                className="flex flex-col items-center p-4 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
              >
                <span className="text-2xl mb-2">📋</span>
                <span className="text-sm text-gray-700 dark:text-gray-300">Anfragen</span>
              </Link>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

function ChildDashboard() {
  const { user } = useAuth();
  const [account, setAccount] = useState<AccountDto | null>(null);
  const [pendingRequests, setPendingRequests] = useState<MoneyRequestDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showExpenseModal, setShowExpenseModal] = useState(false);

  const loadData = async () => {
    try {
      const accountData = await accountApi.getMyAccount();
      setAccount(accountData);

      const requestsData = await moneyRequestApi.getMyRequests();
      setPendingRequests(requestsData.filter((r) => r.status === MoneyRequestStatus.Pending));
    } catch (error) {
      console.error('Failed to load dashboard data:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
          Hallo, {user?.nickname}! 👋
        </h1>
      </div>

      {/* Balance Card */}
      <div className="card bg-gradient-to-r from-blue-500 to-blue-600 dark:from-blue-600 dark:to-blue-700 text-white">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-blue-100">Dein Guthaben</p>
            <p className="text-4xl font-bold mt-2">
              {account?.balance.toFixed(2) ?? '0.00'} €
            </p>
          </div>
          <div className="text-6xl opacity-50">💰</div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-3 gap-4">
        <button
          onClick={() => setShowExpenseModal(true)}
          className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow text-left"
        >
          <div className="flex flex-col items-center text-center">
            <span className="text-3xl mb-2">💸</span>
            <span className="font-medium text-gray-900 dark:text-gray-100">Ausgabe</span>
            <span className="text-sm text-gray-500 dark:text-gray-400">Geld ausgeben</span>
          </div>
        </button>
        <Link
          to="/account/history"
          className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow"
        >
          <div className="flex flex-col items-center text-center">
            <span className="text-3xl mb-2">📊</span>
            <span className="font-medium text-gray-900 dark:text-gray-100">Verlauf</span>
            <span className="text-sm text-gray-500 dark:text-gray-400">Transaktionen</span>
          </div>
        </Link>
        <Link
          to="/my-requests"
          className="card hover:shadow-lg dark:hover:shadow-gray-900/50 transition-shadow"
        >
          <div className="flex flex-col items-center text-center">
            <span className="text-3xl mb-2">🙋</span>
            <span className="font-medium text-gray-900 dark:text-gray-100">Anfragen</span>
            <span className="text-sm text-gray-500 dark:text-gray-400">
              {pendingRequests.length > 0 ? `${pendingRequests.length} offen` : 'Geld bitten'}
            </span>
          </div>
        </Link>
      </div>

      {/* Interest Info */}
      {account?.interestEnabled && account.interestRate && (
        <div className="card bg-green-50 dark:bg-green-900/30 border border-green-200 dark:border-green-800">
          <div className="flex items-center space-x-3">
            <span className="text-2xl">📈</span>
            <div>
              <p className="font-medium text-green-800 dark:text-green-300">Zinsen aktiv!</p>
              <p className="text-sm text-green-600 dark:text-green-400">
                Du bekommst {account.interestRate}% Zinsen auf dein Guthaben.
              </p>
            </div>
          </div>
        </div>
      )}

      {/* Expense Modal */}
      {showExpenseModal && (
        <ExpenseModal
          balance={account?.balance ?? 0}
          onClose={() => setShowExpenseModal(false)}
          onSuccess={() => {
            setShowExpenseModal(false);
            loadData();
          }}
        />
      )}
    </div>
  );
}

function ExpenseModal({
  balance,
  onClose,
  onSuccess,
}: {
  balance: number;
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [amount, setAmount] = useState('');
  const [category, setCategory] = useState('');
  const [description, setDescription] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    const numAmount = parseFloat(amount);
    if (isNaN(numAmount) || numAmount <= 0) {
      setError('Bitte gib einen gültigen Betrag ein');
      return;
    }

    if (numAmount > balance) {
      setError(`Du hast nur ${balance.toFixed(2)} € Guthaben`);
      return;
    }

    setIsLoading(true);

    try {
      await accountApi.withdraw({
        amount: numAmount,
        category: category || undefined,
        description: description || undefined,
      });
      onSuccess();
    } catch (err) {
      setError('Ausgabe konnte nicht gespeichert werden');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 dark:bg-opacity-70 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-md w-full p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">💸 Ausgabe erfassen</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 text-2xl">
            ×
          </button>
        </div>

        <div className="mb-4 p-3 bg-blue-50 dark:bg-blue-900/30 rounded-lg">
          <p className="text-sm text-blue-700 dark:text-blue-300">
            Verfügbar: <strong>{balance.toFixed(2)} €</strong>
          </p>
        </div>

        {error && (
          <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="label">Betrag (€) *</label>
            <input
              type="number"
              step="0.01"
              min="0.01"
              max={balance}
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              className="input"
              placeholder="5.00"
              required
            />
          </div>

          <div>
            <label className="label">Kategorie</label>
            <select
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="input"
            >
              <option value="">-- Wähle eine Kategorie --</option>
              {ExpenseCategories.map((cat) => (
                <option key={cat} value={cat}>
                  {cat}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="label">Beschreibung</label>
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="input"
              placeholder="z.B. Eis im Park"
              maxLength={200}
            />
          </div>

          <div className="flex space-x-3">
            <button type="button" onClick={onClose} className="btn-secondary flex-1">
              Abbrechen
            </button>
            <button type="submit" disabled={isLoading} className="btn-primary flex-1">
              {isLoading ? 'Speichern...' : 'Ausgabe speichern'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function RelativeDashboard() {
  const { user } = useAuth();
  const [families, setFamilies] = useState<FamilyDto[]>([]);
  const [accounts, setAccounts] = useState<AccountDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      try {
        const familiesData = await familyApi.getMyFamilies();
        setFamilies(familiesData);

        if (familiesData.length > 0) {
          const accountsData = await accountApi.getFamilyAccounts(familiesData[0].id);
          setAccounts(accountsData);
        }
      } catch (error) {
        console.error('Failed to load dashboard data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, []);

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
          Willkommen, {user?.nickname}!
        </h1>
      </div>

      {families.length === 0 ? (
        <div className="card text-center py-12">
          <span className="text-6xl">📬</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Keine Familie</h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Du bist noch keiner Familie beigetreten. Warte auf eine Einladung.
          </p>
        </div>
      ) : (
        <>
          <div className="card">
            <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">
              Kindern Geld schenken
            </h2>
            <p className="text-gray-600 dark:text-gray-400 mb-4">
              Wähle ein Kind aus, um ihm ein Geldgeschenk zu machen.
            </p>
            <div className="space-y-3">
              {accounts.map((account) => (
                <Link
                  key={account.id}
                  to={`/accounts/${account.id}`}
                  className="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors"
                >
                  <div className="flex items-center space-x-3">
                    <div className="w-12 h-12 bg-pink-100 dark:bg-pink-900 rounded-full flex items-center justify-center">
                      <span className="text-pink-600 dark:text-pink-400 font-medium text-lg">
                        {account.ownerName.charAt(0).toUpperCase()}
                      </span>
                    </div>
                    <div>
                      <span className="font-medium text-gray-900 dark:text-gray-100 block">{account.ownerName}</span>
                      <span className="text-sm text-gray-500 dark:text-gray-400">Geldgeschenk machen</span>
                    </div>
                  </div>
                  <span className="text-2xl">🎁</span>
                </Link>
              ))}
            </div>
          </div>
        </>
      )}
    </div>
  );
}

function LoadingSpinner() {
  return (
    <div className="flex items-center justify-center py-12">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 dark:border-blue-400"></div>
    </div>
  );
}
