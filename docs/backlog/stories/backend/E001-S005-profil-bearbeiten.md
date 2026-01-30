# E001-S005: Profil bearbeiten

## Status: Fertig

## Epic
E001 - Benutzerverwaltung, Authentifizierung & Sicherheit

## User Story

Als **angemeldeter Benutzer** möchte ich **mein Profil bearbeiten können (Name, E-Mail, Passwort)**, damit **meine persönlichen Daten aktuell bleiben**.

## Akzeptanzkriterien

- [ ] Vorname und Nachname können geändert werden
- [ ] E-Mail-Adresse kann geändert werden (mit Bestätigung)
- [ ] Bei E-Mail-Änderung wird eine Bestätigungsmail an die neue Adresse gesendet
- [ ] Passwort kann geändert werden (altes Passwort erforderlich)
- [ ] Passwortänderung erfordert zusätzliche MFA-Verifizierung
- [ ] Profilbild kann hochgeladen werden (optional)
- [ ] Änderungen werden in Audit-Log protokolliert

## API-Endpunkte

```
GET /api/users/me

Response 200:
{
  "id": "guid",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "role": "Parent",
  "profileImageUrl": "string | null",
  "mfaEnabled": true,
  "createdAt": "datetime"
}

---

PATCH /api/users/me

Request:
{
  "firstName": "string",
  "lastName": "string"
}

Response 200:
{
  "message": "Profil aktualisiert"
}

---

POST /api/users/me/change-email

Request:
{
  "newEmail": "string",
  "mfaCode": "string"
}

Response 200:
{
  "message": "Bestätigungsmail an neue Adresse versendet"
}

---

POST /api/users/me/change-password

Request:
{
  "currentPassword": "string",
  "newPassword": "string",
  "mfaCode": "string"
}

Response 200:
{
  "message": "Passwort erfolgreich geändert"
}
```

## Technische Hinweise

- E-Mail-Änderung erst nach Klick auf Bestätigungslink aktiv
- Alte E-Mail erhält Benachrichtigung über angeforderte Änderung
- Passwortänderung invalidiert alle anderen Sessions
- Profilbild: Max. 2MB, nur JPG/PNG, wird auf 256x256 skaliert
- MFA-Verifizierung bei sicherheitsrelevanten Änderungen (E-Mail, Passwort)

## Story Points

3

## Priorität

Mittel
