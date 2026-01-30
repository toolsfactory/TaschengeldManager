# Story E005-S046: Anfrage zurückziehen (Kind)

## Epic
E005 - Anfragen-System

## User Story

Als **Kind** möchte ich **meine Geldanfrage zurückziehen können**, damit **ich sie stornieren kann, wenn ich sie nicht mehr benötige**.

## Akzeptanzkriterien

- [ ] Gegeben eine ausstehende Anfrage, wenn das Kind sie zurückzieht, dann wird sie als "Zurückgezogen" markiert
- [ ] Gegeben eine zurückgezogene Anfrage, wenn der Prozess abgeschlossen ist, dann müssen die Eltern sie nicht mehr bearbeiten
- [ ] Gegeben eine bereits bearbeitete Anfrage, wenn sie zurückgezogen werden soll, dann wird dies verhindert
- [ ] Gegeben eine zurückgezogene Anfrage, wenn sie angezeigt wird, dann ist sie in der Historie sichtbar
- [ ] Gegeben ein Kind, wenn es eine Anfrage zurückzieht, dann kann es danach eine neue Anfrage stellen

## API-Endpunkt

```
POST /api/me/requests/{requestId}/withdraw
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Ich habe mir das Geld selbst gespart"
}

Response 200:
{
  "requestId": "guid",
  "status": "Withdrawn",
  "withdrawnAt": "datetime",
  "reason": "Ich habe mir das Geld selbst gespart"
}

Response 400:
{
  "message": "Anfrage wurde bereits bearbeitet und kann nicht zurückgezogen werden"
}

Response 403:
{
  "message": "Dies ist nicht deine Anfrage"
}

Response 404:
{
  "message": "Anfrage nicht gefunden"
}
```

## Technische Notizen

- Status auf "Withdrawn" setzen
- Nur wenn Status = Pending
- withdrawReason optional aber empfohlen
- Keine Benachrichtigung an Eltern (optional konfigurierbar)
- Anfrage bleibt in Historie sichtbar
- Kind kann unbegrenzt neue Anfragen stellen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-046-01 | Ausstehende Anfrage zurückziehen | 200 + Status Withdrawn |
| TC-E005-046-02 | Mit Grund | 200 + Grund gespeichert |
| TC-E005-046-03 | Bereits genehmigt | 400 Fehler |
| TC-E005-046-04 | Fremde Anfrage | 403 Forbidden |
| TC-E005-046-05 | Neue Anfrage nach Rückzug | Möglich |

## Story Points

2

## Priorität

Mittel
