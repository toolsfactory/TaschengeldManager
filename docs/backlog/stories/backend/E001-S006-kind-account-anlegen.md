# E001-S006: Kind-Account anlegen (durch Eltern)

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Elternteil** möchte ich **einen Account für mein Kind anlegen können**, damit **mein Kind die App nutzen und sein Taschengeld verwalten kann**.

## Akzeptanzkriterien

- [ ] Elternteil kann Kind-Account mit Nickname und PIN erstellen
- [ ] Kind-Accounts benötigen keine E-Mail-Adresse
- [ ] PIN muss mindestens 4 Ziffern haben
- [ ] Nickname muss innerhalb der Familie eindeutig sein
- [ ] Kind wird automatisch der Familie des Elternteils zugeordnet
- [ ] Geburtsdatum kann optional angegeben werden (für altersgerechte Funktionen)
- [ ] Kind erhält automatisch die Rolle "Child"
- [ ] Nach Erstellung muss MFA für das Kind eingerichtet werden

## API-Endpunkt

```
POST /api/users/children

Request:
{
  "nickname": "string",
  "pin": "string",
  "birthDate": "date | null",
  "avatarId": "int | null"
}

Response 201:
{
  "childId": "guid",
  "nickname": "string",
  "familyId": "guid",
  "requiresMfaSetup": true
}

Response 400:
{
  "errors": {
    "nickname": ["Nickname bereits in dieser Familie vergeben"],
    "pin": ["PIN muss mindestens 4 Ziffern haben"]
  }
}
```

## Technische Hinweise

- PIN wird wie Passwort mit Argon2id gehashed
- Kind-Login verwendet Family-Code + Nickname + PIN
- Family-Code ist ein 6-stelliger alphanumerischer Code pro Familie
- Maximale Anzahl Kinder pro Familie: 10 (konfigurierbar)
- Avatar-Auswahl aus vordefinierter Liste (kindgerecht)
- Kind kann später selbst E-Mail hinzufügen (ab bestimmtem Alter)

## Story Points

3

## Priorität

Hoch - MVP-Blocker
