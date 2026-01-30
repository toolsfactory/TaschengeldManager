# Story W002-S05: Eltern-Ansicht für Kinderkonten

## Epic

W002 - Kassenbuch-Ansicht für Kinder

## Status

FERTIG

## User Story

Als **Elternteil** möchte ich **das Kassenbuch meiner Kinder einsehen können**, damit **ich ihre Finanzen überprüfen und bei Bedarf unterstützen kann**.

## Akzeptanzkriterien

- [x] Gegeben ein Elternteil, wenn es auf ein Kinderkonto klickt, dann kann es die Kassenbuch-Ansicht aufrufen
- [x] Gegeben die Eltern-Ansicht, wenn sie geöffnet wird, dann ist ein Dropdown zur Auswahl des Kindes verfügbar
- [x] Gegeben die Kassenbuch-Ansicht für ein Kind, wenn ein Elternteil sie sieht, dann sind alle Funktionen identisch zur Kind-Ansicht
- [x] Gegeben die Navigation, wenn ein Elternteil eingeloggt ist, dann gibt es einen Link "Kassenbücher" oder die Funktion ist über die Kontodetails erreichbar

## Technische Hinweise

### Route für Eltern-Ansicht

```tsx
// App.tsx - Neue Route
<Route
  path="/accounts/:accountId/cashbook"
  element={<ProtectedRoute><Cashbook /></ProtectedRoute>}
/>
```

### Cashbook.tsx - Erweiterung für Eltern

```tsx
import { useParams } from 'react-router-dom';

export function Cashbook() {
  const { accountId } = useParams<{ accountId?: string }>();
  const { user } = useAuth();

  const isParentView = !!accountId && user?.role === UserRole.Parent;
  const [selectedChild, setSelectedChild] = useState<string | null>(accountId || null);
  const [children, setChildren] = useState<AccountDto[]>([]);

  useEffect(() => {
    if (user?.role === UserRole.Parent) {
      loadFamilyAccounts();
    }
  }, [user]);

  const loadFamilyAccounts = async () => {
    // Lade alle Kinderkonten der Familie
    const families = await familyApi.getMyFamilies();
    if (families.length > 0) {
      const accounts = await accountApi.getFamilyAccounts(families[0].id);
      setChildren(accounts);
      if (!selectedChild && accounts.length > 0) {
        setSelectedChild(accounts[0].id);
      }
    }
  };

  // Kind-Auswahl für Eltern
  if (user?.role === UserRole.Parent && !accountId) {
    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
          📖 Kassenbücher
        </h1>

        {/* Kind-Auswahl */}
        <div className="card">
          <label className="label">Kind auswählen</label>
          <select
            value={selectedChild || ''}
            onChange={(e) => setSelectedChild(e.target.value)}
            className="input"
          >
            {children.map((child) => (
              <option key={child.id} value={child.id}>
                {child.ownerName}
              </option>
            ))}
          </select>
        </div>

        {/* Kassenbuch-Inhalt */}
        {selectedChild && (
          <CashbookContent accountId={selectedChild} />
        )}
      </div>
    );
  }

  // Standard Kind-Ansicht
  return <CashbookContent />;
}
```

### Link in AccountDetails.tsx (Eltern-Ansicht)

```tsx
// Button zum Kassenbuch hinzufügen
<Link
  to={`/accounts/${account.id}/cashbook`}
  className="btn-secondary"
>
  📖 Kassenbuch ansehen
</Link>
```

### Navigation für Eltern (Layout.tsx)

```tsx
// Neuer Nav-Link für Eltern
{
  to: '/cashbooks',
  label: '📖 Kassenbücher',
  roles: [UserRole.Parent]
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W002-40 | Elternteil öffnet Kassenbücher | Kind-Auswahl wird angezeigt |
| TC-W002-41 | Elternteil wählt Kind | Kassenbuch des Kindes wird geladen |
| TC-W002-42 | Elternteil öffnet Konto-Details | Link "Kassenbuch ansehen" ist sichtbar |
| TC-W002-43 | Elternteil klickt auf Kassenbuch-Link | Kassenbuch für dieses Kind wird angezeigt |
| TC-W002-44 | Familie mit mehreren Kindern | Alle Kinder sind im Dropdown wählbar |
| TC-W002-45 | Elternteil ohne Kinder | Meldung "Keine Kinderkonten vorhanden" |

## Story Points

2

## Priorität

Mittel
