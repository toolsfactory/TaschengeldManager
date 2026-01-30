# E001-S062: Alle anderen Sessions beenden

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **mit einem Klick alle anderen Anmeldesitzungen beenden können**, damit **ich bei Verdacht auf unbefugten Zugriff schnell alle Geräte abmelden kann**.

## Akzeptanzkriterien

- [ ] Alle Sessions außer der aktuellen werden beendet
- [ ] Warnhinweis mit Anzahl der zu beendenden Sessions
- [ ] MFA-Bestätigung vor dem Ausführen (Sicherheit)
- [ ] Biometrie-Tokens auf anderen Geräten werden ebenfalls invalidiert
- [ ] Bestätigung mit Anzahl der beendeten Sessions
- [ ] Aktuelle Session bleibt aktiv
- [ ] E-Mail-Benachrichtigung über Massenabmeldung

## API-Endpunkt

```
DELETE /api/auth/sessions/others

Request:
{
  "mfaCode": "123456",
  "includeBiometric": true
}

Response 200:
{
  "message": "Alle anderen Geräte wurden abgemeldet",
  "terminatedSessions": 4,
  "terminatedBiometricDevices": 2,
  "currentSessionPreserved": true
}

Response 400:
{
  "error": "Keine anderen aktiven Sessions vorhanden"
}
```

## Technische Hinweise

- Atomare Operation: Alle Sessions in einer Transaktion beenden
- Alle Refresh Tokens des Benutzers außer aktuellem invalidieren
- Optional: Auch Biometrie-Tokens invalidieren
- E-Mail mit: Zeitpunkt, Anzahl beendeter Sessions, IP der auslösenden Session
- Bei Kind-Account: Eltern werden benachrichtigt
- Performance: Batch-Update statt einzelne Queries

## Story Points

2

## Priorität

Mittel
