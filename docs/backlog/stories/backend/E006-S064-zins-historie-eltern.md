# Story E006-S064: Zins-Historie anzeigen (Eltern)

## Epic
E006 - Zinsen

## User Story

Als **Elternteil** möchte ich **die Zins-Historie meiner Kinder einsehen können**, damit **ich einen Überblick über alle Zinsgutschriften habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es die Zins-Historie abruft, dann werden alle Zinsgutschriften angezeigt
- [ ] Gegeben eine Zins-Übersicht, wenn sie angezeigt wird, dann enthält sie Datum, Betrag und Berechnungsgrundlage
- [ ] Gegeben mehrere Kinder, wenn die Historie angezeigt wird, dann kann nach Kind gefiltert werden
- [ ] Gegeben eine Zins-Historie, wenn sie angezeigt wird, dann wird die Gesamtsumme berechnet
- [ ] Gegeben Zinsen verschiedener Perioden, wenn sie angezeigt werden, dann sind sie chronologisch sortiert

## API-Endpunkte

### Zins-Historie eines Kindes
```
GET /api/families/{familyId}/children/{childId}/interest/history?year=2024&page=1
Authorization: Bearer {token}

Response 200:
{
  "childId": "guid",
  "childName": "Max",
  "currentSettings": {
    "interestRate": 3.0,
    "interval": "Monthly",
    "calculationMethod": "EndOfPeriod"
  },
  "entries": [
    {
      "interestEntryId": "guid",
      "transactionId": "guid",
      "amount": 0.75,
      "interestRate": 3.0,
      "calculatedOn": "date",
      "periodStart": "date",
      "periodEnd": "date",
      "baseBalance": 300.00,
      "calculationMethod": "EndOfPeriod"
    }
  ],
  "summary": {
    "totalInterestThisYear": 9.00,
    "totalInterestAllTime": 25.50,
    "averageMonthlyInterest": 0.75
  },
  "pagination": {...}
}
```

### Zins-Übersicht Familie
```
GET /api/families/{familyId}/interest/summary?year=2024
Authorization: Bearer {token}

Response 200:
{
  "familyId": "guid",
  "year": 2024,
  "totalInterestPaid": 45.00,
  "byChild": [
    {
      "childId": "guid",
      "childName": "Max",
      "interestEnabled": true,
      "interestRate": 3.0,
      "totalInterestThisYear": 25.00,
      "lastInterest": {
        "amount": 0.75,
        "date": "date"
      }
    }
  ],
  "byMonth": [
    {
      "month": "2024-01",
      "totalInterest": 3.75
    }
  ]
}
```

## Technische Notizen

- InterestEntry Entity für detaillierte Historie
- Filter: year, childId, fromDate, toDate
- Pagination für lange Historie
- Aggregationen für Summary
- Verknüpfung mit Transaktionen über transactionId

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E006-064-01 | Historie eines Kindes | 200 mit Einträgen |
| TC-E006-064-02 | Familien-Übersicht | Aggregiert pro Kind |
| TC-E006-064-03 | Nach Jahr filtern | Nur Einträge des Jahres |
| TC-E006-064-04 | Keine Zinsen | 200 mit leerem Array |
| TC-E006-064-05 | Monatliche Aufschlüsselung | Korrekte Summen |

## Story Points

3

## Priorität

Mittel
