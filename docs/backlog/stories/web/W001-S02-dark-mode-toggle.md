# Story W001-S02: Dark Mode Toggle-Komponente

## Epic

W001 - Web UI Dark Mode

## Status

**FERTIG** - 2026-01-22

## User Story

Als **Benutzer** möchte ich **auf jeder Seite einen Schalter haben, um zwischen Hell- und Dunkel-Modus zu wechseln**, damit **ich jederzeit das Farbschema ändern kann**.

## Akzeptanzkriterien

- [x] Gegeben die Layout-Komponente, wenn sie gerendert wird, dann ist der Toggle-Schalter im Header sichtbar
- [x] Gegeben der Toggle-Schalter, wenn er angeklickt wird, dann wechselt das Theme sofort
- [x] Gegeben der aktive Modus, wenn er "dark" ist, dann zeigt der Toggle ein Sonnen-Symbol (zum Wechsel auf Light)
- [x] Gegeben der aktive Modus, wenn er "light" ist, dann zeigt der Toggle ein Mond-Symbol (zum Wechsel auf Dark)
- [x] Gegeben der Toggle, wenn er gerendert wird, dann ist er barrierefrei (aria-label)

## Technische Hinweise

### src/components/ThemeToggle.tsx

```typescript
import { useTheme } from '../contexts/ThemeContext';

export function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();

  return (
    <button
      onClick={toggleTheme}
      className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
      aria-label={theme === 'dark' ? 'Zu hellem Modus wechseln' : 'Zu dunklem Modus wechseln'}
      title={theme === 'dark' ? 'Heller Modus' : 'Dunkler Modus'}
    >
      {theme === 'dark' ? (
        // Sun icon for switching to light mode
        <svg
          className="w-5 h-5 text-yellow-400"
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path
            fillRule="evenodd"
            d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414zM4 11a1 1 0 100-2H3a1 1 0 000 2h1z"
            clipRule="evenodd"
          />
        </svg>
      ) : (
        // Moon icon for switching to dark mode
        <svg
          className="w-5 h-5 text-gray-600"
          fill="currentColor"
          viewBox="0 0 20 20"
        >
          <path
            d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z"
          />
        </svg>
      )}
    </button>
  );
}
```

### Integration in Layout.tsx (Header)

```tsx
import { ThemeToggle } from './ThemeToggle';

// Im Header, neben dem User-Dropdown:
<div className="flex items-center space-x-4">
  <ThemeToggle />
  {/* User dropdown ... */}
</div>
```

### Alternative: Toggle auf Login/Register (vor Auth)

Für nicht eingeloggte Seiten den Toggle ebenfalls einbinden:

```tsx
// Login.tsx / Register.tsx - oben rechts
<div className="absolute top-4 right-4">
  <ThemeToggle />
</div>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W001-10 | Toggle sichtbar im Header | Toggle-Button wird angezeigt |
| TC-W001-11 | Click auf Toggle im Light Mode | Wechsel zu Dark Mode, Mond zu Sonne |
| TC-W001-12 | Click auf Toggle im Dark Mode | Wechsel zu Light Mode, Sonne zu Mond |
| TC-W001-13 | Tastatur-Navigation | Toggle per Tab erreichbar, per Enter/Space aktivierbar |
| TC-W001-14 | Toggle auf Login-Seite | Toggle funktioniert auch ohne Authentifizierung |

## Story Points

2

## Priorität

Hoch
