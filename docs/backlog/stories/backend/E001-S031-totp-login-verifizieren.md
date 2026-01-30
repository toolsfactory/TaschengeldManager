# E001-S031: TOTP bei Login verifizieren

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer mit aktiviertem TOTP** möchte ich **nach der Passworteingabe meinen TOTP-Code eingeben müssen**, damit **mein Account durch den zweiten Faktor geschützt ist**.

## Akzeptanzkriterien

- [ ] Nach erfolgreichem Passwort-Login wird TOTP-Code abgefragt
- [ ] Gültiger 6-stelliger Code wird akzeptiert (bzw. 4-stellig für Kinder)
- [ ] Code ist nur einmal verwendbar (Replay-Schutz)
- [ ] Toleranz von +/- 1 Zeitfenster wird akzeptiert
- [ ] Nach 3 fehlgeschlagenen TOTP-Versuchen wird auf Backup-Code verwiesen
- [ ] Bei Erfolg werden JWT und Refresh Token ausgestellt
- [ ] Option zur Verwendung eines Backup-Codes wird angeboten

## API-Endpunkt

```
POST /api/auth/mfa/totp/verify

Request:
{
  "mfaToken": "temporary-token-from-login",
  "code": "123456"
}

Response 200:
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "expiresIn": 3600
}

Response 400:
{
  "error": "Ungültiger Code",
  "attemptsRemaining": 2
}

Response 429:
{
  "error": "Zu viele Versuche. Bitte verwende einen Backup-Code.",
  "canUseBackupCode": true
}
```

## Technische Hinweise

- Replay-Schutz: Letzten verwendeten Zeitstempel speichern
- mfaToken ist kurzlebig (5 Minuten) und einmalig verwendbar
- Rate-Limiting auf TOTP-Verifizierung (max. 5 Versuche pro mfaToken)
- Clock-Drift-Toleranz: Codes aus t-1 und t+1 akzeptieren
- Bei Erfolg: mfaToken invalidieren
- Fehlgeschlagene MFA-Versuche im LoginAttempt protokollieren

## Story Points

2

## Priorität

Hoch - MVP-Blocker
