# Story E005-S047: Benachrichtigung bei neuer Anfrage

## Epic
E005 - Anfragen-System

## User Story

Als **Elternteil** möchte ich **benachrichtigt werden, wenn mein Kind eine neue Geldanfrage stellt**, damit **ich zeitnah darauf reagieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine neue Anfrage, wenn sie erstellt wird, dann werden alle Eltern der Familie benachrichtigt
- [ ] Gegeben eine Benachrichtigung, wenn sie gesendet wird, dann enthält sie Kind, Betrag und Anfragetitel
- [ ] Gegeben eine Benachrichtigung, wenn sie empfangen wird, dann kann sie direkt zur Anfrage navigieren
- [ ] Gegeben Benachrichtigungseinstellungen, wenn sie konfiguriert sind, dann werden sie respektiert
- [ ] Gegeben eine dringende Anfrage, wenn sie gestellt wird, dann wird dies in der Benachrichtigung hervorgehoben

## Hinweis

Diese Story ist als Teil von **E007 - Benachrichtigungen** geplant und wird dort implementiert.

## Vorläufige Spezifikation

### Benachrichtigungs-Payload
```json
{
  "type": "NewMoneyRequest",
  "familyId": "guid",
  "requestId": "guid",
  "childId": "guid",
  "childName": "Max",
  "amount": 15.00,
  "title": "Neues Buch",
  "urgency": "Normal|Urgent",
  "createdAt": "datetime"
}
```

### Push-Nachricht
```
Titel: Neue Geldanfrage von Max
Body: Max bittet um 15,00 € für "Neues Buch"
Action: Zur Anfrage → /requests/{requestId}
```

## Technische Notizen

- Push-Notification Service integrieren (Firebase/APNs)
- Alle Eltern der Familie benachrichtigen
- Benachrichtigungs-Präferenzen pro User beachten
- Deep-Link zur Anfrage in der App
- Badge-Counter für unbearbeitete Anfragen

## Abhängigkeiten

- E007: Benachrichtigungssystem

## Story Points

3

## Priorität

Mittel (Teil von E007)
