# Story E003-S025: Transaktion mit Notiz versehen

## Epic
E003 - Transaktionen

## User Story

Als **Benutzer** möchte ich **Transaktionen mit einer Notiz versehen können**, damit **ich zusätzliche Details oder Erinnerungen hinzufügen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine neue Transaktion, wenn sie erstellt wird, dann kann eine optionale Notiz hinzugefügt werden
- [ ] Gegeben eine existierende Transaktion, wenn sie bearbeitet wird, dann kann eine Notiz hinzugefügt oder geändert werden
- [ ] Gegeben eine Notiz, wenn sie eingegeben wird, dann ist sie auf maximal 500 Zeichen begrenzt
- [ ] Gegeben eine Transaktion mit Notiz, wenn sie angezeigt wird, dann ist die Notiz sichtbar
- [ ] Gegeben ein Kind, wenn es eine Transaktion erstellt, dann kann nur es selbst oder die Eltern die Notiz lesen

## API-Endpunkt

### Notiz zu Transaktion hinzufügen/ändern
```
PATCH /api/transactions/{transactionId}/note
Authorization: Bearer {token}
Content-Type: application/json

{
  "note": "Gekauft im Spielzeugladen am Marktplatz"
}

Response 200:
{
  "transactionId": "guid",
  "note": "Gekauft im Spielzeugladen am Marktplatz",
  "updatedAt": "datetime"
}

Response 400:
{
  "errors": {
    "note": ["Notiz darf maximal 500 Zeichen lang sein"]
  }
}

Response 403:
{
  "message": "Keine Berechtigung für diese Transaktion"
}
```

### Notiz löschen
```
DELETE /api/transactions/{transactionId}/note
Authorization: Bearer {token}

Response 204: No Content
```

## Technische Notizen

- Notiz als optionales Textfeld in Transaktionstabelle
- Maximale Länge: 500 Zeichen
- UTF-8 Encoding für Sonderzeichen/Emojis
- Zugriffsrechte: Nur Ersteller oder Eltern können bearbeiten
- Notiz wird bei Stornierung beibehalten

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E003-025-01 | Notiz hinzufügen | 200 + Notiz gespeichert |
| TC-E003-025-02 | Notiz ändern | 200 + aktualisierte Notiz |
| TC-E003-025-03 | Notiz zu lang | 400 Validierungsfehler |
| TC-E003-025-04 | Notiz löschen | 204 + Notiz entfernt |
| TC-E003-025-05 | Fremde Transaktion | 403 Forbidden |

## Story Points

2

## Priorität

Niedrig
