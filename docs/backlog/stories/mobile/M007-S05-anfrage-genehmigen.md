# Story M007-S05: Anfrage genehmigen

## Epic
M007 - Geldanfragen

## User Story

Als **Elternteil** möchte ich **eine Geldanfrage meines Kindes genehmigen können**, damit **das Geld auf sein Konto überwiesen wird und es seinen Wunsch erfüllen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine offene Anfrage, wenn ich sie öffne, dann sehe ich einen "Genehmigen"-Button
- [ ] Gegeben den Genehmigen-Button, wenn ich darauf tippe, dann kann ich den Betrag bestätigen oder anpassen
- [ ] Gegeben die Genehmigung, wenn ich bestätige, dann wird der Betrag automatisch auf das Kinderkonto gebucht
- [ ] Gegeben eine erfolgreiche Genehmigung, wenn sie abgeschlossen ist, dann erhält das Kind eine Benachrichtigung
- [ ] Gegeben die Genehmigung, wenn ich eine Nachricht hinzufüge, dann sieht das Kind diese Nachricht

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Anfrage        │
├─────────────────────────────┤
│                             │
│  Anfrage von Emma           │
│                             │
│  ┌─────────────────────────┐│
│  │ Betrag: 15,00 €         ││
│  │                         ││
│  │ Wofür: Lego-Set         ││
│  │                         ││
│  │ Dringlichkeit: Normal   ││
│  │                         ││
│  │ Erstellt: Vor 2 Stunden ││
│  └─────────────────────────┘│
│                             │
│  ┌─────────────────────────┐│
│  │        📷 Bild          ││
│  │     [Vorschau]          ││
│  └─────────────────────────┘│
│                             │
│  Genehmigter Betrag:        │
│  ┌───────────────────────┐  │
│  │      15,00 €          │  │
│  └───────────────────────┘  │
│                             │
│  Nachricht (optional):      │
│  ┌───────────────────────┐  │
│  │ Viel Spaß damit!      │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │  ❌ Ablehnen          │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │  ✅ Genehmigen        │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `RequestDetailParentPage.xaml`
- **ViewModel**: `RequestDetailParentViewModel.cs`
- **Service**: `IRequestService.cs`, `ITransactionService.cs`

## API-Endpunkt

```
POST /api/requests/{requestId}/approve
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "approvedAmount": 15.00,
  "message": "Viel Spaß damit!"
}

Response 200:
{
  "message": "Anfrage genehmigt",
  "requestId": "guid",
  "approvedAmount": 15.00,
  "transactionId": "guid",
  "newBalance": 60.00,
  "childNotified": true
}

Response 400:
{
  "error": "already_responded",
  "message": "Diese Anfrage wurde bereits beantwortet"
}
```

## Technische Notizen

- Genehmigter Betrag kann vom angefragten abweichen
- Bei Genehmigung wird automatisch eine Einzahlung erstellt
- Push-Notification an Kind mit optionaler Nachricht
- Transaktion wird mit Referenz zur Anfrage verknüpft
- In-App-Nachricht zusätzlich zur Push-Notification

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-05-01 | Genehmigung mit Originalbetrag | Erfolgreich |
| TC-M007-05-02 | Genehmigung mit angepasstem Betrag | Erfolgreich |
| TC-M007-05-03 | Mit Nachricht | Kind sieht Nachricht |
| TC-M007-05-04 | Bereits beantwortet | Fehler |
| TC-M007-05-05 | Kind-Benachrichtigung | Push wird gesendet |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
