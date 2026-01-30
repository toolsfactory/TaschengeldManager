# Story M008-S01: Wiederkehrende Zahlung erstellen

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine wiederkehrende Zahlung einrichten können**, damit **das Taschengeld automatisch und regelmäßig auf das Kinderkonto überwiesen wird**.

## Akzeptanzkriterien

- [ ] Gegeben die Kontoverwaltung, wenn ich auf "Wiederkehrende Zahlung" tippe, dann öffnet sich das Erstellungsformular
- [ ] Gegeben das Formular, wenn ich Betrag, Intervall und Starttag auswähle, dann kann ich die Zahlung erstellen
- [ ] Gegeben das Intervall, wenn ich es auswähle, dann kann ich zwischen täglich, wöchentlich, monatlich wählen
- [ ] Gegeben eine erstellte Zahlung, wenn das Datum erreicht ist, dann wird automatisch gebucht
- [ ] Gegeben die Zahlung, wenn ich ein Enddatum setze, dann wird die Zahlung nach diesem Datum beendet

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Neue aut. Zahlung│
├─────────────────────────────┤
│                             │
│  Für welches Kind?          │
│  [Emma ▼]                   │
│                             │
│  Betrag:                    │
│  ┌───────────────────────┐  │
│  │       5,00 €          │  │
│  └───────────────────────┘  │
│                             │
│  Beschreibung:              │
│  ┌───────────────────────┐  │
│  │ Wöchentliches         │  │
│  │ Taschengeld           │  │
│  └───────────────────────┘  │
│                             │
│  Wiederholung:              │
│  ┌─────────────────────────┐│
│  │ ( ) Täglich            ││
│  │ (●) Wöchentlich        ││
│  │ ( ) Alle 2 Wochen      ││
│  │ ( ) Monatlich          ││
│  └─────────────────────────┘│
│                             │
│  Wochentag: [Sonntag ▼]     │
│                             │
│  Startet am:                │
│  [20.01.2024 📅]            │
│                             │
│  Endet: (optional)          │
│  [ ] Nie                    │
│  [ ] Nach ___ Zahlungen     │
│  [ ] Am [Datum]             │
│                             │
│  ┌───────────────────────┐  │
│  │   Zahlung erstellen   │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `CreateRecurringPaymentPage.xaml`
- **ViewModel**: `CreateRecurringPaymentViewModel.cs`
- **Model**: `RecurringPayment.cs`
- **Service**: `IRecurringPaymentService.cs`

## API-Endpunkt

```
POST /api/families/{familyId}/recurring-payments
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "childId": "guid",
  "amount": 5.00,
  "description": "Wöchentliches Taschengeld",
  "frequency": "weekly",
  "dayOfWeek": "sunday",
  "dayOfMonth": null,
  "startDate": "2024-01-20",
  "endDate": null,
  "endAfterOccurrences": null
}

Response 201:
{
  "paymentId": "guid",
  "nextExecutionDate": "2024-01-21T00:00:00Z",
  "message": "Wiederkehrende Zahlung erstellt"
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss größer als 0 sein"],
    "frequency": ["Bitte wähle ein Intervall"]
  }
}
```

## Frequenz-Optionen

| Frequenz | Beschreibung | Zusätzliche Felder |
|----------|--------------|-------------------|
| daily | Täglich | - |
| weekly | Wöchentlich | dayOfWeek |
| biweekly | Alle 2 Wochen | dayOfWeek |
| monthly | Monatlich | dayOfMonth (1-28) |

## Technische Notizen

- Backend-Job für automatische Ausführung (Hangfire/Quartz)
- Ausführung um 00:01 Uhr am konfigurierten Tag
- Bei monatlich: Tag max. 28 um Monatsprobleme zu vermeiden
- Zahlung wird als normale Transaktion gebucht mit Referenz
- Bei fehlgeschlagener Ausführung: Retry + Benachrichtigung

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-01-01 | Wöchentliche Zahlung | Wird erstellt |
| TC-M008-01-02 | Monatliche Zahlung | Wird erstellt |
| TC-M008-01-03 | Mit Enddatum | Endet automatisch |
| TC-M008-01-04 | Ohne Betrag | Validierungsfehler |
| TC-M008-01-05 | Automatische Ausführung | Transaktion wird gebucht |

## Story Points

3

## Priorität

Hoch

## Status

⬜ Offen
