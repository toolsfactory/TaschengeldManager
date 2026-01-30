import { useEffect, useState } from 'react';
import { moneyRequestApi } from '../api';
import { MoneyRequestStatus, type MoneyRequestDto } from '../types';

export function MyRequests() {
  const [requests, setRequests] = useState<MoneyRequestDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    setIsLoading(true);
    try {
      const data = await moneyRequestApi.getMyRequests();
      setRequests(data);
    } catch (error) {
      console.error('Failed to load requests:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleWithdraw = async (id: string) => {
    if (!confirm('Möchtest du diese Anfrage wirklich zurückziehen?')) return;

    try {
      await moneyRequestApi.withdraw(id);
      loadRequests();
    } catch (error) {
      console.error('Failed to withdraw:', error);
    }
  };

  const getStatusLabel = (status: MoneyRequestStatus) => {
    switch (status) {
      case MoneyRequestStatus.Pending:
        return 'Wartet auf Antwort';
      case MoneyRequestStatus.Approved:
        return 'Genehmigt! 🎉';
      case MoneyRequestStatus.Rejected:
        return 'Abgelehnt';
      case MoneyRequestStatus.Withdrawn:
        return 'Zurückgezogen';
      default:
        return status;
    }
  };

  const getStatusColor = (status: MoneyRequestStatus) => {
    switch (status) {
      case MoneyRequestStatus.Pending:
        return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-300';
      case MoneyRequestStatus.Approved:
        return 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300';
      case MoneyRequestStatus.Rejected:
        return 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300';
      case MoneyRequestStatus.Withdrawn:
        return 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300';
      default:
        return 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300';
    }
  };

  const getStatusIcon = (status: MoneyRequestStatus) => {
    switch (status) {
      case MoneyRequestStatus.Pending:
        return '⏳';
      case MoneyRequestStatus.Approved:
        return '✅';
      case MoneyRequestStatus.Rejected:
        return '❌';
      case MoneyRequestStatus.Withdrawn:
        return '↩️';
      default:
        return '📋';
    }
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
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Meine Anfragen</h1>
        <button onClick={() => setShowCreateModal(true)} className="btn-primary">
          + Geld anfragen
        </button>
      </div>

      {requests.length === 0 ? (
        <div className="card text-center py-12">
          <span className="text-6xl">🙋</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Keine Anfragen</h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Du hast noch keine Geld-Anfragen gestellt.
          </p>
          <button onClick={() => setShowCreateModal(true)} className="btn-primary mt-4">
            Erste Anfrage stellen
          </button>
        </div>
      ) : (
        <div className="space-y-4">
          {requests.map((request) => (
            <div key={request.id} className="card">
              <div className="flex items-start justify-between">
                <div className="flex items-start space-x-4">
                  <div
                    className={`w-12 h-12 rounded-full flex items-center justify-center ${
                      request.status === MoneyRequestStatus.Approved
                        ? 'bg-green-100 dark:bg-green-900/30'
                        : request.status === MoneyRequestStatus.Rejected
                        ? 'bg-red-100 dark:bg-red-900/30'
                        : request.status === MoneyRequestStatus.Pending
                        ? 'bg-yellow-100 dark:bg-yellow-900/30'
                        : 'bg-gray-100 dark:bg-gray-700'
                    }`}
                  >
                    <span className="text-2xl">{getStatusIcon(request.status)}</span>
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900 dark:text-gray-100">
                      {request.amount.toFixed(2)} € angefragt
                    </p>
                    {request.reason && (
                      <p className="text-gray-600 dark:text-gray-400 mt-1">"{request.reason}"</p>
                    )}
                    <p className="text-sm text-gray-400 dark:text-gray-500 mt-1">
                      {new Date(request.createdAt).toLocaleDateString('de-DE', {
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric',
                        hour: '2-digit',
                        minute: '2-digit',
                      })}
                    </p>
                    {request.responseNote && (
                      <div className="mt-2 p-2 bg-gray-50 dark:bg-gray-700 rounded-lg">
                        <p className="text-sm text-gray-600 dark:text-gray-300">
                          <strong>{request.respondedByName}:</strong> "{request.responseNote}"
                        </p>
                      </div>
                    )}
                  </div>
                </div>
                <span
                  className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(
                    request.status
                  )}`}
                >
                  {getStatusLabel(request.status)}
                </span>
              </div>

              {request.status === MoneyRequestStatus.Pending && (
                <div className="mt-4 pt-4 border-t border-gray-100 dark:border-gray-700">
                  <button
                    onClick={() => handleWithdraw(request.id)}
                    className="btn-secondary text-sm"
                  >
                    Anfrage zurückziehen
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {showCreateModal && (
        <CreateRequestModal onClose={() => setShowCreateModal(false)} onSuccess={loadRequests} />
      )}
    </div>
  );
}

function CreateRequestModal({
  onClose,
  onSuccess,
}: {
  onClose: () => void;
  onSuccess: () => void;
}) {
  const [amount, setAmount] = useState('');
  const [reason, setReason] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (reason.trim().length < 4) {
      setError('Bitte gib einen Grund mit mindestens 4 Zeichen an');
      return;
    }

    setIsLoading(true);

    try {
      await moneyRequestApi.create({
        amount: parseFloat(amount),
        reason: reason.trim(),
      });
      onSuccess();
      onClose();
    } catch (err) {
      setError('Anfrage konnte nicht gesendet werden');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 dark:bg-opacity-70 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-md w-full p-6">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">Geld anfragen</h2>
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
            <label className="label">Wofür brauchst du das Geld? *</label>
            <textarea
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              className="input min-h-[100px]"
              placeholder="z.B. Neues Buch, Kino mit Freunden..."
              required
              minLength={4}
            />
          </div>

          <div className="flex space-x-3">
            <button type="button" onClick={onClose} className="btn-secondary flex-1">
              Abbrechen
            </button>
            <button type="submit" disabled={isLoading} className="btn-primary flex-1">
              {isLoading ? 'Sende...' : 'Anfrage senden'}
            </button>
          </div>
        </form>

        <div className="mt-4 p-3 bg-blue-50 dark:bg-blue-900/30 rounded-lg text-sm text-blue-700 dark:text-blue-300">
          💡 Deine Eltern werden benachrichtigt und können die Anfrage genehmigen oder ablehnen.
        </div>
      </div>
    </div>
  );
}
