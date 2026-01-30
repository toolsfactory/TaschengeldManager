# E001-S036: Login mit Passkey

## Status: Phase 2

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer mit registriertem Passkey** möchte ich **mich nur mit meinem Passkey anmelden können**, damit **ich kein Passwort eingeben muss und trotzdem sicher authentifiziert bin**.

## Akzeptanzkriterien

- [ ] Login mit Passkey ohne Passwort möglich
- [ ] Browser/Gerät zeigt nativen Passkey-Dialog an
- [ ] Passkey ersetzt sowohl Passwort als auch MFA (zwei Faktoren in einem)
- [ ] Nach erfolgreichem Passkey-Login werden JWT und Refresh Token ausgestellt
- [ ] Sign Count wird geprüft und aktualisiert (Replay-Schutz)
- [ ] Fallback auf Passwort + TOTP weiterhin möglich
- [ ] LastUsedAt des Passkeys wird aktualisiert

## API-Endpunkte

```
POST /api/auth/passkey/authenticate/begin

Request:
{
  "email": "user@example.com"  // optional für Discoverable Credentials
}

Response 200:
{
  "challenge": "base64-challenge",
  "timeout": 60000,
  "rpId": "taschengeldmanager.de",
  "allowCredentials": [  // leer für Discoverable
    {
      "type": "public-key",
      "id": "base64-credential-id"
    }
  ],
  "userVerification": "required"
}

---

POST /api/auth/passkey/authenticate/complete

Request:
{
  "credential": {
    "id": "credential-id",
    "rawId": "base64-raw-id",
    "response": {
      "clientDataJSON": "base64",
      "authenticatorData": "base64",
      "signature": "base64",
      "userHandle": "base64"
    },
    "type": "public-key"
  }
}

Response 200:
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "expiresIn": 3600,
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "name": "Max Mustermann"
  }
}

Response 401:
{
  "error": "Passkey-Authentifizierung fehlgeschlagen"
}
```

## Technische Hinweise

- Discoverable Credentials (Resident Keys) bevorzugen
- User Verification ist verpflichtend (Faktor 2 durch Biometrie/PIN am Gerät)
- Sign Count vergleichen: Muss >= gespeichertem Wert sein
- Bei Sign Count Anomalie: Potentiell geklonter Passkey, Session blocken
- Challenge nur 5 Minuten gültig
- Passkey-Login protokollieren mit verwendetem Credential

## Story Points

3

## Priorität

Mittel - Phase 2
