# E001-S061: Einzelne Session beenden

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **eine einzelne Anmeldesitzung auf einem anderen Gerät beenden können**, damit **ich den Zugriff von Geräten entfernen kann, die ich nicht mehr verwende oder die mir verdächtig vorkommen**.

## Akzeptanzkriterien

- [ ] Benutzer kann jede Session außer der aktuellen beenden
- [ ] Aktuelle Session kann nicht versehentlich beendet werden
- [ ] Bestätigungsdialog vor dem Beenden
- [ ] Session wird sofort invalidiert
- [ ] Gerät wird beim nächsten Request abgemeldet
- [ ] Benutzer erhält Bestätigung mit Geräteinfo
- [ ] Eltern können Sessions ihrer Kinder beenden

## API-Endpunkte

```
DELETE /api/auth/sessions/{sessionId}

Response 200:
{
  "message": "Session erfolgreich beendet",
  "terminatedSession": {
    "deviceName": "Chrome auf Windows",
    "location": "Frankfurt, Deutschland"
  }
}

Response 400:
{
  "error": "Die aktuelle Session kann nicht beendet werden. Nutze 'Abmelden' stattdessen."
}

Response 404:
{
  "error": "Session nicht gefunden"
}

---

DELETE /api/auth/sessions/child/{childId}/{sessionId}

(Eltern-Endpunkt)

Request:
{
  "reason": "Gerät verloren"  // optional
}

Response 200:
{
  "message": "Session für Max beendet",
  "terminatedSession": {
    "deviceName": "iPad",
    "location": "Zuhause"
  }
}
```

## Technische Hinweise

- Session-Eintrag: IsRevoked = true setzen
- Refresh Token in Blocklist aufnehmen
- Bei nächstem API-Call mit altem Token: 401 Unauthorized
- Push-Benachrichtigung an beendetes Gerät (falls möglich)
- Audit-Log: Wer hat welche Session wann beendet
- Bei Kind-Session durch Eltern: Kind erhält kindgerechte Meldung

## Story Points

1

## Priorität

Mittel
