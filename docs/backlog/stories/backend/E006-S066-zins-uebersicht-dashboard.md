# Story E006-S066: Zins-Übersicht im Dashboard

## Epic
E006 - Zinsen

## User Story

Als **Elternteil** möchte ich **im Dashboard eine Übersicht über die Zins-Situation aller Kinder sehen**, damit **ich schnell den Status erfassen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Elternteil-Dashboard, wenn es geladen wird, dann wird eine Zins-Zusammenfassung angezeigt
- [ ] Gegeben aktivierte Zinsen, wenn sie angezeigt werden, dann sieht man den Status pro Kind
- [ ] Gegeben Zinsgutschriften im aktuellen Monat, wenn sie angezeigt werden, dann wird die Summe hervorgehoben
- [ ] Gegeben ein Kind ohne aktivierte Zinsen, wenn es angezeigt wird, dann wird ein Aktivierungs-Hinweis gezeigt
- [ ] Gegeben die Zins-Übersicht, wenn sie angeklickt wird, dann führt sie zur detaillierten Ansicht

## Hinweis

Diese Story ist als **Frontend-Story** markiert und wird im Frontend-Backlog implementiert. Die Backend-API wird in den vorherigen Stories (E006-S060 bis E006-S065) bereitgestellt.

## Dashboard-Widget Datenstruktur

```
GET /api/families/{familyId}/dashboard/interest
Authorization: Bearer {token}

Response 200:
{
  "familyId": "guid",
  "summary": {
    "childrenWithInterest": 2,
    "childrenTotal": 3,
    "totalInterestThisMonth": 1.50,
    "totalInterestThisYear": 18.00,
    "nextCalculationDate": "date"
  },
  "children": [
    {
      "childId": "guid",
      "childName": "Max",
      "interestEnabled": true,
      "interestRate": 3.0,
      "currentBalance": 150.00,
      "interestThisMonth": 0.75,
      "interestThisYear": 9.00,
      "status": "Active"
    },
    {
      "childId": "guid",
      "childName": "Lisa",
      "interestEnabled": false,
      "interestRate": null,
      "currentBalance": 50.00,
      "interestThisMonth": 0,
      "interestThisYear": 0,
      "status": "NotEnabled",
      "enableHint": "Zinsen aktivieren?"
    }
  ],
  "trend": {
    "direction": "Up",
    "percentageChange": 5.2,
    "comparedTo": "Letzter Monat"
  }
}
```

## Technische Notizen

- Leichtgewichtiger Endpunkt für Dashboard
- Caching empfohlen (5-15 Minuten)
- Nur aggregierte Daten, keine volle Historie
- Verlinkung zu Detail-Seiten
- Status: Active, Paused, NotEnabled

## Frontend-Anforderungen (zur Info)

- Kompaktes Widget-Design
- Farbcodierung: Grün (aktiv), Grau (inaktiv)
- Trend-Anzeige mit Pfeil
- Quick-Action: "Zinsen aktivieren"

## Abhängigkeiten

- E006-S060 bis E006-S065 (Backend-APIs)
- Frontend-Epic für Dashboard

## Story Points

2 (Backend-API) + Frontend separat

## Priorität

Niedrig (Frontend-Story)
