# Story M008-S07: Historie der ausgeführten Zahlungen

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **die Historie aller ausgeführten automatischen Zahlungen sehen**, damit **ich nachvollziehen kann, wann welche Zahlungen erfolgt sind**.

## Akzeptanzkriterien

- [ ] Gegeben eine wiederkehrende Zahlung, wenn ich die Details öffne, dann sehe ich eine Liste aller Ausführungen
- [ ] Gegeben die Ausführungsliste, wenn eine Ausführung angezeigt wird, dann sehe ich Datum, Betrag und Status
- [ ] Gegeben eine fehlgeschlagene Ausführung, wenn sie angezeigt wird, dann sehe ich den Fehlergrund
- [ ] Gegeben die Historie, wenn ich einen Eintrag antippe, dann werde ich zur entsprechenden Transaktion geleitet
- [ ] Gegeben die Historie, wenn sie angezeigt wird, dann sehe ich auch die Gesamtsumme aller Ausführungen

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Zahlungshistorie│
├─────────────────────────────┤
│                             │
│  Taschengeld für Emma       │
│  5,00 € wöchentlich         │
│                             │
│  Zusammenfassung:           │
│  ┌─────────────────────────┐│
│  │ Ausgeführt: 12x         ││
│  │ Gesamt: 60,00 €         ││
│  │ Erste: 01.11.2023       ││
│  │ Letzte: 14.01.2024      ││
│  └─────────────────────────┘│
│                             │
├─────────────────────────────┤
│  Ausführungen               │
│  ┌─────────────────────────┐│
│  │ ✅ 14.01.2024   5,00 € ││
│  │    Erfolgreich          ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ✅ 07.01.2024   5,00 € ││
│  │    Erfolgreich          ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ⏸️ 31.12.2023   -       ││
│  │    Übersprungen (Pause) ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ✅ 24.12.2023   5,00 € ││
│  │    Erfolgreich          ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ ❌ 17.12.2023   -       ││
│  │    Fehlgeschlagen       ││
│  │    (Serverfehler)       ││
│  └─────────────────────────┘│
│                             │
│  [Mehr laden...]            │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `PaymentHistoryPage.xaml`
- **ViewModel**: `PaymentHistoryViewModel.cs`
- **Model**: `PaymentExecution.cs`

## API-Endpunkt

```
GET /api/recurring-payments/{paymentId}/executions?page=1&pageSize=20
Authorization: Bearer {parent-token}

Response 200:
{
  "paymentId": "guid",
  "description": "Taschengeld",
  "childName": "Emma",
  "summary": {
    "totalExecutions": 12,
    "successfulExecutions": 11,
    "totalAmount": 55.00,
    "firstExecution": "2023-11-01T00:00:00Z",
    "lastExecution": "2024-01-14T00:00:00Z"
  },
  "executions": [
    {
      "executionId": "guid",
      "date": "2024-01-14T00:01:00Z",
      "amount": 5.00,
      "status": "success",
      "transactionId": "guid"
    },
    {
      "executionId": "guid",
      "date": "2023-12-31T00:00:00Z",
      "amount": null,
      "status": "skipped",
      "reason": "Zahlung pausiert"
    },
    {
      "executionId": "guid",
      "date": "2023-12-17T00:01:00Z",
      "amount": null,
      "status": "failed",
      "errorMessage": "Serverfehler bei der Verarbeitung"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 12
}
```

## Status-Typen

| Status | Icon | Beschreibung |
|--------|------|--------------|
| success | ✅ | Erfolgreich ausgeführt |
| failed | ❌ | Fehlgeschlagen |
| skipped | ⏸️ | Übersprungen (Pause) |
| pending | 🕐 | Steht aus |

## Technische Notizen

- Pagination für große Historien
- Verlinkung zur Transaktion bei erfolgreichen Ausführungen
- Fehlgeschlagene Ausführungen mit Grund dokumentieren
- Übersprungene Ausführungen (wg. Pause) auch anzeigen
- Export-Möglichkeit für Archivierung

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-07-01 | Erfolgreiche Ausführungen | Liste mit Grün-Icon |
| TC-M008-07-02 | Fehlgeschlagene Ausführung | Rot mit Fehlergrund |
| TC-M008-07-03 | Übersprungene Ausführung | Grau mit "Pause" |
| TC-M008-07-04 | Tap auf Ausführung | Navigation zu Transaktion |
| TC-M008-07-05 | Gesamtsumme | Korrekt berechnet |

## Story Points

2

## Priorität

Niedrig

## Status

⬜ Offen
