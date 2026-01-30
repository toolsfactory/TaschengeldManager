import { useEffect, useState } from 'react';
import { recurringPaymentApi, familyApi, accountApi } from '../api';
import {
  PaymentInterval,
  type RecurringPaymentDto,
  type AccountDto,
  type FamilyDto,
} from '../types';

export function RecurringPayments() {
  const [payments, setPayments] = useState<RecurringPaymentDto[]>([]);
  const [accounts, setAccounts] = useState<AccountDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      const [paymentsData, familiesData] = await Promise.all([
        recurringPaymentApi.getAll(),
        familyApi.getMyFamilies(),
      ]);
      setPayments(paymentsData);

      if (familiesData.length > 0) {
        const accountsData = await accountApi.getFamilyAccounts(familiesData[0].id);
        setAccounts(accountsData);
      }
    } catch (error) {
      console.error('Failed to load data:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleToggle = async (id: string) => {
    try {
      await recurringPaymentApi.toggleActive(id);
      loadData();
    } catch (error) {
      console.error('Failed to toggle payment:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Möchtest du diese automatische Zahlung wirklich löschen?')) return;

    try {
      await recurringPaymentApi.delete(id);
      setPayments((prev) => prev.filter((p) => p.id !== id));
    } catch (error) {
      console.error('Failed to delete payment:', error);
    }
  };

  const getIntervalLabel = (interval: PaymentInterval) => {
    switch (interval) {
      case PaymentInterval.Weekly:
        return 'Wöchentlich';
      case PaymentInterval.Biweekly:
        return 'Alle 2 Wochen';
      case PaymentInterval.Monthly:
        return 'Monatlich';
      default:
        return interval;
    }
  };

  const getDayLabel = (payment: RecurringPaymentDto) => {
    if (payment.interval === PaymentInterval.Monthly && payment.dayOfMonth) {
      return `am ${payment.dayOfMonth}. des Monats`;
    }
    if (payment.dayOfWeek !== null && payment.dayOfWeek !== undefined) {
      const days = ['Sonntag', 'Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag'];
      return `jeden ${days[payment.dayOfWeek]}`;
    }
    return '';
  };

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
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Automatische Zahlungen</h1>
        <button onClick={() => setShowCreateModal(true)} className="btn-primary">
          + Neue Zahlung
        </button>
      </div>

      {payments.length === 0 ? (
        <div className="card text-center py-12">
          <span className="text-6xl">🔄</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">
            Keine automatischen Zahlungen
          </h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Richte automatische Taschengeld-Zahlungen für deine Kinder ein.
          </p>
          <button onClick={() => setShowCreateModal(true)} className="btn-primary mt-4">
            Erste Zahlung einrichten
          </button>
        </div>
      ) : (
        <div className="space-y-4">
          {payments.map((payment) => (
            <div key={payment.id} className="card">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-4">
                  <div
                    className={`w-12 h-12 rounded-full flex items-center justify-center ${
                      payment.isActive ? 'bg-green-100 dark:bg-green-900/30' : 'bg-gray-100 dark:bg-gray-700'
                    }`}
                  >
                    <span className="text-2xl">{payment.isActive ? '🔄' : '⏸️'}</span>
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900 dark:text-gray-100">
                      {payment.amount.toFixed(2)} € an {payment.childName}
                    </p>
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                      {getIntervalLabel(payment.interval)} {getDayLabel(payment)}
                    </p>
                    {payment.description && (
                      <p className="text-sm text-gray-400 dark:text-gray-500">{payment.description}</p>
                    )}
                    <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">
                      Nächste Zahlung:{' '}
                      {new Date(payment.nextExecutionDate).toLocaleDateString('de-DE')}
                    </p>
                  </div>
                </div>
                <div className="flex items-center space-x-2">
                  <button
                    onClick={() => handleToggle(payment.id)}
                    className={`px-3 py-1 rounded-full text-sm font-medium ${
                      payment.isActive
                        ? 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-900/50'
                        : 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
                    }`}
                  >
                    {payment.isActive ? 'Aktiv' : 'Pausiert'}
                  </button>
                  <button
                    onClick={() => handleDelete(payment.id)}
                    className="text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300 p-2"
                  >
                    🗑️
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {showCreateModal && (
        <CreatePaymentModal
          accounts={accounts}
          onClose={() => setShowCreateModal(false)}
          onSuccess={loadData}
        />
      )}
    </div>
  );
}

function CreatePaymentModal({
  accounts,
  onClose,
  onSuccess,
}: {
  accounts: AccountDto[];
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [accountId, setAccountId] = useState(accounts[0]?.id || '');
  const [amount, setAmount] = useState('');
  const [description, setDescription] = useState('');
  const [interval, setInterval] = useState<PaymentInterval>(PaymentInterval.Weekly);
  const [dayOfWeek, setDayOfWeek] = useState('1');
  const [dayOfMonth, setDayOfMonth] = useState('1');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await recurringPaymentApi.create({
        accountId,
        amount: parseFloat(amount),
        description: description || undefined,
        interval,
        dayOfWeek: interval !== PaymentInterval.Monthly ? parseInt(dayOfWeek) : undefined,
        dayOfMonth: interval === PaymentInterval.Monthly ? parseInt(dayOfMonth) : undefined,
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError('Zahlung konnte nicht erstellt werden');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 dark:bg-opacity-70 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-md w-full p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">Neue automatische Zahlung</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 text-2xl">
            ×
          </button>
        </div>

        {error && (
          <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="label">Kind</label>
            <select
              value={accountId}
              onChange={(e) => setAccountId(e.target.value)}
              className="input"
              required
            >
              {accounts.map((account) => (
                <option key={account.id} value={account.id}>
                  {account.ownerName}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="label">Betrag (€)</label>
            <input
              type="number"
              step="0.01"
              min="0.01"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              className="input"
              placeholder="5.00"
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

          <div>
            <label className="label">Intervall</label>
            <select
              value={interval}
              onChange={(e) => setInterval(Number(e.target.value) as PaymentInterval)}
              className="input"
            >
              <option value={PaymentInterval.Weekly}>Wöchentlich</option>
              <option value={PaymentInterval.Biweekly}>Alle 2 Wochen</option>
              <option value={PaymentInterval.Monthly}>Monatlich</option>
            </select>
          </div>

          {interval !== PaymentInterval.Monthly && (
            <div>
              <label className="label">Wochentag</label>
              <select
                value={dayOfWeek}
                onChange={(e) => setDayOfWeek(e.target.value)}
                className="input"
              >
                <option value="1">Montag</option>
                <option value="2">Dienstag</option>
                <option value="3">Mittwoch</option>
                <option value="4">Donnerstag</option>
                <option value="5">Freitag</option>
                <option value="6">Samstag</option>
                <option value="0">Sonntag</option>
              </select>
            </div>
          )}

          {interval === PaymentInterval.Monthly && (
            <div>
              <label className="label">Tag des Monats</label>
              <select
                value={dayOfMonth}
                onChange={(e) => setDayOfMonth(e.target.value)}
                className="input"
              >
                {Array.from({ length: 28 }, (_, i) => i + 1).map((day) => (
                  <option key={day} value={day}>
                    {day}.
                  </option>
                ))}
              </select>
            </div>
          )}

          <div className="flex space-x-3">
            <button type="button" onClick={onClose} className="btn-secondary flex-1">
              Abbrechen
            </button>
            <button type="submit" disabled={isLoading} className="btn-primary flex-1">
              {isLoading ? 'Erstelle...' : 'Erstellen'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
