# Story E002-S013: Taschengeld-Konto für Kind anlegen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **ein Taschengeld-Konto für mein Kind anlegen können**, damit **Einnahmen und Ausgaben des Kindes erfasst werden können**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind in der Familie, wenn ein Konto angelegt wird, dann wird ein Konto mit Startguthaben 0 erstellt
- [ ] Gegeben ein neues Konto, wenn es angelegt wird, dann kann optional ein Startguthaben angegeben werden
- [ ] Gegeben ein Kind, wenn es bereits ein Konto hat, dann kann kein zweites Konto angelegt werden
- [ ] Gegeben ein neues Konto, wenn es angelegt wird, dann wird die Kontowährung auf EUR gesetzt
- [ ] Gegeben ein angelegtes Konto, wenn der Prozess abgeschlossen ist, dann ist es sofort nutzbar

## API-Endpunkt

```
POST /api/families/{familyId}/children/{childId}/account
Authorization: Bearer {token}
Content-Type: application/json

{
  "initialBalance": 0.00,
  "currency": "EUR"
}

Response 201:
{
  "accountId": "guid",
  "childId": "guid",
  "balance": 0.00,
  "currency": "EUR",
  "createdAt": "datetime"
}

Response 400:
{
  "errors": {
    "initialBalance": ["Startguthaben darf nicht negativ sein"]
  }
}

Response 409:
{
  "message": "Kind hat bereits ein Konto"
}
```

## Technische Notizen

- Decimal für Geldbeträge verwenden (18,2)
- Konto ist 1:1 mit Kind verknüpft
- Bei Startguthaben > 0: Initiale Transaktion erstellen
- Währung vorerst nur EUR unterstützen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-013-01 | Konto mit Startguthaben 0 | 201 mit Konto-Daten |
| TC-E002-013-02 | Konto mit Startguthaben | 201 + initiale Transaktion |
| TC-E002-013-03 | Negatives Startguthaben | 400 Validierungsfehler |
| TC-E002-013-04 | Kind hat bereits Konto | 409 Konflikt |
| TC-E002-013-05 | Kind existiert nicht | 404 Not Found |

## Story Points

3

## Priorität

Hoch
