# Story M006-S03: Kind zur Familie hinzufügen

## Epic
M006 - Familienverwaltung

## User Story

Als **Elternteil** möchte ich **ein Kind zu meiner Familie hinzufügen können**, damit **es ein eigenes Taschengeldkonto bekommt und die App nutzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben die Familienverwaltung, wenn ich auf "Kind hinzufügen" tippe, dann öffnet sich das Formular
- [ ] Gegeben das Formular, wenn ich Vorname, Spitzname und Geburtsdatum eingebe, dann kann ich das Kind anlegen
- [ ] Gegeben das Formular, wenn ich eine PIN für das Kind festlege, dann wird diese sicher gespeichert
- [ ] Gegeben ein erfolgreich angelegtes Kind, wenn der Prozess abgeschlossen ist, dann wird automatisch ein Taschengeldkonto erstellt
- [ ] Gegeben ein angelegtes Kind, wenn ich einen Avatar auswähle, dann wird dieser dem Profil zugeordnet
- [ ] Gegeben ein doppelter Spitzname, wenn ich das Kind anlege, dann erscheint ein Validierungsfehler

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück   Kind hinzufügen │
├─────────────────────────────┤
│                             │
│        ┌─────────┐          │
│        │  👧     │          │
│        │ Avatar  │          │
│        └─────────┘          │
│  [Mädchen] [Junge] [Neutral]│
│                             │
│  Vorname:                   │
│  ┌───────────────────────┐  │
│  │ Emma                  │  │
│  └───────────────────────┘  │
│                             │
│  Spitzname (für Login):     │
│  ┌───────────────────────┐  │
│  │ emma                  │  │
│  └───────────────────────┘  │
│                             │
│  Geburtsdatum:              │
│  ┌───────────────────────┐  │
│  │ 15.03.2016        📅  │  │
│  └───────────────────────┘  │
│                             │
│  PIN für Kind festlegen:    │
│  (4-stellig für App-Login)  │
│  ┌───┐ ┌───┐ ┌───┐ ┌───┐   │
│  │ 1 │ │ 2 │ │ 3 │ │ 4 │   │
│  └───┘ └───┘ └───┘ └───┘   │
│                             │
│  PIN bestätigen:            │
│  ┌───┐ ┌───┐ ┌───┐ ┌───┐   │
│  │ 1 │ │ 2 │ │ 3 │ │ 4 │   │
│  └───┘ └───┘ └───┘ └───┘   │
│                             │
│  ┌───────────────────────┐  │
│  │    Kind hinzufügen    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `AddChildPage.xaml`
- **ViewModel**: `AddChildViewModel.cs`
- **Service**: `IFamilyService.cs`, `IChildService.cs`

## API-Endpunkt

```
POST /api/families/{familyId}/children
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "firstName": "Emma",
  "nickname": "emma",
  "dateOfBirth": "2016-03-15",
  "pin": "1234",
  "avatarType": "girl"
}

Response 201:
{
  "childId": "guid",
  "firstName": "Emma",
  "nickname": "emma",
  "dateOfBirth": "2016-03-15",
  "avatarUrl": "string",
  "account": {
    "accountId": "guid",
    "balance": 0.00
  },
  "loginInfo": {
    "familyCode": "ABC123",
    "nickname": "emma"
  }
}

Response 400:
{
  "errors": {
    "nickname": ["Dieser Spitzname existiert bereits in deiner Familie"]
  }
}
```

## Technische Notizen

- PIN wird gehashed gespeichert (nicht im Klartext)
- Spitzname muss innerhalb der Familie eindeutig sein
- Spitzname nur Kleinbuchstaben und Zahlen erlauben
- Automatische Kontoerstellung als Teil der Transaktion
- Avatar-Auswahl mit vordefinierten kindgerechten Bildern

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M006-03-01 | Valide Daten | Kind wird erstellt |
| TC-M006-03-02 | Doppelter Spitzname | Validierungsfehler |
| TC-M006-03-03 | PIN zu kurz | Validierungsfehler |
| TC-M006-03-04 | PINs stimmen nicht | Validierungsfehler |
| TC-M006-03-05 | Kind erstellt | Konto mit 0€ angelegt |

## Story Points

3

## Priorität

Hoch

## Status

⬜ Offen
