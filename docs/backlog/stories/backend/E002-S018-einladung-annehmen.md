# Story E002-S018: Einladung annehmen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **eingeladene Person** möchte ich **eine Familieneinladung annehmen können**, damit **ich der Familie beitreten kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine gültige Einladung, wenn sie angenommen wird, dann wird die Person der Familie hinzugefügt
- [ ] Gegeben eine abgelaufene Einladung, wenn sie angenommen wird, dann wird ein Fehler angezeigt
- [ ] Gegeben eine bereits verwendete Einladung, wenn sie erneut verwendet wird, dann wird ein Fehler angezeigt
- [ ] Gegeben ein nicht registrierter Benutzer, wenn er die Einladung annimmt, dann muss er sich zuerst registrieren
- [ ] Gegeben eine angenommene Einladung, wenn der Prozess abgeschlossen ist, dann wird die Einladung als verwendet markiert

## API-Endpunkte

### Einladung prüfen
```
GET /api/invitations/{token}

Response 200:
{
  "invitationId": "guid",
  "familyName": "string",
  "invitedBy": "string",
  "role": "Parent",
  "expiresAt": "datetime",
  "isValid": true
}

Response 404:
{
  "message": "Einladung nicht gefunden oder abgelaufen"
}
```

### Einladung annehmen
```
POST /api/invitations/{token}/accept
Authorization: Bearer {token}

Response 200:
{
  "message": "Willkommen in der Familie!",
  "familyId": "guid",
  "familyName": "string",
  "role": "Parent"
}

Response 400:
{
  "message": "Einladung ist abgelaufen"
}

Response 409:
{
  "message": "Sie sind bereits Mitglied einer Familie"
}
```

## Technische Notizen

- Token-Validierung: Ablaufdatum und Verwendungsstatus prüfen
- User muss authentifiziert sein um anzunehmen
- E-Mail-Adresse des Users muss mit Einladung übereinstimmen
- Nach Annahme: Einladung als "Accepted" markieren
- Benachrichtigung an Familien-Admin senden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-018-01 | Gültige Einladung annehmen | 200 + Familie beigetreten |
| TC-E002-018-02 | Abgelaufene Einladung | 400 Fehler |
| TC-E002-018-03 | Bereits verwendete Einladung | 400 Fehler |
| TC-E002-018-04 | E-Mail stimmt nicht überein | 403 Forbidden |
| TC-E002-018-05 | Bereits in anderer Familie | 409 Konflikt |

## Story Points

5

## Priorität

Mittel
