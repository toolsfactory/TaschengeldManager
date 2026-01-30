# Story E003-S024: Transaktion mit Kategorie versehen

## Epic
E003 - Transaktionen

## User Story

Als **Benutzer** möchte ich **Transaktionen mit Kategorien versehen können**, damit **ich meine Ausgaben analysieren und gruppieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine neue Transaktion, wenn sie erstellt wird, dann kann eine Kategorie ausgewählt werden
- [ ] Gegeben ein System, wenn Kategorien abgefragt werden, dann gibt es vordefinierte Standardkategorien
- [ ] Gegeben ein Elternteil, wenn es möchte, dann kann es eigene Kategorien für die Familie erstellen
- [ ] Gegeben eine existierende Transaktion, wenn sie bearbeitet wird, dann kann die Kategorie geändert werden
- [ ] Gegeben Transaktionen mit Kategorien, wenn eine Auswertung erstellt wird, dann werden Ausgaben pro Kategorie gruppiert

## API-Endpunkte

### Kategorien abrufen
```
GET /api/families/{familyId}/categories
Authorization: Bearer {token}

Response 200:
{
  "categories": [
    {
      "categoryId": "guid",
      "name": "Süßigkeiten",
      "icon": "candy",
      "color": "#FF6B6B",
      "isSystem": true,
      "isActive": true
    },
    {
      "categoryId": "guid",
      "name": "Spielzeug",
      "icon": "toy",
      "color": "#4ECDC4",
      "isSystem": true,
      "isActive": true
    }
  ]
}
```

### Kategorie erstellen
```
POST /api/families/{familyId}/categories
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Bücher",
  "icon": "book",
  "color": "#45B7D1"
}

Response 201:
{
  "categoryId": "guid",
  "name": "Bücher",
  "icon": "book",
  "color": "#45B7D1",
  "isSystem": false
}
```

### Kategorie einer Transaktion ändern
```
PATCH /api/transactions/{transactionId}/category
Authorization: Bearer {token}
Content-Type: application/json

{
  "categoryId": "guid"
}

Response 200:
{
  "transactionId": "guid",
  "categoryId": "guid",
  "categoryName": "Bücher"
}
```

## Standardkategorien (System)

- Taschengeld (Einnahme)
- Süßigkeiten
- Spielzeug
- Kleidung
- Bücher/Zeitschriften
- Spiele/Apps
- Sparen
- Geschenke
- Sonstiges

## Technische Notizen

- Systemkategorien bei Familienerstellung kopieren
- Benutzerdefinierte Kategorien pro Familie
- Kategorien können deaktiviert aber nicht gelöscht werden
- Icon-Set für Frontend bereitstellen
- Kategorie "Sonstiges" als Fallback

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-024-01 | Kategorien abrufen | System + benutzerdefinierte |
| TC-E003-024-02 | Neue Kategorie erstellen | 201 mit Kategorie |
| TC-E003-024-03 | Doppelter Name | 400 Validierungsfehler |
| TC-E003-024-04 | Kategorie ändern | 200 + aktualisiert |
| TC-E003-024-05 | Systemkategorie löschen | 403 Forbidden |

## Story Points

5

## Priorität

Mittel
