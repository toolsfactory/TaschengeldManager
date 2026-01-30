import { useEffect, useState } from 'react';
import { moneyRequestApi } from '../api';
import { MoneyRequestStatus, type MoneyRequestDto } from '../types';

export function MoneyRequests() {
  const [requests, setRequests] = useState<MoneyRequestDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [filter, setFilter] = useState<MoneyRequestStatus | 'all'>('all');
  const [respondingTo, setRespondingTo] = useState<string | null>(null);
  const [responseMessage, setResponseMessage] = useState('');

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    setIsLoading(true);
    try {
      const data = await moneyRequestApi.getAll();
      setRequests(data);
    } catch (error) {
      console.error('Failed to load requests:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRespond = async (id: string, approve: boolean) => {
    try {
      await moneyRequestApi.respond(id, {
        approve,
        note: responseMessage || undefined,
      });
      setRespondingTo(null);
      setResponseMessage('');
      loadRequests();
    } catch (error) {
      console.error('Failed to respond:', error);
    }
  };

  const getStatusLabel = (status: MoneyRequestStatus) => {
    switch (status) {
      case MoneyRequestStatus.Pending:
        return 'Ausstehend';
      case MoneyRequestStatus.Approved:
        return 'Genehmigt';
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

  const filteredRequests =
    filter === 'all' ? requests : requests.filter((r) => r.status === filter);

  const pendingCount = requests.filter((r) => r.status === MoneyRequestStatus.Pending).length;

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
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Geld-Anfragen</h1>
          {pendingCount > 0 && (
            <p className="text-sm text-yellow-600 dark:text-yellow-400">{pendingCount} ausstehende Anfrage(n)</p>
          )}
        </div>
      </div>

      {/* Filter */}
      <div className="flex flex-wrap gap-2">
        <button
          onClick={() => setFilter('all')}
          className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
            filter === 'all'
              ? 'bg-blue-600 text-white'
              : 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
          }`}
        >
          Alle ({requests.length})
        </button>
        <button
          onClick={() => setFilter(MoneyRequestStatus.Pending)}
          className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
            filter === MoneyRequestStatus.Pending
              ? 'bg-yellow-600 text-white'
              : 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-700 dark:text-yellow-300 hover:bg-yellow-200 dark:hover:bg-yellow-900/50'
          }`}
        >
          Ausstehend ({requests.filter((r) => r.status === MoneyRequestStatus.Pending).length})
        </button>
        <button
          onClick={() => setFilter(MoneyRequestStatus.Approved)}
          className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
            filter === MoneyRequestStatus.Approved
              ? 'bg-green-600 text-white'
              : 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300 hover:bg-green-200 dark:hover:bg-green-900/50'
          }`}
        >
          Genehmigt
        </button>
        <button
          onClick={() => setFilter(MoneyRequestStatus.Rejected)}
          className={`px-3 py-1 rounded-full text-sm font-medium transition-colors ${
            filter === MoneyRequestStatus.Rejected
              ? 'bg-red-600 text-white'
              : 'bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300 hover:bg-red-200 dark:hover:bg-red-900/50'
          }`}
        >
          Abgelehnt
        </button>
      </div>

      {filteredRequests.length === 0 ? (
        <div className="card text-center py-12">
          <span className="text-6xl">📋</span>
          <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100 mt-4">Keine Anfragen</h2>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            {filter === 'all'
              ? 'Es gibt noch keine Geld-Anfragen von deinen Kindern.'
              : `Keine ${getStatusLabel(filter as MoneyRequestStatus).toLowerCase()}en Anfragen.`}
          </p>
        </div>
      ) : (
        <div className="space-y-4">
          {filteredRequests.map((request) => (
            <div key={request.id} className="card">
              <div className="flex items-start justify-between">
                <div className="flex items-start space-x-4">
                  <div className="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                    <span className="text-blue-600 dark:text-blue-400 font-bold text-lg">
                      {request.childName.charAt(0).toUpperCase()}
                    </span>
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900 dark:text-gray-100">
                      {request.childName} bittet um {request.amount.toFixed(2)} €
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
                      <p className="text-sm text-gray-500 dark:text-gray-400 mt-2">
                        Antwort: "{request.responseNote}"
                      </p>
                    )}
                  </div>
                </div>
                <div className="text-right">
                  <span
                    className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(
                      request.status
                    )}`}
                  >
                    {getStatusLabel(request.status)}
                  </span>
                </div>
              </div>

              {request.status === MoneyRequestStatus.Pending && (
                <div className="mt-4 pt-4 border-t border-gray-100 dark:border-gray-700">
                  {respondingTo === request.id ? (
                    <div className="space-y-3">
                      <input
                        type="text"
                        value={responseMessage}
                        onChange={(e) => setResponseMessage(e.target.value)}
                        className="input"
                        placeholder="Nachricht (optional)"
                      />
                      <div className="flex space-x-3">
                        <button
                          onClick={() => setRespondingTo(null)}
                          className="btn-secondary flex-1"
                        >
                          Abbrechen
                        </button>
                        <button
                          onClick={() => handleRespond(request.id, false)}
                          className="btn-danger flex-1"
                        >
                          Ablehnen
                        </button>
                        <button
                          onClick={() => handleRespond(request.id, true)}
                          className="btn-success flex-1"
                        >
                          Genehmigen
                        </button>
                      </div>
                    </div>
                  ) : (
                    <div className="flex space-x-3">
                      <button
                        onClick={() => setRespondingTo(request.id)}
                        className="btn-secondary flex-1"
                      >
                        Antworten
                      </button>
                      <button
                        onClick={() => handleRespond(request.id, true)}
                        className="btn-success flex-1"
                      >
                        ✓ Genehmigen
                      </button>
                    </div>
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
