# E001-S052: Kind: Biometrie als primärer zweiter Faktor

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Kind mit eigenem Smartphone** möchte ich **mich hauptsächlich mit Fingerabdruck oder Gesichtserkennung anmelden**, damit **ich mich nicht komplizierte Codes merken muss und trotzdem sicher bin**.

## Akzeptanzkriterien

- [ ] Biometrie wird für Kinder als empfohlene MFA-Methode angeboten
- [ ] Nach Ersteinrichtung (PIN + TOTP) kann Biometrie aktiviert werden
- [ ] Biometrie ersetzt TOTP im Alltag (aber TOTP bleibt als Fallback)
- [ ] Eltern müssen Biometrie-Aktivierung bestätigen
- [ ] Kindgerechte Erklärung ("Dein Fingerabdruck ist dein Geheimcode")
- [ ] Bei Gerätewechsel: Neue Biometrie-Einrichtung mit Eltern-Bestätigung
- [ ] Längere Token-Gültigkeit für Kinder (21 Tage statt 14)

## API-Endpunkte

```
POST /api/auth/biometric/enable/child/{childId}

(Erfordert Eltern-Authentifizierung)

Request:
{
  "deviceId": "unique-device-identifier",
  "deviceName": "Max's iPhone",
  "platform": "iOS",
  "parentMfaCode": "123456"
}

Response 201:
{
  "biometricToken": "secure-token",
  "expiresAt": "2024-02-10T10:30:00Z",
  "deviceId": "unique-device-identifier",
  "message": "Toll! Du kannst dich jetzt mit deinem Fingerabdruck anmelden!"
}

---

GET /api/auth/biometric/child/{childId}/devices

(Eltern können Geräte des Kindes einsehen)

Response 200:
{
  "devices": [
    {
      "deviceId": "guid",
      "deviceName": "Max's iPhone",
      "platform": "iOS",
      "activatedAt": "2024-01-15T10:30:00Z",
      "lastUsedAt": "2024-01-20T08:15:00Z"
    }
  ]
}
```

## Technische Hinweise

- Biometrie-Token für Kinder: 21 Tage Gültigkeit (konfigurierbar)
- Eltern-Bestätigung via Push-Notification oder MFA-Code
- Kind kann max. 2 Geräte mit Biometrie haben
- Bei Änderung der Biometrie am Gerät: Eltern werden benachrichtigt
- Fallback auf PIN + TOTP wenn Biometrie fehlschlägt
- Eltern können Biometrie für Kind jederzeit remote deaktivieren

## Story Points

2

## Priorität

Hoch - MVP für Mobile
