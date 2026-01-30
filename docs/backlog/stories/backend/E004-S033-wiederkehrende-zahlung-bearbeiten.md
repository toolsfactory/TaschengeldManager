# Story E004-S033: Wiederkehrende Zahlung bearbeiten

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine bestehende wiederkehrende Zahlung bearbeiten können**, damit **ich Betrag, Intervall oder andere Details anpassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine bestehende Zahlung, wenn sie bearbeitet wird, dann können Name, Betrag und Beschreibung geändert werden
- [ ] Gegeben eine Änderung, wenn sie gespeichert wird, dann werden zukünftige Zahlungen mit den neuen Werten ausgeführt
- [ ] Gegeben eine Änderung des Betrags, wenn sie gespeichert wird, dann bleiben vergangene Transaktionen unverändert
- [ ] Gegeben eine pausierte Zahlung, wenn sie bearbeitet wird, dann bleibt sie pausiert
- [ ] Gegeben eine Bearbeitung, wenn sie durchgeführt wird, dann wird ein Änderungsprotokoll erstellt

## API-Endpunkt

```
PUT /api/recurring-payments/{paymentId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Taschengeld erhöht",
  "amount": 15.00,
  "interval": "Weekly",
  "dayOfWeek": 1,
  "dayOfMonth": null,
  "endDate": "date (optional)",
  "description": "Ab Januar erhöht"
}

Response 200:
{
  "recurringPaymentId": "guid",
  "name": "Taschengeld erhöht",
  "amount": 15.00,
  "interval": "Weekly",
  "dayOfWeek": 1,
  "nextExecutionDate": "date",
  "status": "Active",
  "updatedAt": "datetime",
  "changeHistory": [
    {
      "changedAt": "datetime",
      "changedBy": "string",
      "changes": "Betrag: 10.00 → 15.00"
    }
  ]
}

Response 400:
{
  "errors": {
    "amount": ["Betrag muss positiv sein"]
  }
}

Response 403:
{
  "message": "Keine Berechtigung für diese Zahlung"
}

Response 404:
{
  "message": "Zahlung nicht gefunden"
}
```

## Technische Notizen

- Änderungsprotokoll in separater Tabelle
- Vergangene Transaktionen werden nie verändert
- Beim Speichern: nextExecutionDate neu berechnen
- Status bleibt erhalten (Active/Paused)
- Audit-Trail: Wer hat wann was geändert

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-033-01 | Betrag ändern | 200 + neuer Betrag für Zukunft |
| TC-E004-033-02 | Name ändern | 200 + neuer Name |
| TC-E004-033-03 | Intervall ändern | 200 + nächstes Datum neu berechnet |
| TC-E004-033-04 | Fremde Zahlung | 403 Forbidden |
| TC-E004-033-05 | Änderungsprotokoll | Änderung dokumentiert |

## Story Points

3

## Priorität

Mittel
