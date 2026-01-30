# Story E003-S021: Ausgabe erfassen (Kind)

## Epic
E003 - Transaktionen

## User Story

Als **Kind** möchte ich **meine Ausgaben selbst erfassen können**, damit **ich lerne, mein Geld zu verwalten und den Überblick zu behalten**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Konto, wenn es eine Ausgabe erfasst, dann wird der Betrag vom Kontostand abgezogen
- [ ] Gegeben eine Ausgabe, wenn sie erfasst wird, dann muss eine Beschreibung angegeben werden
- [ ] Gegeben eine Ausgabe, wenn das Guthaben nicht ausreicht, dann wird ein Fehler angezeigt
- [ ] Gegeben eine Ausgabe, wenn sie erfasst wird, dann kann optional eine Kategorie ausgewählt werden
- [ ] Gegeben eine erfasste Ausgabe, wenn sie abgeschlossen ist, dann werden die Eltern informiert (optional konfigurierbar)

## API-Endpunkt

```
POST /api/me/transactions
Authorization: Bearer {token}
Content-Type: application/json

{
  "type": "Expense",
  "amount": 5.50,
  "description": "Eis",
  "categoryId": "guid (optional)",
  "note": "string (optional)"
}

Response 201:
{
  "transactionId": "guid",
  "type": "Expense",
  "amount": -5.50,
  "description": "Eis",
  "category": "Süßigkeiten",
  "newBalance": 79.75,
  "createdAt": "datetime"
}

Response 400:
{
  "errors": {
    "amount": ["Nicht genügend Guthaben"],
    "description": ["Beschreibung ist erforderlich"]
  }
}

Response 404:
{
  "message": "Kein Konto vorhanden"
}
```

## Technische Notizen

- Ausgaben werden als negative Beträge gespeichert
- Guthaben-Check vor Transaktion (kein Überziehen möglich)
- Kinderfreundliche Fehlermeldungen
- Kategorien: vordefinierte Liste + benutzerdefinierte
- Benachrichtigung an Eltern (je nach Einstellung)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-021-01 | Valide Ausgabe | 201 + Kontostand reduziert |
| TC-E003-021-02 | Nicht genügend Guthaben | 400 mit Hinweis |
| TC-E003-021-03 | Ohne Beschreibung | 400 Validierungsfehler |
| TC-E003-021-04 | Mit Kategorie | 201 + Kategorie gespeichert |
| TC-E003-021-05 | Eltern werden benachrichtigt | Push an Eltern |

## Story Points

3

## Priorität

Hoch
