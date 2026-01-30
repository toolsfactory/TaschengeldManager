import { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { ThemeToggle } from '../components/ThemeToggle';

interface DbStats {
  users: number;
  families: number;
}

type LoginMode = 'parent' | 'child';

export function Login() {
  const [mode, setMode] = useState<LoginMode>('parent');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [familyCode, setFamilyCode] = useState('');
  const [nickname, setNickname] = useState('');
  const [pin, setPin] = useState('');
  const [mfaToken, setMfaToken] = useState('');
  const [mfaCode, setMfaCode] = useState('');
  const [showMfa, setShowMfa] = useState(false);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [dbStats, setDbStats] = useState<DbStats | null>(null);
  const [dbLoading, setDbLoading] = useState(true);

  const { login, childLogin, verifyTotp } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
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

  const from = (location.state as { from?: { pathname: string } })?.from?.pathname || '/dashboard';

  const handleParentLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      const result = await login({ email, password });
      if (result.mfaRequired && result.mfaToken) {
        setMfaToken(result.mfaToken);
        setShowMfa(true);
      } else {
        navigate(from, { replace: true });
      }
    } catch (err) {
      setError('Ungültige E-Mail oder Passwort');
    } finally {
      setIsLoading(false);
    }
  };

  const handleChildLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      const result = await childLogin({ familyCode, nickname, pin });
      if (result.mfaRequired && result.mfaToken) {
        setMfaToken(result.mfaToken);
        setShowMfa(true);
      } else {
        navigate(from, { replace: true });
      }
    } catch (err) {
      setError('Ungültige Anmeldedaten');
    } finally {
      setIsLoading(false);
    }
  };

  const handleMfaVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await verifyTotp(mfaToken, mfaCode);
      navigate(from, { replace: true });
    } catch (err) {
      setError('Ungültiger Code');
    } finally {
      setIsLoading(false);
    }
  };

  if (showMfa) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 px-4 transition-colors">
        <div className="absolute top-4 right-4">
          <ThemeToggle />
        </div>
        <div className="max-w-md w-full">
          <div className="card">
            <div className="text-center mb-6">
              <span className="text-4xl">🔐</span>
              <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">Zwei-Faktor-Authentifizierung</h1>
              <p className="text-gray-600 dark:text-gray-400 mt-1">Gib den Code aus deiner Authenticator-App ein</p>
            </div>

            <form onSubmit={handleMfaVerify} className="space-y-4">
              {error && (
                <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg">
                  {error}
                </div>
              )}

              <div>
                <label htmlFor="mfaCode" className="label">
                  Code
                </label>
                <input
                  type="text"
                  id="mfaCode"
                  value={mfaCode}
                  onChange={(e) => setMfaCode(e.target.value)}
                  className="input text-center text-2xl tracking-widest"
                  placeholder="000000"
                  maxLength={6}
                  autoFocus
                  required
                />
              </div>

              <button type="submit" className="btn-primary w-full" disabled={isLoading}>
                {isLoading ? 'Überprüfe...' : 'Bestätigen'}
              </button>

              <button
                type="button"
                className="btn-secondary w-full"
                onClick={() => {
                  setShowMfa(false);
                  setMfaCode('');
                  setMfaToken('');
                }}
              >
                Zurück
              </button>
            </form>
          </div>
        </div>
      </div>
    );
  }

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
                <span className={`px-2 py-0.5 rounded text-xs ${!dbStats ? 'bg-gray-600 text-white' : isDbEmpty ? 'bg-red-600 text-white' : 'bg-green-600 text-white'}`}>
                  DB: {!dbStats ? 'N/A' : isDbEmpty ? 'Leer' : `${dbStats.users} User`}
                </span>
              )}
              <a
                href="https://localhost:17071"
                target="_blank"
                rel="noopener noreferrer"
                className="underline hover:no-underline"
              >
                Aspire
              </a>
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
              <p className="text-gray-600 dark:text-gray-400 mt-1">Anmelden</p>
            </div>

          {/* Mode Switcher */}
          <div className="flex rounded-lg bg-gray-100 dark:bg-gray-700 p-1 mb-6">
            <button
              type="button"
              className={`flex-1 py-2 px-4 rounded-md text-sm font-medium transition-colors ${
                mode === 'parent'
                  ? 'bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 shadow-sm'
                  : 'text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200'
              }`}
              onClick={() => setMode('parent')}
            >
              Eltern
            </button>
            <button
              type="button"
              className={`flex-1 py-2 px-4 rounded-md text-sm font-medium transition-colors ${
                mode === 'child'
                  ? 'bg-white dark:bg-gray-600 text-gray-900 dark:text-gray-100 shadow-sm'
                  : 'text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200'
              }`}
              onClick={() => setMode('child')}
            >
              Kind
            </button>
          </div>

          {error && (
            <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
              {error}
            </div>
          )}

          {mode === 'parent' ? (
            <form onSubmit={handleParentLogin} className="space-y-4">
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
                  placeholder="••••••••"
                  required
                />
              </div>

              <button type="submit" className="btn-primary w-full" disabled={isLoading}>
                {isLoading ? 'Anmelden...' : 'Anmelden'}
              </button>
            </form>
          ) : (
            <form onSubmit={handleChildLogin} className="space-y-4">
              <div>
                <label htmlFor="familyCode" className="label">
                  Familiencode
                </label>
                <input
                  type="text"
                  id="familyCode"
                  value={familyCode}
                  onChange={(e) => setFamilyCode(e.target.value.toUpperCase())}
                  className="input uppercase tracking-widest text-center"
                  placeholder="ABC123"
                  maxLength={6}
                  required
                />
              </div>

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
                  placeholder="Max"
                  required
                />
              </div>

              <div>
                <label htmlFor="pin" className="label">
                  PIN
                </label>
                <input
                  type="password"
                  id="pin"
                  value={pin}
                  onChange={(e) => setPin(e.target.value)}
                  className="input text-center tracking-widest"
                  placeholder="••••"
                  maxLength={6}
                  required
                />
              </div>

              <button type="submit" className="btn-primary w-full" disabled={isLoading}>
                {isLoading ? 'Anmelden...' : 'Anmelden'}
              </button>
            </form>
          )}

          {mode === 'parent' && (
            <p className="text-center text-sm text-gray-600 dark:text-gray-400 mt-6">
              Noch kein Konto?{' '}
              <Link to="/register" className="text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 font-medium">
                Registrieren
              </Link>
            </p>
          )}
        </div>
      </div>
    </div>
  </div>
  );
}
