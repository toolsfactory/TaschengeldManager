# E001-S063: Vertrauenswürdige Geräte verwalten

## Status: Phase 2

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **bestimmte Geräte als vertrauenswürdig markieren können**, damit **ich auf diesen Geräten nicht bei jedem Login MFA durchführen muss**.

## Akzeptanzkriterien

- [ ] Gerät kann nach erfolgreichem MFA-Login als vertrauenswürdig markiert werden
- [ ] Auf vertrauenswürdigen Geräten: Nur Passwort erforderlich (MFA wird übersprungen)
- [ ] Vertrauenswürdigkeit gilt für 30 Tage (konfigurierbar)
- [ ] Liste aller vertrauenswürdigen Geräte einsehbar
- [ ] Vertrauenswürdigkeit kann jederzeit entfernt werden
- [ ] Bei Passwortänderung: Alle vertrauenswürdigen Geräte werden zurückgesetzt
- [ ] Maximal 5 vertrauenswürdige Geräte pro Benutzer

## API-Endpunkte

```
POST /api/auth/trusted-devices

(Nach erfolgreichem MFA-Login)

Request:
{
  "deviceFingerprint": "unique-device-hash",
  "deviceName": "Mein Arbeits-PC"
}

Response 201:
{
  "trustedDeviceId": "guid",
  "deviceName": "Mein Arbeits-PC",
  "expiresAt": "2024-02-20T10:30:00Z",
  "message": "Gerät als vertrauenswürdig gespeichert"
}

---

GET /api/auth/trusted-devices

Response 200:
{
  "devices": [
    {
      "id": "guid",
      "deviceName": "Mein Arbeits-PC",
      "browser": "Chrome 120",
      "platform": "Windows 11",
      "trustedAt": "2024-01-20T10:30:00Z",
      "expiresAt": "2024-02-20T10:30:00Z",
      "lastUsedAt": "2024-01-20T15:00:00Z"
    }
  ],
  "maxDevices": 5,
  "remainingSlots": 4
}

---

DELETE /api/auth/trusted-devices/{id}

Response 200:
{
  "message": "Vertrauenswürdiges Gerät entfernt"
}

---

POST /api/auth/login

Request:
{
  "email": "user@example.com",
  "password": "string",
  "deviceFingerprint": "unique-device-hash"
}

Response 200 (Vertrauenswürdiges Gerät):
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "mfaSkipped": true,
  "trustedDevice": "Mein Arbeits-PC"
}
```

## Technische Hinweise

- Device-Fingerprint: Hash aus Browser-Eigenschaften (Canvas, Fonts, etc.)
- Fingerprint wird gehashed gespeichert
- Bei Änderungen am Gerät (Browser-Update): Neuer Fingerprint, erneut MFA
- Vertrauenswürdigkeit verlängert sich nicht automatisch
- Sicherheitshinweis: Nur auf privaten Geräten aktivieren
- Nicht verfügbar für Kind-Accounts (immer MFA erforderlich)

## Story Points

3

## Priorität

Niedrig - Phase 2
