# E001-S071: Account entsperren

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Elternteil** möchte ich **den gesperrten Account meines Kindes wieder entsperren können**, damit **mein Kind nach Ablauf der Strafe oder nach Klärung wieder Zugriff erhält**.

## Akzeptanzkriterien

- [ ] Eltern können Kind-Account manuell entsperren
- [ ] MFA-Bestätigung vor Entsperrung erforderlich
- [ ] Kind kann sich nach Entsperrung sofort wieder anmelden
- [ ] Benachrichtigung an Kind über Entsperrung (kindgerecht)
- [ ] Sperr-Historie wird aufbewahrt (für Eltern einsehbar)
- [ ] Automatische Entsperrung bei Ablauf der Sperrdauer
- [ ] Beide Elternteile können entsperren

## API-Endpunkte

```
POST /api/users/children/{childId}/unlock

Request:
{
  "mfaCode": "123456"
}

Response 200:
{
  "message": "Account für Max entsperrt",
  "unlockedAt": "2024-01-22T15:00:00Z"
}

Response 400:
{
  "error": "Account ist nicht gesperrt"
}

---

GET /api/users/children/{childId}/lock-history

Response 200:
{
  "history": [
    {
      "lockedAt": "2024-01-20T10:30:00Z",
      "lockedBy": "Papa",
      "reason": "Handyverbot",
      "unlockedAt": "2024-01-22T15:00:00Z",
      "unlockedBy": "Mama",
      "duration": "P2D"
    },
    {
      "lockedAt": "2024-01-05T09:00:00Z",
      "lockedBy": "Mama",
      "reason": "Zu viel Bildschirmzeit",
      "unlockedAt": "2024-01-05T18:00:00Z",
      "unlockedBy": "system",  // automatisch
      "duration": "PT9H"
    }
  ]
}
```

## Technische Hinweise

- Bei Entsperrung: IsLocked = false, LockedUntil = null
- Sperr-Historie in separater Tabelle für Audit
- Hintergrund-Job prüft alle 5 Minuten auf abgelaufene Sperren
- Push-Benachrichtigung an Kind bei Entsperrung
- E-Mail an andere Elternteile bei manueller Entsperrung
- unlockedBy = "system" bei automatischer Entsperrung

## Story Points

1

## Priorität

Mittel
