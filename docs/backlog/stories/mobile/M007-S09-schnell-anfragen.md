# Story M007-S09: Schnell-Anfragen

## Epic
M007 - Geldanfragen

## User Story

Als **Kind** möchte ich **häufig verwendete Anfragen schnell wiederholen können**, damit **ich nicht jedes Mal alles neu eingeben muss**.

## Akzeptanzkriterien

- [ ] Gegeben das Anfrage-Formular, wenn ich es öffne, dann sehe ich Schnellauswahl-Buttons für häufige Anfragen
- [ ] Gegeben eine Schnell-Anfrage, wenn ich darauf tippe, dann werden Betrag und Grund automatisch eingetragen
- [ ] Gegeben eine genehmigte Anfrage, wenn ich sie als Vorlage speichern will, dann kann ich das tun
- [ ] Gegeben gespeicherte Vorlagen, wenn ich eine auswähle, dann wird sie sofort in das Formular geladen
- [ ] Gegeben die Vorlagen, wenn ich sie verwalten will, dann kann ich sie bearbeiten oder löschen

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück    Geld anfragen  │
├─────────────────────────────┤
│                             │
│  Schnell-Anfragen:          │
│  ┌─────────────────────────┐│
│  │ 🍦 Eis      │ 🎮 Games  ││
│  │   3,00 €   │   10,00 € ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 📚 Bücher  │ ⭐ Eigene  ││
│  │   15,00 €  │  Vorlage  ││
│  └─────────────────────────┘│
│                             │
│  ───── oder selbst ─────    │
│                             │
│  Betrag:                    │
│  ┌───────────────────────┐  │
│  │       0,00 €          │  │
│  └───────────────────────┘  │
│                             │
│  ... (Rest des Formulars)   │
│                             │
└─────────────────────────────┘

Vorlagen verwalten:
┌─────────────────────────────┐
│  ← Zurück    Meine Vorlagen │
├─────────────────────────────┤
│                             │
│  ┌─────────────────────────┐│
│  │ 🍦 Eis          3,00 € ││
│  │ "Eis im Schwimmbad"    ││
│  │ [Bearbeiten] [Löschen] ││
│  └─────────────────────────┘│
│  ┌─────────────────────────┐│
│  │ 🎮 Games       10,00 € ││
│  │ "Neues Handyspiel"     ││
│  │ [Bearbeiten] [Löschen] ││
│  └─────────────────────────┘│
│                             │
│  [+ Neue Vorlage]           │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `CreateRequestPage.xaml` (erweitert), `ManageTemplatesPage.xaml`
- **ViewModel**: `CreateRequestViewModel.cs`, `ManageTemplatesViewModel.cs`
- **Model**: `RequestTemplate.cs`

## API-Endpunkte

```
GET /api/children/{childId}/request-templates
Authorization: Bearer {child-token}

Response 200:
{
  "templates": [
    {
      "templateId": "guid",
      "name": "Eis",
      "icon": "ice_cream",
      "amount": 3.00,
      "reason": "Eis im Schwimmbad",
      "isDefault": false
    }
  ],
  "defaultTemplates": [
    {
      "templateId": "default-1",
      "name": "Süßigkeiten",
      "icon": "candy",
      "amount": 2.00,
      "reason": "Süßigkeiten kaufen",
      "isDefault": true
    }
  ]
}

POST /api/children/{childId}/request-templates
Authorization: Bearer {child-token}
Content-Type: application/json

{
  "name": "Kino",
  "icon": "movie",
  "amount": 12.00,
  "reason": "Kino mit Freunden"
}

Response 201:
{
  "templateId": "guid",
  "message": "Vorlage erstellt"
}

DELETE /api/children/{childId}/request-templates/{templateId}
Authorization: Bearer {child-token}

Response 200:
{
  "message": "Vorlage gelöscht"
}
```

## Vordefinierte Vorlagen (Systemstandard)

| Icon | Name | Standardbetrag |
|------|------|----------------|
| 🍦 | Eis | 3,00 € |
| 🍬 | Süßigkeiten | 2,00 € |
| 📚 | Bücher | 15,00 € |
| 🎮 | Games | 10,00 € |
| 🎬 | Kino | 12,00 € |

## Technische Notizen

- Systemvorlagen sind nicht löschbar
- Eigene Vorlagen lokal + Server speichern
- Maximal 10 eigene Vorlagen
- Vorlagen mit Icons für kindgerechte Darstellung
- Tap auf Vorlage füllt Formular aus, kann dann angepasst werden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-09-01 | Schnell-Anfrage tippen | Formular wird gefüllt |
| TC-M007-09-02 | Vorlage erstellen | Wird gespeichert |
| TC-M007-09-03 | Vorlage löschen | Wird entfernt |
| TC-M007-09-04 | Systemvorlage löschen | Nicht möglich |
| TC-M007-09-05 | Max. Vorlagen erreicht | Hinweis erscheint |

## Story Points

2

## Priorität

Niedrig

## Status

⬜ Offen
