# Story M008-S04: Zahlung pausieren/fortsetzen

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine wiederkehrende Zahlung vorübergehend pausieren und später fortsetzen können**, damit **ich die Zahlung bei Bedarf (z.B. Urlaub) aussetzen kann, ohne sie zu löschen**.

## Akzeptanzkriterien

- [ ] Gegeben eine aktive Zahlung, wenn ich auf "Pausieren" tippe, dann wird die automatische Ausführung gestoppt
- [ ] Gegeben eine pausierte Zahlung, wenn sie in der Übersicht erscheint, dann ist sie als "Pausiert" markiert
- [ ] Gegeben eine pausierte Zahlung, wenn ich auf "Fortsetzen" tippe, dann wird sie wieder aktiviert
- [ ] Gegeben eine fortgesetzte Zahlung, wenn sie aktiviert wird, dann wird die nächste Ausführung neu berechnet
- [ ] Gegeben die Pausierung, wenn ich sie durchführe, dann kann ich optional einen Grund angeben

## UI-Entwurf

```
Pausieren-Dialog:
┌─────────────────────────────┐
│  Zahlung pausieren?         │
├─────────────────────────────┤
│                             │
│  Taschengeld für Emma       │
│  5,00 € wöchentlich         │
│                             │
│  Während der Pause werden   │
│  keine automatischen        │
│  Zahlungen ausgeführt.      │
│                             │
│  Grund (optional):          │
│  ┌───────────────────────┐  │
│  │ z.B. Urlaub           │  │
│  └───────────────────────┘  │
│                             │
│  [Abbrechen] [Pausieren]    │
│                             │
└─────────────────────────────┘

Fortsetzen-Dialog:
┌─────────────────────────────┐
│  Zahlung fortsetzen?        │
├─────────────────────────────┤
│                             │
│  Taschengeld für Emma       │
│  5,00 € wöchentlich         │
│                             │
│  Pausiert seit: 15.01.2024  │
│  Grund: Urlaub              │
│                             │
│  Nächste Ausführung:        │
│  Sonntag, 28.01.2024        │
│                             │
│  [Abbrechen] [Fortsetzen]   │
│                             │
└─────────────────────────────┘

In der Übersicht:
┌─────────────────────────────┐
│ 💰 Taschengeld      5,00 € │
│    Wöchentlich, Sonntag    │
│    ⏸️ Pausiert seit 15.01. │
│    Grund: Urlaub           │
│    [▶️ Fortsetzen]         │
└─────────────────────────────┘
```

## Page/ViewModel

- **Dialog**: `PausePaymentDialog.xaml`, `ResumePaymentDialog.xaml`
- **ViewModel**: `PausePaymentViewModel.cs`
- **Service**: `IRecurringPaymentService.cs`

## API-Endpunkte

```
POST /api/recurring-payments/{paymentId}/pause
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "reason": "Urlaub"
}

Response 200:
{
  "message": "Zahlung pausiert",
  "pausedAt": "2024-01-15T10:00:00Z"
}

POST /api/recurring-payments/{paymentId}/resume
Authorization: Bearer {parent-token}

Response 200:
{
  "message": "Zahlung fortgesetzt",
  "nextExecutionDate": "2024-01-28T00:00:00Z"
}
```

## Technische Notizen

- Pausierte Zahlungen werden bei Scheduler-Lauf übersprungen
- Bei Fortsetzung: Nächstes Datum basierend auf heutigem Datum berechnen
- Verpasste Zahlungen werden NICHT nachgeholt
- Pause-Grund wird für Übersichtlichkeit gespeichert
- Pause/Fortsetzung wird in Historie geloggt

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-04-01 | Zahlung pausieren | Status = paused |
| TC-M008-04-02 | Pausierte Zahlung fortsetzen | Status = active |
| TC-M008-04-03 | Ausführung während Pause | Wird übersprungen |
| TC-M008-04-04 | Nächstes Datum nach Fortsetzung | Korrekt berechnet |
| TC-M008-04-05 | Pause mit Grund | Grund wird gespeichert |

## Story Points

1

## Priorität

Mittel

## Status

⬜ Offen
