# Story E002-S025: Verwandten anlegen (durch Eltern)

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **einen Verwandten ohne E-Mail-Adresse anlegen können**, damit **auch technisch weniger versierte Verwandte erfasst werden können**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es einen Verwandten anlegt, dann wird dieser ohne eigenen Account erstellt
- [ ] Gegeben ein angelegter Verwandter, wenn er angelegt wird, dann können Eltern in seinem Namen Überweisungen tätigen
- [ ] Gegeben ein angelegter Verwandter, wenn er angelegt wird, dann kann später eine E-Mail hinzugefügt werden
- [ ] Gegeben ein Verwandter ohne Account, wenn Eltern eine Überweisung tätigen, dann wird diese korrekt dokumentiert
- [ ] Gegeben ein Verwandter mit E-Mail, wenn diese hinzugefügt wird, dann erhält er eine Einladung zur Aktivierung

## API-Endpunkte

### Verwandten anlegen
```
POST /api/families/{familyId}/relatives
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "string",
  "lastName": "string",
  "relationshipType": "Grandparent|Uncle|Aunt|Other",
  "relationshipName": "string (z.B. 'Oma Helga')",
  "email": "string (optional)"
}

Response 201:
{
  "relativeId": "guid",
  "firstName": "string",
  "lastName": "string",
  "relationshipType": "Grandparent",
  "relationshipName": "Oma Helga",
  "hasAccount": false,
  "createdAt": "datetime"
}
```

### Überweisung im Namen des Verwandten
```
POST /api/families/{familyId}/relatives/{relativeId}/transfers
Authorization: Bearer {token}
Content-Type: application/json

{
  "childId": "guid",
  "amount": 50.00,
  "occasion": "Geburtstag",
  "message": "string (optional)"
}

Response 201:
{
  "transferId": "guid",
  "amount": 50.00,
  "fromRelative": "Oma Helga",
  "toChild": "Max",
  "recordedBy": "guid",
  "createdAt": "datetime"
}
```

### E-Mail nachträglich hinzufügen
```
PATCH /api/families/{familyId}/relatives/{relativeId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "string"
}

Response 200:
{
  "relativeId": "guid",
  "email": "string",
  "invitationSent": true
}
```

## Technische Notizen

- Verwandter ohne User-Account in eigener Tabelle
- Bei Überweisung: recordedBy = Elternteil-ID speichern
- Bei E-Mail-Hinzufügung: Automatisch Einladung senden
- Bei Einladungsannahme: Verwandten-Eintrag mit User verknüpfen
- Überweisungen werden dann dem neuen Account zugeordnet

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-025-01 | Verwandten ohne E-Mail anlegen | 201 mit Verwandten-Daten |
| TC-E002-025-02 | Verwandten mit E-Mail anlegen | 201 + Einladung versendet |
| TC-E002-025-03 | Überweisung im Namen tätigen | 201 + korrekte Zuordnung |
| TC-E002-025-04 | E-Mail nachträglich hinzufügen | 200 + Einladung versendet |
| TC-E002-025-05 | Doppelter Name | Erlaubt (verschiedene IDs) |

## Story Points

5

## Priorität

Mittel
