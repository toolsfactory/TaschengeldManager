# Story M008-S06: Nächste Ausführung anzeigen

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **sehen, wann die nächste automatische Zahlung ausgeführt wird**, damit **ich den Überblick über anstehende Zahlungen behalte**.

## Akzeptanzkriterien

- [ ] Gegeben eine aktive Zahlung, wenn sie in der Übersicht angezeigt wird, dann sehe ich das Datum der nächsten Ausführung
- [ ] Gegeben die nächste Ausführung, wenn sie heute ist, dann wird "Heute" angezeigt
- [ ] Gegeben die nächste Ausführung, wenn sie morgen ist, dann wird "Morgen" angezeigt
- [ ] Gegeben das Dashboard, wenn es angezeigt wird, dann sehe ich eine Zusammenfassung der nächsten Zahlungen
- [ ] Gegeben mehrere Zahlungen am selben Tag, wenn sie angezeigt werden, dann sind sie gruppiert

## UI-Entwurf

```
Dashboard-Widget:
┌─────────────────────────────┐
│  📅 Anstehende Zahlungen    │
├─────────────────────────────┤
│                             │
│  Heute                      │
│  ┌─────────────────────────┐│
│  │ 👧 Emma     +5,00 €     ││
│  │    Taschengeld          ││
│  └─────────────────────────┘│
│                             │
│  Morgen                     │
│  (keine)                    │
│                             │
│  Diese Woche                │
│  ┌─────────────────────────┐│
│  │ 👦 Max Jr. +3,00 €      ││
│  │    Freitag, 26.01.      ││
│  └─────────────────────────┘│
│                             │
│  [Alle Zahlungen →]         │
│                             │
└─────────────────────────────┘

Detailansicht einer Zahlung:
┌─────────────────────────────┐
│  💰 Taschengeld             │
├─────────────────────────────┤
│                             │
│  Für: Emma                  │
│  Betrag: 5,00 €             │
│  Intervall: Wöchentlich     │
│                             │
│  ─────────────────────────  │
│                             │
│  📅 Nächste Ausführung:     │
│  Sonntag, 21.01.2024        │
│  (in 2 Tagen)               │
│                             │
│  ─────────────────────────  │
│                             │
│  Übernächste Ausführungen:  │
│  • 28.01.2024               │
│  • 04.02.2024               │
│  • 11.02.2024               │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Widget**: `UpcomingPaymentsWidget.xaml`
- **Page**: `RecurringPaymentDetailPage.xaml`
- **ViewModel**: `UpcomingPaymentsViewModel.cs`

## API-Endpunkt

```
GET /api/families/{familyId}/recurring-payments/upcoming?days=7
Authorization: Bearer {parent-token}

Response 200:
{
  "today": [
    {
      "paymentId": "guid",
      "childName": "Emma",
      "description": "Taschengeld",
      "amount": 5.00,
      "executionDate": "2024-01-20T00:00:00Z"
    }
  ],
  "tomorrow": [],
  "thisWeek": [
    {
      "paymentId": "guid",
      "childName": "Max Jr.",
      "description": "Taschengeld",
      "amount": 3.00,
      "executionDate": "2024-01-26T00:00:00Z"
    }
  ],
  "totalThisMonth": 38.00
}

GET /api/recurring-payments/{paymentId}/upcoming-dates?count=5
Authorization: Bearer {parent-token}

Response 200:
{
  "upcomingDates": [
    "2024-01-21T00:00:00Z",
    "2024-01-28T00:00:00Z",
    "2024-02-04T00:00:00Z",
    "2024-02-11T00:00:00Z",
    "2024-02-18T00:00:00Z"
  ]
}
```

## Technische Notizen

- Relative Datums-Anzeige (Heute, Morgen, In X Tagen)
- Vorausschau der nächsten 3-5 Ausführungen berechnen
- Dashboard-Widget zeigt nur nächste 7 Tage
- Monatliche Gesamtsumme berechnen
- Cache für schnelle Anzeige, aktualisieren bei Änderungen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-06-01 | Ausführung heute | "Heute" angezeigt |
| TC-M008-06-02 | Ausführung morgen | "Morgen" angezeigt |
| TC-M008-06-03 | Ausführung in 5 Tagen | Wochentag + Datum |
| TC-M008-06-04 | Mehrere am selben Tag | Gruppiert anzeigen |
| TC-M008-06-05 | Vorschau der nächsten 5 | Korrekt berechnet |

## Story Points

1

## Priorität

Mittel

## Status

⬜ Offen
