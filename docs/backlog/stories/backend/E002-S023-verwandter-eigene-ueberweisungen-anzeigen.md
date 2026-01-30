# Story E002-S023: Verwandter: Eigene Überweisungen anzeigen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Verwandter** möchte ich **meine bisherigen Überweisungen an die Kinder sehen können**, damit **ich einen Überblick über meine Geschenke habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein Verwandter, wenn er seine Überweisungen abruft, dann werden alle getätigten Überweisungen angezeigt
- [ ] Gegeben eine Überweisungsliste, wenn sie angezeigt wird, dann enthält sie Datum, Betrag, Kind und Anlass
- [ ] Gegeben ein Verwandter mit mehreren Überweisungen, wenn er die Liste abruft, dann kann er nach Kind filtern
- [ ] Gegeben ein Verwandter, wenn er die Liste abruft, dann wird eine Gesamtsumme pro Jahr angezeigt
- [ ] Gegeben ein Verwandter, wenn er die Liste abruft, dann werden nur seine eigenen Überweisungen angezeigt

## API-Endpunkt

```
GET /api/me/transfers?childId={childId}&year={year}
Authorization: Bearer {token}

Response 200:
{
  "totalAmount": 250.00,
  "totalThisYear": 100.00,
  "transfers": [
    {
      "transferId": "guid",
      "childId": "guid",
      "childName": "string",
      "amount": 50.00,
      "occasion": "Geburtstag",
      "message": "Alles Gute!",
      "createdAt": "datetime"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 5,
    "totalPages": 1
  }
}
```

### Zusammenfassung pro Kind
```
GET /api/me/transfers/summary
Authorization: Bearer {token}

Response 200:
{
  "summaryByChild": [
    {
      "childId": "guid",
      "childName": "string",
      "totalAmount": 150.00,
      "transferCount": 3,
      "lastTransfer": "datetime"
    }
  ],
  "summaryByYear": [
    {
      "year": 2024,
      "totalAmount": 100.00,
      "transferCount": 2
    }
  ]
}
```

## Technische Notizen

- Pagination für lange Listen
- Filter: childId, year, fromDate, toDate
- Sortierung: neueste zuerst (Standard)
- Nur eigene Überweisungen anzeigen (userId aus Token)
- Aggregate-Queries für Zusammenfassungen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-023-01 | Alle Überweisungen abrufen | 200 mit Liste |
| TC-E002-023-02 | Nach Kind filtern | Nur Überweisungen an dieses Kind |
| TC-E002-023-03 | Nach Jahr filtern | Nur Überweisungen aus diesem Jahr |
| TC-E002-023-04 | Keine Überweisungen | 200 mit leerem Array |
| TC-E002-023-05 | Zusammenfassung abrufen | Aggregierte Daten |

## Story Points

3

## Priorität

Niedrig
