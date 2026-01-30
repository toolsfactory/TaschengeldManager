# Story E005-S041: Anfrage mit Begründung versehen

## Epic
E005 - Anfragen-System

## User Story

Als **Kind** möchte ich **meiner Geldanfrage eine Begründung hinzufügen können**, damit **meine Eltern verstehen, wofür ich das Geld benötige**.

## Akzeptanzkriterien

- [ ] Gegeben eine neue Anfrage, wenn sie erstellt wird, dann kann eine ausführliche Begründung eingegeben werden
- [ ] Gegeben eine Begründung, wenn sie eingegeben wird, dann ist sie auf maximal 1000 Zeichen begrenzt
- [ ] Gegeben eine bestehende Anfrage, wenn sie noch ausstehend ist, dann kann die Begründung nachträglich ergänzt werden
- [ ] Gegeben eine Begründung, wenn sie angezeigt wird, dann ist sie für die Eltern vollständig sichtbar
- [ ] Gegeben eine Anfrage ohne Begründung, wenn sie erstellt wird, dann ist sie trotzdem gültig

## API-Endpunkte

### Begründung bei Erstellung (in E005-S040 integriert)
```json
{
  "amount": 15.00,
  "title": "Neues Buch",
  "reason": "Ich möchte gerne das neue Harry Potter Buch kaufen. Meine Freunde haben es schon und ich möchte mitreden können."
}
```

### Begründung nachträglich hinzufügen/ändern
```
PATCH /api/me/requests/{requestId}/reason
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Ich möchte gerne das neue Harry Potter Buch kaufen. Meine Freunde haben es schon gelesen."
}

Response 200:
{
  "requestId": "guid",
  "reason": "Ich möchte gerne das neue Harry Potter Buch kaufen. Meine Freunde haben es schon gelesen.",
  "updatedAt": "datetime"
}

Response 400:
{
  "errors": {
    "reason": ["Begründung darf maximal 1000 Zeichen lang sein"]
  }
}

Response 403:
{
  "message": "Anfrage kann nicht mehr bearbeitet werden"
}
```

## Technische Notizen

- Begründung als optionales Textfeld (NVARCHAR(1000))
- Nur änderbar wenn Status = Pending
- UTF-8 für Sonderzeichen und Emojis
- Änderung an bestehender Anfrage triggert keine neue Benachrichtigung
- Begründung wird bei Genehmigung/Ablehnung in Transaktion übernommen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E005-041-01 | Anfrage mit Begründung | 201 + Begründung gespeichert |
| TC-E005-041-02 | Anfrage ohne Begründung | 201 (optional) |
| TC-E005-041-03 | Begründung zu lang | 400 Validierungsfehler |
| TC-E005-041-04 | Begründung nachträglich | 200 + aktualisiert |
| TC-E005-041-05 | Genehmigte Anfrage ändern | 403 Forbidden |

## Story Points

2

## Priorität

Mittel
