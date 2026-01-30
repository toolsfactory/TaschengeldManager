# Epic M010: Offline-Funktionalität

**Status:** Offen (0/19 SP)

## Beschreibung

Die App funktioniert auch ohne Internetverbindung. Grundfunktionen wie Kontostand anzeigen und Ausgaben erfassen sind offline verfügbar. Synchronisation erfolgt automatisch bei Netzwerkverbindung.

## Business Value

Kinder nutzen die App oft unterwegs ohne stabiles Internet. Offline-Funktionalität stellt sicher, dass die App jederzeit nutzbar ist.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M010-S01 | Offline-Erkennung und Banner | 2 | Offen |
| M010-S02 | Kontostand aus Cache laden | 2 | Offen |
| M010-S03 | Transaktionen lokal cachen | 3 | Offen |
| M010-S04 | Offline-Ausgaben in Queue speichern | 3 | Offen |
| M010-S05 | Sync bei Netzwerkverbindung | 5 | Offen |
| M010-S06 | Konfliktauflösung bei Sync | 3 | Offen |
| M010-S07 | "Zuletzt aktualisiert" Anzeige | 1 | Offen |

**Gesamt: 19 SP**

## Abhängigkeiten

- M001 (Projekt-Setup) - SQLite
- M002 (Authentifizierung)
- M004 (Kind-Konto)

## Akzeptanzkriterien (Epic-Level)

- [ ] App erkennt Online/Offline-Status automatisch
- [ ] Offline-Banner wird angezeigt wenn keine Verbindung
- [ ] Kontostand wird aus lokalem Cache geladen
- [ ] Letzte Transaktionen werden gecacht
- [ ] Ausgaben können offline erfasst werden
- [ ] Offline-Ausgaben werden in Queue gespeichert
- [ ] Bei Netzwerkverbindung wird automatisch synchronisiert
- [ ] Konflikte werden sinnvoll aufgelöst
- [ ] "Zuletzt aktualisiert" zeigt Aktualität der Daten

## Architektur

### Offline-First Pattern
```
┌─────────────────────────────────────────────┐
│                    UI                        │
└─────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────┐
│              Service Layer                   │
│  ┌─────────────────────────────────────────┐│
│  │  if (online) {                          ││
│  │    data = await api.GetData();          ││
│  │    await localDb.Save(data);            ││
│  │  } else {                               ││
│  │    data = await localDb.GetData();      ││
│  │  }                                      ││
│  └─────────────────────────────────────────┘│
└─────────────────────────────────────────────┘
         │                    │
         ▼                    ▼
┌─────────────────┐  ┌─────────────────┐
│   API (Remote)   │  │ SQLite (Local)  │
└─────────────────┘  └─────────────────┘
```

### Sync Queue
```
┌─────────────────────────────────────────────┐
│               Offline Action                 │
│  ┌─────────────────────────────────────────┐│
│  │  {                                      ││
│  │    id: guid,                            ││
│  │    type: "Withdrawal",                  ││
│  │    payload: { amount, category, note }, ││
│  │    createdAt: timestamp,                ││
│  │    status: "pending"                    ││
│  │  }                                      ││
│  └─────────────────────────────────────────┘│
└─────────────────────────────────────────────┘
```

## UI-Entwurf

### Offline-Banner
```
┌─────────────────────────────────┐
│ ⚠️ Offline - Daten evtl. veraltet│
├─────────────────────────────────┤
│                                 │
│  Kontostand (Stand: vor 2h)     │
│  ┌──────────────┐               │
│  │  150,00 €    │               │
│  └──────────────┘               │
│                                 │
│  [  + Ausgabe erfassen  ]       │
│                                 │
│  Letzte Transaktionen           │
│  (gecacht)                      │
│  ...                            │
│                                 │
└─────────────────────────────────┘
```

### Sync-Indikator
```
┌─────────────────────────────────┐
│ 🔄 Synchronisiere... (2/5)      │
├─────────────────────────────────┤
│  ...                            │
└─────────────────────────────────┘
```

## Konfliktauflösung

| Konflikt | Lösung |
|----------|--------|
| Gleiche Transaktion doppelt | Server-Version bevorzugen |
| Kontostand unterschiedlich | Server-Version übernehmen |
| Offline-Ausgabe > aktueller Kontostand | Warnung anzeigen, Ausgabe speichern |

## Gecachte Daten

| Daten | Cache-Dauer | Update-Trigger |
|-------|-------------|----------------|
| Kontostand | 24h | Bei jedem App-Start |
| Transaktionen (letzte 30) | 24h | Bei jedem App-Start |
| Familienmitglieder | 7 Tage | Bei Familienänderung |
| Kategorien | Unbegrenzt | Nie (statisch) |

## Priorität

**Mittel** - Wichtig für mobile Nutzung

## Story Points

19 SP (0 SP abgeschlossen)
