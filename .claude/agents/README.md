# Multi-Agent Setup - TaschengeldManager

Dieses Dokument beschreibt das Multi-Agenten-System für die Entwicklung des TaschengeldManager-Projekts.

## Agenten-Übersicht

```
                            ┌─────────────┐
                            │    User     │
                            └──────┬──────┘
                                   │
                                   ▼
                            ┌─────────────┐
                            │     TPO     │
                            │  (Product)  │
                            └──────┬──────┘
                                   │
                    ┌──────────────┼──────────────┐
                    │              │              │
                    ▼              ▼              ▼
             ┌─────────────┐ ┌─────────┐ ┌─────────────┐
             │  Architekt  │ │   QA    │ │ Entwickler  │
             │ (Technik)   │ │(Quality)│ │  (Impl.)    │
             └─────────────┘ └─────────┘ └──────┬──────┘
                                                │
                                    ┌───────────┴───────────┐
                                    │                       │
                                    ▼                       ▼
                             ┌─────────────┐         ┌─────────────┐
                             │  Backend    │         │   Mobile    │
                             │    Dev      │         │    Dev      │
                             └─────────────┘         └─────────────┘
```

## Agenten

| Agent | Datei | Hauptaufgabe |
|-------|-------|--------------|
| **TPO** | [tpo.md](./tpo.md) | Anforderungen → Epics & Stories |
| **Architekt** | [architect.md](./architect.md) | Technische Vision & Koordination |
| **Backend Dev** | [backend-developer.md](./backend-developer.md) | ASP.NET Core API Entwicklung |
| **Mobile Dev** | [mobile-developer.md](./mobile-developer.md) | .NET MAUI App Entwicklung |
| **QA** | [qa.md](./qa.md) | Testplanung & Qualitätssicherung |

## Typische Workflows

### 1. Neue Feature-Anforderung

```
User → TPO → Architekt → Backend Dev + Mobile Dev → QA
```

1. **User** beschreibt Anforderung
2. **TPO** erstellt Epic und Stories mit Akzeptanzkriterien
3. **Architekt** prüft technische Machbarkeit, definiert Schnittstellen
4. **Backend Dev** implementiert API
5. **Mobile Dev** implementiert UI (parallel oder sequentiell)
6. **QA** testet und validiert

### 2. Bug-Fix

```
User/QA → Architekt/Dev → QA
```

1. **User** oder **QA** meldet Bug
2. **Architekt** analysiert Ursache (bei komplexen Issues)
3. **Backend Dev** oder **Mobile Dev** behebt Bug
4. **QA** validiert Fix

### 3. Architektur-Entscheidung

```
TPO/Dev → Architekt → Team-Review → Dokumentation
```

1. **TPO** oder **Dev** identifiziert Entscheidungsbedarf
2. **Architekt** analysiert Optionen, erstellt ADR-Entwurf
3. Relevante Agenten reviewen
4. **Architekt** dokumentiert finale Entscheidung

## Kommunikations-Matrix

|  | TPO | Architekt | Backend | Mobile | QA |
|--|-----|-----------|---------|--------|-----|
| **TPO** | - | Machbarkeit | API-Specs | UX-Details | Testkriterien |
| **Architekt** | Impact | - | Patterns | Patterns | Testbarkeit |
| **Backend** | Anforderungen | Review | - | Contracts | Bugs |
| **Mobile** | UX-Fragen | Review | API-Fragen | - | Bugs |
| **QA** | Akzeptanz | NFRs | Test-Support | Test-Support | - |

## Agent-Aktivierung

Um einen spezifischen Agenten zu aktivieren, verwende den Kontext aus der jeweiligen Konfigurationsdatei:

```
Aktiviere den [Agent-Name] Agent und arbeite im Kontext von .claude/agents/[agent-file].md
```

### Beispiele

```
Aktiviere den TPO Agent - ich möchte eine neue Funktion beschreiben.
```

```
Aktiviere den Backend Developer Agent - implementiere den Account-Endpoint.
```

```
Aktiviere den QA Agent - erstelle Testfälle für die Login-Funktion.
```

## Artefakte und Pfade

| Bereich | Pfad | Verantwortlich |
|---------|------|----------------|
| Backlog | `/docs/backlog/` | TPO |
| Architektur | `/docs/architecture/` | Architekt |
| API-Specs | `/docs/api/` | Architekt, Backend Dev |
| Testpläne | `/docs/testing/` | QA |
| Backend-Code | `/src/TaschengeldManager.Api/` | Backend Dev |
| Core-Code | `/src/TaschengeldManager.Core/` | Backend Dev, Mobile Dev |
| Mobile-Code | `/src/TaschengeldManager.Mobile/` | Mobile Dev |
| Tests | `/tests/` | Alle Devs, QA |

## Best Practices

### Für effektive Agent-Nutzung

1. **Kontext bereitstellen**: Gib dem Agenten relevante Hintergrundinformationen
2. **Klare Aufgaben**: Definiere eindeutig, was der Agent tun soll
3. **Iterativ arbeiten**: Lass den Agenten in kleinen Schritten arbeiten
4. **Feedback geben**: Korrigiere und lenke bei Bedarf

### Für Team-Koordination

1. **TPO zuerst**: Neue Features immer erst durch TPO spezifizieren lassen
2. **Architekt einbeziehen**: Bei technischen Unklarheiten oder größeren Änderungen
3. **QA früh einbinden**: Testkriterien sollten vor der Implementierung klar sein
4. **Dokumentation pflegen**: Entscheidungen und Änderungen dokumentieren
