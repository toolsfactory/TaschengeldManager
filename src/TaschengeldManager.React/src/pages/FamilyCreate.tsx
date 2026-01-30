import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { familyApi } from '../api';

export function FamilyCreate() {
  const [name, setName] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await familyApi.createFamily({ name });
      navigate('/family');
    } catch (err) {
      setError('Familie konnte nicht erstellt werden');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto">
      <div className="mb-6">
        <Link to="/dashboard" className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 text-sm">
          ← Zurück zum Dashboard
        </Link>
      </div>

      <div className="card">
        <div className="text-center mb-6">
          <span className="text-4xl">👨‍👩‍👧‍👦</span>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">Familie erstellen</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            Erstelle eine Familie, um Kinderkonten zu verwalten.
          </p>
        </div>

        {error && (
          <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="name" className="label">
              Familienname
            </label>
            <input
              type="text"
              id="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="input"
              placeholder="z.B. Familie Müller"
              required
            />
          </div>

          <button type="submit" className="btn-primary w-full" disabled={isLoading}>
            {isLoading ? 'Erstelle...' : 'Familie erstellen'}
          </button>
        </form>

        <div className="mt-6 p-4 bg-blue-50 dark:bg-blue-900/30 rounded-lg">
          <h3 className="font-medium text-blue-900 dark:text-blue-200">Was passiert als nächstes?</h3>
          <ul className="mt-2 text-sm text-blue-700 dark:text-blue-300 space-y-1">
            <li>• Du erhältst einen Familiencode für Kinder</li>
            <li>• Du kannst Kinder hinzufügen und Konten erstellen</li>
            <li>• Du kannst Verwandte einladen</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
