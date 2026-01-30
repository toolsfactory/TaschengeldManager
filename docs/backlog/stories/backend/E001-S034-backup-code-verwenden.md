# E001-S034: Backup-Code verwenden

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer ohne Zugriff auf mein Authenticator-Gerät** möchte ich **einen Backup-Code statt des TOTP-Codes verwenden können**, damit **ich trotzdem auf mein Konto zugreifen kann**.

## Akzeptanzkriterien

- [ ] Backup-Code kann anstelle des TOTP-Codes beim Login verwendet werden
- [ ] Jeder Backup-Code ist nur einmal verwendbar
- [ ] Nach Verwendung wird der Code als "verwendet" markiert
- [ ] Benutzer wird nach erfolgreichem Login informiert, wie viele Codes noch übrig sind
- [ ] Warnung bei wenigen verbleibenden Codes (< 3)
- [ ] Empfehlung, neue Codes zu generieren, wenn wenige übrig sind
- [ ] Format-Toleranz: Bindestriche und Leerzeichen werden ignoriert

## API-Endpunkt

```
POST /api/auth/mfa/backup-codes/verify

Request:
{
  "mfaToken": "temporary-token-from-login",
  "backupCode": "A1B2-C3D4-E5F6"
}

Response 200:
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "expiresIn": 3600,
  "remainingBackupCodes": 6,
  "warning": "Nur noch 6 Backup-Codes übrig. Bitte neue generieren."
}

Response 400:
{
  "error": "Ungültiger oder bereits verwendeter Backup-Code"
}
```

## Technische Hinweise

- Code-Eingabe normalisieren: Uppercase, Bindestriche/Leerzeichen entfernen
- Timing-sicherer Vergleich der Hashes
- Verwendeter Code: `usedAt` Timestamp setzen, nicht löschen (Audit)
- Rate-Limiting: Max. 5 Backup-Code-Versuche pro mfaToken
- Bei < 3 verbleibenden Codes: Warnung im Response
- Bei 0 verbleibenden Codes: Nur noch TOTP oder Passkey möglich

## Story Points

1

## Priorität

Hoch - MVP-Blocker
