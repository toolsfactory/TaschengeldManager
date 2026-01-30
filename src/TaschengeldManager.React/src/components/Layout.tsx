import { useEffect, useState, useRef } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { ThemeToggle } from './ThemeToggle';
import { UserRole } from '../types';

interface NavItem {
  label: string;
  path: string;
  roles?: UserRole[];
}

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/dashboard' },
  { label: 'Konten', path: '/accounts', roles: [UserRole.Parent] },
  { label: 'Familie', path: '/family', roles: [UserRole.Parent] },
  { label: 'Kassenbücher', path: '/cashbooks', roles: [UserRole.Parent] },
  { label: 'Automatische Zahlungen', path: '/recurring-payments', roles: [UserRole.Parent] },
  { label: 'Geld-Anfragen', path: '/money-requests', roles: [UserRole.Parent] },
  { label: 'Kassenbuch', path: '/cashbook', roles: [UserRole.Child] },
  { label: 'Meine Anfragen', path: '/my-requests', roles: [UserRole.Child] },
  { label: 'Transaktionen', path: '/account/history', roles: [UserRole.Child] },
];

interface DbStats {
  users: number;
  families: number;
  accounts: number;
  transactions: number;
}

export function Layout({ children }: { children: React.ReactNode }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [dbStats, setDbStats] = useState<DbStats | null>(null);
  const [dbLoading, setDbLoading] = useState(true);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const userMenuRef = useRef<HTMLDivElement>(null);

  const isDevelopment = import.meta.env.DEV;

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
        setIsUserMenuOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  useEffect(() => {
    if (isDevelopment) {
      fetch('/api/dev/stats')
        .then((res) => (res.ok ? res.json() : null))
        .then((data) => setDbStats(data))
        .catch(() => setDbStats(null))
        .finally(() => setDbLoading(false));
    }
  }, [isDevelopment]);

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const visibleNavItems = navItems.filter(
    (item) => !item.roles || (user && item.roles.includes(user.role))
  );

  const getRoleLabel = (role: UserRole) => {
    switch (role) {
      case UserRole.Parent:
        return 'Elternteil';
      case UserRole.Child:
        return 'Kind';
      case UserRole.Relative:
        return 'Verwandte/r';
      default:
        return role;
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

      {/* Header */}
      <header className="bg-white dark:bg-gray-800 shadow-sm dark:shadow-gray-900/50 border-b border-gray-200 dark:border-gray-700 transition-colors">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* Logo */}
            <Link to="/dashboard" className="flex items-center space-x-2">
              <span className="text-2xl">💰</span>
              <span className="text-xl font-bold text-gray-900 dark:text-gray-100">TaschengeldManager</span>
            </Link>

            {/* User Menu */}
            <div className="flex items-center space-x-4">
              <ThemeToggle />
              <div className="relative" ref={userMenuRef}>
                <button
                  onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                  className="flex items-center space-x-1 text-gray-600 dark:text-gray-300 hover:text-gray-900 dark:hover:text-gray-100 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 dark:focus:ring-offset-gray-800 rounded-full"
                  aria-expanded={isUserMenuOpen}
                  aria-haspopup="true"
                >
                  <div className="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                    <span className="text-blue-600 dark:text-blue-400 font-medium">
                      {user?.nickname?.charAt(0).toUpperCase()}
                    </span>
                  </div>
                  <svg
                    className={`w-4 h-4 transition-transform ${isUserMenuOpen ? 'rotate-180' : ''}`}
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                  </svg>
                </button>
                {isUserMenuOpen && (
                  <div className="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50">
                    <div className="py-1">
                      <div className="px-4 py-2 border-b border-gray-200 dark:border-gray-700">
                        <p className="text-sm font-medium text-gray-900 dark:text-gray-100">{user?.nickname}</p>
                        <p className="text-xs text-gray-500 dark:text-gray-400">{user && getRoleLabel(user.role)}</p>
                      </div>
                      <Link
                        to="/settings/mfa"
                        onClick={() => setIsUserMenuOpen(false)}
                        className="block px-4 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700"
                      >
                        MFA-Einstellungen
                      </Link>
                      <button
                        onClick={() => {
                          setIsUserMenuOpen(false);
                          handleLogout();
                        }}
                        className="block w-full text-left px-4 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-gray-100 dark:hover:bg-gray-700"
                      >
                        Abmelden
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </header>

      {/* Navigation */}
      <nav className="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 transition-colors">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex space-x-8 overflow-x-auto">
            {visibleNavItems.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={`py-4 px-1 border-b-2 text-sm font-medium whitespace-nowrap ${
                  location.pathname === item.path ||
                  (item.path !== '/dashboard' && location.pathname.startsWith(item.path))
                    ? 'border-blue-500 text-blue-600 dark:border-blue-400 dark:text-blue-400'
                    : 'border-transparent text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 hover:border-gray-300 dark:hover:border-gray-600'
                }`}
              >
                {item.label}
              </Link>
            ))}
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">{children}</main>

      {/* Footer */}
      <footer className="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 mt-auto transition-colors">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <p className="text-center text-sm text-gray-500 dark:text-gray-400">
            © {new Date().getFullYear()} TaschengeldManager - Familien-Finanzverwaltung
          </p>
        </div>
      </footer>
    </div>
  );
}
