# Story E002-S022: Verwandter: Geld an Kind überweisen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Verwandter** möchte ich **einem Kind der Familie Geld überweisen können**, damit **ich Geschenke oder Zuwendungen digital geben kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Verwandter mit Familienzugang, wenn er Geld überweist, dann wird der Betrag dem Kinderkonto gutgeschrieben
- [ ] Gegeben eine Überweisung, wenn sie getätigt wird, dann kann ein Anlass/Nachricht hinzugefügt werden
- [ ] Gegeben eine Überweisung, wenn sie abgeschlossen ist, dann wird das Kind benachrichtigt
- [ ] Gegeben eine Überweisung, wenn sie abgeschlossen ist, dann wird sie in der Historie des Verwandten angezeigt
- [ ] Gegeben ein Verwandter, wenn er überweist, dann sieht er alle Kinder der Familie zur Auswahl

## API-Endpunkte

### Kinder für Überweisung anzeigen
```
GET /api/families/{familyId}/children/for-transfer
Authorization: Bearer {token}

Response 200:
{
  "children": [
    {
      "childId": "guid",
      "firstName": "string",
      "lastName": "string",
      "age": 10
    }
  ]
}
```

### Geld überweisen
```
POST /api/families/{familyId}/children/{childId}/transfers
Authorization: Bearer {token}
Content-Type: application/json

{
  "amount": 50.00,
  "occasion": "Geburtstag|Weihnachten|Ostern|Zeugnis|Sonstiges",
  "message": "string (optional)"
}

Response 201:
{
  "transferId": "guid",
  "amount": 50.00,
  "childName": "string",
  "occasion": "Geburtstag",
  "message": "Alles Gute zum Geburtstag!",
  "createdAt": "datetime"
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss positiv sein"]
  }
}

Response 403:
{
  "message": "Keine Berechtigung für diese Familie"
}
```

## Technische Notizen

- Transaktion vom Typ "RelativeTransfer" erstellen
- Keine Obergrenze für Überweisungen (optional konfigurierbar durch Eltern)
- Push-Benachrichtigung an Kind und Eltern
- Anlass wird als Kategorie in der Transaktion gespeichert
- Kein Rückgängigmachen durch Verwandte möglich

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-022-01 | Geld an Kind überweisen | 201 + Kontostand aktualisiert |
| TC-E002-022-02 | Mit Nachricht überweisen | 201 + Nachricht gespeichert |
| TC-E002-022-03 | Negativer Betrag | 400 Validierungsfehler |
| TC-E002-022-04 | Kind ohne Konto | 400 Fehler |
| TC-E002-022-05 | Fremde Familie | 403 Forbidden |

## Story Points

5

## Priorität

Mittel
