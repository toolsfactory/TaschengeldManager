# E001-S073: Verdächtige Login-Versuche melden

## Status: Phase 2

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **automatisch benachrichtigt werden, wenn verdächtige Login-Versuche auf meinem Account stattfinden**, damit **ich bei potenziellem Missbrauch sofort reagieren kann**.

## Akzeptanzkriterien

- [ ] Automatische Erkennung verdächtiger Login-Aktivitäten
- [ ] Push-Benachrichtigung bei verdächtigem Login
- [ ] E-Mail-Warnung mit Details zum Vorfall
- [ ] Direkter Link zum Beenden aller Sessions
- [ ] Verdächtige Kriterien: Neuer Standort, neues Gerät, ungewöhnliche Zeit, viele Fehlversuche
- [ ] Benutzer kann Benachrichtigungseinstellungen anpassen
- [ ] Eltern werden bei verdächtigen Aktivitäten auf Kind-Accounts benachrichtigt

## API-Endpunkte

```
POST /api/auth/suspicious-activity/report

(Intern vom System aufgerufen)

Request:
{
  "userId": "guid",
  "activityType": "new_location",
  "details": {
    "ipAddress": "203.0.113.50",
    "location": "Moskau, Russland",
    "previousLocation": "Frankfurt, Deutschland"
  }
}

---

GET /api/auth/suspicious-activity

Response 200:
{
  "recentAlerts": [
    {
      "id": "guid",
      "timestamp": "2024-01-19T22:30:00Z",
      "type": "new_location",
      "description": "Login von neuem Standort: Moskau, Russland",
      "actionTaken": "user_notified",
      "resolved": false
    }
  ]
}

---

PATCH /api/users/me/notification-settings

Request:
{
  "suspiciousLoginEmail": true,
  "suspiciousLoginPush": true,
  "loginFromNewDevice": true,
  "failedLoginAttempts": true,
  "failedLoginThreshold": 3
}

Response 200:
{
  "message": "Benachrichtigungseinstellungen aktualisiert"
}

---

POST /api/auth/suspicious-activity/{id}/resolve

Request:
{
  "wasMe": true,
  "action": "trusted_location"  // oder "changed_password", "none"
}

Response 200:
{
  "message": "Aktivität als legitim markiert"
}
```

## Technische Hinweise

- Verdächtigkeitskriterien:
  - Neuer Standort: > 500km Entfernung zum letzten Login
  - Neues Gerät: User-Agent nie zuvor gesehen
  - Impossible Travel: Login aus anderem Land innerhalb 2 Stunden
  - Brute Force: > 5 Fehlversuche in 10 Minuten
  - Ungewöhnliche Zeit: Login zwischen 2-5 Uhr (benutzerspezifisch)
- ML-Modell für Anomalie-Erkennung (Phase 2+)
- E-Mail-Template mit "Das war ich" / "Das war ich nicht" Buttons
- Bei "Das war ich nicht": Sofort alle Sessions beenden, Passwort-Reset erzwingen
- Rate-Limiting auf Benachrichtigungen (max. 5/Tag)

## Story Points

5

## Priorität

Niedrig - Phase 2
