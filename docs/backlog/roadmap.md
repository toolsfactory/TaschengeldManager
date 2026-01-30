# TaschengeldManager - Product Roadmap

## Vision

Eine Familien-App zur Taschengeldverwaltung, die Kindern den Umgang mit Geld beibringt und Eltern die Verwaltung erleichtert.

## Zielgruppen

| Rolle | Beschreibung |
|-------|--------------|
| **Eltern** | Verwalten Taschengeld, genehmigen Anfragen, sehen Übersichten |
| **Verwandte** | Können Kindern Geld schenken, sehen nur eigene Überweisungen |
| **Kinder** | Sehen Kontostand, erfassen Ausgaben, stellen Anfragen |

---

## Implementierungsreihenfolge (Plattformen)

```
┌─────────────────────────────────────────────────────────────────┐
│                IMPLEMENTIERUNGSREIHENFOLGE                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│   1. API            2. Android           3. iOS                 │
│   ─────────────     ─────────────        ──────────             │
│                                                                 │
│   ┌─────────┐       ┌─────────────┐      ┌───────────┐         │
│   │   API   │  ──►  │   Android   │  ──► │    iOS    │         │
│   │ Backend │       │    MAUI     │      │   MAUI    │         │
│   └─────────┘       └─────────────┘      └───────────┘         │
│                                                                 │
│   Priorität:        Priorität:           Priorität:            │
│   ✅ FERTIG         HÖCHSTE              SPÄTER                 │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

| Reihenfolge | Plattform | Technologie | Status |
|-------------|-----------|-------------|--------|
| **1** | API (Backend) | ASP.NET Core, Aspire | ✅ Fertig |
| **2** | Android | .NET MAUI | 🔜 In Planung |
| **3** | iOS | .NET MAUI | Später |

### Begründung

1. **API zuerst**: Fundament für alle Clients ✅ Abgeschlossen
2. **Mobile-First**: Primäre Nutzung auf Smartphones erwartet
3. **Android zuerst**: Größere Marktreichweite in der Zielgruppe
4. **iOS später**: Shared Codebase mit Android, separater App Store Process

---

## Epics-Übersicht

### Backend Epics (API) ✅ ABGESCHLOSSEN

| Epic | Beschreibung | Priorität | SP | Status |
|------|--------------|-----------|-----|--------|
| E001 | Benutzerverwaltung, Auth & Sicherheit (MFA) | Hoch | 55 | ✅ API fertig |
| E002 | Familien- & Kontoverwaltung | Hoch | 34 | ✅ API fertig |
| E003 | Transaktionen (Einnahmen/Ausgaben) | Hoch | 21 | ✅ API fertig |
| E004 | Automatische Taschengeld-Zahlungen | Hoch | 21 | ✅ API fertig |
| E005 | Anfragen-System (Kinder → Eltern) | Hoch | 21 | ✅ API fertig |
| E006 | Zinsen für Taschengeldkonto | Mittel | 21 | ✅ API fertig |

### Mobile Epics (Android)

| Epic | Beschreibung | Priorität | SP | Status |
|------|--------------|-----------|-----|--------|
| M001 | Projekt-Setup & Infrastruktur | Hoch | 21 | ✅ Fertig |
| M002 | Authentifizierung | Hoch | 35 | 🔶 27/35 SP |
| M003 | Dashboard & Navigation | Hoch | 23 | 🔶 18/23 SP |
| M004 | Kontoverwaltung (Kind) | Hoch | 16 | 🔶 13/16 SP |
| M005 | Kontoverwaltung (Eltern) | Hoch | 22 | ⬜ Offen |
| M006 | Familienverwaltung | Hoch | 27 | ⬜ Offen |
| M007 | Geldanfragen | Mittel | 16 | ⬜ Offen |
| M008 | Automatische Zahlungen | Mittel | 13 | ⬜ Offen |
| M009 | Geschenke (Verwandte) | Mittel | 12 | ⬜ Offen |
| M010 | Offline-Funktionalität | Mittel | 19 | ⬜ Offen |
| M011 | Push-Benachrichtigungen | Mittel | 14 | ⬜ Offen |
| M012 | Profil & Account | Mittel | 16 | ⬜ Offen |
| M013 | Error Handling & Feedback | Hoch | 12 | ⬜ Offen |
| M014 | App-Lifecycle & Qualität | Mittel | 13 | ⬜ Offen |

**Mobile Gesamt: 259 SP** (79 SP abgeschlossen, 180 SP offen)

### Zukünftige Epics

| Epic | Beschreibung | Priorität | SP |
|------|--------------|-----------|-----|
| E007 | Benachrichtigungen (Push) | Mittel | - |
| E008 | Sparziele | Mittel | - |
| E009 | Statistiken & Auswertungen | Mittel | 34 |
| E010 | Payment-Integration (echte Transfers) | Niedrig | - |

---

## Release-Plan

### Release 1.0 - API ✅ FERTIG

```
Plattform:   API Backend
Epics:       E001, E002, E003, E004, E005, E006

Features:
✅ Registrierung & Login mit MFA (TOTP/Passkeys)
✅ Familien erstellen & verwalten
✅ Kinder & Verwandte einladen
✅ Taschengeld-Konten
✅ Transaktionen erfassen
✅ Automatische Taschengeld-Zahlungen
✅ Anfragen-System
✅ Zinsen
```

### Release 2.0 - Android MVP (In Arbeit)

```
Plattform:   Android (MAUI)
Epics:       M001, M002, M003, M004, M005, M006, M013

Features:
✅ Projekt-Setup mit MVVM
✅ Login (Eltern & Kinder)
✅ Biometrie-Login (Fingerprint)
✅ Rollenbasierte Dashboards
✅ Kontostand & Transaktionen (Kind)
✅ Ausgaben erfassen
○ Kontoverwaltung (Eltern)
○ Familienverwaltung
○ Error Handling & Feedback

Status: 79/134 SP (59%)
```

### Release 2.1 - Android Complete

```
Plattform:   Android (MAUI)
Epics:       M007, M008, M009, M010, M011, M012

Features:
○ Geldanfragen (Kind → Eltern)
○ Automatische Zahlungen verwalten
○ Geschenke-System (Verwandte)
○ Push-Benachrichtigungen
○ Profil & Account-Verwaltung
○ Offline-Funktionalität
○ Sync bei Netzwerkverbindung

Status: 0/90 SP (0%)
```

### Release 2.2 - Android Polish

```
Plattform:   Android (MAUI)
Epics:       M014, verbleibende Stories aus M002-M004

Features:
○ App-Lifecycle (Version-Check, Deep-Links)
○ Crash-Reporting & Analytics
○ Store-Vorbereitung
○ Auth-Erweiterungen (Passwort vergessen, etc.)
○ Onboarding & Empty States

Status: 0/35 SP (0%)
```

### Release 3.0 - iOS & Erweiterungen

```
Plattform:   iOS (MAUI)
Epics:       E007, E008, E009

Features:
○ iOS App (shared codebase)
○ Face ID
○ Push-Benachrichtigungen
○ Sparziele
○ Erweiterte Statistiken
```

### Future - Payment Integration

```
Epics:       E010

Features:
○ Echte Geldtransfers
○ Bank-Anbindung
```

---

## Entwicklungs-Streams

### Stream 1: Backend/API ✅ ABGESCHLOSSEN
```
Verantwortlich: Backend Developer Agent
Technologie:    ASP.NET Core, Aspire, PostgreSQL, Valkey
Status:         ✅ Alle Epics implementiert (E001-E006)
```

### Stream 2: Android Mobile 🔶 AKTIV
```
Verantwortlich: Mobile Developer Agent
Technologie:    .NET MAUI, CommunityToolkit.Mvvm, Refit, SQLite

Fortschritt: 79/259 SP (31%)

Abgeschlossen:
✅ M001: Projekt-Setup & Infrastruktur (21 SP)
✅ M002: Authentifizierung - Basis (27/35 SP)
✅ M003: Dashboard & Navigation - Basis (18/23 SP)
✅ M004: Kontoverwaltung Kind - Basis (13/16 SP)

In Arbeit / Nächste Schritte:
○ M005: Kontoverwaltung (Eltern) - 22 SP
○ M006: Familienverwaltung - 27 SP
○ M013: Error Handling - 12 SP

Später:
○ M007: Geldanfragen - 16 SP
○ M008: Automatische Zahlungen - 13 SP
○ M009: Geschenke - 12 SP
○ M010: Offline-Funktionalität - 19 SP
○ M011: Push-Benachrichtigungen - 14 SP
○ M012: Profil & Account - 16 SP
○ M014: App-Lifecycle - 13 SP
```

### Stream 3: iOS (Später)
```
Verantwortlich: Mobile Developer Agent
Technologie:    .NET MAUI (shared codebase mit Android)

Start:          Nach Android-Release
Reihenfolge:
1. iOS-spezifische Anpassungen
2. Face ID Integration
3. App Store Submission
```

---

## Metriken & Ziele

### API (Release 1.0) ✅
| Metrik | Ziel | Status |
|--------|------|--------|
| API Coverage | 100% der Endpoints | ✅ |
| Test Coverage (Backend) | > 70% | ✅ |

### Android (Release 2.0)
| Metrik | Ziel |
|--------|------|
| Feature-Parität mit API | 100% |
| Crash-free Rate | > 99% |
| App-Start Zeit | < 3 Sekunden |
| Offline-Verfügbarkeit | Basis-Funktionen |

---

## Prinzipien

1. **API-First**: Backend definiert den Contract, Clients folgen
2. **Mobile-First**: Primärer Fokus auf Android-App
3. **Shared Code**: Maximale Wiederverwendung zwischen Plattformen
4. **Einfachheit**: Kinder sollen die App intuitiv bedienen können
5. **Datenschutz**: Sensible Finanzdaten müssen geschützt sein
6. **Security by Default**: MFA und Biometrie von Anfang an
7. **Offline-First**: Grundfunktionen auch ohne Internet
