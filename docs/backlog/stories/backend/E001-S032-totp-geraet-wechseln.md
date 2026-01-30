# E001-S032: TOTP-Gerät wechseln

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **mein TOTP auf ein neues Gerät übertragen können**, damit **ich nach einem Gerätewechsel weiterhin sicher auf mein Konto zugreifen kann**.

## Akzeptanzkriterien

- [ ] Benutzer muss sich mit aktuellem TOTP oder Backup-Code verifizieren
- [ ] Neues TOTP-Secret wird generiert
- [ ] Neuer QR-Code wird angezeigt
- [ ] Benutzer muss Code vom neuen Gerät eingeben zur Bestätigung
- [ ] Nach Bestätigung wird das alte Secret invalidiert
- [ ] Neue Backup-Codes werden generiert (alte werden ungültig)
- [ ] Alle aktiven Sessions bleiben bestehen

## API-Endpunkte

```
POST /api/auth/mfa/totp/reset

Request:
{
  "currentCode": "123456"  // oder Backup-Code
}

Response 200:
{
  "secret": "NEWBASE32SECRET",
  "qrCodeDataUrl": "data:image/png;base64,...",
  "manualEntryKey": "XXXX XXXX XXXX XXXX"
}

---

POST /api/auth/mfa/totp/reset/confirm

Request:
{
  "newCode": "654321"
}

Response 200:
{
  "success": true,
  "backupCodes": [
    "AAAA-BBBB-CCCC",
    ...
  ],
  "message": "TOTP erfolgreich auf neues Gerät übertragen"
}
```

## Technische Hinweise

- Altes Secret erst nach Bestätigung des neuen Codes löschen
- Temporäres neues Secret in separater Spalte speichern bis Bestätigung
- Reset-Prozess hat 10-Minuten-Timeout
- Audit-Log-Eintrag für TOTP-Reset erstellen
- E-Mail-Benachrichtigung an Benutzer senden
- Bei Kind-Accounts: Eltern werden benachrichtigt

## Story Points

2

## Priorität

Mittel
