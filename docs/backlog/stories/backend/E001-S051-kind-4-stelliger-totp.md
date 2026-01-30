# E001-S051: Kind: 4-stelliger TOTP-Code

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Kind** möchte ich **einen kürzeren Geheimcode eingeben müssen**, damit **ich mir den Code leichter merken und schneller eingeben kann**.

## Akzeptanzkriterien

- [ ] Kinder-TOTP generiert 4-stellige Codes statt 6-stellig
- [ ] Größere Toleranz bei Zeitabweichung (+/- 2 Fenster statt 1)
- [ ] Kindgerechte Fehleranzeige bei falschem Code
- [ ] Mehr Versuche erlaubt bevor Lockout (5 statt 3)
- [ ] Eltern werden bei mehreren Fehlversuchen benachrichtigt
- [ ] Code-Eingabe mit großen, kindgerechten Buttons
- [ ] Kompatibilität mit Standard-Authenticator-Apps (zeigt 6-stellig, Kind gibt erste 4 ein)

## API-Endpunkt

```
POST /api/auth/mfa/totp/verify

Request:
{
  "mfaToken": "temporary-token-from-login",
  "code": "1234"
}

Response 200:
{
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token",
  "expiresIn": 3600
}

Response 400:
{
  "error": "Der Code war leider falsch. Probier es nochmal!",
  "attemptsRemaining": 4,
  "hint": "Schau auf die ersten 4 Zahlen in deiner App"
}
```

## Technische Hinweise

- 4-stelliger Code: Erste 4 Ziffern des Standard-6-stelligen TOTP
- Alternative: Separater TOTP-Algorithmus mit 4-stelliger Ausgabe
- Sicherheitshinweis: 4-stelliger Code hat 10.000 Kombinationen (ausreichend mit Rate-Limiting)
- Zeitfenster-Toleranz: 2 Fenster = 150 Sekunden total
- Rate-Limiting angepasst: 5 Versuche pro mfaToken
- Push-Benachrichtigung an Eltern nach 3 Fehlversuchen

## Story Points

1

## Priorität

Hoch - MVP
