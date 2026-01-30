import { useState } from 'react';
import { authApi } from '../api';
import { useAuth } from '../contexts/AuthContext';
import type { SetupTotpResponse } from '../types';

export function MfaSetup() {
  const { user, refreshUser } = useAuth();
  const [step, setStep] = useState<'initial' | 'setup' | 'verify' | 'backup'>('initial');
  const [setupData, setSetupData] = useState<SetupTotpResponse | null>(null);
  const [verifyCode, setVerifyCode] = useState('');
  const [backupCodes, setBackupCodes] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleStartSetup = async () => {
    setError('');
    setIsLoading(true);

    try {
      const data = await authApi.setupTotp();
      setSetupData(data);
      setStep('setup');
    } catch (err) {
      setError('TOTP-Setup konnte nicht gestartet werden');
    } finally {
      setIsLoading(false);
    }
  };

  const handleVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!setupData) return;

    setError('');
    setIsLoading(true);

    try {
      await authApi.activateTotp(setupData.setupToken, verifyCode);
      setStep('backup');
    } catch (err) {
      setError('Code ungültig. Bitte versuche es erneut.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleGenerateBackupCodes = async () => {
    setError('');
    setIsLoading(true);

    try {
      const data = await authApi.generateBackupCodes();
      setBackupCodes(data.codes);
      await refreshUser();
    } catch (err) {
      setError('Backup-Codes konnten nicht generiert werden');
    } finally {
      setIsLoading(false);
    }
  };

  if (step === 'initial') {
    return (
      <div className="max-w-md mx-auto">
        <div className="card">
          <div className="text-center mb-6">
            <span className="text-4xl">🔐</span>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">
              Zwei-Faktor-Authentifizierung
            </h1>
            <p className="text-gray-600 dark:text-gray-400 mt-1">
              Schütze dein Konto mit einer zusätzlichen Sicherheitsebene.
            </p>
          </div>

          {error && (
            <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
              {error}
            </div>
          )}

          <div className="space-y-4">
            {user?.mfaEnabled ? (
              <div className="p-4 bg-green-50 dark:bg-green-900/30 rounded-lg">
                <div className="flex items-center space-x-3">
                  <span className="text-2xl">✅</span>
                  <div>
                    <p className="font-medium text-green-800 dark:text-green-300">MFA ist aktiviert</p>
                    <p className="text-sm text-green-600 dark:text-green-400">
                      Dein Konto ist mit 2FA geschützt.
                    </p>
                  </div>
                </div>
              </div>
            ) : (
              <div className="p-4 bg-yellow-50 dark:bg-yellow-900/30 rounded-lg">
                <div className="flex items-center space-x-3">
                  <span className="text-2xl">⚠️</span>
                  <div>
                    <p className="font-medium text-yellow-800 dark:text-yellow-300">MFA nicht aktiviert</p>
                    <p className="text-sm text-yellow-600 dark:text-yellow-400">
                      Aktiviere 2FA für mehr Sicherheit.
                    </p>
                  </div>
                </div>
              </div>
            )}

            {!user?.mfaEnabled && (
              <button
                onClick={handleStartSetup}
                disabled={isLoading}
                className="btn-primary w-full"
              >
                {isLoading ? 'Wird vorbereitet...' : 'MFA einrichten'}
              </button>
            )}

            <button
              onClick={handleGenerateBackupCodes}
              disabled={isLoading}
              className="btn-secondary w-full"
            >
              {isLoading ? 'Generiere...' : 'Neue Backup-Codes generieren'}
            </button>
          </div>

          <div className="mt-6 p-4 bg-blue-50 dark:bg-blue-900/30 rounded-lg">
            <h3 className="font-medium text-blue-900 dark:text-blue-200">Was ist 2FA?</h3>
            <ul className="mt-2 text-sm text-blue-700 dark:text-blue-300 space-y-1">
              <li>• Zusätzlicher Schutz neben deinem Passwort</li>
              <li>• Verwendet eine Authenticator-App</li>
              <li>• Generiert alle 30 Sekunden einen neuen Code</li>
            </ul>
          </div>
        </div>
      </div>
    );
  }

  if (step === 'setup' && setupData) {
    return (
      <div className="max-w-md mx-auto">
        <div className="card">
          <div className="text-center mb-6">
            <span className="text-4xl">📱</span>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">App einrichten</h1>
            <p className="text-gray-600 dark:text-gray-400 mt-1">
              Scanne den QR-Code mit deiner Authenticator-App.
            </p>
          </div>

          {error && (
            <div className="bg-red-50 dark:bg-red-900/30 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-400 px-4 py-3 rounded-lg mb-4">
              {error}
            </div>
          )}

          <div className="space-y-4">
            {/* QR Code Placeholder - keep white background for readability */}
            <div className="flex justify-center p-4 bg-white border dark:border-gray-600 rounded-lg">
              <img
                src={`https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(
                  setupData.qrCodeUri
                )}`}
                alt="QR Code"
                className="w-48 h-48"
              />
            </div>

            <div className="p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <p className="text-xs text-gray-500 dark:text-gray-400 mb-1">Oder manuell eingeben:</p>
              <code className="text-sm font-mono break-all dark:text-gray-200">{setupData.secretKey}</code>
            </div>

            <form onSubmit={handleVerify} className="space-y-4">
              <div>
                <label className="label">Code aus der App eingeben</label>
                <input
                  type="text"
                  value={verifyCode}
                  onChange={(e) => setVerifyCode(e.target.value)}
                  className="input text-center text-2xl tracking-widest"
                  placeholder="000000"
                  maxLength={6}
                  required
                />
              </div>

              <button type="submit" disabled={isLoading} className="btn-primary w-full">
                {isLoading ? 'Überprüfe...' : 'Code bestätigen'}
              </button>
            </form>

            <button onClick={() => setStep('initial')} className="btn-secondary w-full">
              Abbrechen
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (step === 'backup') {
    return (
      <div className="max-w-md mx-auto">
        <div className="card">
          <div className="text-center mb-6">
            <span className="text-4xl">🎉</span>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">MFA aktiviert!</h1>
            <p className="text-gray-600 dark:text-gray-400 mt-1">
              Sichere jetzt deine Backup-Codes.
            </p>
          </div>

          {backupCodes.length > 0 ? (
            <div className="space-y-4">
              <div className="p-4 bg-yellow-50 dark:bg-yellow-900/30 border border-yellow-200 dark:border-yellow-800 rounded-lg">
                <p className="text-sm text-yellow-800 dark:text-yellow-300 font-medium mb-2">
                  ⚠️ Speichere diese Codes sicher ab!
                </p>
                <p className="text-xs text-yellow-700 dark:text-yellow-400">
                  Falls du keinen Zugriff auf deine Authenticator-App hast, kannst du diese Codes verwenden.
                </p>
              </div>

              <div className="grid grid-cols-2 gap-2">
                {backupCodes.map((code, index) => (
                  <div
                    key={index}
                    className="p-2 bg-gray-100 dark:bg-gray-700 rounded font-mono text-center text-sm dark:text-gray-200"
                  >
                    {code}
                  </div>
                ))}
              </div>

              <button
                onClick={() => {
                  navigator.clipboard.writeText(backupCodes.join('\n'));
                  alert('Codes wurden in die Zwischenablage kopiert!');
                }}
                className="btn-secondary w-full"
              >
                📋 Codes kopieren
              </button>

              <button onClick={() => setStep('initial')} className="btn-primary w-full">
                Fertig
              </button>
            </div>
          ) : (
            <div className="space-y-4">
              <p className="text-gray-600 text-center">
                Generiere Backup-Codes für den Notfall.
              </p>

              <button
                onClick={handleGenerateBackupCodes}
                disabled={isLoading}
                className="btn-primary w-full"
              >
                {isLoading ? 'Generiere...' : 'Backup-Codes generieren'}
              </button>

              <button onClick={() => setStep('initial')} className="btn-secondary w-full">
                Überspringen
              </button>
            </div>
          )}
        </div>
      </div>
    );
  }

  return null;
}
