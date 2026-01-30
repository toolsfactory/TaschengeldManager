# Story M005-S06: Zinsen konfigurieren

## Epic
M005 - Kontoverwaltung Eltern

## User Story

Als **Elternteil** möchte ich **Zinsen für das Kinderkonto konfigurieren können**, damit **mein Kind spielerisch das Prinzip von Sparzinsen erlernt**.

## Akzeptanzkriterien

- [ ] Gegeben die Kontoeinstellungen, wenn ich die Zins-Option öffne, dann kann ich Zinsen aktivieren oder deaktivieren
- [ ] Gegeben aktivierte Zinsen, wenn ich den Zinssatz eingebe, dann kann ich einen Wert zwischen 0% und 20% wählen
- [ ] Gegeben die Zinseinstellungen, wenn ich den Berechnungszeitraum wähle, dann kann ich zwischen monatlich und jährlich wählen
- [ ] Gegeben konfigurierte Zinsen, wenn ich speichere, dann werden die Einstellungen für das Konto übernommen
- [ ] Gegeben ein Zins-Auszahlungstag, wenn ich ihn konfiguriere, dann kann ich den Tag im Monat wählen (1-28)
- [ ] Gegeben die Zinseinstellungen, wenn ich einen Maximal-Guthaben für Verzinsung setze, dann werden nur Beträge bis zu dieser Grenze verzinst

## UI-Entwurf

```
┌─────────────────────────────┐
│  ← Zurück   Zinseinstellungen│
├─────────────────────────────┤
│                             │
│  Konto: Emma                │
│                             │
│  Zinsen aktiv               │
│  [=========○=====] An       │
│                             │
│  Zinssatz:                  │
│  ┌───────────────────────┐  │
│  │     3,5 % pro Jahr    │  │
│  └───────────────────────┘  │
│  [----●-----------------]   │
│  0%                    20%  │
│                             │
│  Zinsberechnung:            │
│  (●) Monatlich              │
│  ( ) Jährlich               │
│                             │
│  Auszahlung am:             │
│  [1. des Monats ▼]          │
│                             │
│  Maximalbetrag verzinsen:   │
│  ┌───────────────────────┐  │
│  │ 100,00 € (0 = unbegr.)│  │
│  └───────────────────────┘  │
│                             │
│  ℹ️ Bei 45,00 € Guthaben    │
│  erhält Emma ca. 0,13 €/Mon │
│                             │
│  ┌───────────────────────┐  │
│  │     Speichern         │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Page/ViewModel

- **Page**: `InterestSettingsPage.xaml`
- **ViewModel**: `InterestSettingsViewModel.cs`
- **Model**: `InterestConfiguration.cs`

## API-Endpunkte

```
GET /api/children/{childId}/account/interest-settings
Authorization: Bearer {parent-token}

Response 200:
{
  "enabled": true,
  "ratePercent": 3.5,
  "calculationPeriod": "monthly",
  "payoutDay": 1,
  "maxBalanceForInterest": 100.00
}

PUT /api/children/{childId}/account/interest-settings
Authorization: Bearer {parent-token}
Content-Type: application/json

{
  "enabled": true,
  "ratePercent": 3.5,
  "calculationPeriod": "monthly",
  "payoutDay": 1,
  "maxBalanceForInterest": 100.00
}

Response 200:
{
  "message": "Zinseinstellungen gespeichert",
  "nextInterestDate": "2024-02-01T00:00:00Z",
  "estimatedNextInterest": 0.13
}
```

## Technische Notizen

- Zinssatz als Dezimalzahl speichern (3.5 = 3,5%)
- Zinsberechnung: Bei monatlich = Jahreszins / 12
- Zinseszins wird berücksichtigt
- Auszahlungstag max. 28 um Monatsprobleme zu vermeiden
- Zinshistorie separat speichern für Nachvollziehbarkeit

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M005-06-01 | Zinsen aktivieren | Einstellungen sichtbar |
| TC-M005-06-02 | Zinssatz 5% setzen | Wird gespeichert |
| TC-M005-06-03 | Zinssatz 25% setzen | Validierungsfehler |
| TC-M005-06-04 | Monatliche Berechnung | Korrekter Vorschauwert |
| TC-M005-06-05 | Max-Betrag setzen | Nur bis Grenze verzinst |

## Story Points

3

## Priorität

Mittel

## Status

⬜ Offen
