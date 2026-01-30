# E001-S001: Registrierung als Elternteil

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Elternteil** möchte ich **mich mit meiner E-Mail-Adresse und einem Passwort registrieren können**, damit **ich einen Account erstellen und die App für meine Familie einrichten kann**.

## Akzeptanzkriterien

- [ ] E-Mail-Adresse wird als eindeutiger Identifier verwendet
- [ ] Passwort muss mindestens 8 Zeichen lang sein
- [ ] Passwort muss mindestens einen Großbuchstaben, einen Kleinbuchstaben und eine Zahl enthalten
- [ ] Bei bereits registrierter E-Mail wird eine aussagekräftige Fehlermeldung angezeigt
- [ ] Nach erfolgreicher Registrierung wird der Benutzer aufgefordert, MFA einzurichten
- [ ] Passwort wird mit Argon2id gehashed gespeichert
- [ ] E-Mail-Format wird serverseitig validiert
- [ ] Account wird mit Rolle "Parent" erstellt

## API-Endpunkt

```
POST /api/auth/register

Request:
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string"
}

Response 201:
{
  "userId": "guid",
  "email": "string",
  "requiresMfaSetup": true
}

Response 400:
{
  "errors": {
    "email": ["E-Mail bereits registriert"],
    "password": ["Mindestens 8 Zeichen erforderlich"]
  }
}
```

## Technische Hinweise

- Passwort-Hashing mit Argon2id (nicht BCrypt)
- Rate-Limiting: Max. 5 Registrierungsversuche pro IP/Minute
- E-Mail wird in Kleinbuchstaben normalisiert
- Keine automatische Anmeldung nach Registrierung - erst nach MFA-Setup

## Story Points

3

## Priorität

Hoch - MVP-Blocker
