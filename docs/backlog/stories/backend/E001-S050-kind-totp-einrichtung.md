# E001-S050: Kind: Vereinfachte TOTP-Einrichtung

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Kind** möchte ich **mit Hilfe meiner Eltern einen einfachen Geheimcode einrichten können**, damit **mein Konto sicher ist und ich verstehe, warum Sicherheit wichtig ist**.

## Akzeptanzkriterien

- [ ] TOTP-Einrichtung für Kinder wird von Eltern initiiert
- [ ] Kindgerechte Erklärung des Zwecks ("Dein Geheimcode")
- [ ] Eltern scannen QR-Code mit eigenem Gerät oder Gerät des Kindes
- [ ] Kind muss ersten Code selbst eingeben (Lerneffekt)
- [ ] Bei Erfolg: Kindgerechte Erfolgsmeldung und Lob
- [ ] Eltern erhalten Kopie der Backup-Codes
- [ ] Option: Eltern können TOTP auf eigenem Gerät für Kind verwalten

## API-Endpunkte

```
POST /api/auth/mfa/totp/setup/child/{childId}

(Eltern-Authentifizierung erforderlich)

Response 200:
{
  "secret": "BASE32SECRET",
  "qrCodeDataUrl": "data:image/png;base64,...",
  "manualEntryKey": "XXXX XXXX XXXX XXXX",
  "issuer": "TaschengeldManager",
  "childName": "Max",
  "simplified": true,
  "codeLength": 4
}

---

POST /api/auth/mfa/totp/verify-setup/child/{childId}

Request:
{
  "code": "1234",
  "parentMfaCode": "123456"
}

Response 200:
{
  "success": true,
  "backupCodes": [
    "AAAA-BBBB",
    "CCCC-DDDD",
    ...
  ],
  "message": "Super! Dein Geheimcode ist jetzt aktiv!"
}
```

## Technische Hinweise

- Kind-TOTP: 4-stelliger Code statt 6-stellig (siehe S051)
- QR-Code enthält Hinweis auf vereinfachten Modus
- Eltern erhalten E-Mail mit Backup-Codes des Kindes
- Bei sehr jungen Kindern: Option für Eltern-verwaltetes TOTP
- Altersabhängige Texte und Erklärungen
- Eltern können TOTP jederzeit für Kind zurücksetzen

## Story Points

2

## Priorität

Hoch - MVP
