# Story M006-S06: Verwandten einladen (per Email)

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **Verwandte per E-Mail zur Familie einladen können**, damit **Großeltern, Tanten oder Onkel ebenfalls Geld auf Kinderkonten einzahlen können**.

## Akzeptanzkriterien

- [ ] Gegeben die Familienverwaltung, wenn ich auf "Verwandten einladen" tippe, dann öffnet sich das Einladungsformular
- [ ] Gegeben das Einladungsformular, wenn ich E-Mail, Name und Rolle eingebe, dann wird eine Einladung verschickt
- [ ] Gegeben eine verschickte Einladung, wenn sie gesendet wurde, dann enthält die E-Mail einen Einladungslink
- [ ] Gegeben eine Einladung, wenn ich die Berechtigungen festlege, dann kann ich bestimmen was der Verwandte darf
- [ ] Gegeben eine bereits eingeladene E-Mail, wenn ich sie erneut einlade, dann wird die bestehende Einladung aktualisiert

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Einladung senden │
├─────────────────────────────┤
│                             │
│  Verwandten zur Familie     │
│  einladen                   │
│                             │
│  E-Mail-Adresse:            │
│  ┌───────────────────────┐  │
│  │ oma@example.com       │  │
│  └───────────────────────┘  │
│                             │
│  Name (wird angezeigt):     │
│  ┌───────────────────────┐  │
│  │ Oma Helga             │  │
│  └───────────────────────┘  │
│                             │
│  Rolle:                     │
│  [Verwandter ▼]             │
│                             │
│  Berechtigungen:            │
│  ┌─────────────────────────┐│
│  │ [✓] Kontostände sehen  ││
│  │ [✓] Einzahlungen       ││
│  │ [ ] Ausgaben erfassen  ││
│  │ [ ] Transaktionen sehen││
│  └─────────────────────────┘│
│                             │
│  Für diese Kinder:          │
│  [✓] Emma  [✓] Max Jr.      │
│                             │
│  Persönliche Nachricht:     │
│  ┌───────────────────────┐  │
│  │ Liebe Oma, damit du...│  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │   Einladung senden    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `InviteRelativePage.xaml`
- **ViewModel**: `InviteRelativeViewModel.cs`
- **Service**: `IInvitationService.cs`

## API-Endpunkt

```
POST /api/families/{familyId}/invitations
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "email": "oma@example.com",
  "name": "Oma Helga",
  "role": "relative",
  "permissions": {
    "canViewBalance": true,
    "canDeposit": true,
    "canRecordExpense": false,
    "canViewTransactions": false
  },
  "childIds": ["guid1", "guid2"],
  "personalMessage": "Liebe Oma, damit du..."
}

Response 201:
{
  "invitationId": "guid",
  "email": "oma@example.com",
  "status": "sent",
  "expiresAt": "2024-01-27T10:00:00Z",
  "inviteLink": "https://app.taschengeld.de/invite/ABC123XYZ"
}

Response 400:
{
  "errors": {
    "email": ["Ungültige E-Mail-Adresse"]
  }
}
```

## Technische Notizen

- Einladungslink mit Token (gültig für 7 Tage)
- E-Mail-Template mit Branding und persönlicher Nachricht
- Berechtigungen granular pro Kind einstellbar
- Rollen: "relative" (Verwandter), "parent" (zweiter Elternteil)
- Rate-Limiting für Einladungen (max. 10/Tag)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-06-01 | Gültige E-Mail | Einladung wird gesendet |
| TC-M006-06-02 | Ungültige E-Mail | Validierungsfehler |
| TC-M006-06-03 | Bereits eingeladen | Einladung aktualisiert |
| TC-M006-06-04 | Mit Berechtigungen | Korrekt gespeichert |
| TC-M006-06-05 | Ohne Kinder ausgewählt | Validierungsfehler |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
