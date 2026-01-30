# Story E005-S048: Benachrichtigung bei Entscheidung

## Epic
E005 - Anfragen-System

## User Story

Als **Kind** möchte ich **benachrichtigt werden, wenn meine Eltern über meine Geldanfrage entschieden haben**, damit **ich das Ergebnis sofort erfahre**.

## Akzeptanzkriterien

- [ ] Gegeben eine genehmigte Anfrage, wenn die Entscheidung erfolgt, dann wird das Kind benachrichtigt
- [ ] Gegeben eine abgelehnte Anfrage, wenn die Entscheidung erfolgt, dann wird das Kind benachrichtigt
- [ ] Gegeben eine Genehmigung, wenn sie benachrichtigt wird, dann enthält sie den gutgeschriebenen Betrag
- [ ] Gegeben eine Ablehnung, wenn sie benachrichtigt wird, dann enthält sie einen Hinweis auf den Grund
- [ ] Gegeben eine Benachrichtigung, wenn sie empfangen wird, dann kann sie direkt zum Konto/zur Anfrage navigieren

## Hinweis

Diese Story ist als Teil von **E007 - Benachrichtigungen** geplant und wird dort implementiert.

## Vorläufige Spezifikation

### Benachrichtigungs-Payload (Genehmigung)
```json
{
  "type": "RequestApproved",
  "requestId": "guid",
  "title": "Neues Buch",
  "requestedAmount": 15.00,
  "approvedAmount": 15.00,
  "newBalance": 40.50,
  "approvedBy": "Mama",
  "approvedAt": "datetime"
}
```

### Benachrichtigungs-Payload (Ablehnung)
```json
{
  "type": "RequestDeclined",
  "requestId": "guid",
  "title": "Neues Buch",
  "requestedAmount": 50.00,
  "declinedBy": "Papa",
  "declinedAt": "datetime",
  "hasReason": true
}
```

### Push-Nachrichten
```
Genehmigung:
Titel: Anfrage genehmigt! 🎉
Body: Deine Anfrage "Neues Buch" wurde genehmigt. 15,00 € wurden gutgeschrieben.
Action: Zum Konto → /account

Ablehnung:
Titel: Anfrage nicht genehmigt
Body: Deine Anfrage "Neues Buch" wurde nicht genehmigt. Tippe für Details.
Action: Zur Anfrage → /requests/{requestId}
```

## Technische Notizen

- Push-Notification an Kind senden
- Unterschiedliche Nachrichten für Genehmigung/Ablehnung
- Bei Genehmigung: Positiver Ton, neuen Kontostand zeigen
- Bei Ablehnung: Neutraler Ton, zur Begründung navigieren
- Badge-Counter aktualisieren

## Abhängigkeiten

- E007: Benachrichtigungssystem

## Story Points

3

## Priorität

Mittel (Teil von E007)
