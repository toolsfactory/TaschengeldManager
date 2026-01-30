# Story M005-S01: Alle Kinderkonten anzeigen

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **eine Übersicht aller Kinderkonten meiner Familie sehen**, damit **ich den aktuellen Kontostand jedes Kindes auf einen Blick erfassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggtes Elternteil, wenn es die Kontoübersicht öffnet, dann werden alle Kinderkonten der Familie angezeigt
- [ ] Gegeben mehrere Kinder, wenn die Liste angezeigt wird, dann zeigt jede Zeile Name, Avatar und aktuellen Kontostand
- [ ] Gegeben ein Kinderkonto, wenn ich darauf tippe, dann werde ich zur Konto-Detailansicht weitergeleitet
- [ ] Gegeben keine Kinder in der Familie, wenn die Übersicht angezeigt wird, dann erscheint ein Hinweis mit der Möglichkeit ein Kind hinzuzufügen
- [ ] Gegeben die Kontenübersicht, wenn ich nach unten ziehe, dann werden die Daten aktualisiert (Pull-to-Refresh)

## UI-Entwurf

```
┌─────────────────────────────┐
│  ☰  Kinderkonten       🔔   │
├─────────────────────────────┤
│                             │
│  Gesamtguthaben: 127,50 €   │
│                             │
├─────────────────────────────┤
│  ┌─────────────────────────┐│
│  │ 👧 Emma                 ││
│  │ Kontostand: 45,00 €     ││
│  │ Letzte Aktivität: Heute ││
│  └─────────────────────────┘│
│                             │
│  ┌─────────────────────────┐│
│  │ 👦 Max                  ││
│  │ Kontostand: 82,50 €     ││
│  │ Letzte Aktivität: Gestern│
│  └─────────────────────────┘│
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ChildAccountsOverviewPage.xaml`
- **ViewModel**: `ChildAccountsOverviewViewModel.cs`
- **Model**: `ChildAccountSummary.cs`

## API-Endpunkt

```
GET /api/family/children/accounts
Authorization: Bearer {parent-token}

Response 200:
{
  "totalBalance": 127.50,
  "children": [
    {
      "childId": "guid",
      "firstName": "Emma",
      "nickname": "emma",
      "avatarUrl": "string",
      "accountId": "guid",
      "balance": 45.00,
      "lastActivityDate": "2024-01-15T10:30:00Z"
    }
  ]
}
```

## Technische Notizen

- Kontostand sollte cached werden für schnelle Anzeige
- Bei Rückkehr zur Seite automatisch aktualisieren
- Avatar als Fallback mit Initialen wenn kein Bild vorhanden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-01-01 | Elternteil mit 2 Kindern | Liste zeigt beide Kinder |
| TC-M005-01-02 | Elternteil ohne Kinder | Leerer Zustand mit Hinweis |
| TC-M005-01-03 | Pull-to-Refresh | Daten werden neu geladen |
| TC-M005-01-04 | Tap auf Kinderkarte | Navigation zur Detailansicht |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
