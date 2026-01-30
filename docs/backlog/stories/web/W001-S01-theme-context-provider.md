# Story W001-S01: Theme Context und Provider

## Epic

W001 - Web UI Dark Mode

## Status

**FERTIG** - 2026-01-22

## User Story

Als **Benutzer** möchte ich **dass meine Theme-Einstellung (Hell/Dunkel) gespeichert wird**, damit **ich beim nächsten Besuch nicht erneut umschalten muss**.

## Akzeptanzkriterien

- [x] Gegeben der ThemeContext, wenn er initialisiert wird, dann liest er die gespeicherte Präferenz aus LocalStorage
- [x] Gegeben keine gespeicherte Präferenz, wenn die App startet, dann wird die System-Präferenz verwendet
- [x] Gegeben ein Theme-Wechsel, wenn der Benutzer umschaltet, dann wird die neue Einstellung in LocalStorage gespeichert
- [x] Gegeben der ThemeProvider, wenn er die App umschließt, dann ist das Theme in allen Komponenten verfügbar

## Technische Hinweise

### src/contexts/ThemeContext.tsx

```typescript
import { createContext, useContext, useEffect, useState, ReactNode } from 'react';

type Theme = 'light' | 'dark';

interface ThemeContextType {
  theme: Theme;
  toggleTheme: () => void;
  setTheme: (theme: Theme) => void;
}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

const THEME_KEY = 'taschengeld-theme';

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setThemeState] = useState<Theme>(() => {
    // Check localStorage first
    const stored = localStorage.getItem(THEME_KEY);
    if (stored === 'light' || stored === 'dark') {
      return stored;
    }
    // Fall back to system preference
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  });

  useEffect(() => {
    // Apply theme to document
    const root = document.documentElement;
    if (theme === 'dark') {
      root.classList.add('dark');
    } else {
      root.classList.remove('dark');
    }
    // Persist preference
    localStorage.setItem(THEME_KEY, theme);
  }, [theme]);

  // Listen for system preference changes
  useEffect(() => {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    const handleChange = (e: MediaQueryListEvent) => {
      if (!localStorage.getItem(THEME_KEY)) {
        setThemeState(e.matches ? 'dark' : 'light');
      }
    };
    mediaQuery.addEventListener('change', handleChange);
    return () => mediaQuery.removeEventListener('change', handleChange);
  }, []);

  const toggleTheme = () => {
    setThemeState(prev => prev === 'light' ? 'dark' : 'light');
  };

  const setTheme = (newTheme: Theme) => {
    setThemeState(newTheme);
  };

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme, setTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

export function useTheme() {
  const context = useContext(ThemeContext);
  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
}
```

### App.tsx Integration

```typescript
import { ThemeProvider } from './contexts/ThemeContext';

function App() {
  return (
    <ThemeProvider>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          {/* ... routes */}
        </AuthProvider>
      </QueryClientProvider>
    </ThemeProvider>
  );
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W001-01 | App startet ohne gespeicherte Präferenz, System ist dark | Dark Mode aktiv |
| TC-W001-02 | App startet ohne gespeicherte Präferenz, System ist light | Light Mode aktiv |
| TC-W001-03 | App startet mit gespeicherter Präferenz "dark" | Dark Mode aktiv |
| TC-W001-04 | Theme wechseln | LocalStorage wird aktualisiert |
| TC-W001-05 | Seite neu laden | Gespeichertes Theme bleibt erhalten |

## Story Points

2

## Priorität

Hoch
