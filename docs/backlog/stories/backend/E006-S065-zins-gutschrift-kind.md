# Story E006-S065: Zins-Gutschrift anzeigen (Kind)

## Epic
E006 - Zinsen

## User Story

Als **Kind** möchte ich **meine Zins-Gutschriften sehen können**, damit **ich verstehe, wie mein Erspartes wächst**.

## Akzeptanzkriterien

- [ ] Gegeben ein Kind mit Zinsen, wenn es die Gutschriften abruft, dann werden alle Zinszahlungen angezeigt
- [ ] Gegeben eine Zins-Übersicht, wenn sie angezeigt wird, dann ist sie kindgerecht aufbereitet
- [ ] Gegeben Zinsgutschriften, wenn sie angezeigt werden, dann sind sie in der Transaktionshistorie erkennbar
- [ ] Gegeben ein Kind, wenn es die Zinsen sieht, dann versteht es den Zusammenhang zwischen Sparen und Zinsen
- [ ] Gegeben monatliche Zinsen, wenn sie angezeigt werden, dann wird eine einfache Grafik/Trend gezeigt

## API-Endpunkt

```
GET /api/me/interest
Authorization: Bearer {token}

Response 200:
{
  "interestEnabled": true,
  "currentRate": 3.0,
  "currentRateExplanation": "Du bekommst 3% Zinsen pro Jahr auf dein Erspartes",
  "nextInterestDate": "date",
  "estimatedNextInterest": 0.80,
  "totalInterestEarned": 12.50,
  "recentInterest": [
    {
      "amount": 0.75,
      "date": "date",
      "message": "Super! Du hast 0,75 € Zinsen bekommen, weil du so gut gespart hast!"
    }
  ],
  "savingsGrowth": {
    "lastMonth": 0.75,
    "last3Months": 2.25,
    "thisYear": 9.00
  }
}
```

### Zins-Erklärung für Kinder
```
GET /api/me/interest/explanation
Authorization: Bearer {token}

Response 200:
{
  "title": "Was sind Zinsen?",
  "explanation": "Zinsen sind wie eine Belohnung fürs Sparen! Wenn du Geld auf deinem Konto lässt, bekommst du jeden Monat ein bisschen extra dazu.",
  "example": {
    "balance": 100.00,
    "rate": 3.0,
    "monthlyInterest": 0.25,
    "yearlyInterest": 3.00,
    "message": "Wenn du 100 € sparst, bekommst du jeden Monat etwa 25 Cent dazu!"
  }
}
```

## Technische Notizen

- Kindgerechte Sprache und Erklärungen
- Positive Verstärkung ("Super!", "Toll gespart!")
- Einfache Zahlen und Beispiele
- Geschätzte nächste Zinsen für Motivation
- Keine komplexen Finanzterminologien

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-E006-065-01 | Zinsen abrufen | 200 mit kindgerechter Übersicht |
| TC-E006-065-02 | Keine Zinsen aktiviert | Info dass Zinsen möglich sind |
| TC-E006-065-03 | Erklärung abrufen | Kindgerechte Texte |
| TC-E006-065-04 | Geschätzte nächste Zinsen | Korrektes Estimate |
| TC-E006-065-05 | In Transaktionshistorie | Zinsen als Typ erkennbar |

## Story Points

3

## Priorität

Mittel
