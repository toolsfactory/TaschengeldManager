# E001-S003: Logout (alle Geräte)

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **angemeldeter Benutzer** möchte ich **mich abmelden können, optional von allen Geräten gleichzeitig**, damit **ich meine Sitzung sicher beenden und unbefugten Zugriff verhindern kann**.

## Akzeptanzkriterien

- [ ] Standard-Logout beendet nur die aktuelle Session
- [ ] Option "Von allen Geräten abmelden" beendet alle aktiven Sessions
- [ ] Refresh Token wird bei Logout invalidiert
- [ ] Access Token wird zur Blocklist hinzugefügt (für verbleibende Gültigkeitsdauer)
- [ ] Biometrie-Token werden bei "alle Geräte" ebenfalls invalidiert
- [ ] Nach Logout ist kein Zugriff mit dem alten Token mehr möglich

## API-Endpunkte

```
POST /api/auth/logout

Request:
{
  "allDevices": false
}

Response 200:
{
  "message": "Erfolgreich abgemeldet"
}

---

Alternative für alle Geräte:
DELETE /api/auth/sessions/all

Response 200:
{
  "message": "Von allen Geräten abgemeldet",
  "sessionsTerminated": 3
}
```

## Technische Hinweise

- Token-Blocklist mit Redis oder In-Memory-Cache implementieren
- Blocklist-Einträge nur für verbleibende Token-Lebensdauer speichern
- Bei "alle Geräte": Alle Refresh Tokens und Biometrie-Tokens des Users invalidieren
- Session-Tabelle: IsRevoked auf true setzen

## Story Points

2

## Priorität

Hoch - MVP-Blocker
