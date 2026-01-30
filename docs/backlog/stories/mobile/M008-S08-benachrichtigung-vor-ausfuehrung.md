# Story M008-S08: Benachrichtigung vor Ausführung

## Epic
M008 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **optional vor einer automatischen Zahlung benachrichtigt werden**, damit **ich die Zahlung bei Bedarf noch stoppen oder anpassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine wiederkehrende Zahlung, wenn ich sie erstelle, dann kann ich eine Vorab-Benachrichtigung aktivieren
- [ ] Gegeben aktivierte Benachrichtigung, wenn die Zahlung am nächsten Tag fällig ist, dann erhalte ich eine Push-Nachricht
- [ ] Gegeben die Benachrichtigung, wenn ich darauf tippe, dann kann ich die Zahlung direkt bearbeiten oder pausieren
- [ ] Gegeben die Benachrichtigungseinstellungen, wenn ich sie konfiguriere, dann kann ich wählen wie lange vorher ich benachrichtigt werde
- [ ] Gegeben eine pausierte Zahlung, wenn sie pausiert ist, dann erhalte ich keine Vorab-Benachrichtigung

## UI-Entwurf

```
In Zahlungs-Einstellungen:
┌─────────────────────────────┐
│  Benachrichtigungen         │
├─────────────────────────────┤
│                             │
│  Vor Ausführung             │
│  benachrichtigen            │
│  [=========○=====] An       │
│                             │
│  Wann benachrichtigen?      │
│  ┌─────────────────────────┐│
│  │ (●) 1 Tag vorher        ││
│  │ ( ) 2 Tage vorher       ││
│  │ ( ) 1 Stunde vorher     ││
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘

Push-Benachrichtigung:
┌─────────────────────────────┐
│  📅 TaschengeldManager      │
│                             │
│  Morgen wird das Taschengeld│
│  für Emma (5,00 €) auto-    │
│  matisch überwiesen.        │
│                             │
│  [Bearbeiten] [OK]          │
│                             │
└─────────────────────────────┘

Nach Tippen auf Benachrichtigung:
┌─────────────────────────────┐
│  ← Zurück   Anstehende Zahl.│
├─────────────────────────────┤
│                             │
│  ┌─────────────────────────┐│
│  │ 💰 Taschengeld          ││
│  │    für Emma             ││
│  │                         ││
│  │ Betrag: 5,00 €          ││
│  │ Ausführung: Morgen      ││
│  │ (21.01.2024, 00:01 Uhr) ││
│  └─────────────────────────┘│
│                             │
│  Was möchtest du tun?       │
│                             │
│  ┌───────────────────────┐  │
│  │   ✅ Ausführen lassen  │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │   ✏️ Betrag anpassen   │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │   ⏸️ Diesmal aussetzen │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │   ⏹️ Zahlung pausieren │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `UpcomingPaymentActionPage.xaml`
- **ViewModel**: `UpcomingPaymentActionViewModel.cs`
- **Service**: `INotificationService.cs`, `IRecurringPaymentService.cs`

## API-Endpunkte

```
PUT /api/recurring-payments/{paymentId}/notification-settings
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "notifyBeforeExecution": true,
  "notifyHoursBefore": 24
}

Response 200:
{
  "message": "Benachrichtigungseinstellungen gespeichert"
}

POST /api/recurring-payments/{paymentId}/skip-next
Authorization: Bearer {parent-token}

Response 200:
{
  "message": "Nächste Ausführung wird übersprungen",
  "skippedDate": "2024-01-21T00:00:00Z",
  "newNextExecutionDate": "2024-01-28T00:00:00Z"
}
```

## Push-Notification Payload

```json
{
  "type": "upcoming_payment",
  "paymentId": "guid",
  "title": "Taschengeld morgen",
  "body": "Morgen wird das Taschengeld für Emma (5,00 €) automatisch überwiesen.",
  "actions": [
    {"id": "edit", "title": "Bearbeiten"},
    {"id": "skip", "title": "Aussetzen"}
  ]
}
```

## Technische Notizen

- Scheduled Notifications über Backend-Job
- Deep Link in Notification für direkte Navigation
- "Diesmal aussetzen" überspringt nur nächste Ausführung
- Actionable Notifications für iOS/Android
- Benachrichtigungszeit konfigurierbar (1/2 Tage, 1 Stunde vorher)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M008-08-01 | Benachrichtigung aktiviert | Push 1 Tag vorher |
| TC-M008-08-02 | 2 Tage vorher | Push 2 Tage vorher |
| TC-M008-08-03 | "Diesmal aussetzen" | Nur nächste übersprungen |
| TC-M008-08-04 | Pausierte Zahlung | Keine Benachrichtigung |
| TC-M008-08-05 | Tap auf Notification | Navigation zur Aktion |

## Story Points

1

## Priorität

Niedrig

## Status

⬜ Offen
