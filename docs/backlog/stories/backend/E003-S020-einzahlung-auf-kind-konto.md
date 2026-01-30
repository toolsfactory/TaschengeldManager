# Story E003-S020: Einzahlung auf Kind-Konto (Eltern)

## Epic
E003 - Transaktionen

## User Story

Als **Elternteil** möchte ich **Geld auf das Konto meines Kindes einzahlen können**, damit **ich Taschengeld oder Sonderzahlungen gutschreiben kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es eine Einzahlung tätigt, dann wird der Betrag dem Kinderkonto gutgeschrieben
- [ ] Gegeben eine Einzahlung, wenn sie getätigt wird, dann kann eine Beschreibung hinzugefügt werden
- [ ] Gegeben eine Einzahlung, wenn sie getätigt wird, dann kann eine Kategorie zugewiesen werden
- [ ] Gegeben eine erfolgreiche Einzahlung, wenn der Prozess abgeschlossen ist, dann wird der neue Kontostand angezeigt
- [ ] Gegeben eine Einzahlung, wenn sie getätigt wird, dann wird das Kind benachrichtigt

## API-Endpunkt

```
POST /api/families/{familyId}/children/{childId}/transactions
Authorization: Bearer {token}
Content-Type: application/json

{
  "type": "Deposit",
  "amount": 10.00,
  "description": "Taschengeld März",
  "categoryId": "guid (optional)",
  "note": "string (optional)"
}

Response 201:
{
  "transactionId": "guid",
  "type": "Deposit",
  "amount": 10.00,
  "description": "Taschengeld März",
  "category": "Taschengeld",
  "newBalance": 85.25,
  "createdBy": "guid",
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
  "message": "Keine Berechtigung für dieses Konto"
}

Response 404:
{
  "message": "Kind oder Konto nicht gefunden"
}
```

## Technische Notizen

- Transaktion mit positiven Betrag und Typ "Deposit"
- Kontostand atomar aktualisieren (Transaction/Lock)
- Audit-Log für alle Einzahlungen
- Push-Benachrichtigung an Kind senden
- Validierung: Betrag > 0, max. 2 Dezimalstellen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-020-01 | Valide Einzahlung | 201 + Kontostand aktualisiert |
| TC-E003-020-02 | Mit Kategorie | 201 + Kategorie gespeichert |
| TC-E003-020-03 | Negativer Betrag | 400 Validierungsfehler |
| TC-E003-020-04 | Kind ohne Konto | 404 Not Found |
| TC-E003-020-05 | Fremdes Kind | 403 Forbidden |

## Story Points

3

## Priorität

Hoch
