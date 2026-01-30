# E001-S060: Aktive Sessions anzeigen

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **alle meine aktiven Anmeldesitzungen sehen können**, damit **ich einen Überblick habe, wo ich überall angemeldet bin und verdächtige Aktivitäten erkennen kann**.

## Akzeptanzkriterien

- [ ] Liste aller aktiven Sessions wird angezeigt
- [ ] Für jede Session: Gerätename, Browser/App, IP-Adresse, Standort (grob)
- [ ] Zeitpunkt der Anmeldung und letzte Aktivität sichtbar
- [ ] Aktuelle Session ist markiert ("Dieses Gerät")
- [ ] Verdächtige Sessions werden hervorgehoben (ungewöhnlicher Standort)
- [ ] Eltern können Sessions ihrer Kinder einsehen
- [ ] Sortierung nach letzter Aktivität (neueste zuerst)

## API-Endpunkte

```
GET /api/auth/sessions

Response 200:
{
  "sessions": [
    {
      "id": "guid",
      "deviceName": "Chrome auf Windows",
      "browser": "Chrome 120",
      "platform": "Windows 11",
      "ipAddress": "192.168.1.100",
      "location": "Frankfurt, Deutschland",
      "createdAt": "2024-01-15T10:30:00Z",
      "lastActivityAt": "2024-01-20T08:15:00Z",
      "isCurrent": true,
      "isSuspicious": false
    },
    {
      "id": "guid",
      "deviceName": "Safari auf iPhone",
      "browser": "Safari Mobile",
      "platform": "iOS 17",
      "ipAddress": "203.0.113.50",
      "location": "Unbekannt",
      "createdAt": "2024-01-18T14:00:00Z",
      "lastActivityAt": "2024-01-19T20:30:00Z",
      "isCurrent": false,
      "isSuspicious": true
    }
  ],
  "totalCount": 2
}

---

GET /api/auth/sessions/child/{childId}

(Eltern-Endpunkt)

Response 200:
{
  "childName": "Max",
  "sessions": [...]
}
```

## Technische Hinweise

- User-Agent-Parsing für Gerät/Browser-Erkennung
- GeoIP-Lookup für Standort (MaxMind GeoLite2 oder ähnlich)
- Verdächtig markieren bei: Neuer Standort, VPN/Proxy, Tor-Exit-Node
- Session-Tabelle mit: RefreshTokenHash, DeviceInfo, IpAddress, CreatedAt, LastActivityAt
- LastActivityAt bei jedem API-Call aktualisieren
- Pagination für Benutzer mit vielen Sessions (>10)

## Story Points

2

## Priorität

Mittel
