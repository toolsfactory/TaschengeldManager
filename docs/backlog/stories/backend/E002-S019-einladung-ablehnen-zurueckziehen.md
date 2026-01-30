# Story E002-S019: Einladung ablehnen/zurückziehen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **eingeladene Person** möchte ich **eine Einladung ablehnen können**, und als **Familien-Admin** möchte ich **eine Einladung zurückziehen können**, damit **ungewollte Einladungen verwaltet werden können**.

## Akzeptanzkriterien

- [ ] Gegeben eine gültige Einladung, wenn der Eingeladene sie ablehnt, dann wird sie als abgelehnt markiert
- [ ] Gegeben eine abgelehnte Einladung, wenn der Admin informiert wird, dann erhält er eine Benachrichtigung
- [ ] Gegeben eine ausstehende Einladung, wenn der Admin sie zurückzieht, dann wird sie ungültig
- [ ] Gegeben eine zurückgezogene Einladung, wenn der Eingeladene sie nutzen will, dann wird ein Fehler angezeigt
- [ ] Gegeben eine Einladung, wenn sie abgelehnt/zurückgezogen wird, dann kann eine neue versendet werden

## API-Endpunkte

### Einladung ablehnen (durch Eingeladenen)
```
POST /api/invitations/{token}/decline

Response 200:
{
  "message": "Einladung wurde abgelehnt"
}

Response 404:
{
  "message": "Einladung nicht gefunden"
}
```

### Einladung zurückziehen (durch Admin)
```
DELETE /api/families/{familyId}/invitations/{invitationId}
Authorization: Bearer {token}

Response 200:
{
  "message": "Einladung wurde zurückgezogen"
}

Response 403:
{
  "message": "Keine Berechtigung für diese Aktion"
}

Response 404:
{
  "message": "Einladung nicht gefunden"
}
```

## Technische Notizen

- Status-Enum: Pending, Accepted, Declined, Revoked, Expired
- Bei Ablehnung: Benachrichtigung an Admin (optional)
- Zurückgezogene Einladungen behalten für Audit-Zwecke
- Gleiche E-Mail kann erneut eingeladen werden nach Ablehnung/Zurückziehen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-019-01 | Einladung ablehnen | 200 + Status = Declined |
| TC-E002-019-02 | Bereits abgelehnte Einladung | 400 Fehler |
| TC-E002-019-03 | Admin zieht zurück | 200 + Status = Revoked |
| TC-E002-019-04 | Nicht-Admin zieht zurück | 403 Forbidden |
| TC-E002-019-05 | Zurückgezogene Einladung nutzen | 404 Fehler |

## Story Points

3

## Priorität

Niedrig
