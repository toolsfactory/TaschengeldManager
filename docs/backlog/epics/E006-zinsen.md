# Epic E006: Zinsen für Taschengeldkonto

## Beschreibung

Eltern können für jedes Kind optional Zinsen auf das Taschengeld-Guthaben aktivieren. Der Zinssatz ist pro Kind individuell festlegbar. Zinsen werden automatisch berechnet und gutgeschrieben.

## Business Value

Pädagogisches Feature: Kinder lernen das Konzept von Sparen und Zinsen. Motiviert zum Sparen, da das Guthaben "wächst". Eltern können spielerisch finanzielle Bildung vermitteln.

## Stories

- [x] S060 - Zinsen für Kind aktivieren/deaktivieren ✅
- [x] S061 - Zinssatz pro Kind festlegen ✅
- [x] S062 - Zinsintervall wählen (monatlich/jährlich) ✅
- [x] S063 - Zinsen automatisch berechnen und gutschreiben (Backend-Job) ✅
- [x] S064 - Zins-Historie anzeigen (Eltern) ✅
- [x] S065 - Zins-Gutschrift anzeigen (Kind) ✅
- [ ] S066 - Zins-Übersicht im Dashboard 🔜 Frontend

## Abhängigkeiten

- E001 (Benutzerverwaltung)
- E002 (Kontoverwaltung)
- E003 (Transaktionen - für Gutschrift)

## Akzeptanzkriterien (Epic-Level)

### Konfiguration
- [x] Zinsen sind **optional** und standardmäßig deaktiviert ✅
- [x] Zinsen können **pro Kind** individuell aktiviert werden ✅
- [x] Zinssatz ist **pro Kind** festlegbar (z.B. 0.5% - 10%) ✅
- [x] Zinsintervall wählbar: monatlich oder jährlich ✅
- [x] Nur **Eltern** können Zinsen konfigurieren ✅

### Berechnung
- [x] Zinsen werden auf den **aktuellen Kontostand** berechnet ✅
- [x] Zinsberechnung erfolgt zum Ende des Intervalls (Monatsende/Jahresende) ✅
- [x] Zinseszins-Effekt (Zinsen werden dem Konto gutgeschrieben und verzinsen sich mit) ✅
- [x] Zinsen werden auf 2 Dezimalstellen gerundet ✅

### Transparenz
- [x] Jede Zins-Gutschrift ist als Transaktion sichtbar (Typ: `Interest`) ✅
- [x] Kinder sehen Zins-Gutschriften in ihrer Transaktionshistorie ✅
- [x] Eltern sehen eine Übersicht aller Zins-Gutschriften ✅

## Datenmodell (Entwurf)

```
AccountSettings
├── Id
├── AccountId → Account
├── InterestEnabled (bool, default: false)
├── InterestRate (decimal, z.B. 2.5 für 2.5%)
├── InterestInterval (Monthly/Yearly)
├── LastInterestCalculation (DateTime?)
└── UpdatedAt

Transaction (Erweiterung)
├── Type: Interest (neu)
└── Metadata: { "rate": 2.5, "period": "2025-01" }
```

## Zinsberechnung

### Formel (einfache Zinsen pro Periode)
```
Zinsen = Kontostand × (Zinssatz / 100) × (1 / Perioden pro Jahr)

Beispiel monatlich:
Kontostand: 100 EUR
Zinssatz: 6% p.a.
Zinsen = 100 × 0.06 × (1/12) = 0.50 EUR
```

### Backend-Job
```
Täglich ausführen:
1. Finde alle Accounts mit InterestEnabled = true
2. Prüfe ob Intervall abgelaufen (Monatsende/Jahresende)
3. Berechne Zinsen auf aktuellen Kontostand
4. Erstelle Transaction vom Typ "Interest"
5. Aktualisiere LastInterestCalculation
```

## UI-Entwurf (Eltern - Zins-Konfiguration)

```
┌─────────────────────────────────────┐
│  ← Zurück    Zinsen für Max         │
├─────────────────────────────────────┤
│                                     │
│  Zinsen aktivieren                  │
│  ┌─────┐                            │
│  │ ON  │  ←── Toggle                │
│  └─────┘                            │
│                                     │
│  Zinssatz (% pro Jahr)              │
│  ┌─────────────────────────────┐    │
│  │  2.5                        │    │
│  └─────────────────────────────┘    │
│                                     │
│  Gutschrift-Intervall               │
│  ○ Monatlich                        │
│  ● Jährlich                         │
│                                     │
│  ─────────────────────────────────  │
│  Vorschau:                          │
│  Bei 100 EUR Guthaben:              │
│  → 2.50 EUR Zinsen pro Jahr         │
│  → 0.21 EUR Zinsen pro Monat        │
│                                     │
│  ┌─────────────────────────────┐    │
│  │        Speichern            │    │
│  └─────────────────────────────┘    │
│                                     │
└─────────────────────────────────────┘
```

## UI-Entwurf (Kind - Zins-Gutschrift)

```
┌─────────────────────────────────────┐
│  Transaktionen                      │
├─────────────────────────────────────┤
│                                     │
│  01.02.2025                         │
│  ┌─────────────────────────────┐    │
│  │ 🏦 Zinsen Januar            │    │
│  │    + 0.42 EUR               │    │
│  └─────────────────────────────┘    │
│                                     │
│  28.01.2025                         │
│  ┌─────────────────────────────┐    │
│  │ 🎁 Geschenk von Oma         │    │
│  │    + 20.00 EUR              │    │
│  └─────────────────────────────┘    │
│                                     │
└─────────────────────────────────────┘
```

## Beispiel-Szenarien

### Szenario 1: Monatliche Zinsen
- Kind "Max" hat 50 EUR Guthaben
- Eltern aktivieren Zinsen: 6% p.a., monatlich
- Ende Januar: 50 × 0.06 / 12 = **0.25 EUR** Gutschrift
- Neuer Kontostand: 50.25 EUR

### Szenario 2: Jährliche Zinsen
- Kind "Lisa" hat 200 EUR Guthaben
- Eltern aktivieren Zinsen: 3% p.a., jährlich
- Ende Dezember: 200 × 0.03 = **6.00 EUR** Gutschrift
- Neuer Kontostand: 206 EUR

## Priorität

**Mittel** - Nice-to-have, nicht MVP-kritisch

## Story Points (geschätzt)

21 (Summe aller Stories)

| Story | SP |
|-------|-----|
| S060 - Aktivieren/Deaktivieren | 2 |
| S061 - Zinssatz festlegen | 2 |
| S062 - Intervall wählen | 2 |
| S063 - Backend-Job | 8 |
| S064 - Historie (Eltern) | 3 |
| S065 - Gutschrift (Kind) | 2 |
| S066 - Dashboard | 2 |
