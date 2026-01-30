# Story W001-S06: Formular- und Detail-Seiten Dark Mode

## Epic

W001 - Web UI Dark Mode

## Status

**FERTIG** - 2026-01-22

## User Story

Als **Benutzer** möchte ich **alle Formular- und Detailseiten im Dark Mode nutzen können**, damit **die gesamte Anwendung ein konsistentes dunkles Erscheinungsbild hat**.

## Akzeptanzkriterien

- [x] Gegeben die Konten-Übersicht (Accounts.tsx), wenn Dark Mode aktiv ist, dann sind alle Konten-Karten angepasst
- [x] Gegeben die Konto-Details (AccountDetails.tsx), wenn Dark Mode aktiv ist, dann sind Transaktionsliste und Aktionen lesbar
- [x] Gegeben die Transaktionshistorie, wenn Dark Mode aktiv ist, dann sind Einnahmen/Ausgaben farblich unterscheidbar
- [x] Gegeben alle Formulare (AddChild, FamilyCreate, etc.), wenn Dark Mode aktiv ist, dann sind Labels, Inputs und Buttons angepasst
- [x] Gegeben Modals/Dialoge, wenn Dark Mode aktiv ist, dann haben sie einen dunklen Hintergrund
- [x] Gegeben Tabellen (RecurringPayments, MoneyRequests), wenn Dark Mode aktiv ist, dann sind Zeilen abwechselnd und lesbar

## Technische Hinweise

### Accounts.tsx / AccountDetails.tsx

```tsx
// Account Card
<div className="bg-white dark:bg-gray-800 rounded-lg shadow hover:shadow-lg dark:shadow-gray-900/50 transition-shadow">
  <h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100">{nickname}</h3>
  <p className="text-gray-500 dark:text-gray-400">Kontostand</p>
  <p className="text-2xl font-bold text-green-600 dark:text-green-400">€ {balance}</p>
</div>

// Transaction List
<div className="divide-y divide-gray-200 dark:divide-gray-700">
  <div className="py-3 flex justify-between">
    <span className="text-gray-900 dark:text-gray-100">{description}</span>
    <span className={type === 'deposit'
      ? 'text-green-600 dark:text-green-400'
      : 'text-red-600 dark:text-red-400'
    }>
      {type === 'deposit' ? '+' : '-'}€ {amount}
    </span>
  </div>
</div>
```

### TransactionHistory.tsx

```tsx
// Filter Buttons
<button className={`px-3 py-1 rounded-full text-sm ${
  filter === 'all'
    ? 'bg-blue-600 text-white'
    : 'bg-gray-200 dark:bg-gray-700 text-gray-700 dark:text-gray-300'
}`}>

// Date Headers
<div className="text-sm font-medium text-gray-500 dark:text-gray-400 py-2">
  {formatDate(date)}
</div>
```

### Form Pages (AddChild, FamilyCreate, InviteMember, etc.)

```tsx
// Page Container
<div className="max-w-md mx-auto">
  <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
    <h1 className="text-xl font-bold text-gray-900 dark:text-gray-100 mb-6">
      Kind hinzufügen
    </h1>

    <form className="space-y-4">
      <div>
        <label className="label">Spitzname</label>
        <input className="input" />
      </div>

      // Help Text
      <p className="text-sm text-gray-500 dark:text-gray-400">
        Die PIN muss 4 Ziffern haben
      </p>

      // Action Buttons
      <div className="flex space-x-3">
        <button className="btn-secondary flex-1">Abbrechen</button>
        <button className="btn-primary flex-1">Speichern</button>
      </div>
    </form>
  </div>
</div>
```

### RecurringPayments.tsx / MoneyRequests.tsx (Tabellen)

```tsx
// Table Container
<div className="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
  <table className="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
    <thead className="bg-gray-50 dark:bg-gray-900">
      <tr>
        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
          Beschreibung
        </th>
      </tr>
    </thead>
    <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
      <tr className="hover:bg-gray-50 dark:hover:bg-gray-700">
        <td className="px-6 py-4 text-gray-900 dark:text-gray-100">
          {item.description}
        </td>
      </tr>
    </tbody>
  </table>
</div>

// Status Badges
<span className={`px-2 py-1 text-xs font-semibold rounded-full ${
  status === 'pending'
    ? 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300'
    : status === 'approved'
    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300'
    : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300'
}`}>
```

### MfaSetup.tsx

```tsx
// QR Code Container (heller Hintergrund für QR)
<div className="bg-white p-4 rounded-lg inline-block">
  <QRCode value={totpUri} />
</div>

// Backup Codes
<div className="bg-gray-100 dark:bg-gray-900 rounded-lg p-4 font-mono text-sm">
  {backupCodes.map(code => (
    <div key={code} className="text-gray-800 dark:text-gray-200">{code}</div>
  ))}
</div>
```

### FamilyManage.tsx

```tsx
// Member List
<div className="bg-white dark:bg-gray-800 rounded-lg shadow">
  <div className="p-4 border-b border-gray-200 dark:border-gray-700">
    <h2 className="text-lg font-semibold text-gray-900 dark:text-gray-100">Familienmitglieder</h2>
  </div>

  // Member Row
  <div className="p-4 flex items-center justify-between hover:bg-gray-50 dark:hover:bg-gray-700">
    <div>
      <p className="font-medium text-gray-900 dark:text-gray-100">{member.nickname}</p>
      <p className="text-sm text-gray-500 dark:text-gray-400">{member.role}</p>
    </div>
    <button className="text-red-600 dark:text-red-400 hover:text-red-800 dark:hover:text-red-300">
      Entfernen
    </button>
  </div>
</div>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W001-50 | Accounts-Übersicht im Dark Mode | Alle Konto-Karten dunkel |
| TC-W001-51 | AccountDetails im Dark Mode | Transaktionen mit korrekten Farben |
| TC-W001-52 | AddChild-Formular im Dark Mode | Alle Felder lesbar und funktional |
| TC-W001-53 | FamilyManage im Dark Mode | Mitgliederliste mit Hover-Effekten |
| TC-W001-54 | RecurringPayments-Tabelle im Dark Mode | Zeilen abwechselnd, Header dunkel |
| TC-W001-55 | MoneyRequests Status-Badges | Farben im Dark Mode angepasst |
| TC-W001-56 | MFA-Setup QR-Code | QR-Code auf hellem Hintergrund lesbar |
| TC-W001-57 | Transaktionshistorie Filter | Aktiver Filter hervorgehoben |

## Story Points

3

## Priorität

Mittel
