# Epic W003: Web Benutzer-Menü & Authentifizierung

## Status

In Arbeit

## Beschreibung

Implementierung eines funktionalen Benutzermenüs für die React Web-Anwendung mit Logout-Funktionalität, Profil-Zugang und Sicherheitseinstellungen.

## Business Value

- **Benutzerfreundlichkeit**: Schneller Zugang zu wichtigen Kontofunktionen
- **Sicherheit**: Einfaches Abmelden zum Schutz des Kontos
- **Konsistenz**: Einheitliches Verhalten über alle Plattformen (Web, Mobile)

## Betroffene Benutzerrollen

- **Alle**: Eltern, Kinder, Verwandte

## User Stories

| Story ID | Titel | Status | Story Points |
|----------|-------|--------|--------------|
| W003-S01 | Benutzer-Dropdown-Menü | FERTIG | 2 |
| W003-S02 | Logout-Funktionalität | FERTIG | 1 |

## Akzeptanzkriterien (Epic-Level)

### Funktional
- [x] Benutzer-Avatar mit Dropdown-Menü im Header
- [x] Klick-basiertes Öffnen/Schließen des Menüs
- [x] Anzeige von Benutzername und Rolle
- [x] Logout-Button mit Server-Session-Invalidierung
- [x] Navigation zu MFA-Einstellungen

### Technisch
- [x] Click-Outside schließt das Menü
- [x] Accessibility: aria-expanded, aria-haspopup, Focus-Ring
- [x] Dark Mode Unterstützung
- [x] Responsive Design

## Abhängigkeiten

- E001 (Backend Auth) - API-Endpunkte

## Priorität

**Hoch** - Basis-Funktionalität

## Geschätzter Gesamtaufwand

3 Story Points
