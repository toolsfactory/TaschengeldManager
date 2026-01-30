# Story M006-S13: Familie löschen

## Epic
M006 - Familienverwaltung

## User Story

Als **Familien-Ersteller** möchte ich **die gesamte Familie löschen können**, damit **ich alle Daten entfernen kann, wenn die App nicht mehr genutzt wird**.

## Akzeptanzkriterien

- [ ] Gegeben die Familieneinstellungen, wenn ich auf "Familie löschen" tippe, dann sehe ich eine deutliche Warnung
- [ ] Gegeben die Löschbestätigung, wenn ich mein Passwort und "LÖSCHEN" eingebe, dann wird die Familie gelöscht
- [ ] Gegeben eine gelöschte Familie, wenn der Prozess abgeschlossen ist, dann werden alle zugehörigen Daten archiviert
- [ ] Gegeben eine gelöschte Familie, wenn die Löschung erfolgt ist, dann können sich keine Mitglieder mehr einloggen
- [ ] Gegeben mehrere Kinder mit Guthaben, wenn ich die Familie lösche, dann erhalte ich eine Zusammenfassung aller Kontostände

## UI-Entwurf

```
┌─────────────────────────────┐
│  ⛔ Familie löschen         │
├─────────────────────────────┤
│                             │
│  ⚠️ ACHTUNG!                │
│                             │
│  Du bist dabei, die gesamte │
│  Familie "Familie Müller"   │
│  zu löschen.                │
│                             │
│  Dies betrifft:             │
│  ┌─────────────────────────┐│
│  │ • 2 Elternteile         ││
│  │ • 1 Verwandter          ││
│  │ • 2 Kinderkonten        ││
│  │                         ││
│  │ Gesamt-Guthaben:        ││
│  │ 127,50 €                ││
│  └─────────────────────────┘│
│                             │
│  Diese Aktion kann NICHT    │
│  rückgängig gemacht werden! │
│                             │
│  Zur Bestätigung:           │
│                             │
│  1. Dein Passwort:          │
│  ┌───────────────────────┐  │
│  │ ••••••••              │  │
│  └───────────────────────┘  │
│                             │
│  2. Tippe "LÖSCHEN":        │
│  ┌───────────────────────┐  │
│  │                       │  │
│  └───────────────────────┘  │
│                             │
│  ┌───────────────────────┐  │
│  │      Abbrechen        │  │
│  └───────────────────────┘  │
│  ┌───────────────────────┐  │
│  │  🗑️ Familie löschen   │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `DeleteFamilyPage.xaml`
- **ViewModel**: `DeleteFamilyViewModel.cs`
- **Service**: `IFamilyService.cs`

## API-Endpunkt

```
DELETE /api/families/{familyId}
Authorization: Bearer {creator-token}
Content-Type: application/json

{
  "password": "string",
  "confirmationText": "LÖSCHEN"
}

Response 200:
{
  "message": "Familie wurde gelöscht",
  "deletedAt": "2024-01-20T15:00:00Z",
  "summary": {
    "parentsRemoved": 2,
    "relativesRemoved": 1,
    "childrenRemoved": 2,
    "totalBalanceArchived": 127.50
  }
}

Response 400:
{
  "error": "confirmation_mismatch",
  "message": "Bitte tippe 'LÖSCHEN' zur Bestätigung"
}

Response 403:
{
  "error": "not_creator",
  "message": "Nur der Familien-Ersteller kann die Familie löschen"
}
```

## Technische Notizen

- Nur der Familien-Ersteller kann die Familie löschen
- Daten werden archiviert (Soft-Delete), nicht physisch gelöscht
- Alle Sessions aller Familienmitglieder werden invalidiert
- E-Mail-Benachrichtigung an alle Mitglieder
- 30-Tage-Wiederherstellungsfrist (optional)
- DSGVO-konforme Datenaufbewahrung beachten

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-13-01 | Gültige Löschung | Familie wird gelöscht |
| TC-M006-13-02 | Falsches Passwort | Fehler 401 |
| TC-M006-13-03 | Falscher Bestätigungstext | Fehler "confirmation" |
| TC-M006-13-04 | Nicht-Ersteller löscht | Fehler 403 |
| TC-M006-13-05 | Nach Löschung einloggen | Nicht möglich |

## Story Points

2

## Priorität

Niedrig

## Status

⬜ Offen
