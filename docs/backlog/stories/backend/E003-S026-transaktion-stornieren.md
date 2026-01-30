# Story E003-S026: Transaktion stornieren (nur Eltern)

## Epic
E003 - Transaktionen

## User Story

Als **Elternteil** möchte ich **eine Transaktion stornieren können**, damit **ich Fehleingaben korrigieren kann ohne die Historie zu verfälschen**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es eine Transaktion storniert, dann wird eine Gegenbuchung erstellt
- [ ] Gegeben eine stornierte Transaktion, wenn sie angezeigt wird, dann ist sie als storniert markiert
- [ ] Gegeben eine Stornierung, wenn sie durchgeführt wird, dann muss ein Grund angegeben werden
- [ ] Gegeben ein Kind, wenn es eine Stornierung versucht, dann wird dies abgelehnt
- [ ] Gegeben eine bereits stornierte Transaktion, wenn sie erneut storniert werden soll, dann wird dies verhindert

## API-Endpunkt

```
POST /api/transactions/{transactionId}/cancel
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Fehlerhafte Eingabe"
}

Response 200:
{
  "originalTransactionId": "guid",
  "cancellationTransactionId": "guid",
  "originalAmount": -15.00,
  "cancellationAmount": 15.00,
  "reason": "Fehlerhafte Eingabe",
  "newBalance": 79.75,
  "cancelledAt": "datetime",
  "cancelledBy": "guid"
}

Response 400:
{
  "errors": {
    "reason": ["Stornierungsgrund ist erforderlich"]
  }
}

Response 403:
{
  "message": "Nur Eltern können Transaktionen stornieren"
}

Response 409:
{
  "message": "Transaktion wurde bereits storniert"
}
```

## Technische Notizen

- Keine echte Löschung - Stornierung durch Gegenbuchung
- Original-Transaktion erhält Flag: isCancelled = true
- Gegenbuchung referenziert Original: cancelsTransactionId
- Stornierungsgrund für Audit-Trail speichern
- Kontostand wird durch Gegenbuchung korrigiert
- Zinsen auf stornierte Beträge nachträglich korrigieren (optional)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-026-01 | Einzahlung stornieren | Gegenbuchung erstellt |
| TC-E003-026-02 | Ausgabe stornieren | Betrag wird gutgeschrieben |
| TC-E003-026-03 | Ohne Grund stornieren | 400 Validierungsfehler |
| TC-E003-026-04 | Kind versucht Stornierung | 403 Forbidden |
| TC-E003-026-05 | Bereits stornierte Transaktion | 409 Konflikt |

## Story Points

5

## Priorität

Mittel
