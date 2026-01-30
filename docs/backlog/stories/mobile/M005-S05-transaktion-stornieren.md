# Story M005-S05: Transaktion stornieren

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **eine fehlerhafte Transaktion stornieren können**, damit **ich versehentliche oder falsche Buchungen korrigieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine Transaktion in der Historie, wenn ich lange darauf drücke oder das Kontextmenü öffne, dann sehe ich die Option "Stornieren"
- [ ] Gegeben die Stornierungsoption, wenn ich sie auswähle, dann werde ich um Bestätigung gebeten
- [ ] Gegeben eine bestätigte Stornierung, wenn sie durchgeführt wird, dann wird eine Gegenbuchung erstellt
- [ ] Gegeben eine stornierte Transaktion, wenn die Stornierung abgeschlossen ist, dann wird der Kontostand entsprechend angepasst
- [ ] Gegeben eine stornierte Transaktion, wenn sie in der Historie angezeigt wird, dann ist sie als "storniert" markiert
- [ ] Gegeben eine Stornierung, wenn sie durchgeführt wird, dann muss ein Grund angegeben werden

## UI-Entwurf

```
┌─────────────────────────────┐
│  Transaktion stornieren?    │
├─────────────────────────────┤
│                             │
│  Transaktion:               │
│  ┌─────────────────────────┐│
│  │ ▲ Taschengeld    +5,00 €││
│  │   15. Jan 2024           ││
│  └─────────────────────────┘│
│                             │
│  Grund für Stornierung:     │
│  ┌───────────────────────┐  │
│  │ ○ Falscher Betrag     │  │
│  │ ○ Falsche Person      │  │
│  │ ○ Doppelte Buchung    │  │
│  │ ○ Sonstiges           │  │
│  └───────────────────────┘  │
│                             │
│  Notiz (optional):          │
│  ┌───────────────────────┐  │
│  │                       │  │
│  └───────────────────────┘  │
│                             │
│  ⚠️ Diese Aktion kann nicht │
│  rückgängig gemacht werden  │
│                             │
│  [Abbrechen] [Stornieren]   │
│                             │
└─────────────────────────────┘

Nach Stornierung in Historie:
┌─────────────────────────────┐
│ ▲ Taschengeld       +5,00 €│
│   15. Jan 2024  [STORNIERT] │
│   ↳ Storno: -5,00 €         │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `CancelTransactionDialog.xaml`
- **ViewModel**: `CancelTransactionViewModel.cs`
- **Service**: `ITransactionService.cs`

## API-Endpunkt

```
POST /api/transactions/{transactionId}/cancel
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "reason": "wrong_amount",
  "notes": "Sollte 10€ statt 5€ sein"
}

Response 200:
{
  "originalTransactionId": "guid",
  "cancelTransactionId": "guid",
  "newBalance": 40.00,
  "cancelledAt": "2024-01-15T16:00:00Z"
}

Response 400:
{
  "error": "already_cancelled",
  "message": "Diese Transaktion wurde bereits storniert"
}

Response 400:
{
  "error": "cannot_cancel",
  "message": "Diese Transaktion kann nicht storniert werden"
}
```

## Technische Notizen

- Stornierung erstellt eine Gegenbuchung (nicht Löschung)
- Original-Transaktion wird mit Referenz zur Stornierung markiert
- Stornierungen sollten auditierbar sein (wer, wann, warum)
- Bereits stornierte Transaktionen können nicht erneut storniert werden
- Bestimmte Transaktionstypen (z.B. Systemzinsen) sind evtl. nicht stornierbar

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-05-01 | Einzahlung stornieren | Kontostand -Betrag |
| TC-M005-05-02 | Ausgabe stornieren | Kontostand +Betrag |
| TC-M005-05-03 | Bereits stornierte Transaktion | Fehler, nicht möglich |
| TC-M005-05-04 | Stornierung ohne Grund | Validierungsfehler |
| TC-M005-05-05 | Storno in Historie | Als storniert markiert |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
