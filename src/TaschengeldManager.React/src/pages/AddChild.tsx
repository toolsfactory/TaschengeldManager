import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { familyApi } from '../api';
import type { FamilyDto } from '../types';

export function AddChild() {
  const [families, setFamilies] = useState<FamilyDto[]>([]);
  const [selectedFamilyId, setSelectedFamilyId] = useState('');
  const [nickname, setNickname] = useState('');
  const [pin, setPin] = useState('');
  const [confirmPin, setConfirmPin] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const loadFamilies = async () => {
      try {
        const data = await familyApi.getMyFamilies();
        setFamilies(data);
        if (data.length > 0) {
          setSelectedFamilyId(data[0].id);
        }
      } catch (error) {
        console.error('Failed to load families:', error);
      }
    };

    loadFamilies();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (pin !== confirmPin) {
      setError('PINs stimmen nicht überein');
      return;
    }

    if (pin.length < 4) {
      setError('PIN muss mindestens 4 Zeichen haben');
      return;
    }

    setIsLoading(true);

    try {
      const result = await familyApi.addChild(selectedFamilyId, { nickname, pin });
      navigate(`/accounts/${result.accountId}`);
    } catch (err) {
      setError('Kind konnte nicht hinzugefügt werden');
    } finally {
      setIsLoading(false);
    }
  };

  const selectedFamily = families.find((f) => f.id === selectedFamilyId);

  return (
    <div className="max-w-md mx-auto">
      <div className="mb-6">
        <Link to="/family" className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 text-sm">
          ← Zurück zur Familie
        </Link>
      </div>

      <div className="card">
        <div className="text-center mb-6">
          <span className="text-4xl">👶</span>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">Kind hinzufügen</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            Erstelle ein Konto für ein Kind in deiner Familie.
          </p>
        </div>

        {error && (
          <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          {families.length > 1 && (
            <div>
              <label htmlFor="family" className="label">
                Familie
              </label>
              <select
                id="family"
                value={selectedFamilyId}
                onChange={(e) => setSelectedFamilyId(e.target.value)}
                className="input"
                required
              >
                {families.map((family) => (
                  <option key={family.id} value={family.id}>
                    {family.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          <div>
            <label htmlFor="nickname" className="label">
              Spitzname
            </label>
            <input
              type="text"
              id="nickname"
              value={nickname}
              onChange={(e) => setNickname(e.target.value)}
              className="input"
              placeholder="z.B. Max"
              required
            />
          </div>

          <div>
            <label htmlFor="pin" className="label">
              PIN (für Kind-Login)
            </label>
            <input
              type="password"
              id="pin"
              value={pin}
              onChange={(e) => setPin(e.target.value)}
              className="input text-center tracking-widest"
              placeholder="••••"
              minLength={4}
              maxLength={6}
              required
            />
          </div>

          <div>
            <label htmlFor="confirmPin" className="label">
              PIN bestätigen
            </label>
            <input
              type="password"
              id="confirmPin"
              value={confirmPin}
              onChange={(e) => setConfirmPin(e.target.value)}
              className="input text-center tracking-widest"
              placeholder="••••"
              minLength={4}
              maxLength={6}
              required
            />
          </div>

          <button type="submit" className="btn-primary w-full" disabled={isLoading}>
            {isLoading ? 'Wird hinzugefügt...' : 'Kind hinzufügen'}
          </button>
        </form>

        {selectedFamily && (
          <div className="mt-6 p-4 bg-blue-50 dark:bg-blue-900/30 rounded-lg">
            <h3 className="font-medium text-blue-900 dark:text-blue-200">Kind-Login Daten</h3>
            <p className="text-sm text-blue-700 dark:text-blue-300 mt-1">
              Das Kind kann sich mit folgenden Daten anmelden:
            </p>
            <ul className="mt-2 text-sm text-blue-700 dark:text-blue-300 space-y-1">
              <li>
                <strong>Familiencode:</strong>{' '}
                <span className="font-mono">{selectedFamily.familyCode}</span>
              </li>
              <li>
                <strong>Spitzname:</strong> {nickname || '(wird eingegeben)'}
              </li>
              <li>
                <strong>PIN:</strong> (wird festgelegt)
              </li>
            </ul>
          </div>
        )}
      </div>
    </div>
  );
}
