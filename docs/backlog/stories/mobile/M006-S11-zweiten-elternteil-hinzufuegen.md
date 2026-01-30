# Story M006-S11: Zweiten Elternteil hinzufügen

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **einen zweiten Elternteil zur Familie hinzufügen können**, damit **beide Elternteile die Kinderkonten verwalten können**.

## Akzeptanzkriterien

- [ ] Gegeben die Familienverwaltung, wenn ich auf "Elternteil einladen" tippe, dann öffnet sich das Einladungsformular
- [ ] Gegeben das Einladungsformular, wenn ich die E-Mail des Partners eingebe, dann wird eine spezielle Eltern-Einladung verschickt
- [ ] Gegeben die Eltern-Einladung, wenn sie angenommen wird, dann hat der neue Elternteil volle Verwaltungsrechte
- [ ] Gegeben ein zweiter Elternteil, wenn er hinzugefügt wird, dann kann er alle Familienfunktionen nutzen
- [ ] Gegeben zwei Elternteile, wenn beide angemeldet sind, dann sehen beide die gleichen Daten in Echtzeit

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Elternteil einladen│
├─────────────────────────────┤
│                             │
│  Partner zur Familie        │
│  einladen                   │
│                             │
│  Lade deinen Partner ein,   │
│  damit ihr gemeinsam das    │
│  Taschengeld verwalten könnt│
│                             │
│  E-Mail-Adresse:            │
│  ┌───────────────────────┐  │
│  │ partner@example.com   │  │
│  └───────────────────────┘  │
│                             │
│  Name:                      │
│  ┌───────────────────────┐  │
│  │ Anna Müller           │  │
│  └───────────────────────┘  │
│                             │
│  ℹ️ Dein Partner erhält     │
│  volle Verwaltungsrechte:   │
│                             │
│  ┌─────────────────────────┐│
│  │ ✓ Kinder hinzufügen    ││
│  │ ✓ Einzahlungen/Ausgaben││
│  │ ✓ Einstellungen ändern ││
│  │ ✓ Weitere einladen     ││
│  └─────────────────────────┘│
│                             │
│  Persönliche Nachricht:     │
│  ┌───────────────────────┐  │
│  │ Schatz, damit wir...  │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │   Einladung senden    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `InviteParentPage.xaml`
- **ViewModel**: `InviteParentViewModel.cs`
- **Service**: `IInvitationService.cs`

## API-Endpunkt

```
POST /api/families/{familyId}/invitations
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "email": "partner@example.com",
  "name": "Anna Müller",
  "role": "parent",
  "personalMessage": "Schatz, damit wir gemeinsam..."
}

Response 201:
{
  "invitationId": "guid",
  "email": "partner@example.com",
  "role": "parent",
  "status": "sent",
  "expiresAt": "2024-01-27T10:00:00Z"
}

Response 400:
{
  "error": "max_parents_reached",
  "message": "Maximale Anzahl an Elternteilen erreicht"
}
```

## Technische Notizen

- Elternrolle = volle Administratorrechte
- Maximal 2 Elternteile pro Familie (konfigurierbar)
- Eltern-spezifisches E-Mail-Template
- Nach Annahme: Echtzeit-Sync aller Daten (SignalR)
- Beide Eltern können alle Einstellungen ändern

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-11-01 | Gültige Einladung | E-Mail wird gesendet |
| TC-M006-11-02 | Bereits 2 Eltern | Fehler "max erreicht" |
| TC-M006-11-03 | Einladung angenommen | Volle Rechte |
| TC-M006-11-04 | Bereits eingeladen | Einladung aktualisieren |
| TC-M006-11-05 | Sync nach Beitritt | Daten sofort sichtbar |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
