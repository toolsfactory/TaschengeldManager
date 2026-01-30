# E001-S070: Account sperren (durch Eltern)

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Elternteil** möchte ich **den Account meines Kindes vorübergehend sperren können**, damit **ich bei Regelverstößen oder Verdacht auf Missbrauch sofort handeln kann**.

## Akzeptanzkriterien

- [ ] Eltern können Kind-Account mit einem Klick sperren
- [ ] Sperrgrund kann angegeben werden (optional)
- [ ] Gesperrtes Kind wird sofort von allen Geräten abgemeldet
- [ ] Gesperrtes Kind kann sich nicht mehr anmelden
- [ ] Kind sieht kindgerechte Meldung ("Bitte sprich mit deinen Eltern")
- [ ] Eltern können Sperrdauer festlegen (oder unbegrenzt)
- [ ] Benachrichtigung an beide Elternteile bei Sperrung

## API-Endpunkte

```
POST /api/users/children/{childId}/lock

Request:
{
  "reason": "Handyverbot diese Woche",
  "duration": "P7D",  // ISO 8601 Duration, null für unbegrenzt
  "mfaCode": "123456"
}

Response 200:
{
  "message": "Account für Max gesperrt",
  "lockedUntil": "2024-01-27T10:30:00Z",
  "sessionsTerminated": 2
}

---

GET /api/users/children/{childId}/lock-status

Response 200:
{
  "isLocked": true,
  "lockedAt": "2024-01-20T10:30:00Z",
  "lockedBy": "Papa",
  "reason": "Handyverbot diese Woche",
  "lockedUntil": "2024-01-27T10:30:00Z"
}

---

POST /api/auth/child-login

Response 403 (Gesperrter Account):
{
  "error": "account_locked",
  "message": "Dein Konto ist gerade pausiert. Bitte sprich mit deinen Eltern.",
  "lockedUntil": "2024-01-27T10:30:00Z"
}
```

## Technische Hinweise

- User-Tabelle: IsLocked, LockedAt, LockedUntil, LockedReason
- Bei Sperrung: Alle aktiven Sessions und Biometrie-Tokens invalidieren
- Automatische Entsperrung nach Ablauf der Sperrdauer (Hintergrund-Job)
- Push-Benachrichtigung an Kind-Gerät bei Sperrung
- Audit-Log: Wer hat wann gesperrt/entsperrt
- Nur Parent-Rolle kann Kind sperren (nicht Relatives)

## Story Points

2

## Priorität

Mittel
