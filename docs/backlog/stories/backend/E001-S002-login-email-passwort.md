# E001-S002: Login mit E-Mail + Passwort

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **registrierter Benutzer** möchte ich **mich mit meiner E-Mail-Adresse und meinem Passwort anmelden können**, damit **ich auf mein Konto zugreifen kann**.

## Akzeptanzkriterien

- [ ] Login mit gültiger E-Mail und Passwort ist möglich
- [ ] Bei ungültigen Anmeldedaten wird eine generische Fehlermeldung angezeigt (keine Hinweise ob E-Mail existiert)
- [ ] Nach erfolgreichem Login wird der MFA-Schritt eingeleitet
- [ ] Fehlgeschlagene Login-Versuche werden protokolliert
- [ ] Nach 5 fehlgeschlagenen Versuchen wird der Account temporär gesperrt (15 Min)
- [ ] JWT Access Token und Refresh Token werden nach vollständiger Authentifizierung (inkl. MFA) ausgestellt
- [ ] Login-Versuch wird in LoginAttempt-Tabelle protokolliert

## API-Endpunkt

```
POST /api/auth/login

Request:
{
  "email": "string",
  "password": "string"
}

Response 200 (MFA erforderlich):
{
  "mfaRequired": true,
  "mfaToken": "temporary-token",
  "availableMethods": ["totp", "backup_code"]
}

Response 401:
{
  "error": "Ungültige Anmeldedaten"
}

Response 423:
{
  "error": "Account vorübergehend gesperrt",
  "lockedUntil": "2024-01-20T12:30:00Z"
}
```

## Technische Hinweise

- Timing-sichere Passwort-Vergleiche verwenden
- Keine Unterscheidung zwischen "E-Mail nicht gefunden" und "Passwort falsch" in der Fehlermeldung
- Rate-Limiting: Max. 10 Login-Versuche pro IP/Minute
- mfaToken ist ein kurzlebiges Token (5 Min) für den MFA-Verifizierungsschritt
- IP-Adresse und User-Agent bei jedem Login-Versuch speichern

## Story Points

2

## Priorität

Hoch - MVP-Blocker
