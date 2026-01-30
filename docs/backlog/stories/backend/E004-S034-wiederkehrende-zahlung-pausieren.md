# Story E004-S034: Wiederkehrende Zahlung pausieren

## Epic
E004 - Automatische Zahlungen

## User Story

Als **Elternteil** möchte ich **eine wiederkehrende Zahlung vorübergehend pausieren können**, damit **ich sie bei Bedarf ohne Löschen unterbrechen kann**.

## Akzeptanzkriterien

- [ ] Gegeben eine aktive Zahlung, wenn sie pausiert wird, dann werden keine weiteren Zahlungen ausgeführt
- [ ] Gegeben eine pausierte Zahlung, wenn sie angezeigt wird, dann ist der Status deutlich erkennbar
- [ ] Gegeben eine pausierte Zahlung, wenn sie reaktiviert wird, dann wird das nächste Ausführungsdatum neu berechnet
- [ ] Gegeben eine Pausierung, wenn sie durchgeführt wird, dann kann optional ein Grund angegeben werden
- [ ] Gegeben eine pausierte Zahlung, wenn der nächste Zahlungstag kommt, dann wird keine Zahlung ausgeführt

## API-Endpunkte

### Zahlung pausieren
```
POST /api/recurring-payments/{paymentId}/pause
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Urlaub"
}

Response 200:
{
  "recurringPaymentId": "guid",
  "status": "Paused",
  "pausedAt": "datetime",
  "pauseReason": "Urlaub",
  "previousNextExecution": "date"
}

Response 400:
{
  "message": "Zahlung ist bereits pausiert"
}
```

### Zahlung reaktivieren
```
POST /api/recurring-payments/{paymentId}/resume
Authorization: Bearer {token}

Response 200:
{
  "recurringPaymentId": "guid",
  "status": "Active",
  "resumedAt": "datetime",
  "nextExecutionDate": "date",
  "pauseDuration": "5 Tage"
}

Response 400:
{
  "message": "Zahlung ist nicht pausiert"
}
```

## Technische Notizen

- Status-Enum erweitern: Active, Paused, Completed, Cancelled
- pausedAt Timestamp speichern
- Bei Resume: nextExecutionDate ab heute neu berechnen
- Verpasste Zahlungen werden NICHT nachgeholt
- Optional: Erinnerung wenn länger als X Tage pausiert

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E004-034-01 | Aktive Zahlung pausieren | Status = Paused |
| TC-E004-034-02 | Bereits pausierte Zahlung | 400 Fehler |
| TC-E004-034-03 | Pausierte Zahlung reaktivieren | Status = Active, neues Datum |
| TC-E004-034-04 | Job bei pausierter Zahlung | Keine Ausführung |
| TC-E004-034-05 | Mit Pausengrund | Grund wird gespeichert |

## Story Points

3

## Priorität

Mittel
