# Story E003-S023: Transaktionshistorie anzeigen

## Epic
E003 - Transaktionen

## User Story

Als **Benutzer (Kind oder Elternteil)** möchte ich **die Transaktionshistorie eines Kontos einsehen können**, damit **ich alle Einnahmen und Ausgaben nachvollziehen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Benutzer, wenn er die Historie abruft, dann werden alle Transaktionen chronologisch angezeigt
- [ ] Gegeben eine Transaktionsliste, wenn sie angezeigt wird, dann enthält sie Datum, Betrag, Beschreibung und Kategorie
- [ ] Gegeben ein Kind, wenn es die Historie abruft, dann sieht es nur sein eigenes Konto
- [ ] Gegeben ein Elternteil, wenn es die Historie abruft, dann kann es alle Kinderkonten einsehen
- [ ] Gegeben eine lange Historie, wenn sie abgerufen wird, dann wird Pagination verwendet

## API-Endpunkte

### Kind: Eigene Transaktionen
```
GET /api/me/transactions?page=1&pageSize=20&fromDate={date}&toDate={date}&categoryId={guid}
Authorization: Bearer {token}

Response 200:
{
  "transactions": [
    {
      "transactionId": "guid",
      "type": "Expense|Deposit|Interest|RelativeTransfer",
      "amount": -5.50,
      "description": "Eis",
      "category": "Süßigkeiten",
      "note": "string",
      "createdBy": "guid",
      "createdByName": "selbst",
      "createdAt": "datetime",
      "isCancelled": false
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  },
  "summary": {
    "totalIncome": 100.00,
    "totalExpenses": 55.50,
    "netChange": 44.50
  }
}
```

### Eltern: Transaktionen eines Kindes
```
GET /api/families/{familyId}/children/{childId}/transactions?page=1&pageSize=20
Authorization: Bearer {token}

Response 200:
{
  "childId": "guid",
  "childName": "string",
  "currentBalance": 64.75,
  "transactions": [...],
  "pagination": {...},
  "summary": {...}
}
```

## Technische Notizen

- Pagination mit Offset oder Cursor-based
- Filter: Datum (von/bis), Kategorie, Typ
- Sortierung: neueste zuerst (Standard)
- Summary: Einnahmen/Ausgaben im gewählten Zeitraum
- Stornierte Transaktionen markiert anzeigen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-023-01 | Alle Transaktionen abrufen | 200 mit Liste |
| TC-E003-023-02 | Mit Datumsfilter | Nur Transaktionen im Zeitraum |
| TC-E003-023-03 | Mit Kategoriefilter | Nur Transaktionen der Kategorie |
| TC-E003-023-04 | Leere Historie | 200 mit leerem Array |
| TC-E003-023-05 | Pagination | Korrekte Seiten-Aufteilung |

## Story Points

3

## Priorität

Hoch
