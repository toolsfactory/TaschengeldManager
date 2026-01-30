# Story M006-S04: Kind-PIN ändern

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **die PIN meines Kindes ändern können**, damit **ich die Sicherheit gewährleisten oder eine vergessene PIN zurücksetzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben das Kind-Profil, wenn ich auf "PIN ändern" tippe, dann öffnet sich der Änderungsdialog
- [ ] Gegeben der Änderungsdialog, wenn ich mein Eltern-Passwort eingebe, dann wird meine Berechtigung bestätigt
- [ ] Gegeben die Berechtigung, wenn ich eine neue 4-stellige PIN eingebe und bestätige, dann wird die PIN aktualisiert
- [ ] Gegeben die PIN-Änderung, wenn sie erfolgreich war, dann erhält das Kind optional eine Benachrichtigung
- [ ] Gegeben eine ungültige PIN (nicht 4-stellig), wenn ich speichern will, dann erscheint ein Validierungsfehler

## UI-Entwurf

```
┌─────────────────────────────┐
│  × PIN ändern               │
├─────────────────────────────┤
│                             │
│  PIN ändern für Emma        │
│                             │
│  Zur Sicherheit:            │
│  Dein Passwort eingeben     │
│  ┌───────────────────────┐  │
│  │ ••••••••              │  │
│  └───────────────────────┘  │
│                             │
│  Neue PIN (4-stellig):      │
│  ┌───┐ ┌───┐ ┌───┐ ┌───┐   │
│  │ _ │ │ _ │ │ _ │ │ _ │   │
│  └───┘ └───┘ └───┘ └───┘   │
│                             │
│  Neue PIN bestätigen:       │
│  ┌───┐ ┌───┐ ┌───┐ ┌───┐   │
│  │ _ │ │ _ │ │ _ │ │ _ │   │
│  └───┘ └───┘ └───┘ └───┘   │
│                             │
│  [ ] Kind über neue PIN     │
│      informieren            │
│                             │
│  ┌───────────────────────┐  │
│  │     PIN speichern     │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `ChangeChildPinPage.xaml` (als Modal)
- **ViewModel**: `ChangeChildPinViewModel.cs`
- **Service**: `IChildService.cs`

## API-Endpunkt

```
PUT /api/children/{childId}/pin
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "parentPassword": "string",
  "newPin": "5678",
  "notifyChild": true
}

Response 200:
{
  "message": "PIN erfolgreich geändert",
  "childNotified": true
}

Response 400:
{
  "errors": {
    "newPin": ["PIN muss genau 4 Ziffern haben"]
  }
}

Response 401:
{
  "error": "invalid_password",
  "message": "Falsches Passwort"
}
```

## Technische Notizen

- Eltern-Passwort als Sicherheitsbestätigung erforderlich
- Neue PIN wird gehashed gespeichert
- Optional: In-App-Nachricht an Kind senden
- PIN-Eingabe mit Numpad-Tastatur
- Nach 3 falschen Passwort-Versuchen: Rate-Limiting

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-04-01 | Gültige Änderung | PIN wird aktualisiert |
| TC-M006-04-02 | Falsches Eltern-Passwort | Fehler 401 |
| TC-M006-04-03 | PIN nur 3 Ziffern | Validierungsfehler |
| TC-M006-04-04 | PINs stimmen nicht | Validierungsfehler |
| TC-M006-04-05 | Mit Benachrichtigung | Kind wird informiert |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
