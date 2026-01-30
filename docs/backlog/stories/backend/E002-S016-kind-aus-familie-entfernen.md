# Story E002-S016: Kind aus Familie entfernen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **ein Kind aus der Familie entfernen können**, damit **ich die Familienmitglieder verwalten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind in der Familie, wenn es entfernt wird, dann wird der Account deaktiviert
- [ ] Gegeben ein entferntes Kind, wenn der Prozess abgeschlossen ist, dann bleiben die Transaktionsdaten für Berichte erhalten
- [ ] Gegeben ein Kind mit Guthaben, wenn es entfernt wird, dann wird eine Warnung angezeigt
- [ ] Gegeben ein Kind mit aktivem Konto, wenn es entfernt wird, dann wird das Konto eingefroren
- [ ] Gegeben ein entferntes Kind, wenn es wieder hinzugefügt werden soll, dann kann es reaktiviert werden

## API-Endpunkt

```
DELETE /api/families/{familyId}/children/{childId}
Authorization: Bearer {token}

Response 200:
{
  "message": "Kind wurde aus der Familie entfernt",
  "childId": "guid",
  "accountFrozen": true,
  "remainingBalance": 25.50
}

Response 400:
{
  "message": "Kind hat noch offene Anfragen. Bitte zuerst bearbeiten."
}

Response 403:
{
  "message": "Keine Berechtigung für diese Aktion"
}
```

### Reaktivierung
```
POST /api/families/{familyId}/children/{childId}/reactivate
Authorization: Bearer {token}

Response 200:
{
  "message": "Kind wurde reaktiviert",
  "childId": "guid"
}
```

## Technische Notizen

- Soft-Delete implementieren (IsActive = false)
- Konto auf "Frozen" Status setzen
- Transaktionshistorie bleibt erhalten
- Ausstehende Anfragen müssen zuerst bearbeitet werden
- Wiederkehrende Zahlungen pausieren

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-016-01 | Kind ohne Guthaben entfernen | 200 Erfolg |
| TC-E002-016-02 | Kind mit Guthaben entfernen | 200 mit Warnung |
| TC-E002-016-03 | Kind mit offenen Anfragen | 400 Fehler |
| TC-E002-016-04 | Nicht-Admin versucht zu löschen | 403 Forbidden |
| TC-E002-016-05 | Kind reaktivieren | 200 Erfolg |

## Story Points

3

## Priorität

Mittel
