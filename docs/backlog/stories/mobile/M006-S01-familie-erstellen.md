# Story M006-S01: Familie erstellen

## Epic
M006 - Familienverwaltung

## User Story

Als **neu registriertes Elternteil** möchte ich **eine Familie erstellen können**, damit **ich meine Kinder hinzufügen und die App nutzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein registriertes Elternteil ohne Familie, wenn es sich einloggt, dann wird es aufgefordert eine Familie zu erstellen
- [ ] Gegeben der Familien-Erstellungsdialog, wenn ich einen Familiennamen eingebe, dann wird die Familie erstellt
- [ ] Gegeben eine erstellte Familie, wenn sie angelegt wird, dann wird automatisch ein eindeutiger Familien-Code generiert
- [ ] Gegeben eine erfolgreiche Familienerstellung, wenn der Prozess abgeschlossen ist, dann bin ich automatisch als Elternteil zugeordnet
- [ ] Gegeben eine Familie, wenn sie erstellt wird, dann kann ich optional einen Avatar oder ein Bild hochladen

## UI-Entwurf

```
┌─────────────────────────────┐
│     TaschengeldManager      │
├─────────────────────────────┤
│                             │
│  Willkommen! 🎉             │
│                             │
│  Um loszulegen, erstelle    │
│  deine Familie.             │
│                             │
│        ┌─────────┐          │
│        │  📷     │          │
│        │ +Bild   │          │
│        └─────────┘          │
│                             │
│  Familienname:              │
│  ┌───────────────────────┐  │
│  │ z.B. Familie Müller   │  │
│  └───────────────────────┘  │
│                             │
│  ℹ️ Du kannst später        │
│  weitere Familienmitglieder │
│  einladen.                  │
│                             │
│  ┌───────────────────────┐  │
│  │   Familie erstellen   │  │
│  └───────────────────────┘  │
│                             │
│  ─────── oder ───────       │
│                             │
│  [Einer Familie beitreten]  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `CreateFamilyPage.xaml`
- **ViewModel**: `CreateFamilyViewModel.cs`
- **Service**: `IFamilyService.cs`

## API-Endpunkt

```
POST /api/families
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "name": "Familie Müller",
  "avatarBase64": "optional-base64-string"
}

Response 201:
{
  "familyId": "guid",
  "name": "Familie Müller",
  "familyCode": "ABC123",
  "createdAt": "2024-01-15T10:00:00Z",
  "members": [
    {
      "userId": "guid",
      "name": "Max Müller",
      "role": "parent",
      "isCreator": true
    }
  ]
}

Response 400:
{
  "errors": {
    "name": ["Familienname ist erforderlich"]
  }
}
```

## Technische Notizen

- Familien-Code: 6 alphanumerische Zeichen, eindeutig
- Familien-Code sollte leicht lesbar sein (keine verwechselbaren Zeichen wie 0/O, 1/l)
- Ersteller wird automatisch als Parent mit Admin-Rechten zugeordnet
- Avatar-Upload optional, komprimiert speichern

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-01-01 | Gültiger Familienname | Familie wird erstellt |
| TC-M006-01-02 | Leerer Familienname | Validierungsfehler |
| TC-M006-01-03 | Mit Avatar | Avatar wird gespeichert |
| TC-M006-01-04 | Familien-Code | Eindeutiger Code generiert |
| TC-M006-01-05 | Benutzer bereits in Familie | Fehler oder Hinweis |

## Story Points

2

## Priorität

Hoch

## Status

⬜ Offen
