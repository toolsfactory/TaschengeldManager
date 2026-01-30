# Epic M006: Familienverwaltung

**Status:** ⬜ Offen (0/27 SP)

## Beschreibung

Eltern können Familien erstellen, Kinder hinzufügen, Verwandte einladen und Berechtigungen verwalten.

## Business Value

Familien-Management ist die Grundlage für die Multi-User-Funktionalität. Einfaches Einladen von Verwandten erweitert den Nutzerkreis.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M006-S01 | Familie erstellen | 2 | ⬜ |
| M006-S02 | Familienmitglieder-Liste | 2 | ⬜ |
| M006-S03 | Kind zur Familie hinzufügen | 3 | ⬜ |
| M006-S04 | Kind-PIN ändern | 2 | ⬜ |
| M006-S05 | Kind aus Familie entfernen | 1 | ⬜ |
| M006-S06 | Verwandten einladen (per Email) | 2 | ⬜ |
| M006-S07 | Einladungen verwalten (ausstehend/widerrufen) | 2 | ⬜ |
| M006-S08 | Einladung annehmen (Deep Link) | 3 | ⬜ |
| M006-S09 | Familien-Code anzeigen/teilen | 1 | ⬜ |
| M006-S10 | Kind-Profil bearbeiten (Name, Avatar, Geburtstag) | 2 | ⬜ |
| M006-S11 | Zweiten Elternteil hinzufügen | 2 | ⬜ |
| M006-S12 | Elternteil/Verwandten entfernen | 1 | ⬜ |
| M006-S13 | Familie löschen (mit Bestätigung) | 2 | ⬜ |
| M006-S14 | Verwandten-Berechtigungen verwalten | 2 | ⬜ |

**Gesamt: 27 SP**

## Abhängigkeiten

- M001-M003 (Basis-Setup)
- M002 (für Verwandten-Login nach Einladung)

## Akzeptanzkriterien (Epic-Level)

- [ ] Familie kann erstellt werden
- [ ] Familienmitglieder werden aufgelistet
- [ ] Kinder können hinzugefügt werden (Name, PIN, Startguthaben)
- [ ] Kind-PIN kann geändert werden
- [ ] Kinder können entfernt werden (mit Bestätigung)
- [ ] Verwandte können per Email eingeladen werden
- [ ] Einladungen können zurückgezogen werden
- [ ] Deep Link öffnet App und zeigt Einladung
- [ ] Familien-Code kann geteilt werden (Share Sheet)
- [ ] Kind-Profile können bearbeitet werden
- [ ] Zweiter Elternteil kann hinzugefügt werden
- [ ] Verwandten-Berechtigungen sind konfigurierbar

## Deep Link Schema

```
taschengeld://invite/{token}
```

## Priorität

**Hoch** - Grundlage für Multi-User

## Story Points

27 SP (0 SP abgeschlossen)
