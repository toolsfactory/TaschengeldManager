# E001-S035: Passkey registrieren (WebAuthn)

## Status: Phase 2

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **einen Passkey für meinen Account registrieren können**, damit **ich mich sicher und bequem ohne Passwort anmelden kann**.

## Akzeptanzkriterien

- [ ] WebAuthn-Registrierungsflow wird gestartet
- [ ] Browser/Gerät zeigt nativen Passkey-Dialog an
- [ ] Passkey wird mit Gerätename gespeichert
- [ ] Benutzer kann Passkey benennen (z.B. "MacBook Pro", "iPhone")
- [ ] Passkey kann als MFA-Methode verwendet werden
- [ ] Mehrere Passkeys pro Account möglich
- [ ] Bestehende MFA-Verifizierung vor Passkey-Registrierung erforderlich

## API-Endpunkte

```
POST /api/auth/mfa/passkey/register/begin

Request:
{
  "deviceName": "Mein MacBook"
}

Response 200:
{
  "challenge": "base64-challenge",
  "rp": {
    "name": "TaschengeldManager",
    "id": "taschengeldmanager.de"
  },
  "user": {
    "id": "base64-user-id",
    "name": "user@example.com",
    "displayName": "Max Mustermann"
  },
  "pubKeyCredParams": [...],
  "timeout": 60000,
  "attestation": "none",
  "authenticatorSelection": {
    "residentKey": "preferred",
    "userVerification": "required"
  }
}

---

POST /api/auth/mfa/passkey/register/complete

Request:
{
  "credential": {
    "id": "credential-id",
    "rawId": "base64-raw-id",
    "response": {
      "clientDataJSON": "base64",
      "attestationObject": "base64"
    },
    "type": "public-key"
  },
  "deviceName": "Mein MacBook"
}

Response 201:
{
  "passkeyId": "guid",
  "deviceName": "Mein MacBook",
  "createdAt": "datetime",
  "message": "Passkey erfolgreich registriert"
}
```

## Technische Hinweise

- WebAuthn Level 2 Spezifikation implementieren
- FIDO2-Server-Library verwenden (z.B. Fido2-net-lib)
- Challenge: 32 Bytes kryptografisch sicher
- User ID: Opaque Identifier (nicht E-Mail)
- Attestation: "none" für Datenschutz (keine Geräte-Identifikation)
- Public Key und Credential ID in Datenbank speichern
- Sign Count für Replay-Schutz tracken

## Story Points

5

## Priorität

Mittel - Phase 2
