# Story E002-S012: Familienmitglieder anzeigen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **alle Mitglieder meiner Familie sehen können**, damit **ich einen Überblick über alle verwalteten Personen habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil mit Familie, wenn es die Mitgliederliste abruft, dann werden alle Kinder angezeigt
- [ ] Gegeben ein Elternteil mit Familie, wenn es die Mitgliederliste abruft, dann werden alle anderen Elternteile angezeigt
- [ ] Gegeben ein Elternteil mit Familie, wenn es die Mitgliederliste abruft, dann werden alle Verwandten angezeigt
- [ ] Gegeben eine Mitgliederliste, wenn sie angezeigt wird, dann enthält sie Name, Rolle und Status jedes Mitglieds
- [ ] Gegeben ausstehende Einladungen, wenn die Liste angezeigt wird, dann sind diese separat markiert

## API-Endpunkt

```
GET /api/families/{familyId}/members
Authorization: Bearer {token}

Response 200:
{
  "familyId": "guid",
  "familyName": "string",
  "members": [
    {
      "userId": "guid",
      "firstName": "string",
      "lastName": "string",
      "role": "Parent|Child|Relative",
      "isAdmin": true,
      "status": "Active|Invited|Inactive",
      "joinedAt": "datetime"
    }
  ],
  "pendingInvitations": [
    {
      "invitationId": "guid",
      "email": "string",
      "role": "Parent|Relative",
      "invitedAt": "datetime",
      "expiresAt": "datetime"
    }
  ]
}

Response 403:
{
  "message": "Keine Berechtigung für diese Familie"
}
```

## Technische Notizen

- Sortierung: Eltern zuerst, dann Kinder, dann Verwandte
- Nur aktive und eingeladene Mitglieder anzeigen
- Einladungen mit Ablaufdatum versehen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-012-01 | Familie mit Mitgliedern | 200 mit vollständiger Liste |
| TC-E002-012-02 | Leere Familie | 200 mit leerem Array |
| TC-E002-012-03 | Mit ausstehenden Einladungen | Einladungen in separatem Array |
| TC-E002-012-04 | Fremde Familie | 403 Forbidden |
| TC-E002-012-05 | Kind fragt Mitglieder ab | Eingeschränkte Sicht (nur Namen) |

## Story Points

3

## Priorität

Hoch
