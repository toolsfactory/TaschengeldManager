# Story W001-S03: Tailwind Dark Mode Konfiguration

## Epic

W001 - Web UI Dark Mode

## Status

**FERTIG** - 2026-01-22

## User Story

Als **Entwickler** möchte ich **Tailwind CSS so konfigurieren, dass Dark Mode über eine CSS-Klasse gesteuert wird**, damit **der Dark Mode programmatisch per JavaScript umgeschaltet werden kann**.

## Akzeptanzkriterien

- [x] Gegeben die Tailwind-Konfiguration, wenn darkMode auf "class" gesetzt ist, dann reagieren dark:-Klassen auf die "dark"-Klasse am html-Element
- [x] Gegeben die index.css, wenn sie aktualisiert wird, dann sind globale Dark-Mode-Stile definiert
- [x] Gegeben die benutzerdefinierten CSS-Klassen (btn, card, input), wenn Dark Mode aktiv ist, dann passen sie sich an
- [x] Gegeben das Farbschema, wenn es definiert ist, dann sind Kontrast und Lesbarkeit in beiden Modi gewährleistet

## Technische Hinweise

### tailwind.config.js erstellen (falls nicht vorhanden)

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class', // Wichtig: class-basierter Dark Mode
  theme: {
    extend: {
      // Optionale Theme-Erweiterungen
    },
  },
  plugins: [],
}
```

### Alternativ: Tailwind v4 CSS-basierte Konfiguration

In `src/index.css` oben hinzufügen:

```css
@import "tailwindcss";

/* Dark Mode aktivieren über class-Strategie */
@custom-variant dark (&:where(.dark, .dark *));
```

### src/index.css - Aktualisierte Custom Classes

```css
@import "tailwindcss";

/* Dark Mode Variante */
@custom-variant dark (&:where(.dark, .dark *));

/* Base Layer für Dark Mode */
@layer base {
  body {
    @apply bg-gray-50 text-gray-900 dark:bg-gray-900 dark:text-gray-100;
  }
}

/* Custom Component Classes */
@layer components {
  .btn {
    @apply px-4 py-2 rounded-lg font-medium transition-colors duration-200 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed;
  }

  .btn-primary {
    @apply btn bg-blue-600 text-white hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600;
  }

  .btn-secondary {
    @apply btn bg-gray-200 text-gray-800 hover:bg-gray-300 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600;
  }

  .btn-danger {
    @apply btn bg-red-600 text-white hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600;
  }

  .btn-success {
    @apply btn bg-green-600 text-white hover:bg-green-700 dark:bg-green-500 dark:hover:bg-green-600;
  }

  .input {
    @apply w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors
           dark:bg-gray-800 dark:border-gray-600 dark:text-gray-100 dark:focus:ring-blue-400 dark:focus:border-blue-400;
  }

  .card {
    @apply bg-white rounded-xl shadow-md p-6
           dark:bg-gray-800 dark:shadow-gray-900/50;
  }

  .label {
    @apply block text-sm font-medium text-gray-700 mb-1
           dark:text-gray-300;
  }

  .error-text {
    @apply text-sm text-red-600 mt-1
           dark:text-red-400;
  }
}
```

### Dark Mode Farbpalette

| Element | Light Mode | Dark Mode |
|---------|------------|-----------|
| Background | gray-50 (#F9FAFB) | gray-900 (#111827) |
| Surface/Card | white (#FFFFFF) | gray-800 (#1F2937) |
| Text Primary | gray-900 (#111827) | gray-100 (#F3F4F6) |
| Text Secondary | gray-600 (#4B5563) | gray-400 (#9CA3AF) |
| Primary Action | blue-600 (#2563EB) | blue-500 (#3B82F6) |
| Border | gray-300 (#D1D5DB) | gray-600 (#4B5563) |
| Error | red-600 (#DC2626) | red-400 (#F87171) |
| Success | green-600 (#16A34A) | green-500 (#22C55E) |

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W001-20 | HTML-Element hat class "dark" | Alle dark:-Präfixe werden angewendet |
| TC-W001-21 | Body im Dark Mode | Hintergrund dunkel, Text hell |
| TC-W001-22 | Buttons im Dark Mode | Lesbare Farben, guter Kontrast |
| TC-W001-23 | Input-Felder im Dark Mode | Dunkel mit hellem Text |
| TC-W001-24 | Cards im Dark Mode | Dezent abgesetzter Hintergrund |
| TC-W001-25 | Farbkontrast prüfen (WCAG) | Mindestens 4.5:1 für Text |

## Story Points

3

## Priorität

Hoch
