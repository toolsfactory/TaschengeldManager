# Story M008-S03: Zahlung bearbeiten

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine bestehende wiederkehrende Zahlung bearbeiten können**, damit **ich Betrag, Intervall oder andere Details anpassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine bestehende Zahlung, wenn ich auf "Bearbeiten" tippe, dann öffnet sich das Bearbeitungsformular
- [ ] Gegeben das Bearbeitungsformular, wenn ich den Betrag ändere, dann wird er ab der nächsten Ausführung verwendet
- [ ] Gegeben das Bearbeitungsformular, wenn ich das Intervall ändere, dann wird die nächste Ausführung neu berechnet
- [ ] Gegeben die Änderungen, wenn ich speichere, dann werden sie sofort wirksam
- [ ] Gegeben eine pausierte Zahlung, wenn ich sie bearbeite, dann kann ich sie auch wieder aktivieren

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Zahlung bearbeiten│
├─────────────────────────────┤
│                             │
│  Taschengeld für Emma       │
│                             │
│  Status: [Aktiv ▼]          │
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
│  [Wöchentlich ▼]            │
│                             │
│  Wochentag:                 │
│  [Sonntag ▼]                │
│                             │
│  Endet:                     │
│  [Nie ▼]                    │
│                             │
│  ─────────────────────────  │
│  Statistik:                 │
│  Erstellt: 01.01.2024       │
│  Ausgeführt: 3x             │
│  Gesamt: 15,00 €            │
│  ─────────────────────────  │
│                             │
│  ┌───────────────────────┐  │
│  │  Änderungen speichern │  │
│  └───────────────────────┘  │
│                             │
│  [🗑️ Zahlung löschen]       │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `EditRecurringPaymentPage.xaml`
- **ViewModel**: `EditRecurringPaymentViewModel.cs`
- **Service**: `IRecurringPaymentService.cs`

## API-Endpunkt

```
PUT /api/recurring-payments/{paymentId}
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "amount": 6.00,
  "description": "Wöchentliches Taschengeld (erhöht)",
  "frequency": "weekly",
  "dayOfWeek": "sunday",
  "status": "active",
  "endDate": null
}

Response 200:
{
  "paymentId": "guid",
  "nextExecutionDate": "2024-01-21T00:00:00Z",
  "message": "Zahlung aktualisiert"
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss größer als 0 sein"]
  }
}
```

## Technische Notizen

- Änderungen gelten ab nächster Ausführung
- Bisherige Ausführungen bleiben unverändert
- Status kann auf "active" oder "paused" gesetzt werden
- Bei Intervall-Änderung: Nächste Ausführung neu berechnen
- Änderungshistorie für Audit speichern

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-03-01 | Betrag ändern | Ab nächster Ausführung |
| TC-M008-03-02 | Intervall ändern | Nächstes Datum neu berechnet |
| TC-M008-03-03 | Pausiert zu Aktiv | Zahlung wird wieder ausgeführt |
| TC-M008-03-04 | Beschreibung ändern | Wird gespeichert |
| TC-M008-03-05 | Ungültiger Betrag | Validierungsfehler |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
