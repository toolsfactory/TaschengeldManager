# E001-S043: Fallback auf PIN/Passwort bei Biometrie-Fehler

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **Mobile-App-Nutzer** möchte ich **bei fehlgeschlagener Biometrie auf PIN oder Passwort zurückgreifen können**, damit **ich auch dann Zugang zu meinem Konto habe, wenn die Biometrie nicht funktioniert**.

## Akzeptanzkriterien

- [ ] Nach 3 fehlgeschlagenen Biometrie-Versuchen wird Fallback angeboten
- [ ] Kinder: Fallback auf PIN + vereinfachter TOTP
- [ ] Erwachsene: Fallback auf Passwort + TOTP
- [ ] Biometrie-Token bleibt gültig (kein Reset nötig nach Fallback)
- [ ] Benutzer kann Fallback auch direkt wählen ("Andere Anmeldemethode")
- [ ] Bei gesperrter Biometrie: Nur Fallback möglich für 15 Minuten
- [ ] Fehlgeschlagene Versuche werden protokolliert

## API-Endpunkte

```
POST /api/auth/biometric/verify

Response 429 (nach 3 Fehlversuchen):
{
  "error": "Biometrie vorübergehend gesperrt",
  "lockedUntil": "2024-01-20T10:45:00Z",
  "fallbackRequired": true,
  "fallbackMethods": ["password_mfa", "pin_mfa"]
}

---

POST /api/auth/login (Standard-Login als Fallback)

Request:
{
  "email": "user@example.com",
  "password": "string"
}

(folgt normalem Login-Flow mit MFA)

---

POST /api/auth/child-login (Kind-Login als Fallback)

Request:
{
  "familyCode": "ABC123",
  "nickname": "Max",
  "pin": "1234"
}

(folgt Kind-Login-Flow mit vereinfachtem MFA)
```

## Technische Hinweise

- Biometrie-Sperre ist gerätespezifisch, nicht accountweit
- Fallback-Login setzt Biometrie-Fehlerzähler zurück
- Bei verdächtigem Verhalten (viele Fehlversuche): Push an Eltern (bei Kind)
- Lockout-Zeit progressiv erhöhen bei wiederholten Sperren
- Client-seitig: Native Biometrie-Fehlermeldungen anzeigen
- Differenzierung: Biometrie nicht erkannt vs. abgebrochen

## Story Points

2

## Priorität

Hoch - MVP für Mobile
