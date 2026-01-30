# Story M005-S03: Einzahlung auf Kind-Konto

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **Geld auf das Konto meines Kindes einzahlen können**, damit **das Kind über das Geld verfügen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Konto-Detailansicht, wenn ich auf "Einzahlung" tippe, dann öffnet sich ein Einzahlungsdialog
- [ ] Gegeben der Einzahlungsdialog, wenn ich einen Betrag eingebe, dann wird der Betrag validiert (positiv, max. 2 Dezimalstellen)
- [ ] Gegeben eine gültige Einzahlung, wenn ich bestätige, dann wird der Kontostand sofort aktualisiert
- [ ] Gegeben eine erfolgreiche Einzahlung, wenn sie abgeschlossen ist, dann erscheint die Transaktion in der Historie
- [ ] Gegeben der Einzahlungsdialog, wenn ich eine Beschreibung eingebe, dann wird diese bei der Transaktion gespeichert
- [ ] Gegeben vordefinierte Beträge (5€, 10€, 20€), wenn ich darauf tippe, dann wird der Betrag automatisch eingetragen

## UI-Entwurf

```
┌─────────────────────────────┐
│  × Einzahlung               │
├─────────────────────────────┤
│                             │
│  Einzahlung für Emma        │
│                             │
│  Betrag:                    │
│  ┌───────────────────────┐  │
│  │           €           │  │
│  │        0,00           │  │
│  └───────────────────────┘  │
│                             │
│  Schnellauswahl:            │
│  [5 €] [10 €] [20 €] [50 €] │
│                             │
│  Beschreibung (optional):   │
│  ┌───────────────────────┐  │
│  │ z.B. Taschengeld      │  │
│  └───────────────────────┘  │
│                             │
│  Kategorie:                 │
│  [Taschengeld ▼]            │
│                             │
│  ┌───────────────────────┐  │
│  │     Einzahlen         │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `DepositPage.xaml` (als Modal/BottomSheet)
- **ViewModel**: `DepositViewModel.cs`
- **Service**: `ITransactionService.cs`

## API-Endpunkt

```
POST /api/children/{childId}/transactions/deposit
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "amount": 10.00,
  "description": "Taschengeld",
  "category": "allowance",
  "notes": "Wöchentliches Taschengeld"
}

Response 201:
{
  "transactionId": "guid",
  "newBalance": 55.00,
  "transaction": {
    "transactionId": "guid",
    "date": "2024-01-15T14:30:00Z",
    "amount": 10.00,
    "type": "deposit",
    "description": "Taschengeld"
  }
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss größer als 0 sein"]
  }
}
```

## Technische Notizen

- Optimistic UI Update: Kontostand sofort anpassen, bei Fehler zurückrollen
- Beträge immer mit 2 Dezimalstellen formatieren
- Haptic Feedback bei erfolgreicher Einzahlung
- Kategorien: Taschengeld, Geschenk, Belohnung, Sonstiges

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-03-01 | Gültige Einzahlung 10€ | Kontostand +10€ |
| TC-M005-03-02 | Schnellauswahl 5€ | Betrag wird eingetragen |
| TC-M005-03-03 | Negativer Betrag | Validierungsfehler |
| TC-M005-03-04 | Betrag mit 3 Dezimalen | Auf 2 Dezimalen runden |
| TC-M005-03-05 | Ohne Beschreibung | Einzahlung erfolgreich |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
