# E001-S038: Mehrere Passkeys verwalten

## Status: Phase 2

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer mit mehreren Geräten** möchte ich **Passkeys für alle meine Geräte registrieren und verwalten können**, damit **ich von jedem meiner Geräte bequem und sicher auf meinen Account zugreifen kann**.

## Akzeptanzkriterien

- [ ] Bis zu 10 Passkeys pro Account möglich
- [ ] Übersicht aller registrierten Passkeys mit Details
- [ ] Passkey-Namen können nachträglich geändert werden
- [ ] Sortierung nach letzter Verwendung oder Erstellungsdatum
- [ ] Hinweis bei lange nicht verwendeten Passkeys (> 90 Tage)
- [ ] Empfehlung zur Löschung inaktiver Passkeys
- [ ] Unterscheidung zwischen Plattform- und Roaming-Authenticators

## API-Endpunkte

```
GET /api/auth/mfa/passkeys

Response 200:
{
  "passkeys": [
    {
      "id": "guid",
      "deviceName": "MacBook Pro",
      "authenticatorType": "platform",
      "createdAt": "2024-01-15T10:30:00Z",
      "lastUsedAt": "2024-01-20T08:15:00Z",
      "isInactive": false
    },
    {
      "id": "guid",
      "deviceName": "YubiKey 5",
      "authenticatorType": "cross-platform",
      "createdAt": "2023-10-01T09:00:00Z",
      "lastUsedAt": "2023-10-15T11:00:00Z",
      "isInactive": true
    }
  ],
  "maxPasskeys": 10,
  "remainingSlots": 8
}

---

PATCH /api/auth/mfa/passkeys/{id}

Request:
{
  "deviceName": "Arbeits-MacBook"
}

Response 200:
{
  "id": "guid",
  "deviceName": "Arbeits-MacBook",
  "message": "Passkey-Name aktualisiert"
}
```

## Technische Hinweise

- Authenticator-Typ aus Attestation auslesen (platform vs. cross-platform)
- Inaktiv-Schwelle konfigurierbar (Standard: 90 Tage)
- Bei Erreichen des Limits: Hinweis auf inaktive Passkeys zur Löschung
- Statistik: Welcher Passkey wird am häufigsten verwendet
- Backup-Empfehlung: Mindestens ein Hardware-Token (YubiKey) registrieren

## Story Points

3

## Priorität

Niedrig - Phase 2
