# Epic M009: Geschenke (Verwandten-Rolle)

**Status:** Offen (0/12 SP)

## Beschreibung

Verwandte können über die App Geldgeschenke an Kinder senden und ihre Geschenke-Historie einsehen.

## Business Value

Erweitert den Nutzerkreis über die Kernfamilie hinaus. Oma, Opa, Onkel, Tante können direkt Geschenke senden. Vereinfacht Geldgeschenke zu Geburtstagen etc.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M009-S01 | Kind auswählen für Geschenk | 2 | Offen |
| M009-S02 | Geschenk senden (Betrag + Nachricht) | 2 | Offen |
| M009-S03 | Eigene Geschenke-Historie | 2 | Offen |
| M009-S04 | Geburtstags-Erinnerungen | 2 | Offen |
| M009-S05 | Dankeschön-Nachrichten empfangen | 1 | Offen |
| M009-S06 | Wiederkehrendes Geschenk (z.B. Geburtstag) | 3 | Offen |

**Gesamt: 12 SP**

## Abhängigkeiten

- M001 (Projekt-Setup)
- M002 (Authentifizierung)
- M003 (Navigation)
- M006 (Familienverwaltung) - Einladung als Verwandter

## Akzeptanzkriterien (Epic-Level)

- [ ] Verwandte sehen nur Kinder, zu denen sie Zugang haben
- [ ] Geschenke können mit Betrag und persönlicher Nachricht versendet werden
- [ ] Geschenke-Historie zeigt alle bisherigen Geschenke
- [ ] Kind erhält das Geschenk sofort auf seinem Konto

## UI-Entwurf

### Verwandten-Dashboard
```
┌─────────────────────────────────┐
│  TaschengeldManager      [⚙️]   │
├─────────────────────────────────┤
│                                 │
│  Hallo, Oma Maria!              │
│                                 │
│  Kinder in der Familie          │
│  ┌─────────────────────────────┐│
│  │ 👦 Max                      ││
│  │    [  🎁 Schenken  ]        ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 👧 Lisa                     ││
│  │    [  🎁 Schenken  ]        ││
│  └─────────────────────────────┘│
│                                 │
│  Meine Geschenke                │
│  ┌─────────────────────────────┐│
│  │ 🎁 Max          20,00 EUR  ││
│  │    "Zum Geburtstag!"        ││
│  │    15.01.2026               ││
│  └─────────────────────────────┘│
│  ┌─────────────────────────────┐│
│  │ 🎁 Lisa         15,00 EUR  ││
│  │    "Für gute Noten"         ││
│  │    10.01.2026               ││
│  └─────────────────────────────┘│
│                                 │
├─────────────────────────────────┤
│  [🏠]         [🎁]        [⚙️]  │
│  Home       Geschenke     Mehr  │
└─────────────────────────────────┘
```

### Geschenk senden
```
┌─────────────────────────────────┐
│  ← Zurück    Geschenk an Max    │
├─────────────────────────────────┤
│                                 │
│  🎁                             │
│                                 │
│  Wie viel möchtest du schenken? │
│  ┌───────────────────────────┐  │
│  │        20,00 €            │  │
│  └───────────────────────────┘  │
│                                 │
│  Persönliche Nachricht          │
│  ┌───────────────────────────┐  │
│  │                           │  │
│  │ Alles Gute zum Geburtstag,│  │
│  │ mein Schatz!              │  │
│  │                           │  │
│  └───────────────────────────┘  │
│                                 │
│  ┌─────────────────────────────┐│
│  │     🎁 Geschenk senden      ││
│  └─────────────────────────────┘│
│                                 │
│  Max freut sich bestimmt! 🎉    │
│                                 │
└─────────────────────────────────┘
```

## Einschränkungen für Verwandte

- Können **nur** Geld schenken
- Sehen **nicht** den Kontostand der Kinder
- Sehen **nur** eigene Geschenke
- Können **keine** Transaktionen einsehen

## Priorität

**Mittel** - Erweiterung des Nutzerkreises

## Story Points

12 SP (0 SP abgeschlossen)
