# Story M007-S06: Anfrage ablehnen

## Epic
M007 - Geldanfragen

## User Story

Als **Elternteil** möchte ich **eine Geldanfrage meines Kindes ablehnen können**, damit **ich erklären kann, warum der Wunsch gerade nicht erfüllt werden kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine offene Anfrage, wenn ich auf "Ablehnen" tippe, dann öffnet sich ein Dialog
- [ ] Gegeben der Ablehnungs-Dialog, wenn ich einen Grund eingebe, dann kann ich die Ablehnung absenden
- [ ] Gegeben eine abgelehnte Anfrage, wenn die Ablehnung abgeschlossen ist, dann erhält das Kind eine Benachrichtigung
- [ ] Gegeben die Ablehnung, wenn sie erfolgt ist, dann sieht das Kind den Ablehnungsgrund
- [ ] Gegeben eine Ablehnung ohne Grund, wenn ich absenden will, dann werde ich aufgefordert einen Grund einzugeben

## UI-Entwurf

```
┌─────────────────────────────┐
│  × Anfrage ablehnen         │
├─────────────────────────────┤
│                             │
│  Anfrage von Emma           │
│  Betrag: 50,00 €            │
│  Wofür: Spielkonsole        │
│                             │
│  Warum lehnst du ab?        │
│  (Kind sieht diesen Text)   │
│                             │
│  Schnellauswahl:            │
│  ┌─────────────────────────┐│
│  │ ○ Zu teuer              ││
│  │ ○ Nicht notwendig       ││
│  │ ○ Spar dafür            ││
│  │ ○ Vielleicht später     ││
│  │ ○ Eigener Text          ││
│  └─────────────────────────┘│
│                             │
│  Begründung:                │
│  ┌───────────────────────┐  │
│  │ Das ist zu teuer.     │  │
│  │ Spar dafür bitte von  │  │
│  │ deinem Taschengeld.   │  │
│  └───────────────────────┘  │
│                             │
│  💡 Tipp: Eine freundliche  │
│  Erklärung hilft deinem     │
│  Kind zu verstehen.         │
│                             │
│  ┌───────────────────────┐  │
│  │      Abbrechen        │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │  Ablehnen & Erklären  │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `RejectRequestDialog.xaml`
- **ViewModel**: `RejectRequestViewModel.cs`
- **Service**: `IRequestService.cs`

## API-Endpunkt

```
POST /api/requests/{requestId}/reject
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "reason": "too_expensive",
  "message": "Das ist zu teuer. Spar dafür bitte von deinem Taschengeld."
}

Response 200:
{
  "message": "Anfrage abgelehnt",
  "requestId": "guid",
  "rejectedAt": "2024-01-20T18:00:00Z",
  "childNotified": true
}

Response 400:
{
  "errors": {
    "message": ["Bitte gib einen Grund für die Ablehnung an"]
  }
}
```

## Vordefinierte Ablehnungsgründe

| Code | Text (für Eltern) | Text (für Kind) |
|------|-------------------|-----------------|
| too_expensive | Zu teuer | Das ist leider zu teuer |
| not_necessary | Nicht notwendig | Das brauchst du gerade nicht |
| save_for_it | Spar dafür | Spar dafür von deinem Taschengeld |
| maybe_later | Vielleicht später | Vielleicht ein andermal |
| custom | Eigener Text | [Eigener Text] |

## Technische Notizen

- Grund ist Pflichtfeld (kindgerechte Erklärung)
- Vordefinierte Gründe für schnelle Auswahl
- Kindgerechte Formulierung der Benachrichtigung
- Push-Notification mit freundlichem Ton
- Ablehnung wird in Anfrage-Historie gespeichert

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-06-01 | Ablehnung mit Grund | Erfolgreich |
| TC-M007-06-02 | Vordefinierter Grund | Text wird eingetragen |
| TC-M007-06-03 | Ohne Grund | Validierungsfehler |
| TC-M007-06-04 | Kind-Benachrichtigung | Push mit Grund |
| TC-M007-06-05 | Kind sieht Ablehnung | Grund ist sichtbar |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
