# E001-S040: Biometrie aktivieren (Face ID / Fingerprint)

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Mobile-App-Nutzer** möchte ich **Biometrie (Face ID oder Fingerprint) für den Login aktivieren können**, damit **ich mich schnell und bequem anmelden kann, ohne jedes Mal Passwort und MFA eingeben zu müssen**.

## Akzeptanzkriterien

- [ ] Biometrie kann nach erfolgreichem Login aktiviert werden
- [ ] Gerät muss Biometrie unterstützen (Face ID, Touch ID, Fingerprint)
- [ ] Benutzer muss sich vollständig authentifizieren (Passwort + MFA) vor Aktivierung
- [ ] Ein gerätespezifisches Biometrie-Token wird generiert
- [ ] Token ist 14 Tage gültig (konfigurierbar)
- [ ] Benutzer wird über Sicherheitsimplikationen informiert
- [ ] Biometrie-Aktivierung wird protokolliert

## API-Endpunkt

```
POST /api/auth/biometric/enable

Request:
{
  "deviceId": "unique-device-identifier",
  "deviceName": "iPhone 15 Pro",
  "platform": "iOS"
}

Response 201:
{
  "biometricToken": "secure-token",
  "expiresAt": "2024-02-03T10:30:00Z",
  "deviceId": "unique-device-identifier",
  "message": "Biometrie erfolgreich aktiviert"
}

Response 400:
{
  "error": "Biometrie bereits für dieses Gerät aktiviert"
}
```

## Technische Hinweise

- Biometrie-Token: Kryptografisch sicherer 256-Bit-Token
- Token wird gehashed in Datenbank gespeichert
- Gerät speichert Token sicher in Keychain (iOS) / KeyStore (Android)
- Device-ID aus Keychain generieren (persistent, aber nicht trackbar)
- Bei Biometrie-Änderung am Gerät (neue Fingerprints): Token invalidieren
- Pro Benutzer max. 5 Geräte mit Biometrie

## Story Points

3

## Priorität

Hoch - MVP für Mobile
