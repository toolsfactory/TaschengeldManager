# E001-S041: Login mit Biometrie

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Mobile-App-Nutzer mit aktivierter Biometrie** möchte ich **mich mit Face ID oder Fingerprint anmelden können**, damit **ich schnellen Zugriff auf mein Konto habe, ohne Passwort und MFA-Code eingeben zu müssen**.

## Akzeptanzkriterien

- [ ] Login mit Biometrie ist nach erfolgreicher Aktivierung möglich
- [ ] Biometrie-Prüfung erfolgt auf dem Gerät (nicht Server)
- [ ] Bei erfolgreicher Biometrie wird Biometrie-Token an Server gesendet
- [ ] Server validiert Token und stellt neue Access/Refresh Tokens aus
- [ ] Bei ungültigem/abgelaufenem Token: Fallback auf vollständigen Login
- [ ] Biometrie-Login protokolliert Gerät und Zeitpunkt
- [ ] Token-Gültigkeit wird bei jedem Login um 14 Tage verlängert

## API-Endpunkt

```
POST /api/auth/biometric/verify

Request:
{
  "deviceId": "unique-device-identifier",
  "biometricToken": "secure-token"
}

Response 200:
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "expiresIn": 3600,
  "biometricTokenRefreshed": true,
  "newBiometricToken": "new-secure-token",
  "newBiometricExpiresAt": "2024-02-03T10:30:00Z"
}

Response 401:
{
  "error": "Biometrie-Token ungültig oder abgelaufen",
  "requiresFullLogin": true
}
```

## Technische Hinweise

- Biometrie-Verifizierung findet lokal auf dem Gerät statt
- Server kennt keine biometrischen Daten (nur Token)
- Token-Rotation: Bei jedem erfolgreichen Login neues Token ausstellen
- Altes Token nach erfolgreicher Rotation invalidieren
- Rate-Limiting: Max. 10 Biometrie-Versuche pro Gerät/Stunde
- Bei 3 fehlgeschlagenen Versuchen: Biometrie für dieses Gerät sperren

## Story Points

2

## Priorität

Hoch - MVP für Mobile
