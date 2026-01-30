# Story W003-S02: Logout-Funktionalität

## Epic

W003 - Web Benutzer-Menü & Authentifizierung

## Status

FERTIG

## User Story

Als **angemeldeter Benutzer** möchte ich **mich sicher abmelden können**, damit **mein Konto geschützt ist, wenn ich den Computer verlasse**.

## Akzeptanzkriterien

- [x] Gegeben ein angemeldeter Benutzer, wenn er auf "Abmelden" klickt, dann wird die Server-Session invalidiert
- [x] Gegeben ein Logout, wenn er durchgeführt wird, dann werden alle lokalen Tokens gelöscht (accessToken, refreshToken, user)
- [x] Gegeben ein Logout, wenn er erfolgreich ist, dann wird der Benutzer zur Login-Seite weitergeleitet
- [x] Gegeben ein Server-Fehler beim Logout, wenn er auftritt, dann wird der Benutzer trotzdem lokal abgemeldet

## Technische Implementierung

### AuthContext.tsx

```tsx
const logout = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  if (refreshToken) {
    try {
      await authApi.logout(refreshToken);  // POST /api/auth/logout
    } catch {
      // Fehler ignorieren, lokalen State trotzdem löschen
    }
  }
  setUser(null);
  apiClient.clearTokens();  // localStorage leeren
};
```

### auth.ts (API)

```tsx
async logout(refreshToken: string): Promise<void> {
  try {
    await apiClient.post('/auth/logout', { refreshToken });
  } finally {
    apiClient.clearTokens();
  }
}
```

### Layout.tsx (UI)

```tsx
const handleLogout = async () => {
  await logout();
  navigate('/login');
};

<button onClick={handleLogout}>
  Abmelden
</button>
```

## Ablauf

```
┌────────────────────────────────────────┐
│ User klickt "Abmelden"                 │
└──────────────┬─────────────────────────┘
               ▼
┌────────────────────────────────────────┐
│ POST /api/auth/logout { refreshToken } │
│ → Server invalidiert Session           │
└──────────────┬─────────────────────────┘
               ▼
┌────────────────────────────────────────┐
│ localStorage.clear()                   │
│ - accessToken                          │
│ - refreshToken                         │
│ - user                                 │
└──────────────┬─────────────────────────┘
               ▼
┌────────────────────────────────────────┐
│ setUser(null)                          │
│ navigate('/login')                     │
└────────────────────────────────────────┘
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-W003-10 | Normaler Logout | Server-Call, Tokens gelöscht, Redirect zu /login |
| TC-W003-11 | Server nicht erreichbar | Lokaler Logout erfolgreich, Redirect zu /login |
| TC-W003-12 | Dropdown schließt nach Logout | Menü ist zu, Redirect erfolgt |
| TC-W003-13 | Nach Logout geschützte Route | Redirect zu /login |

## Abhängigkeiten

- E001-S003 (Backend Logout API)

## Story Points

1

## Priorität

Hoch
