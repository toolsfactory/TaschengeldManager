# Story E004-S035: Wiederkehrende Zahlung löschen

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine wiederkehrende Zahlung dauerhaft löschen können**, damit **ich nicht mehr benötigte Zahlungen entfernen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine wiederkehrende Zahlung, wenn sie gelöscht wird, dann werden keine weiteren Zahlungen ausgeführt
- [ ] Gegeben eine gelöschte Zahlung, wenn sie entfernt wird, dann bleiben die bereits ausgeführten Transaktionen erhalten
- [ ] Gegeben eine Löschung, wenn sie durchgeführt wird, dann wird eine Bestätigung angefordert
- [ ] Gegeben eine gelöschte Zahlung, wenn sie nicht mehr existiert, dann ist sie nicht wiederherstellbar
- [ ] Gegeben eine Löschung, wenn sie abgeschlossen ist, dann wird sie aus der aktiven Übersicht entfernt

## API-Endpunkt

```
DELETE /api/recurring-payments/{paymentId}
Authorization: Bearer {token}

Response 200:
{
  "message": "Wiederkehrende Zahlung wurde gelöscht",
  "recurringPaymentId": "guid",
  "name": "Taschengeld",
  "totalExecutedPayments": 12,
  "totalAmountPaid": 120.00,
  "deletedAt": "datetime"
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

### Alternativ: Soft-Delete mit Status
```
POST /api/recurring-payments/{paymentId}/cancel
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Kind ist ausgezogen"
}

Response 200:
{
  "recurringPaymentId": "guid",
  "status": "Cancelled",
  "cancelledAt": "datetime",
  "cancelReason": "Kind ist ausgezogen"
}
```

## Technische Notizen

- Soft-Delete empfohlen (Status = Cancelled)
- Vergangene Transaktionen referenzieren weiterhin die Zahlung
- Bei Hard-Delete: Cascade auf Transaktionen vermeiden
- Audit-Log für Löschung führen
- Gelöschte Zahlungen in Archiv-Ansicht verfügbar (optional)

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-035-01 | Aktive Zahlung löschen | Status = Cancelled |
| TC-E004-035-02 | Pausierte Zahlung löschen | Status = Cancelled |
| TC-E004-035-03 | Vergangene Transaktionen | Bleiben erhalten |
| TC-E004-035-04 | Fremde Zahlung löschen | 403 Forbidden |
| TC-E004-035-05 | Bereits gelöschte Zahlung | 404 Not Found |

## Story Points

2

## Priorität

Mittel
