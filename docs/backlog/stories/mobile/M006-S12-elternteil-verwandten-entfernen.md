# Story M006-S12: Elternteil/Verwandten entfernen

## Epic
M006 - Familienverwaltung

## User Story

Als **Familien-Admin** möchte ich **ein Familienmitglied (Elternteil oder Verwandten) entfernen können**, damit **ich die Zugriffsrechte auf die Familie verwalten kann**.

## Akzeptanzkriterien

- [ ] Gegeben das Mitgliederprofil, wenn ich auf "Entfernen" tippe, dann werde ich um Bestätigung gebeten
- [ ] Gegeben die Bestätigung, wenn ich mein Passwort eingebe, dann wird das Mitglied entfernt
- [ ] Gegeben ein entferntes Mitglied, wenn es entfernt wurde, dann hat es keinen Zugriff mehr auf die Familie
- [ ] Gegeben der letzte Elternteil, wenn ich ihn entfernen will, dann wird dies verhindert
- [ ] Gegeben der Familien-Ersteller, wenn ein anderes Mitglied ihn entfernen will, dann ist dies nicht möglich

## UI-Entwurf

```
┌─────────────────────────────┐
│  ⚠️ Mitglied entfernen?     │
├─────────────────────────────┤
│                             │
│  Möchtest du Oma Helga      │
│  wirklich aus der Familie   │
│  entfernen?                 │
│                             │
│  ┌─────────────────────────┐│
│  │ 👤 Oma Helga            ││
│  │    Verwandter           ││
│  │    Mitglied seit:       ││
│  │    15.01.2024           ││
│  └─────────────────────────┘│
│                             │
│  Nach dem Entfernen:        │
│  • Kein Zugriff mehr auf    │
│    Kinderkonten             │
│  • Kann keine Einzahlungen  │
│    mehr tätigen             │
│  • Kann erneut eingeladen   │
│    werden                   │
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
│  │    Entfernen          │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `RemoveMemberDialog.xaml`
- **ViewModel**: `RemoveMemberViewModel.cs`
- **Service**: `IFamilyService.cs`

## API-Endpunkt

```
DELETE /api/families/{familyId}/members/{memberId}
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "parentPassword": "string"
}

Response 200:
{
  "message": "Mitglied wurde entfernt"
}

Response 400:
{
  "error": "last_parent",
  "message": "Der letzte Elternteil kann nicht entfernt werden"
}

Response 403:
{
  "error": "cannot_remove_creator",
  "message": "Der Familien-Ersteller kann nicht entfernt werden"
}

Response 401:
{
  "error": "invalid_password",
  "message": "Falsches Passwort"
}
```

## Technische Notizen

- Nur Elternteile können andere Mitglieder entfernen
- Familien-Ersteller kann nicht entfernt werden (nur Familie löschen)
- Mindestens ein Elternteil muss verbleiben
- Entfernte Mitglieder können später erneut eingeladen werden
- Session des entfernten Mitglieds wird invalidiert

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-12-01 | Verwandten entfernen | Erfolgreich entfernt |
| TC-M006-12-02 | Zweiten Elternteil entfernen | Erfolgreich entfernt |
| TC-M006-12-03 | Letzten Elternteil entfernen | Fehler "nicht möglich" |
| TC-M006-12-04 | Ersteller entfernen | Fehler "nicht möglich" |
| TC-M006-12-05 | Falsches Passwort | Fehler 401 |

## Story Points

1

## Priorität

Niedrig

## Status

⬜ Offen
