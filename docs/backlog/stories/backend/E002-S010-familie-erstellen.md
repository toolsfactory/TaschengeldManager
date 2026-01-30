# Story E002-S010: Familie erstellen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **eine neue Familie erstellen können**, damit **ich meine Kinder und deren Taschengeld verwalten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein registriertes Elternteil, wenn es eine Familie erstellt, dann wird eine neue Familie mit eindeutiger ID angelegt
- [ ] Gegeben eine erfolgreiche Familienerstellung, wenn der Prozess abgeschlossen ist, dann ist das Elternteil automatisch Administrator der Familie
- [ ] Gegeben ein Elternteil das bereits eine Familie hat, wenn es eine weitere Familie erstellen möchte, dann wird dies verhindert (1 Familie pro Elternteil)
- [ ] Gegeben eine neue Familie, wenn sie erstellt wird, dann kann ein Familienname angegeben werden

## API-Endpunkt

```
POST /api/families
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "string"
}

Response 201:
{
  "familyId": "guid",
  "name": "string",
  "createdAt": "datetime",
  "createdBy": "guid"
}

Response 400:
{
  "errors": {
    "name": ["Familienname ist erforderlich"]
  }
}

Response 409:
{
  "message": "Sie sind bereits Mitglied einer Familie"
}
```

## Technische Notizen

- Familie als Aggregate Root implementieren
- Elternteil wird automatisch als FamilyAdmin markiert
- FamilyId als Fremdschlüssel in User-Tabelle
- Soft-Delete für Familien implementieren

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-010-01 | Valide Familienerstellung | 201 mit Familie-Daten |
| TC-E002-010-02 | Ohne Familienname | 400 mit Validierungsfehler |
| TC-E002-010-03 | Elternteil hat bereits Familie | 409 Konflikt |
| TC-E002-010-04 | Nicht authentifiziert | 401 Unauthorized |

## Story Points

3

## Priorität

Hoch
