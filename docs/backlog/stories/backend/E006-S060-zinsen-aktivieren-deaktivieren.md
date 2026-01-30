# Story E006-S060: Zinsen für Kind aktivieren/deaktivieren

## Epic
E006 - Zinsen

## User Story

Als **Elternteil** möchte ich **die Zinsfunktion für jedes Kind einzeln aktivieren oder deaktivieren können**, damit **ich pädagogisch wertvoll das Sparen belohnen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Konto, wenn Zinsen aktiviert werden, dann werden ab sofort Zinsen berechnet
- [ ] Gegeben aktivierte Zinsen, wenn sie deaktiviert werden, dann werden keine weiteren Zinsen berechnet
- [ ] Gegeben eine Zinsen-Aktivierung, wenn sie erfolgt, dann wird das Kind benachrichtigt
- [ ] Gegeben ein Kind, wenn Zinsen aktiviert werden, dann wird ein Standard-Zinssatz vorgeschlagen
- [ ] Gegeben deaktivierte Zinsen, wenn sie deaktiviert werden, dann bleiben bisherige Zinsgutschriften erhalten

## API-Endpunkte

### Zinsen aktivieren
```
POST /api/families/{familyId}/children/{childId}/interest/enable
Authorization: Bearer {token}
Content-Type: application/json

{
  "interestRate": 3.0,
  "interval": "Monthly",
  "calculationMethod": "EndOfPeriod"
}

Response 200:
{
  "childId": "guid",
  "interestEnabled": true,
  "interestRate": 3.0,
  "interval": "Monthly",
  "calculationMethod": "EndOfPeriod",
  "nextCalculationDate": "date",
  "enabledAt": "datetime"
}

Response 400:
{
  "errors": {
    "interestRate": ["Zinssatz muss zwischen 0 und 100 liegen"]
  }
}
```

### Zinsen deaktivieren
```
POST /api/families/{familyId}/children/{childId}/interest/disable
Authorization: Bearer {token}

Response 200:
{
  "childId": "guid",
  "interestEnabled": false,
  "totalInterestEarned": 5.25,
  "disabledAt": "datetime"
}
```

### Zinsen-Status abrufen
```
GET /api/families/{familyId}/children/{childId}/interest
Authorization: Bearer {token}

Response 200:
{
  "childId": "guid",
  "interestEnabled": true,
  "interestRate": 3.0,
  "interval": "Monthly",
  "calculationMethod": "EndOfPeriod",
  "nextCalculationDate": "date",
  "totalInterestEarned": 12.50,
  "lastCalculationDate": "date",
  "lastInterestAmount": 0.75
}
```

## Technische Notizen

- InterestSettings Entity pro Kinderkonto
- Aktivierung/Deaktivierung ändert nur den Status
- Historische Zinsen bleiben immer erhalten
- Standard-Zinssatz: 3% (konfigurierbar)
- Benachrichtigung an Kind bei Aktivierung

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E006-060-01 | Zinsen aktivieren | 200 + interestEnabled = true |
| TC-E006-060-02 | Zinsen deaktivieren | 200 + interestEnabled = false |
| TC-E006-060-03 | Ungültiger Zinssatz | 400 Validierungsfehler |
| TC-E006-060-04 | Kind ohne Konto | 404 Not Found |
| TC-E006-060-05 | Historische Zinsen | Bleiben nach Deaktivierung |

## Story Points

3

## Priorität

Mittel
