# E001-S030: TOTP einrichten (QR-Code scannen)

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **TOTP (Time-based One-Time Password) mit einer Authenticator-App einrichten können**, damit **ich meinen Account mit einem zweiten Faktor absichern kann**.

## Akzeptanzkriterien

- [ ] System generiert ein TOTP-Secret für den Benutzer
- [ ] QR-Code wird angezeigt, der in Authenticator-Apps gescannt werden kann
- [ ] Alternativ wird das Secret als Text angezeigt (für manuelle Eingabe)
- [ ] Benutzer muss einen gültigen Code eingeben, um TOTP zu aktivieren
- [ ] Nach erfolgreicher Aktivierung werden Backup-Codes generiert und angezeigt
- [ ] TOTP-Secret wird verschlüsselt in der Datenbank gespeichert
- [ ] QR-Code enthält korrektes otpauth://-Format mit App-Name und Benutzername

## API-Endpunkte

```
POST /api/auth/mfa/totp/setup

Response 200:
{
  "secret": "BASE32SECRET",
  "qrCodeDataUrl": "data:image/png;base64,...",
  "manualEntryKey": "XXXX XXXX XXXX XXXX",
  "issuer": "TaschengeldManager"
}

---

POST /api/auth/mfa/totp/verify-setup

Request:
{
  "code": "123456"
}

Response 200:
{
  "success": true,
  "backupCodes": [
    "AAAA-BBBB-CCCC",
    "DDDD-EEEE-FFFF",
    ...
  ]
}

Response 400:
{
  "error": "Ungültiger Code"
}
```

## Technische Hinweise

- TOTP nach RFC 6238 implementieren
- Secret: 20 Bytes (160 Bit), Base32-kodiert
- Zeitfenster: 30 Sekunden
- Algorithmus: SHA-1 (Standard für Kompatibilität)
- Code-Länge: 6 Ziffern (Standard) oder 4 Ziffern (Kind-Modus)
- QR-Code-Format: `otpauth://totp/TaschengeldManager:{email}?secret={secret}&issuer=TaschengeldManager`
- Secret mit AES-256-GCM verschlüsseln vor Speicherung
- Toleranz: 1 Zeitfenster vor/nach (90 Sekunden total)

## Story Points

3

## Priorität

Hoch - MVP-Blocker
