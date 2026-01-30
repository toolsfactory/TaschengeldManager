# Story M007-S01: Anfrage erstellen

## Epic
M007 - Geldanfragen

## User Story

Als **Kind** möchte ich **eine Geldanfrage an meine Eltern stellen können**, damit **ich um zusätzliches Taschengeld oder Geld für einen bestimmten Wunsch bitten kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggtes Kind, wenn ich auf "Geld anfragen" tippe, dann öffnet sich das Anfrageformular
- [ ] Gegeben das Anfrageformular, wenn ich Betrag und Grund eingebe, dann kann ich die Anfrage absenden
- [ ] Gegeben eine abgesendete Anfrage, wenn sie erfolgreich war, dann sehen meine Eltern eine Benachrichtigung
- [ ] Gegeben das Anfrageformular, wenn ich einen Betrag > 100€ eingebe, dann erhalte ich einen Hinweis
- [ ] Gegeben die Anfrage, wenn ich ein Bild als Begründung hinzufüge, dann wird es mit der Anfrage gespeichert

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Geld anfragen  │
├─────────────────────────────┤
│                             │
│  Wieviel möchtest du        │
│  anfragen?                  │
│                             │
│  ┌───────────────────────┐  │
│  │         €             │  │
│  │       0,00            │  │
│  └───────────────────────┘  │
│                             │
│  Schnellauswahl:            │
│  [2 €] [5 €] [10 €] [20 €]  │
│                             │
│  Wofür brauchst du das      │
│  Geld?                      │
│  ┌───────────────────────┐  │
│  │ z.B. Neues Spielzeug  │  │
│  │                       │  │
│  │                       │  │
│  └───────────────────────┘  │
│                             │
│  Bild hinzufügen (optional):│
│  ┌─────────┐                │
│  │  📷     │                │
│  │ + Foto  │                │
│  └─────────┘                │
│                             │
│  Dringlichkeit:             │
│  ( ) Normal                 │
│  ( ) Wenn möglich bald      │
│  ( ) Dringend               │
│                             │
│  ┌───────────────────────┐  │
│  │   Anfrage absenden    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `CreateRequestPage.xaml`
- **ViewModel**: `CreateRequestViewModel.cs`
- **Model**: `MoneyRequest.cs`
- **Service**: `IRequestService.cs`

## API-Endpunkt

```
POST /api/children/{childId}/requests
Authorization: Bearer {child-token}
Content-Type: application/json

{
  "amount": 15.00,
  "reason": "Neues Lego-Set",
  "imageBase64": "optional-base64-string",
  "urgency": "normal"
}

Response 201:
{
  "requestId": "guid",
  "amount": 15.00,
  "reason": "Neues Lego-Set",
  "status": "pending",
  "createdAt": "2024-01-20T14:00:00Z",
  "notificationSent": true
}

Response 400:
{
  "errors": {
    "amount": ["Bitte gib einen Betrag ein"],
    "reason": ["Bitte gib einen Grund an"]
  }
}
```

## Technische Notizen

- Push-Notification an alle Elternteile senden
- Bilder komprimieren vor Upload (max 1MB)
- Maximal 1 offene Anfrage pro Kind (konfigurierbar)
- Kindgerechte UI mit großen Buttons und einfacher Sprache
- Dringlichkeit beeinflusst Sortierung für Eltern

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-01-01 | Gültige Anfrage | Wird erstellt |
| TC-M007-01-02 | Ohne Betrag | Validierungsfehler |
| TC-M007-01-03 | Ohne Grund | Validierungsfehler |
| TC-M007-01-04 | Mit Bild | Bild wird gespeichert |
| TC-M007-01-05 | Eltern-Benachrichtigung | Push wird gesendet |

## Story Points

3

## Priorität

Hoch

## Status

⬜ Offen
