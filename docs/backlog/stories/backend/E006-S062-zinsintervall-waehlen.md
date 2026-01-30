# Story E006-S062: Zinsintervall wählen

## Epic
E006 - Zinsen

## User Story

Als **Elternteil** möchte ich **das Zinsberechnungs-Intervall wählen können**, damit **ich entscheiden kann, wie häufig Zinsen gutgeschrieben werden**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Zinsen, wenn das Intervall gewählt wird, dann stehen Wöchentlich, Monatlich und Quartalsweise zur Verfügung
- [ ] Gegeben ein gewähltes Intervall, wenn es geändert wird, dann wird das nächste Berechnungsdatum angepasst
- [ ] Gegeben ein Intervall, wenn Zinsen berechnet werden, dann wird der Jahreszins entsprechend umgerechnet
- [ ] Gegeben ein Kind, wenn das Intervall gewählt wird, dann kann auch die Berechnungsmethode festgelegt werden
- [ ] Gegeben eine Intervalländerung, wenn sie erfolgt, dann werden laufende Perioden nicht beeinflusst

## API-Endpunkt

```
PATCH /api/families/{familyId}/children/{childId}/interest/settings
Authorization: Bearer {token}
Content-Type: application/json

{
  "interval": "Weekly|Monthly|Quarterly",
  "calculationMethod": "EndOfPeriod|AverageBalance"
}

Response 200:
{
  "childId": "guid",
  "interval": "Monthly",
  "calculationMethod": "EndOfPeriod",
  "previousNextCalculation": "date",
  "newNextCalculation": "date",
  "updatedAt": "datetime"
}

Response 400:
{
  "errors": {
    "interval": ["Ungültiges Intervall"]
  }
}
```

### Verfügbare Optionen
```
GET /api/interest/options
Authorization: Bearer {token}

Response 200:
{
  "intervals": [
    {
      "value": "Weekly",
      "displayName": "Wöchentlich",
      "divisor": 52
    },
    {
      "value": "Monthly",
      "displayName": "Monatlich",
      "divisor": 12
    },
    {
      "value": "Quarterly",
      "displayName": "Quartalsweise",
      "divisor": 4
    }
  ],
  "calculationMethods": [
    {
      "value": "EndOfPeriod",
      "displayName": "Endstand der Periode",
      "description": "Zinsen auf Kontostand am Ende der Periode"
    },
    {
      "value": "AverageBalance",
      "displayName": "Durchschnittsguthaben",
      "description": "Zinsen auf durchschnittlichen Kontostand"
    }
  ]
}
```

## Technische Notizen

- Zinsformel: (Kontostand * Jahreszins / 100) / Divisor
- EndOfPeriod: Kontostand am letzten Tag
- AverageBalance: Durchschnitt aller Tagesendstände
- Nächste Berechnung neu kalkulieren bei Änderung
- Laufende Periode wird nicht unterbrochen

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E006-062-01 | Wöchentlich wählen | Divisor = 52 |
| TC-E006-062-02 | Monatlich wählen | Divisor = 12 |
| TC-E006-062-03 | Quartalsweise wählen | Divisor = 4 |
| TC-E006-062-04 | Berechnungsmethode ändern | Neue Methode ab nächster Periode |
| TC-E006-062-05 | Ungültiges Intervall | 400 Validierungsfehler |

## Story Points

3

## Priorität

Mittel
