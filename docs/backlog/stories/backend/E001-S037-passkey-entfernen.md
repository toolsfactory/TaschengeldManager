# E001-S037: Passkey entfernen

## Status: Phase 2

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **einen registrierten Passkey entfernen können**, damit **ich nicht mehr verwendete Geräte aus meinem Account entfernen kann**.

## Akzeptanzkriterien

- [ ] Liste aller registrierten Passkeys wird angezeigt
- [ ] Benutzer kann einzelne Passkeys löschen
- [ ] MFA-Verifizierung vor Löschung erforderlich
- [ ] Letzter Passkey kann nur gelöscht werden, wenn alternative MFA aktiv ist
- [ ] Gerätename und Registrierungsdatum werden angezeigt
- [ ] Bestätigungsdialog vor endgültiger Löschung
- [ ] Audit-Log-Eintrag wird erstellt

## API-Endpunkte

```
GET /api/auth/mfa/passkeys

Response 200:
{
  "passkeys": [
    {
      "id": "guid",
      "deviceName": "MacBook Pro",
      "createdAt": "2024-01-15T10:30:00Z",
      "lastUsedAt": "2024-01-20T08:15:00Z"
    },
    {
      "id": "guid",
      "deviceName": "iPhone 15",
      "createdAt": "2024-01-10T14:00:00Z",
      "lastUsedAt": "2024-01-19T20:30:00Z"
    }
  ]
}

---

DELETE /api/auth/mfa/passkeys/{id}

Request:
{
  "mfaCode": "123456"
}

Response 200:
{
  "message": "Passkey erfolgreich entfernt",
  "remainingPasskeys": 1
}

Response 400:
{
  "error": "Letzter Passkey kann nicht entfernt werden ohne alternative MFA-Methode"
}
```

## Technische Hinweise

- Soft-Delete mit Aufbewahrung für Audit-Zwecke (30 Tage)
- Prüfung: Mindestens eine MFA-Methode muss aktiv bleiben
- E-Mail-Benachrichtigung bei Passkey-Löschung
- Credential ID aus WebAuthn-Registrierung wird ungültig
- Bei Löschung: Keine aktiven Sessions beenden

## Story Points

2

## Priorität

Mittel - Phase 2
