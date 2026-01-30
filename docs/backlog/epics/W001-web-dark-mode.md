# Epic W001: Web UI Dark Mode

## Beschreibung

Implementierung eines Dark Mode für die React Web-Anwendung, der auf allen Seiten über einen Toggle-Schalter aktiviert und deaktiviert werden kann. Die Einstellung wird persistent gespeichert und folgt optional den System-Präferenzen.

## Ziel

Benutzer sollen die Web-Anwendung je nach Präferenz im hellen oder dunklen Farbschema nutzen können, um die Augenbelastung zu reduzieren und die Benutzererfahrung zu verbessern.

## Betroffene Benutzerrollen

- Eltern
- Kinder
- Verwandte

## Technischer Kontext

- **Frontend**: React 19 mit Tailwind CSS 4
- **Styling**: Tailwind Dark Mode (`dark:` Präfix)
- **State Management**: React Context für Theme-Status
- **Persistenz**: LocalStorage
- **Layout**: Zentraler Toggle in Layout.tsx

## Status

**ABGESCHLOSSEN** - 2026-01-22

## Stories

| Story ID | Titel | Status | Story Points |
|----------|-------|--------|--------------|
| W001-S01 | Theme Context und Provider | ✅ Fertig | 2 |
| W001-S02 | Dark Mode Toggle-Komponente | ✅ Fertig | 2 |
| W001-S03 | Tailwind Dark Mode Konfiguration | ✅ Fertig | 3 |
| W001-S04 | Login- und Register-Seiten Dark Mode | ✅ Fertig | 2 |
| W001-S05 | Dashboard-Seiten Dark Mode | ✅ Fertig | 3 |
| W001-S06 | Formular- und Detail-Seiten Dark Mode | ✅ Fertig | 3 |

## Akzeptanzkriterien

- [x] Dark Mode Toggle ist auf jeder Seite sichtbar und erreichbar
- [x] Theme-Wechsel erfolgt ohne Neuladen der Seite
- [x] Theme-Einstellung bleibt nach Browser-Neustart erhalten
- [x] Alle Seiten sind in beiden Modi gut lesbar und visuell ansprechend
- [x] System-Präferenz wird beim ersten Besuch berücksichtigt

## Abhängigkeiten

- Keine Backend-Änderungen erforderlich
- Nur Frontend (React/Tailwind)

## Priorität

Mittel

## Geschätzter Gesamtaufwand

15 Story Points
