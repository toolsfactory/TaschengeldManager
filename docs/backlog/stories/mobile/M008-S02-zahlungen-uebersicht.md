# Story M008-S02: Zahlungen-Übersicht

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **alle wiederkehrenden Zahlungen in einer Übersicht sehen**, damit **ich den Überblick über alle automatischen Zahlungen behalte**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn ich die Zahlungsübersicht öffne, dann sehe ich alle aktiven wiederkehrenden Zahlungen
- [ ] Gegeben die Übersicht, wenn eine Zahlung angezeigt wird, dann sehe ich Kind, Betrag, Intervall und nächste Ausführung
- [ ] Gegeben mehrere Zahlungen, wenn sie angezeigt werden, dann sind sie nach Kind gruppiert
- [ ] Gegeben pausierte Zahlungen, wenn sie angezeigt werden, dann sind sie als "Pausiert" markiert
- [ ] Gegeben die Übersicht, wenn ich auf eine Zahlung tippe, dann sehe ich die Details

## UI-Entwurf

```
┌─────────────────────────────┐
│  ☰ Wiederkehrende Zahlungen │
├─────────────────────────────┤
│                             │
│  Nächste Ausführungen       │
│  ┌─────────────────────────┐│
│  │ 📅 Morgen (21.01.)      ││
│  │ Emma - Taschengeld 5€   ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 📅 01.02.2024           ││
│  │ Max - Taschengeld 3€    ││
│  └─────────────────────────┘│
│                             │
├─────────────────────────────┤
│  👧 Emma                    │
│  ┌─────────────────────────┐│
│  │ 💰 Taschengeld   5,00 € ││
│  │    Wöchentlich, Sonntag ││
│  │    Nächste: 21.01.2024  ││
│  │    [Aktiv ✓]            ││
│  └─────────────────────────┘│
│                             │
│  👦 Max Jr.                 │
│  ┌─────────────────────────┐│
│  │ 💰 Taschengeld   3,00 € ││
│  │    Monatlich, 1.        ││
│  │    Nächste: 01.02.2024  ││
│  │    [Aktiv ✓]            ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 🎁 Sparzulage   2,00 € ││
│  │    Monatlich, 15.       ││
│  │    [⏸️ Pausiert]        ││
│  └─────────────────────────┘│
│                             │
│  [+ Neue Zahlung]           │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `RecurringPaymentsPage.xaml`
- **ViewModel**: `RecurringPaymentsViewModel.cs`
- **Model**: `RecurringPayment.cs`

## API-Endpunkt

```
GET /api/families/{familyId}/recurring-payments
Authorization: Bearer {parent-token}

Response 200:
{
  "upcomingExecutions": [
    {
      "paymentId": "guid",
      "childName": "Emma",
      "description": "Taschengeld",
      "amount": 5.00,
      "nextExecutionDate": "2024-01-21T00:00:00Z"
    }
  ],
  "paymentsByChild": [
    {
      "childId": "guid",
      "childName": "Emma",
      "payments": [
        {
          "paymentId": "guid",
          "amount": 5.00,
          "description": "Taschengeld",
          "frequency": "weekly",
          "dayOfWeek": "sunday",
          "nextExecutionDate": "2024-01-21T00:00:00Z",
          "status": "active",
          "totalPaid": 260.00,
          "executionCount": 52
        }
      ]
    }
  ],
  "totalMonthlyAmount": 38.00
}
```

## Technische Notizen

- Gruppierung nach Kind für bessere Übersicht
- Nächste Ausführungen prominent oben anzeigen
- Status: active, paused, ended
- Gesamtbetrag pro Monat berechnen und anzeigen
- Pull-to-Refresh für Aktualisierung

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-02-01 | Mehrere aktive Zahlungen | Alle werden angezeigt |
| TC-M008-02-02 | Pausierte Zahlung | Als pausiert markiert |
| TC-M008-02-03 | Keine Zahlungen | Leerer Zustand |
| TC-M008-02-04 | Nach Kind gruppiert | Korrekte Gruppierung |
| TC-M008-02-05 | Tap auf Zahlung | Navigation zu Details |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
