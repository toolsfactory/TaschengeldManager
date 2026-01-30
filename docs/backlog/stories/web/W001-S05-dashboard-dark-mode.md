# Story W001-S05: Dashboard-Seiten Dark Mode

## Epic

W001 - Web UI Dark Mode

## Status

**FERTIG** - 2026-01-22

## User Story

Als **eingeloggter Benutzer** möchte ich **das Dashboard und alle Übersichtsseiten im Dark Mode nutzen können**, damit **ich die App auch bei schlechten Lichtverhältnissen angenehm bedienen kann**.

## Akzeptanzkriterien

- [x] Gegeben das Parent-Dashboard, wenn Dark Mode aktiv ist, dann sind Stats-Karten, Listen und Buttons angepasst
- [x] Gegeben das Child-Dashboard, wenn Dark Mode aktiv ist, dann ist die Kontostand-Anzeige mit Gradient lesbar
- [x] Gegeben das Relative-Dashboard, wenn Dark Mode aktiv ist, dann ist die Geschenk-Oberfläche angepasst
- [x] Gegeben die Layout-Komponente (Header, Navigation, Footer), wenn Dark Mode aktiv ist, dann sind alle Bereiche angepasst
- [x] Gegeben Hover-Effekte, wenn Dark Mode aktiv ist, dann sind sie weiterhin sichtbar und angemessen

## Technische Hinweise

### Layout.tsx - Anpassungen

```tsx
// Header
<header className="bg-white dark:bg-gray-800 shadow-sm dark:shadow-gray-900/50">
  <div className="text-green-600 dark:text-green-400 font-bold">💰 TaschengeldManager</div>
  // User-Info
  <span className="text-gray-700 dark:text-gray-300">{user.nickname}</span>
  // Dropdown
  <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg">
    <button className="hover:bg-gray-50 dark:hover:bg-gray-700 text-gray-700 dark:text-gray-200">
</header>

// Navigation
<nav className="bg-gray-100 dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700">
  <a className={isActive
    ? 'text-green-600 dark:text-green-400 font-semibold'
    : 'text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-200'
  }>
</nav>

// Main Content Area
<main className="flex-1 bg-gray-50 dark:bg-gray-900">

// Footer
<footer className="bg-gray-100 dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700">
  <span className="text-gray-600 dark:text-gray-400">© 2025 TaschengeldManager</span>
</footer>
```

### Dashboard.tsx - Parent Dashboard

```tsx
// Stats Cards
<div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
  <div className="text-gray-500 dark:text-gray-400 text-sm">Gesamtguthaben</div>
  <div className="text-2xl font-bold text-gray-900 dark:text-gray-100">€ 150,00</div>
</div>

// Kinder-Liste
<div className="bg-white dark:bg-gray-800 rounded-lg shadow">
  <div className="border-b border-gray-200 dark:border-gray-700 p-4">
    <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">Kinder</h2>
  </div>
  <div className="divide-y divide-gray-200 dark:divide-gray-700">
    <div className="p-4 hover:bg-gray-50 dark:hover:bg-gray-700">
      <span className="text-gray-900 dark:text-gray-100">{child.nickname}</span>
      <span className="text-gray-500 dark:text-gray-400">€ {balance}</span>
    </div>
  </div>
</div>

// Quick Action Buttons
<button className="bg-green-50 dark:bg-green-900/30 text-green-700 dark:text-green-300 hover:bg-green-100 dark:hover:bg-green-900/50">
```

### Dashboard.tsx - Child Dashboard

```tsx
// Balance Card mit Gradient (bleibt gut lesbar)
<div className="bg-gradient-to-r from-green-500 to-green-600 dark:from-green-600 dark:to-green-700 text-white rounded-xl p-6">
  <div className="text-white/80">Mein Guthaben</div>
  <div className="text-3xl font-bold">€ 75,50</div>
</div>

// Interest Info Banner
<div className="bg-blue-50 dark:bg-blue-900/30 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
  <span className="text-blue-800 dark:text-blue-200">Dein Zinssatz: 2%</span>
</div>
```

### Dashboard.tsx - Relative Dashboard

```tsx
// Gift Form
<div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
  <h2 className="text-xl font-semibold text-gray-900 dark:text-gray-100">Geschenk senden</h2>
  // Select
  <select className="input">
  // Amount Input
  <input className="input" />
</div>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W001-40 | Parent Dashboard im Dark Mode | Stats-Karten dunkel mit hellem Text |
| TC-W001-41 | Kinder-Liste im Dark Mode | Zeilen unterscheidbar, Hover sichtbar |
| TC-W001-42 | Child Balance-Card im Dark Mode | Gradient weiterhin ansprechend |
| TC-W001-43 | Navigation im Dark Mode | Aktiver Link hervorgehoben |
| TC-W001-44 | Header Dropdown im Dark Mode | Menü-Items lesbar |
| TC-W001-45 | Footer im Dark Mode | Copyright-Text sichtbar |
| TC-W001-46 | Relative Dashboard im Dark Mode | Geschenk-Form funktional |

## Story Points

3

## Priorität

Mittel
