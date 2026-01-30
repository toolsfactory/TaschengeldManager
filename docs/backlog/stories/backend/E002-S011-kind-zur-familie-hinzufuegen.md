# Story E002-S011: Kind zur Familie hinzufügen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **ein Kind zu meiner Familie hinzufügen können**, damit **ich dessen Taschengeld verwalten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil mit Familie, wenn es ein Kind hinzufügt, dann wird ein neuer Kind-Account erstellt
- [ ] Gegeben ein neues Kind, wenn es hinzugefügt wird, dann werden Name, Geburtsdatum und optionaler PIN erfasst
- [ ] Gegeben ein Kind ohne eigene E-Mail, wenn es hinzugefügt wird, dann kann es sich später mit Benutzername/PIN anmelden
- [ ] Gegeben ein Kind mit E-Mail, wenn es hinzugefügt wird, dann erhält es eine Einladung zur Registrierung
- [ ] Gegeben ein hinzugefügtes Kind, wenn der Prozess abgeschlossen ist, dann ist es automatisch der Familie zugeordnet

## API-Endpunkt

```
POST /api/families/{familyId}/children
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "string",
  "lastName": "string",
  "dateOfBirth": "date",
  "username": "string",
  "pin": "string (4-6 digits, optional)",
  "email": "string (optional)"
}

Response 201:
{
  "childId": "guid",
  "firstName": "string",
  "lastName": "string",
  "username": "string",
  "familyId": "guid",
  "createdAt": "datetime"
}

Response 400:
{
  "errors": {
    "firstName": ["Vorname ist erforderlich"],
    "username": ["Benutzername bereits vergeben"]
  }
}

Response 403:
{
  "message": "Keine Berechtigung für diese Familie"
}
```

## Technische Notizen

- Kind-Rolle (Child) automatisch zuweisen
- PIN verschlüsselt speichern (falls angegeben)
- Benutzername muss innerhalb der Familie eindeutig sein
- E-Mail-Einladung über separaten Service senden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-011-01 | Kind mit Basisdaten hinzufügen | 201 mit Kind-Daten |
| TC-E002-011-02 | Kind mit E-Mail hinzufügen | 201 + E-Mail-Einladung |
| TC-E002-011-03 | Doppelter Benutzername | 400 Validierungsfehler |
| TC-E002-011-04 | Fremde Familie | 403 Forbidden |
| TC-E002-011-05 | Kind mit PIN | PIN wird verschlüsselt gespeichert |

## Story Points

5

## Priorität

Hoch
