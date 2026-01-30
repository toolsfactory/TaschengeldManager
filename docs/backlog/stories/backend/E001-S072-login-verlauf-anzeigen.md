# E001-S072: Login-Verlauf anzeigen

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Benutzer** möchte ich **meinen Login-Verlauf einsehen können**, damit **ich verdächtige Anmeldeversuche erkennen und mein Konto besser schützen kann**.

## Akzeptanzkriterien

- [ ] Liste der letzten 50 Login-Versuche (erfolgreich und fehlgeschlagen)
- [ ] Für jeden Eintrag: Zeitpunkt, Gerät, IP, Standort, Erfolg/Fehlschlag
- [ ] Fehlgeschlagene Versuche werden markiert mit Grund
- [ ] Verdächtige Aktivitäten werden hervorgehoben
- [ ] Filterung nach Zeitraum und Status möglich
- [ ] Eltern können Login-Verlauf ihrer Kinder einsehen
- [ ] Export als CSV möglich (für Sicherheitsbewusste)

## API-Endpunkte

```
GET /api/auth/login-history?limit=50&status=all

Response 200:
{
  "history": [
    {
      "id": "guid",
      "timestamp": "2024-01-20T08:15:00Z",
      "success": true,
      "method": "password_mfa",
      "mfaMethod": "totp",
      "deviceName": "Chrome auf Windows",
      "browser": "Chrome 120",
      "platform": "Windows 11",
      "ipAddress": "192.168.1.100",
      "location": "Frankfurt, Deutschland",
      "isSuspicious": false
    },
    {
      "id": "guid",
      "timestamp": "2024-01-19T22:30:00Z",
      "success": false,
      "failureReason": "invalid_password",
      "deviceName": "Unbekanntes Gerät",
      "browser": "Firefox",
      "platform": "Linux",
      "ipAddress": "203.0.113.50",
      "location": "Moskau, Russland",
      "isSuspicious": true
    }
  ],
  "totalCount": 45,
  "suspiciousCount": 3
}

---

GET /api/auth/login-history/child/{childId}

(Eltern-Endpunkt)

Response 200:
{
  "childName": "Max",
  "history": [...]
}

---

GET /api/auth/login-history/export

Response 200 (CSV):
Content-Type: text/csv
Content-Disposition: attachment; filename="login-history-2024-01-20.csv"
```

## Technische Hinweise

- LoginAttempt-Tabelle mit Index auf UserId + Timestamp
- Verdächtig markieren bei: Neuer Standort, ungewöhnliche Zeit, viele Fehlversuche
- GeoIP-Lookup für Standortbestimmung
- Aufbewahrung: 90 Tage, danach automatische Löschung (DSGVO)
- Bei fehlgeschlagenem Login: Keine Benutzer-ID speichern wenn nicht authentifiziert
- Pagination für große Verläufe

## Story Points

2

## Priorität

Mittel
