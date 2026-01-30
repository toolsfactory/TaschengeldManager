# Story M005-S04: Ausgabe für Kind erfassen

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **eine Ausgabe für mein Kind erfassen können**, damit **der Kontostand korrekt geführt wird, wenn das Kind Geld ausgegeben hat**.

## Akzeptanzkriterien

- [ ] Gegeben die Konto-Detailansicht, wenn ich auf "Ausgabe" tippe, dann öffnet sich ein Ausgabendialog
- [ ] Gegeben der Ausgabendialog, wenn ich einen Betrag eingebe, dann wird geprüft ob genügend Guthaben vorhanden ist
- [ ] Gegeben nicht genügend Guthaben, wenn ich die Ausgabe bestätige, dann erscheint eine Warnung mit Option zum Überschreiben
- [ ] Gegeben eine gültige Ausgabe, wenn ich bestätige, dann wird der Kontostand sofort reduziert
- [ ] Gegeben der Ausgabendialog, wenn ich eine Kategorie auswähle, dann wird diese bei der Transaktion gespeichert
- [ ] Gegeben die Ausgabe, wenn sie erfolgreich war, dann erscheint sie in der Transaktionshistorie als Ausgabe

## UI-Entwurf

```
┌─────────────────────────────┐
│  × Ausgabe erfassen         │
├─────────────────────────────┤
│                             │
│  Ausgabe für Emma           │
│  Verfügbar: 45,00 €         │
│                             │
│  Betrag:                    │
│  ┌───────────────────────┐  │
│  │           €           │  │
│  │        0,00           │  │
│  └───────────────────────┘  │
│                             │
│  Wofür wurde das Geld       │
│  ausgegeben?                │
│  ┌───────────────────────┐  │
│  │                       │  │
│  └───────────────────────┘  │
│                             │
│  Kategorie:                 │
│  [Süßigkeiten ▼]            │
│                             │
│  Kategorien:                │
│  🍬 Süßigkeiten  🎮 Spielzeug│
│  📚 Bücher       👕 Kleidung │
│  🎬 Freizeit     📱 Sonstiges│
│                             │
│  ┌───────────────────────┐  │
│  │     Ausgabe buchen    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ExpensePage.xaml` (als Modal/BottomSheet)
- **ViewModel**: `ExpenseViewModel.cs`
- **Service**: `ITransactionService.cs`

## API-Endpunkt

```
POST /api/children/{childId}/transactions/expense
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "amount": 5.50,
  "description": "Eis gekauft",
  "category": "sweets",
  "allowOverdraft": false
}

Response 201:
{
  "transactionId": "guid",
  "newBalance": 39.50,
  "transaction": {
    "transactionId": "guid",
    "date": "2024-01-15T15:00:00Z",
    "amount": -5.50,
    "type": "expense",
    "description": "Eis gekauft",
    "category": "sweets"
  }
}

Response 400:
{
  "error": "insufficient_funds",
  "message": "Nicht genügend Guthaben",
  "currentBalance": 45.00,
  "requestedAmount": 50.00
}
```

## Technische Notizen

- Negativer Kontostand nur mit expliziter Bestätigung erlauben
- Kategorien als Enum mit Icons definieren
- Ausgaben werden als negative Beträge in der Transaktion gespeichert
- Kategorien für Statistiken später nutzbar

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-04-01 | Gültige Ausgabe 5€ | Kontostand -5€ |
| TC-M005-04-02 | Ausgabe > Guthaben | Warnung erscheint |
| TC-M005-04-03 | Überschreitung bestätigt | Ausgabe wird gebucht |
| TC-M005-04-04 | Kategorie auswählen | In Transaktion gespeichert |
| TC-M005-04-05 | Ohne Beschreibung | Validierungsfehler |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
