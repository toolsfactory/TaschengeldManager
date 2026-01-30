import { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { familyApi } from '../api';
import type { FamilyDto } from '../types';

export function InviteMember() {
  const [families, setFamilies] = useState<FamilyDto[]>([]);
  const [selectedFamilyId, setSelectedFamilyId] = useState('');
  const [email, setEmail] = useState('');
  const [relationshipDescription, setRelationshipDescription] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
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
    setSuccess(false);
    setIsLoading(true);

    try {
      await familyApi.inviteMember(selectedFamilyId, {
        email,
        relationshipDescription: relationshipDescription || undefined,
      });
      setSuccess(true);
      setEmail('');
      setRelationshipDescription('');
    } catch (err) {
      setError('Einladung konnte nicht gesendet werden');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto">
      <div className="mb-6">
        <Link to="/family" className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 text-sm">
          ← Zurück zur Familie
        </Link>
      </div>

      <div className="card">
        <div className="text-center mb-6">
          <span className="text-4xl">📧</span>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">Verwandte einladen</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            Lade Großeltern, Onkel, Tanten oder andere Verwandte ein, damit sie Kindern Geldgeschenke machen können.
          </p>
        </div>

        {error && (
          <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
            {error}
          </div>
        )}

        {success && (
          <div className="bg-green-50 dark:bg-green-900/30 border border-green-200 dark:border-green-800 text-green-700 dark:text-green-400 px-4 py-3 rounded-lg mb-4">
            Einladung wurde erfolgreich gesendet!
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
            <label htmlFor="email" className="label">
              E-Mail-Adresse
            </label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="input"
              placeholder="oma@beispiel.de"
              required
            />
          </div>

          <div>
            <label htmlFor="relationship" className="label">
              Beziehung (optional)
            </label>
            <input
              type="text"
              id="relationship"
              value={relationshipDescription}
              onChange={(e) => setRelationshipDescription(e.target.value)}
              className="input"
              placeholder="z.B. Oma, Onkel Hans"
            />
          </div>

          <button type="submit" className="btn-primary w-full" disabled={isLoading}>
            {isLoading ? 'Sende Einladung...' : 'Einladung senden'}
          </button>
        </form>

        <div className="mt-6 p-4 bg-blue-50 dark:bg-blue-900/30 rounded-lg">
          <h3 className="font-medium text-blue-900 dark:text-blue-200">Wie funktioniert's?</h3>
          <ul className="mt-2 text-sm text-blue-700 dark:text-blue-300 space-y-1">
            <li>• Die Person erhält eine E-Mail mit einem Einladungslink</li>
            <li>• Nach Registrierung/Login kann sie die Einladung annehmen</li>
            <li>• Danach kann sie Kindern Geldgeschenke machen</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
