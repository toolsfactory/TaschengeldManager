# Story E002-S015: Kontostand anzeigen (Kind-Sicht)

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Kind** möchte ich **meinen eigenen Kontostand sehen können**, damit **ich weiß, wie viel Geld ich zur Verfügung habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggtes Kind, wenn es sein Konto abruft, dann wird der aktuelle Kontostand angezeigt
- [ ] Gegeben ein Kind mit Konto, wenn es den Kontostand sieht, dann werden die letzten Transaktionen angezeigt
- [ ] Gegeben ein Kind, wenn es sein Konto abruft, dann sieht es nur sein eigenes Konto
- [ ] Gegeben ein Kind, wenn es sein Konto abruft, dann werden ausstehende eigene Anfragen angezeigt
- [ ] Gegeben ein Kind ohne Konto, wenn es das Konto abruft, dann wird eine entsprechende Meldung angezeigt

## API-Endpunkt

```
GET /api/me/account
Authorization: Bearer {token}

Response 200:
{
  "accountId": "guid",
  "balance": 75.25,
  "currency": "EUR",
  "pendingRequests": [
    {
      "requestId": "guid",
      "amount": 10.00,
      "reason": "Neues Buch",
      "status": "Pending",
      "createdAt": "datetime"
    }
  ],
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

Response 404:
{
  "message": "Kein Konto vorhanden. Bitte die Eltern fragen."
}
```

## Technische Notizen

- Endpunkt nutzt Token um Kind zu identifizieren
- Eingeschränkte Sicht: Keine Kontodaten anderer Kinder
- Kindgerechte Fehlermeldungen
- Performance: Nur notwendige Daten laden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-015-01 | Kind mit Konto | 200 mit Kontodetails |
| TC-E002-015-02 | Kind ohne Konto | 404 mit Hinweis |
| TC-E002-015-03 | Elternteil ruft /me/account ab | 403 oder anderer Endpunkt |
| TC-E002-015-04 | Mit ausstehenden Anfragen | Anfragen werden angezeigt |
| TC-E002-015-05 | Mit Transaktionen | Letzte Transaktionen angezeigt |

## Story Points

2

## Priorität

Hoch
