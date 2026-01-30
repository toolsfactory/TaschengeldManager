# Story M006-S10: Kind-Profil bearbeiten

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **das Profil meines Kindes bearbeiten können**, damit **ich Informationen wie Name, Geburtsdatum oder Avatar aktualisieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Familienmitgliederliste, wenn ich auf ein Kind tippe, dann sehe ich dessen Profil
- [ ] Gegeben das Kind-Profil, wenn ich auf "Bearbeiten" tippe, dann kann ich die Daten ändern
- [ ] Gegeben der Bearbeitungsmodus, wenn ich den Vornamen ändere, dann wird er nach Speichern aktualisiert
- [ ] Gegeben der Bearbeitungsmodus, wenn ich das Geburtsdatum ändere, dann wird das Alter neu berechnet
- [ ] Gegeben der Bearbeitungsmodus, wenn ich einen neuen Avatar auswähle, dann wird dieser gespeichert
- [ ] Gegeben der Spitzname, wenn ich ihn ändern will, dann erhalte ich einen Hinweis dass sich auch der Login ändert

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Emma bearbeiten│
├─────────────────────────────┤
│                             │
│        ┌─────────┐          │
│        │  👧     │          │
│        │ Ändern  │          │
│        └─────────┘          │
│                             │
│  Vorname:                   │
│  ┌───────────────────────┐  │
│  │ Emma                  │  │
│  └───────────────────────┘  │
│                             │
│  Spitzname (Login):         │
│  ┌───────────────────────┐  │
│  │ emma                  │  │
│  └───────────────────────┘  │
│  ⚠️ Ändert den Login-Namen  │
│                             │
│  Geburtsdatum:              │
│  ┌───────────────────────┐  │
│  │ 15.03.2016        📅  │  │
│  └───────────────────────┘  │
│                             │
│  Berechnet: 8 Jahre alt     │
│                             │
│  ─────────────────────────  │
│                             │
│  Weitere Optionen:          │
│  [🔑 PIN ändern]            │
│  [🗑️ Kind entfernen]        │
│                             │
│  ┌───────────────────────┐  │
│  │    Änderungen sichern │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `EditChildProfilePage.xaml`
- **ViewModel**: `EditChildProfileViewModel.cs`
- **Service**: `IChildService.cs`

## API-Endpunkte

```
GET /api/children/{childId}
Authorization: Bearer {parent-token}

Response 200:
{
  "childId": "guid",
  "firstName": "Emma",
  "nickname": "emma",
  "dateOfBirth": "2016-03-15",
  "avatarUrl": "string",
  "age": 8,
  "createdAt": "2024-01-15T10:00:00Z"
}

PUT /api/children/{childId}
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "firstName": "Emma Marie",
  "nickname": "emma",
  "dateOfBirth": "2016-03-15",
  "avatarBase64": "optional-new-avatar"
}

Response 200:
{
  "childId": "guid",
  "firstName": "Emma Marie",
  "nickname": "emma",
  "dateOfBirth": "2016-03-15",
  "avatarUrl": "string",
  "message": "Profil aktualisiert"
}

Response 400:
{
  "errors": {
    "nickname": ["Dieser Spitzname ist bereits vergeben"]
  }
}
```

## Technische Notizen

- Spitzname-Änderung: Deutliche Warnung dass sich Login ändert
- Alter automatisch aus Geburtsdatum berechnen
- Avatar-Auswahl: Vordefinierte Avatare + Custom Upload
- Optimistic UI Update für schnelle Reaktion
- Änderungen erst nach "Speichern" persistieren

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-10-01 | Vorname ändern | Wird gespeichert |
| TC-M006-10-02 | Spitzname ändern | Warnung + Speichern |
| TC-M006-10-03 | Spitzname bereits vergeben | Validierungsfehler |
| TC-M006-10-04 | Geburtsdatum ändern | Alter neu berechnet |
| TC-M006-10-05 | Avatar ändern | Neues Bild gespeichert |

## Story Points

2

## Priorität

Mittel

## Status

⬜ Offen
