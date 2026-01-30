# Story E002-S020: Elternteil aus Familie entfernen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Familien-Admin** möchte ich **ein anderes Elternteil aus der Familie entfernen können**, damit **ich die Familienmitglieder verwalten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Familien-Admin, wenn er ein anderes Elternteil entfernt, dann wird dessen Zugang zur Familie deaktiviert
- [ ] Gegeben ein Elternteil, wenn es der einzige Admin ist, dann kann es nicht entfernt werden
- [ ] Gegeben ein entferntes Elternteil, wenn der Prozess abgeschlossen ist, dann hat es keinen Zugriff mehr auf Familiendaten
- [ ] Gegeben ein entferntes Elternteil, wenn es entfernt wird, dann wird es per E-Mail benachrichtigt
- [ ] Gegeben der letzte Admin, wenn er sich selbst entfernen will, dann muss zuerst ein neuer Admin ernannt werden

## API-Endpunkt

```
DELETE /api/families/{familyId}/members/{userId}
Authorization: Bearer {token}

Response 200:
{
  "message": "Familienmitglied wurde entfernt",
  "userId": "guid"
}

Response 400:
{
  "message": "Mindestens ein Admin muss in der Familie bleiben"
}

Response 403:
{
  "message": "Keine Berechtigung für diese Aktion"
}

Response 404:
{
  "message": "Mitglied nicht gefunden"
}
```

### Admin-Rolle übertragen
```
POST /api/families/{familyId}/members/{userId}/make-admin
Authorization: Bearer {token}

Response 200:
{
  "message": "Admin-Rechte wurden übertragen",
  "userId": "guid"
}
```

## Technische Notizen

- Nur Admins können andere Mitglieder entfernen
- Admin kann sich nicht selbst entfernen wenn letzter Admin
- FamilyId beim User auf null setzen
- Alle aktiven Sessions des entfernten Users invalidieren
- E-Mail-Benachrichtigung senden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-020-01 | Elternteil entfernen | 200 + Zugang entzogen |
| TC-E002-020-02 | Letzten Admin entfernen | 400 Fehler |
| TC-E002-020-03 | Nicht-Admin entfernt | 403 Forbidden |
| TC-E002-020-04 | Kind als Admin setzen | 400 Fehler (nur Eltern) |
| TC-E002-020-05 | Admin-Rechte übertragen | 200 + neuer Admin |

## Story Points

3

## Priorität

Niedrig
