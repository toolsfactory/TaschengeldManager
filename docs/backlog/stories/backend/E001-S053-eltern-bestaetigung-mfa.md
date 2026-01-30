# E001-S053: Eltern-Bestätigung als MFA-Alternative für Kinder

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Kind ohne eigenes Authenticator-Gerät** möchte ich **meine Eltern bitten können, meinen Login zu bestätigen**, damit **ich mich auch ohne TOTP-App sicher anmelden kann**.

## Akzeptanzkriterien

- [ ] Kind kann bei MFA-Abfrage "Eltern fragen" wählen
- [ ] Push-Benachrichtigung wird an Eltern-Geräte gesendet
- [ ] Eltern sehen: Welches Kind, welches Gerät, Zeitpunkt
- [ ] Eltern können mit einem Tap bestätigen oder ablehnen
- [ ] Timeout: 5 Minuten für Eltern-Reaktion
- [ ] Kind sieht Warteanzeige mit kindgerechtem Text
- [ ] Bei Ablehnung/Timeout: Kind kann es erneut versuchen oder TOTP nutzen
- [ ] Eltern können diese Funktion pro Kind deaktivieren

## API-Endpunkte

```
POST /api/auth/mfa/parent-approval/request

Request:
{
  "mfaToken": "temporary-token-from-login",
  "deviceInfo": "iPad im Kinderzimmer"
}

Response 200:
{
  "approvalRequestId": "guid",
  "expiresAt": "2024-01-20T10:35:00Z",
  "message": "Wir haben deine Eltern gefragt. Warte kurz..."
}

---

GET /api/auth/mfa/parent-approval/status/{requestId}

(Polling durch Kind-App)

Response 200 (Wartend):
{
  "status": "pending",
  "expiresAt": "2024-01-20T10:35:00Z"
}

Response 200 (Genehmigt):
{
  "status": "approved",
  "accessToken": "jwt-token",
  "refreshToken": "refresh-token"
}

Response 200 (Abgelehnt):
{
  "status": "rejected",
  "message": "Deine Eltern haben den Login abgelehnt."
}

---

POST /api/auth/mfa/parent-approval/respond/{requestId}

(Eltern-Endpunkt)

Request:
{
  "approved": true,
  "parentMfaCode": "123456"
}

Response 200:
{
  "message": "Login für Max bestätigt"
}
```

## Technische Hinweise

- Push-Benachrichtigung an alle registrierten Eltern-Geräte
- Ein Elternteil reicht zur Bestätigung
- Eltern-MFA erforderlich für Bestätigung (Sicherheit)
- Approval-Request in Datenbank mit Status (pending/approved/rejected/expired)
- WebSocket oder Polling für Echtzeit-Status im Kind-UI
- Rate-Limiting: Max. 5 Anfragen pro Kind/Stunde
- Missbrauchsschutz: Eltern können bei Verdacht nachfragen

## Story Points

3

## Priorität

Mittel
