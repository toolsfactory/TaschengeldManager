# Story E003-S022: Ausgabe erfassen (Eltern für Kind)

## Epic
E003 - Transaktionen

## User Story

Als **Elternteil** möchte ich **eine Ausgabe für mein Kind erfassen können**, damit **auch Einkäufe dokumentiert werden, bei denen das Kind nicht selbst zahlt**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es eine Ausgabe erfasst, dann wird der Betrag vom Kinderkonto abgezogen
- [ ] Gegeben eine Ausgabe durch Eltern, wenn sie erfasst wird, dann wird dies als "durch Eltern erfasst" markiert
- [ ] Gegeben eine Ausgabe, wenn das Guthaben nicht ausreicht, dann kann das Elternteil entscheiden ob sie trotzdem gebucht wird
- [ ] Gegeben eine Ausgabe, wenn sie erfasst wird, dann wird das Kind benachrichtigt
- [ ] Gegeben eine negative Bilanz, wenn sie entsteht, dann wird diese angezeigt (kein harter Block für Eltern)

## API-Endpunkt

```
POST /api/families/{familyId}/children/{childId}/transactions
Authorization: Bearer {token}
Content-Type: application/json

{
  "type": "Expense",
  "amount": 15.00,
  "description": "Spielzeug",
  "categoryId": "guid (optional)",
  "note": "string (optional)",
  "allowOverdraft": false
}

Response 201:
{
  "transactionId": "guid",
  "type": "Expense",
  "amount": -15.00,
  "description": "Spielzeug",
  "category": "Spielwaren",
  "newBalance": 64.75,
  "createdBy": "guid",
  "createdByName": "Papa",
  "createdAt": "datetime"
}

Response 400:
{
  "message": "Nicht genügend Guthaben",
  "currentBalance": 10.00,
  "requiredAmount": 15.00,
  "canOverride": true
}
```

## Technische Notizen

- Eltern können Überziehung erlauben (allowOverdraft)
- createdBy unterscheidet Kind- vs. Eltern-Erfassung
- Benachrichtigung an Kind bei Eltern-Erfassung
- Bei negativem Kontostand: Warnung anzeigen
- Audit-Trail für Eltern-Aktionen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-022-01 | Valide Ausgabe | 201 + Kontostand reduziert |
| TC-E003-022-02 | Nicht genügend Guthaben (ohne Override) | 400 mit Option |
| TC-E003-022-03 | Nicht genügend Guthaben (mit Override) | 201 + negatives Konto |
| TC-E003-022-04 | Markierung als Eltern-Erfassung | createdBy enthält Eltern-ID |
| TC-E003-022-05 | Kind wird benachrichtigt | Push an Kind |

## Story Points

3

## Priorität

Hoch
