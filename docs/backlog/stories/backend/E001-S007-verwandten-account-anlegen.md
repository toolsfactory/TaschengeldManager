# E001-S007: Verwandten-Account anlegen (durch Eltern)

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Elternteil** möchte ich **Verwandte (Großeltern, Onkel, Tanten) zu meiner Familie einladen können**, damit **diese den Kindern Geld schenken oder ihr Taschengeld aufstocken können**.

## Akzeptanzkriterien

- [ ] Elternteil kann Einladung per E-Mail an Verwandte senden
- [ ] Einladungslink ist 7 Tage gültig
- [ ] Verwandter registriert sich über Einladungslink
- [ ] Verwandter erhält automatisch Rolle "Relative"
- [ ] Verwandter wird der Familie zugeordnet
- [ ] Beziehungstyp kann angegeben werden (Großeltern, Onkel, Tante, etc.)
- [ ] Verwandter muss eigene MFA einrichten
- [ ] Einladung kann vom Elternteil widerrufen werden

## API-Endpunkte

```
POST /api/families/invitations

Request:
{
  "email": "string",
  "relationshipType": "Grandparent | Uncle | Aunt | Other",
  "customRelationship": "string | null"
}

Response 201:
{
  "invitationId": "guid",
  "email": "string",
  "expiresAt": "datetime",
  "status": "Pending"
}

---

GET /api/families/invitations

Response 200:
{
  "invitations": [
    {
      "id": "guid",
      "email": "string",
      "relationshipType": "Grandparent",
      "status": "Pending | Accepted | Expired | Revoked",
      "createdAt": "datetime",
      "expiresAt": "datetime"
    }
  ]
}

---

DELETE /api/families/invitations/{id}

Response 200:
{
  "message": "Einladung widerrufen"
}

---

POST /api/auth/register/invitation

Request:
{
  "invitationToken": "string",
  "firstName": "string",
  "lastName": "string",
  "password": "string"
}

Response 201:
{
  "userId": "guid",
  "familyId": "guid",
  "requiresMfaSetup": true
}
```

## Technische Hinweise

- Einladungstoken als kryptografisch sicherer Zufallsstring
- Token gehashed in Datenbank speichern
- E-Mail-Template erklärt den Zweck der Einladung
- Verwandter kann mehreren Familien angehören (mit separaten Einladungen)
- Max. 20 aktive Einladungen pro Familie
- Bereits registrierte E-Mail: Direkte Verknüpfung mit Familie (nach Bestätigung)

## Story Points

3

## Priorität

Mittel
