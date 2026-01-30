# Story M007-S08: Anfrage bearbeiten

## Epic
M007 - Geldanfragen

## User Story

Als **Kind** möchte ich **eine noch nicht beantwortete Anfrage bearbeiten können**, damit **ich den Betrag oder den Grund korrigieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine ausstehende Anfrage, wenn ich darauf tippe, dann sehe ich die Option "Bearbeiten"
- [ ] Gegeben den Bearbeitungsmodus, wenn ich den Betrag ändere, dann wird er nach Speichern aktualisiert
- [ ] Gegeben den Bearbeitungsmodus, wenn ich den Grund ändere, dann wird er nach Speichern aktualisiert
- [ ] Gegeben eine bearbeitete Anfrage, wenn sie gespeichert wurde, dann sehen die Eltern die aktualisierten Daten
- [ ] Gegeben eine bereits beantwortete Anfrage, wenn ich sie ansehe, dann ist Bearbeiten nicht möglich

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück  Anfrage bearbeiten│
├─────────────────────────────┤
│                             │
│  Betrag ändern:             │
│  ┌───────────────────────┐  │
│  │         €             │  │
│  │       15,00           │  │
│  └───────────────────────┘  │
│  (Vorher: 20,00 €)          │
│                             │
│  Wofür brauchst du das      │
│  Geld?                      │
│  ┌───────────────────────┐  │
│  │ Lego-Set aus dem      │  │
│  │ Spielzeugladen        │  │
│  │                       │  │
│  └───────────────────────┘  │
│                             │
│  Bild:                      │
│  ┌─────────┐ ┌─────────┐    │
│  │  📷     │ │  🗑️     │    │
│  │ Aktuell │ │Entfernen│    │
│  └─────────┘ └─────────┘    │
│                             │
│  ┌───────────────────────┐  │
│  │  Änderungen speichern │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `EditRequestPage.xaml`
- **ViewModel**: `EditRequestViewModel.cs`
- **Service**: `IRequestService.cs`

## API-Endpunkt

```
PUT /api/requests/{requestId}
Authorization: Bearer {child-token}
Content-Type: application/json

{
  "amount": 15.00,
  "reason": "Lego-Set aus dem Spielzeugladen",
  "imageBase64": "optional-new-image",
  "removeImage": false
}

Response 200:
{
  "message": "Anfrage aktualisiert",
  "requestId": "guid",
  "updatedAt": "2024-01-20T16:00:00Z",
  "parentsNotified": true
}

Response 400:
{
  "error": "already_responded",
  "message": "Diese Anfrage wurde bereits beantwortet"
}
```

## Technische Notizen

- Nur ausstehende Anfragen können bearbeitet werden
- Änderungen werden in History geloggt
- Eltern erhalten Benachrichtigung über Änderung
- Bild kann ersetzt oder entfernt werden
- Dringlichkeit kann auch geändert werden

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M007-08-01 | Betrag ändern | Wird gespeichert |
| TC-M007-08-02 | Grund ändern | Wird gespeichert |
| TC-M007-08-03 | Bild entfernen | Wird entfernt |
| TC-M007-08-04 | Beantwortete Anfrage | Bearbeiten nicht möglich |
| TC-M007-08-05 | Eltern-Benachrichtigung | Push über Änderung |

## Story Points

1

## Priorität

Niedrig

## Status

⬜ Offen
