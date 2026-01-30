# Story M006-S02: Familienmitglieder-Liste

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **alle Mitglieder meiner Familie in einer Liste sehen**, damit **ich einen Überblick über die Familienstruktur habe**.

## Akzeptanzkriterien

- [ ] Gegeben ein eingeloggtes Elternteil, wenn es die Familienverwaltung öffnet, dann sehe ich alle Familienmitglieder
- [ ] Gegeben die Mitgliederliste, wenn sie angezeigt wird, dann werden Mitglieder nach Rolle gruppiert (Eltern, Verwandte, Kinder)
- [ ] Gegeben ein Familienmitglied, wenn es in der Liste angezeigt wird, dann sehe ich Name, Rolle und Avatar
- [ ] Gegeben ein Mitglied mit ausstehender Einladung, wenn es in der Liste erscheint, dann ist es als "Eingeladen" markiert
- [ ] Gegeben die Familienmitgliederliste, wenn ich auf ein Mitglied tippe, dann kann ich dessen Profil bearbeiten oder verwalten

## UI-Entwurf

```
┌─────────────────────────────┐
│  ☰ Familie Müller      ⚙️   │
├─────────────────────────────┤
│                             │
│  Familien-Code: ABC123 📋   │
│                             │
├─────────────────────────────┤
│  👨‍👩‍👧 Eltern (2)              │
│  ┌─────────────────────────┐│
│  │ 👤 Max Müller           ││
│  │    Erstellt am 15.01.24 ││
│  │    [Admin]              ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 👤 Anna Müller          ││
│  │    Beigetreten 16.01.24 ││
│  └─────────────────────────┘│
│                             │
├─────────────────────────────┤
│  👴 Verwandte (1)           │
│  ┌─────────────────────────┐│
│  │ 👤 Oma Helga            ││
│  │    Nur Einzahlungen     ││
│  └─────────────────────────┘│
│                             │
├─────────────────────────────┤
│  👧 Kinder (2)              │
│  ┌─────────────────────────┐│
│  │ 👧 Emma                 ││
│  │    8 Jahre              ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 👦 Max Jr.              ││
│  │    6 Jahre              ││
│  └─────────────────────────┘│
│                             │
│  [+ Mitglied hinzufügen]    │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `FamilyMembersPage.xaml`
- **ViewModel**: `FamilyMembersViewModel.cs`
- **Models**: `FamilyMember.cs`, `MemberRole.cs`

## API-Endpunkt

```
GET /api/families/{familyId}/members
Authorization: Bearer {parent-token}

Response 200:
{
  "familyId": "guid",
  "familyName": "Familie Müller",
  "familyCode": "ABC123",
  "members": [
    {
      "memberId": "guid",
      "userId": "guid",
      "name": "Max Müller",
      "role": "parent",
      "isAdmin": true,
      "avatarUrl": "string",
      "joinedAt": "2024-01-15T10:00:00Z",
      "status": "active"
    },
    {
      "memberId": "guid",
      "childId": "guid",
      "name": "Emma",
      "role": "child",
      "avatarUrl": "string",
      "dateOfBirth": "2016-03-15",
      "status": "active"
    }
  ],
  "pendingInvitations": [
    {
      "invitationId": "guid",
      "email": "opa@example.com",
      "role": "relative",
      "invitedAt": "2024-01-20T10:00:00Z"
    }
  ]
}
```

## Technische Notizen

- Mitglieder nach Rolle gruppieren und sortieren
- Avatar-Fallback mit Initialen
- Ausstehende Einladungen separat oder inline anzeigen
- Admin-Badge für Familien-Ersteller
- Pull-to-Refresh für Aktualisierung

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-02-01 | Familie mit allen Rollen | Korrekte Gruppierung |
| TC-M006-02-02 | Ausstehende Einladung | Als "Eingeladen" markiert |
| TC-M006-02-03 | Tap auf Mitglied | Navigation zum Profil |
| TC-M006-02-04 | Familien-Code kopieren | In Zwischenablage |
| TC-M006-02-05 | Leere Gruppe | Gruppe nicht anzeigen |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
