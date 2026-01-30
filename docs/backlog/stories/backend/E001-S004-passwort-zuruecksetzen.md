# E001-S004: Passwort zurücksetzen

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer, der sein Passwort vergessen hat** möchte ich **mein Passwort über meine E-Mail-Adresse zurücksetzen können**, damit **ich wieder Zugang zu meinem Konto erhalte**.

## Akzeptanzkriterien

- [ ] Passwort-Reset kann über E-Mail-Adresse angefordert werden
- [ ] Reset-Link wird per E-Mail versendet
- [ ] Reset-Token ist nur 1 Stunde gültig
- [ ] Reset-Token kann nur einmal verwendet werden
- [ ] Bei Anfrage für nicht-existierende E-Mail wird trotzdem "E-Mail versendet" angezeigt (Sicherheit)
- [ ] Nach Passwort-Reset werden alle aktiven Sessions beendet
- [ ] Neues Passwort muss den Passwort-Richtlinien entsprechen
- [ ] MFA bleibt nach Passwort-Reset aktiv

## API-Endpunkte

```
POST /api/auth/forgot-password

Request:
{
  "email": "string"
}

Response 200:
{
  "message": "Falls ein Account mit dieser E-Mail existiert, wurde ein Reset-Link versendet"
}

---

POST /api/auth/reset-password

Request:
{
  "token": "string",
  "newPassword": "string"
}

Response 200:
{
  "message": "Passwort erfolgreich geändert"
}

Response 400:
{
  "error": "Token ungültig oder abgelaufen"
}
```

## Technische Hinweise

- Reset-Token als kryptografisch sicherer Zufallsstring (min. 32 Bytes)
- Token gehashed in Datenbank speichern
- Rate-Limiting: Max. 3 Reset-Anfragen pro E-Mail/Stunde
- E-Mail-Template mit klarer Warnung bei nicht angefordertem Reset
- Nach erfolgreichen Reset: Alle Sessions invalidieren, Login erforderlich

## Story Points

3

## Priorität

Hoch - MVP-Blocker
