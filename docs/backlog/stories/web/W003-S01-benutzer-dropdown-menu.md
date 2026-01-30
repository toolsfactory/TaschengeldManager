# Story W003-S01: Benutzer-Dropdown-Menü

## Epic

W003 - Web Benutzer-Menü & Authentifizierung

## Status

FERTIG

## User Story

Als **angemeldeter Benutzer** möchte ich **ein Dropdown-Menü mit meinen Kontofunktionen sehen**, damit **ich schnell auf Einstellungen und Abmelden zugreifen kann**.

## Akzeptanzkriterien

- [x] Gegeben ein angemeldeter Benutzer, wenn er auf seinen Avatar klickt, dann öffnet sich ein Dropdown-Menü
- [x] Gegeben das Dropdown-Menü ist offen, wenn der Benutzer außerhalb klickt, dann schließt sich das Menü
- [x] Gegeben das Dropdown-Menü, wenn es angezeigt wird, dann zeigt es Benutzername und Rolle
- [x] Gegeben das Dropdown-Menü, wenn es angezeigt wird, dann enthält es Links zu MFA-Einstellungen und Abmelden
- [x] Gegeben ein Tastaturnutzer, wenn er das Menü fokussiert, dann ist ein Focus-Ring sichtbar

## Technische Implementierung

### Layout.tsx

```tsx
// State für Dropdown
const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
const userMenuRef = useRef<HTMLDivElement>(null);

// Click-Outside Handler
useEffect(() => {
  const handleClickOutside = (event: MouseEvent) => {
    if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
      setIsUserMenuOpen(false);
    }
  };
  document.addEventListener('mousedown', handleClickOutside);
  return () => document.removeEventListener('mousedown', handleClickOutside);
}, []);

// Dropdown Button mit Chevron
<button
  onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
  aria-expanded={isUserMenuOpen}
  aria-haspopup="true"
>
  <div className="w-8 h-8 bg-blue-100 rounded-full">
    {user?.nickname?.charAt(0).toUpperCase()}
  </div>
  <ChevronIcon className={isUserMenuOpen ? 'rotate-180' : ''} />
</button>

// Dropdown Inhalt
{isUserMenuOpen && (
  <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg">
    <div className="px-4 py-2 border-b">
      <p>{user?.nickname}</p>
      <p>{getRoleLabel(user.role)}</p>
    </div>
    <Link to="/settings/mfa">MFA-Einstellungen</Link>
    <button onClick={handleLogout}>Abmelden</button>
  </div>
)}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W003-01 | Klick auf Avatar | Dropdown öffnet sich |
| TC-W003-02 | Klick außerhalb | Dropdown schließt sich |
| TC-W003-03 | Erneuter Klick auf Avatar | Dropdown schließt sich |
| TC-W003-04 | Dark Mode | Dropdown hat dunkle Farben |
| TC-W003-05 | Mobile Ansicht | Dropdown ist touch-freundlich |

## Story Points

2

## Priorität

Hoch
