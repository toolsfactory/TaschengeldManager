# Story E002-S017: Elternteil zur Familie einladen

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil (Admin)** möchte ich **ein weiteres Elternteil zu meiner Familie einladen können**, damit **wir gemeinsam die Kinder verwalten können**.

## Akzeptanzkriterien

- [ ] Gegeben ein Familien-Admin, wenn er eine E-Mail-Adresse eingibt, dann wird eine Einladung versendet
- [ ] Gegeben eine Einladung, wenn sie versendet wird, dann enthält sie einen eindeutigen Einladungslink
- [ ] Gegeben eine Einladung, wenn sie versendet wird, dann hat sie ein Ablaufdatum (7 Tage)
- [ ] Gegeben eine bereits eingeladene E-Mail, wenn erneut eingeladen wird, dann wird die alte Einladung ersetzt
- [ ] Gegeben ein bereits registrierter Benutzer mit Familie, wenn er eingeladen wird, dann wird dies verhindert

## API-Endpunkt

```
POST /api/families/{familyId}/invitations
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "string",
  "role": "Parent",
  "message": "string (optional)"
}

Response 201:
{
  "invitationId": "guid",
  "email": "string",
  "role": "Parent",
  "expiresAt": "datetime",
  "invitedBy": "string"
}

Response 400:
{
  "errors": {
    "email": ["Ungültige E-Mail-Adresse"]
  }
}

Response 409:
{
  "message": "Diese Person ist bereits Mitglied einer Familie"
}
```

## Technische Notizen

- Einladungs-Token generieren (UUID + Timestamp Hash)
- E-Mail über externen Service versenden
- Einladungen in eigener Tabelle speichern
- Bei existierendem User: Prüfen ob bereits in anderer Familie
- Rate Limiting: Max 10 Einladungen pro Tag

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-017-01 | Valide Einladung | 201 + E-Mail versendet |
| TC-E002-017-02 | Ungültige E-Mail | 400 Validierungsfehler |
| TC-E002-017-03 | Bereits Mitglied einer Familie | 409 Konflikt |
| TC-E002-017-04 | Doppelte Einladung | Alte wird ersetzt |
| TC-E002-017-05 | Nicht-Admin lädt ein | 403 Forbidden |

## Story Points

5

## Priorität

Mittel
