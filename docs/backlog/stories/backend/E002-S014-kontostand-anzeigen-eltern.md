# Story E002-S014: Kontostand anzeigen (Eltern-Sicht)

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **den Kontostand meiner Kinder einsehen können**, damit **ich den finanziellen Überblick behalte**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es die Konten abruft, dann werden alle Kinderkonten der Familie angezeigt
- [ ] Gegeben ein Kinderkonto, wenn es abgerufen wird, dann wird der aktuelle Kontostand angezeigt
- [ ] Gegeben ein Kinderkonto, wenn es abgerufen wird, dann werden die letzten Transaktionen angezeigt
- [ ] Gegeben mehrere Kinder, wenn die Konten abgerufen werden, dann wird eine Gesamtübersicht angezeigt
- [ ] Gegeben ein Konto, wenn es abgerufen wird, dann werden ausstehende Anfragen angezeigt

## API-Endpunkte

### Alle Kinderkonten
```
GET /api/families/{familyId}/accounts
Authorization: Bearer {token}

Response 200:
{
  "familyId": "guid",
  "totalBalance": 150.50,
  "currency": "EUR",
  "accounts": [
    {
      "accountId": "guid",
      "childId": "guid",
      "childName": "string",
      "balance": 75.25,
      "currency": "EUR",
      "pendingRequests": 2,
      "lastTransaction": {
        "amount": -5.00,
        "description": "Eis",
        "date": "datetime"
      }
    }
  ]
}
```

### Einzelnes Kinderkonto
```
GET /api/families/{familyId}/children/{childId}/account
Authorization: Bearer {token}

Response 200:
{
  "accountId": "guid",
  "childId": "guid",
  "childName": "string",
  "balance": 75.25,
  "currency": "EUR",
  "pendingRequests": 2,
  "recentTransactions": [
    {
      "transactionId": "guid",
      "amount": -5.00,
      "type": "Expense",
      "description": "Eis",
      "category": "Süßigkeiten",
      "date": "datetime"
    }
  ]
}
```

## Technische Notizen

- Kontostand in Echtzeit berechnen oder gecached vorhalten
- Letzte 5-10 Transaktionen als Vorschau laden
- Pagination für vollständige Transaktionshistorie verwenden
- Zugriffsrechte: Nur Eltern der Familie

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-014-01 | Alle Konten abrufen | 200 mit Kontoliste |
| TC-E002-014-02 | Einzelnes Konto abrufen | 200 mit Kontodetails |
| TC-E002-014-03 | Kind ohne Konto | 404 Not Found |
| TC-E002-014-04 | Fremde Familie | 403 Forbidden |
| TC-E002-014-05 | Leere Familie | 200 mit leerem Array |

## Story Points

3

## Priorität

Hoch
