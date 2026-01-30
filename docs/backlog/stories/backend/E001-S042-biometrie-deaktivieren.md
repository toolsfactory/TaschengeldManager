# E001-S042: Biometrie deaktivieren

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Mobile-App-Nutzer** möchte ich **Biometrie für ein Gerät deaktivieren können**, damit **ich die Kontrolle über meine Anmeldemethoden behalte und nicht mehr verwendete Geräte entfernen kann**.

## Akzeptanzkriterien

- [ ] Benutzer kann Biometrie für das aktuelle Gerät deaktivieren
- [ ] Benutzer kann Biometrie für andere Geräte remote deaktivieren
- [ ] MFA-Verifizierung vor Deaktivierung erforderlich
- [ ] Biometrie-Token wird sofort invalidiert
- [ ] Gerät muss sich danach vollständig neu authentifizieren
- [ ] Deaktivierung wird protokolliert
- [ ] Liste aller Geräte mit aktiver Biometrie einsehbar

## API-Endpunkte

```
GET /api/auth/biometric/devices

Response 200:
{
  "devices": [
    {
      "deviceId": "guid",
      "deviceName": "iPhone 15 Pro",
      "platform": "iOS",
      "activatedAt": "2024-01-15T10:30:00Z",
      "lastUsedAt": "2024-01-20T08:15:00Z",
      "expiresAt": "2024-02-03T08:15:00Z"
    },
    {
      "deviceId": "guid",
      "deviceName": "iPad Air",
      "platform": "iOS",
      "activatedAt": "2024-01-10T14:00:00Z",
      "lastUsedAt": "2024-01-18T20:30:00Z",
      "expiresAt": "2024-02-01T20:30:00Z"
    }
  ]
}

---

DELETE /api/auth/biometric/{deviceId}

Request:
{
  "mfaCode": "123456"
}

Response 200:
{
  "message": "Biometrie für Gerät deaktiviert",
  "deviceName": "iPhone 15 Pro"
}
```

## Technische Hinweise

- Token in Datenbank als invalidiert markieren (nicht löschen - Audit)
- Bei Remote-Deaktivierung: Push-Benachrichtigung an Gerät senden
- App muss Token aus lokaler Keychain/KeyStore löschen
- Automatische Bereinigung abgelaufener Tokens (Hintergrund-Job)
- Bei Kinder-Accounts: Eltern können Biometrie für Kind-Geräte deaktivieren

## Story Points

2

## Priorität

Mittel
