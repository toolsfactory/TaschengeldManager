# Story E002-S021: Verwandten einladen (per E-Mail)

## Epic
E002 - Familien- & Kontoverwaltung

## User Story

Als **Elternteil** möchte ich **Verwandte (Großeltern, Onkel, Tanten) per E-Mail einladen können**, damit **diese den Kindern Geld überweisen können**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil, wenn es einen Verwandten einlädt, dann wird eine E-Mail mit Einladungslink versendet
- [ ] Gegeben eine Verwandten-Einladung, wenn sie angenommen wird, dann erhält der Verwandte eingeschränkten Zugang
- [ ] Gegeben ein Verwandter, wenn er eingeladen wird, dann kann er eine Beziehung auswählen (Oma, Opa, Onkel, etc.)
- [ ] Gegeben eine Einladung, wenn sie versendet wird, dann hat sie ein Ablaufdatum (14 Tage)
- [ ] Gegeben ein Verwandter, wenn er beitritt, dann sieht er nur die Namen der Kinder (keine Kontostände)

## API-Endpunkt

```
POST /api/families/{familyId}/invitations
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "string",
  "role": "Relative",
  "relationshipType": "Grandparent|Uncle|Aunt|Other",
  "relationshipName": "string (optional, z.B. 'Oma Helga')",
  "message": "string (optional)"
}

Response 201:
{
  "invitationId": "guid",
  "email": "string",
  "role": "Relative",
  "relationshipType": "Grandparent",
  "expiresAt": "datetime"
}

Response 400:
{
  "errors": {
    "email": ["Ungültige E-Mail-Adresse"],
    "relationshipType": ["Beziehungstyp ist erforderlich"]
  }
}
```

## Technische Notizen

- Verwandten-Rolle mit eingeschränkten Rechten
- Beziehungstyp für bessere Anzeige in der App
- Längere Gültigkeit (14 Tage) da oft ältere Personen
- Einfacher Registrierungsprozess für Verwandte
- Nur Lesezugriff auf Kindernamen, kein Zugriff auf Kontostände

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E002-021-01 | Verwandten einladen | 201 + E-Mail versendet |
| TC-E002-021-02 | Ohne Beziehungstyp | 400 Validierungsfehler |
| TC-E002-021-03 | Ungültige E-Mail | 400 Validierungsfehler |
| TC-E002-021-04 | Kind lädt ein | 403 Forbidden |
| TC-E002-021-05 | Doppelte Einladung | Alte wird ersetzt |

## Story Points

3

## Priorität

Mittel
