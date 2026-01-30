# Story M008-S05: Zahlung löschen

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine wiederkehrende Zahlung löschen können**, damit **keine weiteren automatischen Zahlungen mehr ausgeführt werden**.

## Akzeptanzkriterien

- [ ] Gegeben eine bestehende Zahlung, wenn ich auf "Löschen" tippe, dann werde ich um Bestätigung gebeten
- [ ] Gegeben die Bestätigung, wenn ich bestätige, dann wird die Zahlung gelöscht
- [ ] Gegeben eine gelöschte Zahlung, wenn die Löschung abgeschlossen ist, dann werden keine weiteren Ausführungen mehr geplant
- [ ] Gegeben die Löschung, wenn sie erfolgt ist, dann bleiben bereits ausgeführte Transaktionen erhalten
- [ ] Gegeben die Bestätigung, wenn ich abbreche, dann bleibt die Zahlung bestehen

## UI-Entwurf

```
┌─────────────────────────────┐
│  ⚠️ Zahlung löschen?        │
├─────────────────────────────┤
│                             │
│  Möchtest du diese          │
│  wiederkehrende Zahlung     │
│  wirklich löschen?          │
│                             │
│  ┌─────────────────────────┐│
│  │ 💰 Taschengeld          ││
│  │    5,00 € wöchentlich   ││
│  │    für Emma             ││
│  │                         ││
│  │ Erstellt: 01.01.2024    ││
│  │ Ausgeführt: 3x          ││
│  │ Gesamt: 15,00 €         ││
│  └─────────────────────────┘│
│                             │
│  ℹ️ Bereits ausgeführte     │
│  Zahlungen bleiben in der   │
│  Transaktionshistorie.      │
│                             │
│  ┌───────────────────────┐  │
│  │      Abbrechen        │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │   🗑️ Endgültig löschen │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Dialog**: `DeletePaymentDialog.xaml`
- **ViewModel**: `DeletePaymentViewModel.cs`
- **Service**: `IRecurringPaymentService.cs`

## API-Endpunkt

```
DELETE /api/recurring-payments/{paymentId}
Authorization: Bearer {parent-token}

Response 200:
{
  "message": "Wiederkehrende Zahlung gelöscht",
  "deletedAt": "2024-01-20T15:00:00Z",
  "totalExecutions": 3,
  "totalAmount": 15.00
}

Response 404:
{
  "error": "not_found",
  "message": "Zahlung nicht gefunden"
}
```

## Technische Notizen

- Soft-Delete: Zahlung wird als "deleted" markiert
- Bereits ausgeführte Transaktionen bleiben erhalten
- Scheduler überspringt gelöschte Zahlungen
- Löschung wird in Audit-Log protokolliert
- Bei Fehlern: Transaktion zurückrollen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-05-01 | Zahlung löschen | Erfolgreich gelöscht |
| TC-M008-05-02 | Bestätigung abbrechen | Zahlung bleibt |
| TC-M008-05-03 | Bereits gelöschte Zahlung | Fehler 404 |
| TC-M008-05-04 | Transaktionshistorie | Bleibt erhalten |
| TC-M008-05-05 | Nach Löschen | Keine weiteren Ausführungen |

## Story Points

1

## Priorität

Mittel

## Status

⬜ Offen
