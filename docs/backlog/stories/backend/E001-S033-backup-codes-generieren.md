# E001-S033: Backup-Codes generieren

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer mit aktiviertem MFA** möchte ich **Backup-Codes generieren können**, damit **ich auch ohne Zugriff auf mein Authenticator-Gerät auf mein Konto zugreifen kann**.

## Akzeptanzkriterien

- [ ] 10 Backup-Codes werden bei MFA-Einrichtung automatisch generiert
- [ ] Benutzer kann jederzeit neue Backup-Codes anfordern (alte werden ungültig)
- [ ] Codes werden nur einmal angezeigt (Download/Kopieren empfohlen)
- [ ] Jeder Code kann nur einmal verwendet werden
- [ ] Codes werden in lesbarem Format angezeigt (z.B. XXXX-XXXX-XXXX)
- [ ] Benutzer muss sich mit aktuellem MFA verifizieren um neue Codes zu generieren
- [ ] Anzahl verbleibender Codes ist einsehbar

## API-Endpunkte

```
POST /api/auth/mfa/backup-codes/generate

Request:
{
  "mfaCode": "123456"
}

Response 200:
{
  "backupCodes": [
    "A1B2-C3D4-E5F6",
    "G7H8-I9J0-K1L2",
    ...
  ],
  "count": 10,
  "warning": "Diese Codes werden nur einmal angezeigt. Bitte sicher aufbewahren!"
}

---

GET /api/auth/mfa/backup-codes/status

Response 200:
{
  "remainingCodes": 7,
  "totalCodes": 10,
  "lastGenerated": "2024-01-15T10:30:00Z"
}
```

## Technische Hinweise

- Backup-Codes: 12 Zeichen alphanumerisch (ohne verwechselbare Zeichen: 0/O, 1/I/l)
- Codes werden mit Argon2id gehashed gespeichert
- Bei Generierung neuer Codes: Alle alten Codes löschen
- Warnung anzeigen, wenn nur noch 2 Codes übrig
- Codes in Datenbank mit Index für schnelle Suche
- Format: Großbuchstaben und Zahlen, gruppiert für Lesbarkeit

## Story Points

2

## Priorität

Hoch - MVP-Blocker
