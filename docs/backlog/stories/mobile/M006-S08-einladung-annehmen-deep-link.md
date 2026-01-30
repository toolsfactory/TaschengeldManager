# Story M006-S08: Einladung annehmen (Deep Link)

## Epic
M006 - Familienverwaltung

## User Story

Als **eingeladener Verwandter** möchte ich **über einen Link in der Einladungs-E-Mail der Familie beitreten können**, damit **ich einfach und ohne manuellen Code-Eingabe Zugang zur Familiengruppe erhalte**.

## Akzeptanzkriterien

- [ ] Gegeben eine Einladungs-E-Mail, wenn ich auf den Link klicke, dann öffnet sich die App (oder der App Store)
- [ ] Gegeben die App ist installiert, wenn der Deep Link geöffnet wird, dann werde ich zum Registrierungs-/Login-Flow geleitet
- [ ] Gegeben ich bin eingeloggt, wenn der Deep Link verarbeitet wird, dann sehe ich die Einladungsdetails zur Bestätigung
- [ ] Gegeben die Einladungsbestätigung, wenn ich "Annehmen" tippe, dann werde ich der Familie hinzugefügt
- [ ] Gegeben ein abgelaufener Einladungslink, wenn ich ihn öffne, dann sehe ich eine entsprechende Fehlermeldung

## UI-Entwurf

```
Nach Klick auf Link (nicht eingeloggt):
┌─────────────────────────────┐
│     TaschengeldManager      │
├─────────────────────────────┤
│                             │
│  Du wurdest eingeladen!     │
│                             │
│  Familie Müller möchte dich │
│  als Verwandten hinzufügen. │
│                             │
│  Eingeladen von: Max Müller │
│                             │
│  Um fortzufahren, logge dich│
│  ein oder registriere dich. │
│                             │
│  ┌───────────────────────┐  │
│  │      Einloggen        │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │    Registrieren       │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘

Nach Login/Registrierung:
┌─────────────────────────────┐
│  × Einladung                │
├─────────────────────────────┤
│                             │
│       🏠                    │
│  Familie Müller             │
│                             │
│  Du wurdest von Max Müller  │
│  als Verwandter eingeladen. │
│                             │
│  Deine Berechtigungen:      │
│  ┌─────────────────────────┐│
│  │ ✓ Kontostände sehen    ││
│  │ ✓ Einzahlungen tätigen ││
│  │ ✗ Ausgaben erfassen    ││
│  └─────────────────────────┘│
│                             │
│  Für diese Kinder:          │
│  • Emma                     │
│  • Max Jr.                  │
│                             │
│  ┌───────────────────────┐  │
│  │      Ablehnen         │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │  ✓ Einladung annehmen │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `AcceptInvitationPage.xaml`
- **ViewModel**: `AcceptInvitationViewModel.cs`
- **Service**: `IDeepLinkService.cs`, `IInvitationService.cs`

## API-Endpunkte

```
GET /api/invitations/validate/{token}
Response 200:
{
  "invitationId": "guid",
  "familyName": "Familie Müller",
  "invitedBy": "Max Müller",
  "role": "relative",
  "permissions": {
    "canViewBalance": true,
    "canDeposit": true,
    "canRecordExpense": false
  },
  "children": [
    {"childId": "guid", "name": "Emma"},
    {"childId": "guid", "name": "Max Jr."}
  ],
  "expiresAt": "2024-01-27T10:00:00Z"
}

Response 400:
{
  "error": "expired",
  "message": "Diese Einladung ist abgelaufen"
}

POST /api/invitations/{invitationId}/accept
Authorization: Bearer {user-token}

Response 200:
{
  "message": "Du bist jetzt Mitglied der Familie",
  "familyId": "guid",
  "memberId": "guid"
}

POST /api/invitations/{invitationId}/decline
Authorization: Bearer {user-token}

Response 200:
{
  "message": "Einladung abgelehnt"
}
```

## Deep Link Schema

```
URL: https://app.taschengeld.de/invite/{token}
App Schema: taschengeldmanager://invite/{token}

Beispiel: taschengeldmanager://invite/ABC123XYZ789
```

## Technische Notizen

- Deep Link Handler in App.xaml.cs oder AppDelegate/MainActivity
- Token im Deep Link validieren bevor Daten angezeigt werden
- Einladungstoken nach Annahme invalidieren
- Bei nicht installierter App: Weiterleitung zum App Store
- Universal Links (iOS) / App Links (Android) konfigurieren

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-08-01 | Gültiger Link, nicht eingeloggt | Login-Aufforderung |
| TC-M006-08-02 | Gültiger Link, eingeloggt | Einladungsdetails |
| TC-M006-08-03 | Einladung annehmen | Familie beigetreten |
| TC-M006-08-04 | Einladung ablehnen | Zurück zum Start |
| TC-M006-08-05 | Abgelaufener Link | Fehlermeldung |
| TC-M006-08-06 | Bereits verwendeter Link | Fehlermeldung |

## Story Points

3

## Priorität

Mittel

## Status

⬜ Offen
