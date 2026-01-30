# Story W001-S04: Login- und Register-Seiten Dark Mode

## Epic

W001 - Web UI Dark Mode

## Status

**FERTIG** - 2026-01-22

## User Story

Als **Benutzer** möchte ich **auch auf den Login- und Registrierungsseiten den Dark Mode nutzen können**, damit **ich bereits vor dem Einloggen meine bevorzugte Darstellung habe**.

## Akzeptanzkriterien

- [x] Gegeben die Login-Seite, wenn Dark Mode aktiv ist, dann sind Hintergrund, Formulare und Texte angepasst
- [x] Gegeben die Register-Seite, wenn Dark Mode aktiv ist, dann sind alle Elemente gut lesbar
- [x] Gegeben der Theme-Toggle, wenn er auf Login/Register angezeigt wird, dann funktioniert er ohne Authentifizierung
- [x] Gegeben der Development-Banner, wenn Dark Mode aktiv ist, dann ist er weiterhin erkennbar
- [x] Gegeben Tab-Wechsel (Eltern/Kind) auf Login, wenn Dark Mode aktiv ist, dann sind aktive/inaktive Tabs unterscheidbar

## Technische Hinweise

### Login.tsx - Anpassungen

```tsx
// Haupt-Container
<div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex flex-col justify-center py-12 sm:px-6 lg:px-8">

// Logo-Bereich
<div className="text-center">
  <h1 className="text-4xl font-bold text-green-600 dark:text-green-400">💰 TaschengeldManager</h1>
  <p className="mt-2 text-gray-600 dark:text-gray-400">Dein Taschengeld im Griff</p>
</div>

// Card
<div className="bg-white dark:bg-gray-800 py-8 px-4 shadow sm:rounded-lg sm:px-10">

// Tab-Buttons
<button
  className={`... ${activeTab === 'parent'
    ? 'bg-green-600 text-white dark:bg-green-500'
    : 'bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-300 dark:hover:bg-gray-600'}`}
>

// Labels
<label className="block text-sm font-medium text-gray-700 dark:text-gray-300">

// Input-Felder (nutzen .input Klasse)
<input className="input" ... />

// Error-Messages
<div className="text-red-600 dark:text-red-400 text-sm">

// Links
<a className="text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300">
```

### Register.tsx - Anpassungen

```tsx
// Analog zu Login.tsx
<div className="min-h-screen bg-gray-50 dark:bg-gray-900 ...">
<div className="bg-white dark:bg-gray-800 ...">

// Passwort-Stärke Indikator
<div className={`h-1 rounded ${
  strength === 'weak' ? 'bg-red-500' :
  strength === 'medium' ? 'bg-yellow-500 dark:bg-yellow-400' :
  'bg-green-500 dark:bg-green-400'
}`} />
```

### Theme-Toggle auf Auth-Seiten

Beide Seiten sollten den Toggle oben rechts haben:

```tsx
import { ThemeToggle } from '../components/ThemeToggle';

// Am Anfang der Komponente, absolut positioniert
<div className="absolute top-4 right-4 z-10">
  <ThemeToggle />
</div>
```

### Development Banner Anpassung

```tsx
{import.meta.env.DEV && (
  <div className="bg-amber-100 dark:bg-amber-900 border-b border-amber-200 dark:border-amber-800 px-4 py-2 text-sm">
    <span className="font-semibold text-amber-800 dark:text-amber-200">Development Mode</span>
    <span className="text-amber-700 dark:text-amber-300"> | DB: {dbStatus}</span>
    <a href="..." className="text-blue-600 dark:text-blue-400 hover:underline">API Docs</a>
  </div>
)}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W001-30 | Login-Seite im Light Mode | Standard helle Darstellung |
| TC-W001-31 | Login-Seite im Dark Mode | Dunkler Hintergrund, helle Texte |
| TC-W001-32 | Login Tab-Wechsel im Dark Mode | Aktiver Tab klar erkennbar |
| TC-W001-33 | Register-Seite im Dark Mode | Alle Felder gut lesbar |
| TC-W001-34 | Error-Meldungen im Dark Mode | Rot lesbar auf dunklem Hintergrund |
| TC-W001-35 | Toggle auf Login-Seite | Wechsel funktioniert vor Auth |
| TC-W001-36 | Dev-Banner im Dark Mode | Weiterhin sichtbar und lesbar |

## Story Points

2

## Priorität

Mittel
