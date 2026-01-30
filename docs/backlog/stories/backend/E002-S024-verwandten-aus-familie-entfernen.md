# Story E002-S024: Verwandten aus Familie entfernen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **einen Verwandten aus der Familie entfernen können**, damit **ich die Familienmitglieder verwalten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es einen Verwandten entfernt, dann wird dessen Zugang zur Familie deaktiviert
- [ ] Gegeben ein entfernter Verwandter, wenn der Prozess abgeschlossen ist, dann hat er keinen Zugriff mehr auf die Kinder
- [ ] Gegeben ein entfernter Verwandter, wenn er entfernt wird, dann bleiben seine bisherigen Überweisungen erhalten
- [ ] Gegeben ein entfernter Verwandter, wenn er entfernt wird, dann wird er per E-Mail benachrichtigt
- [ ] Gegeben ein Verwandter, wenn er erneut eingeladen werden soll, dann ist dies möglich

## API-Endpunkt

```
DELETE /api/families/{familyId}/relatives/{userId}
Authorization: Bearer {token}

Response 200:
{
  "message": "Verwandter wurde aus der Familie entfernt",
  "userId": "guid",
  "transfersPreserved": true,
  "totalTransferAmount": 250.00
}

Response 403:
{
  "message": "Keine Berechtigung für diese Aktion"
}

Response 404:
{
  "message": "Verwandter nicht gefunden"
}
```

## Technische Notizen

- Soft-Delete: FamilyId auf null setzen
- Überweisungshistorie bleibt für Berichtszwecke erhalten
- Sessions des Verwandten invalidieren
- E-Mail-Benachrichtigung optional
- Verwandter kann später erneut eingeladen werden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-024-01 | Verwandten entfernen | 200 + Zugang entzogen |
| TC-E002-024-02 | Nicht-Elternteil entfernt | 403 Forbidden |
| TC-E002-024-03 | Überweisungen nach Entfernung | Bleiben erhalten |
| TC-E002-024-04 | Verwandter existiert nicht | 404 Not Found |
| TC-E002-024-05 | Erneute Einladung | Möglich |

## Story Points

2

## Priorität

Niedrig
