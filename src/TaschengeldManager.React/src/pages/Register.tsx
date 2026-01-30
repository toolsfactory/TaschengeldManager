import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { ThemeToggle } from '../components/ThemeToggle';

interface DbStats {
  users: number;
  families: number;
}

export function Register() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [nickname, setNickname] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [dbStats, setDbStats] = useState<DbStats | null>(null);
  const [dbLoading, setDbLoading] = useState(true);

  const { register } = useAuth();
  const navigate = useNavigate();
  const isDevelopment = import.meta.env.DEV;

  useEffect(() => {
    if (isDevelopment) {
      fetch('/api/dev/stats')
        .then((res) => (res.ok ? res.json() : null))
        .then((data) => setDbStats(data))
        .catch(() => setDbStats(null))
        .finally(() => setDbLoading(false));
    }
  }, [isDevelopment]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (password !== confirmPassword) {
      setError('Passwörter stimmen nicht überein');
      return;
    }

    if (password.length < 8) {
      setError('Passwort muss mindestens 8 Zeichen lang sein');
      return;
    }

    setIsLoading(true);

    try {
      const result = await register({ email, password, nickname });
      if (result.mfaSetupRequired) {
        // Navigate to login with message to set up MFA after first login
        navigate('/login', {
          state: { message: 'Registrierung erfolgreich! Bitte melde dich an.' },
        });
      } else {
        navigate('/login', {
          state: { message: 'Registrierung erfolgreich! Bitte melde dich an.' },
        });
      }
    } catch (err) {
      setError('Registrierung fehlgeschlagen. E-Mail existiert möglicherweise bereits.');
    } finally {
      setIsLoading(false);
    }
  };

  const isDbEmpty = dbStats?.users === 0;

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 transition-colors">
      {/* Development Mode Banner */}
      {isDevelopment && (
        <div className="bg-amber-500 dark:bg-amber-600 text-amber-950 dark:text-amber-100 py-1 text-sm font-medium">
          <div className="max-w-7xl mx-auto px-4 flex items-center justify-between">
            <span>Development Mode</span>
            <div className="flex items-center gap-4">
              {!dbLoading && (
                <span className={`px-2 py-0.5 rounded text-xs ${isDbEmpty ? 'bg-red-600 text-white' : 'bg-green-600 text-white'}`}>
                  DB: {isDbEmpty ? 'Leer' : `${dbStats?.users} User`}
                </span>
              )}
              <a
                href="http://localhost:5041/scalar/v1"
                target="_blank"
                rel="noopener noreferrer"
                className="underline hover:no-underline"
              >
                API Docs
              </a>
            </div>
          </div>
        </div>
      )}

      <div className="absolute top-4 right-4 z-10" style={{ top: isDevelopment ? 'calc(28px + 1rem)' : '1rem' }}>
        <ThemeToggle />
      </div>

      <div className="flex items-center justify-center px-4" style={{ minHeight: isDevelopment ? 'calc(100vh - 28px)' : '100vh' }}>
        <div className="max-w-md w-full">
          <div className="card">
            <div className="text-center mb-6">
              <span className="text-4xl">💰</span>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">TaschengeldManager</h1>
              <p className="text-gray-600 dark:text-gray-400 mt-1">Neues Konto erstellen</p>
            </div>

          {error && (
            <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
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
                placeholder="Max Mustermann"
                required
              />
            </div>

            <div>
              <label htmlFor="email" className="label">
                E-Mail
              </label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="input"
                placeholder="name@beispiel.de"
                required
              />
            </div>

            <div>
              <label htmlFor="password" className="label">
                Passwort
              </label>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="input"
                placeholder="Mindestens 8 Zeichen"
                minLength={8}
                required
              />
            </div>

            <div>
              <label htmlFor="confirmPassword" className="label">
                Passwort bestätigen
              </label>
              <input
                type="password"
                id="confirmPassword"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                className="input"
                placeholder="Passwort wiederholen"
                required
              />
            </div>

            <button type="submit" className="btn-primary w-full" disabled={isLoading}>
              {isLoading ? 'Registriere...' : 'Registrieren'}
            </button>
          </form>

          <p className="text-center text-sm text-gray-600 dark:text-gray-400 mt-6">
            Bereits ein Konto?{' '}
            <Link to="/login" className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium">
              Anmelden
            </Link>
          </p>
        </div>
      </div>
    </div>
  </div>
  );
}
