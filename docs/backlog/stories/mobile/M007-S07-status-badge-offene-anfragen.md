# Story M007-S07: Status-Badge für offene Anfragen

## Epic
M007 - Geldanfragen

## User Story

Als **Elternteil** möchte ich **auf einen Blick sehen, wie viele offene Geldanfragen es gibt**, damit **ich keine Anfrage meiner Kinder übersehe**.

## Akzeptanzkriterien

- [ ] Gegeben offene Anfragen, wenn ich die App öffne, dann sehe ich ein Badge mit der Anzahl
- [ ] Gegeben das Badge, wenn es angezeigt wird, dann ist es auf dem Anfragen-Icon in der Navigation sichtbar
- [ ] Gegeben eine neue Anfrage, wenn sie eingeht, dann wird das Badge automatisch aktualisiert
- [ ] Gegeben alle Anfragen beantwortet, wenn keine mehr offen ist, dann verschwindet das Badge
- [ ] Gegeben mehrere offene Anfragen, wenn das Badge angezeigt wird, dann zeigt es die korrekte Anzahl

## UI-Entwurf

```
Bottom Navigation:
┌─────────────────────────────────────────┐
│                                         │
│  🏠      👧      📬       ⚙️            │
│ Home   Kinder  Anfragen  Einstellungen  │
│                  🔴2                    │
│                                         │
└─────────────────────────────────────────┘

Tab Bar (Alternative):
┌─────────────────────────────────────────┐
│ Übersicht │ Konten │ Anfragen│ Mehr    │
│           │        │   🔴3   │         │
└─────────────────────────────────────────┘

Dashboard-Karte:
┌─────────────────────────────┐
│  📬 Offene Anfragen    🔴2  │
│                             │
│  Emma wartet auf Antwort    │
│  für "Lego-Set" (15€)       │
│                             │
│  [Alle ansehen →]           │
└─────────────────────────────┘
```

## Page/ViewModel

- **Integration in**: `AppShell.xaml`, `DashboardPage.xaml`
- **ViewModel**: `AppShellViewModel.cs` (Badge-State)
- **Service**: `IRequestService.cs`, `IBadgeService.cs`

## API-Endpunkt

```
GET /api/families/{familyId}/requests/pending-count
Authorization: Bearer {parent-token}

Response 200:
{
  "pendingCount": 2,
  "urgentCount": 1,
  "lastRequestAt": "2024-01-20T14:00:00Z"
}
```

## SignalR Hub

```csharp
// Real-time Badge Updates
public interface IRequestHub
{
    Task OnNewRequest(RequestNotification notification);
    Task OnRequestResponded(string requestId);
}

// Client receives:
{
  "event": "new_request",
  "childName": "Emma",
  "amount": 15.00,
  "pendingCount": 3
}
```

## Technische Notizen

- Badge-Count wird lokal gecached
- Real-time Updates via SignalR wenn neue Anfrage eingeht
- Badge-Farbe: Rot für normale Anfragen, Pulsierend für dringende
- Plattform-natives Badge (iOS App Icon Badge, Android Notification Badge)
- Beim App-Start Badge-Count vom Server holen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-07-01 | 2 offene Anfragen | Badge zeigt "2" |
| TC-M007-07-02 | Neue Anfrage kommt | Badge +1 |
| TC-M007-07-03 | Anfrage beantwortet | Badge -1 |
| TC-M007-07-04 | Alle beantwortet | Badge verschwindet |
| TC-M007-07-05 | App-Start | Korrekter Count |

## Story Points

1

## Priorität

Mittel

## Status

⬜ Offen
