# Story M006-S07: Einladungen verwalten

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **ausstehende Einladungen einsehen und verwalten können**, damit **ich den Überblick über verschickte Einladungen behalte und diese bei Bedarf widerrufen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Familienverwaltung, wenn ich auf "Einladungen" tippe, dann sehe ich alle ausstehenden Einladungen
- [ ] Gegeben eine ausstehende Einladung, wenn sie angezeigt wird, dann sehe ich E-Mail, Datum und Ablaufzeit
- [ ] Gegeben eine Einladung, wenn ich auf "Erneut senden" tippe, dann wird die E-Mail erneut verschickt
- [ ] Gegeben eine Einladung, wenn ich auf "Widerrufen" tippe, dann wird der Einladungslink ungültig
- [ ] Gegeben eine abgelaufene Einladung, wenn sie angezeigt wird, dann ist sie als "Abgelaufen" markiert

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Einladungen    │
├─────────────────────────────┤
│                             │
│  Ausstehende Einladungen    │
│                             │
│  ┌─────────────────────────┐│
│  │ 📧 oma@example.com      ││
│  │ Oma Helga (Verwandter)  ││
│  │ Gesendet: 20.01.2024    ││
│  │ Läuft ab: 27.01.2024    ││
│  │                         ││
│  │ [Erneut senden][Widerru.]│
│  └─────────────────────────┘│
│                             │
│  ┌─────────────────────────┐│
│  │ 📧 opa@example.com      ││
│  │ Opa Hans (Verwandter)   ││
│  │ ⚠️ Abgelaufen           ││
│  │                         ││
│  │ [Erneut einladen]       ││
│  └─────────────────────────┘│
│                             │
├─────────────────────────────┤
│  Angenommene Einladungen    │
│                             │
│  ┌─────────────────────────┐│
│  │ ✅ tante@example.com    ││
│  │ Tante Maria             ││
│  │ Beigetreten: 15.01.2024 ││
│  └─────────────────────────┘│
│                             │
│  [+ Neue Einladung]         │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ManageInvitationsPage.xaml`
- **ViewModel**: `ManageInvitationsViewModel.cs`
- **Service**: `IInvitationService.cs`

## API-Endpunkte

```
GET /api/families/{familyId}/invitations
Authorization: Bearer {parent-token}

Response 200:
{
  "pending": [
    {
      "invitationId": "guid",
      "email": "oma@example.com",
      "name": "Oma Helga",
      "role": "relative",
      "sentAt": "2024-01-20T10:00:00Z",
      "expiresAt": "2024-01-27T10:00:00Z",
      "status": "pending"
    }
  ],
  "expired": [
    {
      "invitationId": "guid",
      "email": "opa@example.com",
      "name": "Opa Hans",
      "expiredAt": "2024-01-15T10:00:00Z",
      "status": "expired"
    }
  ],
  "accepted": [
    {
      "invitationId": "guid",
      "email": "tante@example.com",
      "name": "Tante Maria",
      "acceptedAt": "2024-01-15T14:00:00Z",
      "status": "accepted"
    }
  ]
}

POST /api/invitations/{invitationId}/resend
Authorization: Bearer {parent-token}

Response 200:
{
  "message": "Einladung erneut gesendet",
  "newExpiresAt": "2024-01-27T10:00:00Z"
}

DELETE /api/invitations/{invitationId}
Authorization: Bearer {parent-token}

Response 200:
{
  "message": "Einladung widerrufen"
}
```

## Technische Notizen

- Einladungen nach Status gruppieren (ausstehend, abgelaufen, angenommen)
- Erneut senden verlängert Ablaufzeit
- Widerrufene Einladungen werden aus Liste entfernt
- Abgelaufene Einladungen können erneut gesendet werden
- Real-time Update wenn Einladung angenommen wird (SignalR/WebSocket)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-07-01 | Ausstehende Einladungen | Liste wird angezeigt |
| TC-M006-07-02 | Erneut senden | E-Mail wird verschickt |
| TC-M006-07-03 | Einladung widerrufen | Link wird ungültig |
| TC-M006-07-04 | Abgelaufene Einladung | Als abgelaufen markiert |
| TC-M006-07-05 | Keine Einladungen | Leerer Zustand |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
