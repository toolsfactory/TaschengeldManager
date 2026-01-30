# Story M006-S05: Kind aus Familie entfernen

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **ein Kind aus der Familie entfernen können**, damit **ich die Familienstruktur verwalten kann, falls ein Kind versehentlich angelegt wurde**.

## Akzeptanzkriterien

- [ ] Gegeben das Kind-Profil, wenn ich auf "Entfernen" tippe, dann werde ich um Bestätigung gebeten
- [ ] Gegeben die Bestätigung, wenn ich meinen Passwort eingebe und bestätige, dann wird das Kind entfernt
- [ ] Gegeben ein entferntes Kind, wenn es entfernt wurde, dann werden alle zugehörigen Daten archiviert (nicht gelöscht)
- [ ] Gegeben ein Kind mit Kontostand > 0, wenn ich es entfernen will, dann erhalte ich eine Warnung
- [ ] Gegeben die erfolgreiche Entfernung, wenn sie abgeschlossen ist, dann kann sich das Kind nicht mehr einloggen

## UI-Entwurf

```
┌─────────────────────────────┐
│  ⚠️ Kind entfernen?         │
├─────────────────────────────┤
│                             │
│  Möchtest du Emma wirklich  │
│  aus der Familie entfernen? │
│                             │
│  ┌─────────────────────────┐│
│  │ ⚠️ Aktueller Kontostand:││
│  │    45,00 €              ││
│  │                         ││
│  │ Die Kontodaten werden   ││
│  │ archiviert, können aber ││
│  │ nicht mehr aktiv genutzt││
│  │ werden.                 ││
│  └─────────────────────────┘│
│                             │
│  Zur Bestätigung:           │
│  Dein Passwort eingeben     │
│  ┌───────────────────────┐  │
│  │ ••••••••              │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │      Abbrechen        │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │  🗑️ Endgültig entfernen│  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `RemoveChildDialog.xaml`
- **ViewModel**: `RemoveChildViewModel.cs`
- **Service**: `IChildService.cs`

## API-Endpunkt

```
DELETE /api/families/{familyId}/children/{childId}
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "parentPassword": "string",
  "confirmRemoval": true
}

Response 200:
{
  "message": "Kind wurde aus der Familie entfernt",
  "archivedAt": "2024-01-20T15:00:00Z"
}

Response 400:
{
  "error": "has_balance",
  "message": "Kind hat noch ein Guthaben von 45,00 €",
  "balance": 45.00
}

Response 401:
{
  "error": "invalid_password",
  "message": "Falsches Passwort"
}
```

## Technische Notizen

- Soft-Delete: Daten werden archiviert, nicht physisch gelöscht
- Bei Kontostand > 0: Warnung, aber Entfernung trotzdem möglich
- Kind kann nach Entfernung nicht mehr einloggen
- Archivierte Daten für evtl. spätere Wiederherstellung aufbewahren
- Audit-Log für Compliance

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-05-01 | Kind ohne Guthaben | Entfernung erfolgreich |
| TC-M006-05-02 | Kind mit Guthaben | Warnung, aber möglich |
| TC-M006-05-03 | Falsches Passwort | Fehler 401 |
| TC-M006-05-04 | Nach Entfernung | Kind kann nicht einloggen |
| TC-M006-05-05 | Abbrechen | Kind bleibt erhalten |

## Story Points

1

## Priorität

Niedrig

## Status

⬜ Offen
