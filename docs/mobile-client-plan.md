# Plan: TaschengeldManager Android Client (MAUI)

## Übersicht

Entwicklung eines nativen Android-Clients mit .NET MAUI für die TaschengeldManager-App. Der Blazor Web-Client wird nicht mehr benötigt - Fokus liegt vollständig auf Mobile.

**Plattform:** Android (API 24+, Android 7.0+)
**Framework:** .NET MAUI (.NET 10)
**Architektur:** MVVM mit CommunityToolkit.Mvvm
**API-Client:** Refit
**Lokale Datenbank:** SQLite

---

## Projektstruktur

```
src/TaschengeldManager.Mobile/
├── Platforms/Android/           # Android-spezifischer Code
├── Views/
│   ├── Pages/                   # XAML Seiten
│   │   ├── Auth/                # Login, Register, MFA
│   │   ├── Parent/              # Eltern-spezifische Seiten
│   │   ├── Child/               # Kind-spezifische Seiten
│   │   ├── Relative/            # Verwandten-spezifische Seiten
│   │   └── Shared/              # Gemeinsame Seiten
│   ├── Controls/                # Wiederverwendbare Controls
│   └── Templates/               # DataTemplates
├── ViewModels/
│   ├── Auth/
│   ├── Parent/
│   ├── Child/
│   ├── Relative/
│   └── Shared/
├── Services/
│   ├── Api/                     # Refit API Clients
│   ├── Auth/                    # Token Management, Biometric
│   ├── Navigation/
│   ├── Storage/                 # SecureStorage, SQLite
│   ├── Notifications/           # Push-Benachrichtigungen
│   └── Sync/                    # Offline Sync
├── Models/                      # Lokale Models
├── Converters/                  # Value Converters
├── Resources/
│   ├── Styles/
│   ├── Fonts/
│   └── Images/
├── Helpers/
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs

tests/TaschengeldManager.Mobile.Tests/
├── ViewModels/
├── Services/
└── Mocks/
```

---

## Epics & Stories

### Epic M001: Projekt-Setup & Infrastruktur ✅

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M001-S01 | MAUI-Projekt erstellen und Solution einbinden | 2 | ✅ |
| M001-S02 | NuGet-Pakete konfigurieren (MVVM, Refit, SQLite) | 1 | ✅ |
| M001-S03 | DI-Container und Service-Registrierung (MauiProgram.cs) | 2 | ✅ |
| M001-S04 | Basis-Styles und Theme erstellen (Light/Dark Mode) | 3 | ✅ |
| M001-S05 | Navigation Service mit Shell implementieren | 3 | ✅ |
| M001-S06 | API-Client mit Refit generieren | 3 | ✅ |
| M001-S07 | Lokale SQLite-Datenbank für Offline-Cache | 3 | ✅ |
| M001-S08 | Connectivity-Service für Online/Offline-Erkennung | 2 | ✅ |
| M001-S09 | Test-Projekt aufsetzen mit Mocks | 2 | ✅ |

**Gesamt: 21 SP**

---

### Epic M002: Authentifizierung 🔶

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M002-S01 | Login-Page für Eltern (Email/Passwort) | 3 | ✅ |
| M002-S02 | Registrierungs-Page für Eltern | 3 | ✅ |
| M002-S03 | Kind-Login-Page (FamilienCode + Nickname + PIN) | 3 | ✅ |
| M002-S04 | MFA/TOTP-Eingabe-Page | 2 | ✅ |
| M002-S05 | Token-Management Service (Access/Refresh) | 3 | ✅ |
| M002-S06 | Automatischer Token-Refresh im HttpClient | 2 | ✅ |
| M002-S07 | Biometrie-Aktivierung Dialog | 3 | ✅ |
| M002-S08 | Biometrie-Login (Fingerprint) | 5 | ✅ |
| M002-S09 | Logout-Funktionalität | 1 | ✅ |
| M002-S10 | Passwort-vergessen Flow | 2 | ⬜ |
| M002-S11 | Session-Verwaltung (aktive Sessions anzeigen) | 2 | ⬜ |
| M002-S12 | Verwandten-Login (per Einladungs-Link) | 3 | ⬜ |
| M002-S13 | Email-Verifizierung nach Registrierung | 2 | ⬜ |
| M002-S14 | "Angemeldet bleiben" Funktionalität | 1 | ⬜ |

**Gesamt: 35 SP** (27 SP abgeschlossen, 8 SP offen)

---

### Epic M003: Dashboard & Navigation 🔶

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M003-S01 | AppShell mit rollenbasierter Navigation | 3 | ✅ |
| M003-S02 | Eltern-Dashboard (Familienübersicht) | 5 | ✅ |
| M003-S03 | Kind-Dashboard (eigenes Konto) | 3 | ✅ |
| M003-S04 | Verwandten-Dashboard (Geschenke-Übersicht) | 2 | ✅ |
| M003-S05 | Pull-to-Refresh für alle Listen | 1 | ✅ |
| M003-S06 | Bottom Navigation Bar | 2 | ✅ |
| M003-S07 | Einstellungen-Seite | 2 | ✅ |
| M003-S08 | Onboarding-Screens für neue Benutzer | 3 | ⬜ |
| M003-S09 | Leere Zustände (Empty States) mit Call-to-Action | 2 | ⬜ |

**Gesamt: 23 SP** (18 SP abgeschlossen, 5 SP offen)

---

### Epic M004: Kontoverwaltung (Kind-Perspektive) 🔶

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M004-S01 | Kontostand-Anzeige mit Animation | 2 | ✅ |
| M004-S02 | Transaktionsliste mit Filterung | 3 | ✅ |
| M004-S03 | Ausgabe erfassen (Betrag, Kategorie, Notiz) | 3 | ✅ |
| M004-S04 | Kategorie-Auswahl mit Icons | 2 | ✅ |
| M004-S05 | Transaktionsdetail-Ansicht | 2 | ✅ |
| M004-S06 | Zins-Gutschriften anzeigen | 1 | ✅ |
| M004-S07 | Geschenke-Eingang anzeigen (von Verwandten) | 1 | ⬜ |
| M004-S08 | Dankeschön an Verwandten senden | 2 | ⬜ |

**Gesamt: 16 SP** (13 SP abgeschlossen, 3 SP offen)

---

### Epic M005: Kontoverwaltung (Eltern-Perspektive)

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M005-S01 | Alle Kinderkonten anzeigen | 2 | ⬜ |
| M005-S02 | Konto-Detail mit Transaktionshistorie | 3 | ⬜ |
| M005-S03 | Einzahlung auf Kind-Konto | 2 | ⬜ |
| M005-S04 | Ausgabe für Kind erfassen | 2 | ⬜ |
| M005-S05 | Transaktion stornieren | 2 | ⬜ |
| M005-S06 | Zinsen konfigurieren (Ein/Aus, Rate, Intervall) | 3 | ⬜ |
| M005-S07 | Zins-Vorschau Rechner | 2 | ⬜ |
| M005-S08 | Transaktions-Export (CSV/PDF) | 3 | ⬜ |
| M005-S09 | Monatsübersicht/Statistiken pro Kind | 3 | ⬜ |

**Gesamt: 22 SP**

---

### Epic M006: Familienverwaltung

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

---

### Epic M007: Geldanfragen (Kind → Eltern)

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M007-S01 | Anfrage erstellen (Betrag + Begründung) | 3 | ⬜ |
| M007-S02 | Eigene Anfragen-Liste (Kind) | 2 | ⬜ |
| M007-S03 | Anfrage zurückziehen | 1 | ⬜ |
| M007-S04 | Anfragen-Liste für Eltern | 2 | ⬜ |
| M007-S05 | Anfrage genehmigen mit optionaler Notiz | 2 | ⬜ |
| M007-S06 | Anfrage ablehnen mit Begründung | 2 | ⬜ |
| M007-S07 | Status-Badge für offene Anfragen | 1 | ⬜ |
| M007-S08 | Anfrage bearbeiten (vor Genehmigung) | 1 | ⬜ |
| M007-S09 | Schnell-Anfragen (vordefinierte Beträge) | 2 | ⬜ |

**Gesamt: 16 SP**

---

### Epic M008: Automatische Zahlungen (Taschengeld)

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M008-S01 | Wiederkehrende Zahlung erstellen | 3 | ⬜ |
| M008-S02 | Zahlungen-Übersicht | 2 | ⬜ |
| M008-S03 | Zahlung bearbeiten | 2 | ⬜ |
| M008-S04 | Zahlung pausieren/fortsetzen | 1 | ⬜ |
| M008-S05 | Zahlung löschen | 1 | ⬜ |
| M008-S06 | Nächste Ausführung anzeigen | 1 | ⬜ |
| M008-S07 | Historie der ausgeführten Zahlungen | 2 | ⬜ |
| M008-S08 | Benachrichtigung vor Ausführung | 1 | ⬜ |

**Gesamt: 13 SP**

---

### Epic M009: Geschenke (Verwandten-Rolle)

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M009-S01 | Kind auswählen für Geschenk | 2 | ⬜ |
| M009-S02 | Geschenk senden (Betrag + Nachricht) | 2 | ⬜ |
| M009-S03 | Eigene Geschenke-Historie | 2 | ⬜ |
| M009-S04 | Geburtstags-Erinnerungen | 2 | ⬜ |
| M009-S05 | Dankeschön-Nachrichten empfangen | 1 | ⬜ |
| M009-S06 | Wiederkehrendes Geschenk (z.B. Geburtstag) | 3 | ⬜ |

**Gesamt: 12 SP**

---

### Epic M010: Offline-Funktionalität

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M010-S01 | Offline-Erkennung und Banner | 2 | ⬜ |
| M010-S02 | Kontostand aus Cache laden | 2 | ⬜ |
| M010-S03 | Transaktionen lokal cachen | 3 | ⬜ |
| M010-S04 | Offline-Ausgaben in Queue speichern | 3 | ⬜ |
| M010-S05 | Sync bei Netzwerkverbindung | 5 | ⬜ |
| M010-S06 | Konfliktauflösung bei Sync | 3 | ⬜ |
| M010-S07 | "Zuletzt aktualisiert" Anzeige | 1 | ⬜ |

**Gesamt: 19 SP**

---

### Epic M011: Push-Benachrichtigungen

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M011-S01 | Firebase Cloud Messaging Setup (Android) | 3 | ⬜ |
| M011-S02 | Push-Token Registrierung beim Backend | 2 | ⬜ |
| M011-S03 | Benachrichtigung: Neue Geldanfrage (Eltern) | 1 | ⬜ |
| M011-S04 | Benachrichtigung: Anfrage genehmigt/abgelehnt (Kind) | 1 | ⬜ |
| M011-S05 | Benachrichtigung: Neues Geschenk erhalten (Kind) | 1 | ⬜ |
| M011-S06 | Benachrichtigung: Taschengeld eingegangen (Kind) | 1 | ⬜ |
| M011-S07 | Benachrichtigungs-Einstellungen (pro Typ ein/aus) | 2 | ⬜ |
| M011-S08 | In-App Benachrichtigungs-Center | 3 | ⬜ |

**Gesamt: 14 SP**

---

### Epic M012: Profil & Account-Verwaltung

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M012-S01 | Eigenes Profil anzeigen | 1 | ⬜ |
| M012-S02 | Profil bearbeiten (Name, Avatar) | 2 | ⬜ |
| M012-S03 | Passwort ändern | 2 | ⬜ |
| M012-S04 | Email ändern (mit Verifizierung) | 3 | ⬜ |
| M012-S05 | MFA aktivieren/deaktivieren | 2 | ⬜ |
| M012-S06 | Account löschen (mit Bestätigung) | 2 | ⬜ |
| M012-S07 | Datenschutz-Einstellungen | 1 | ⬜ |
| M012-S08 | DSGVO-Datenexport | 3 | ⬜ |

**Gesamt: 16 SP**

---

### Epic M013: Error Handling & User Feedback

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M013-S01 | Globale Exception-Behandlung | 2 | ⬜ |
| M013-S02 | Toast/Snackbar Service für Feedback | 2 | ⬜ |
| M013-S03 | Retry-Mechanismus bei API-Fehlern | 2 | ⬜ |
| M013-S04 | Validierungs-Feedback in Formularen | 2 | ⬜ |
| M013-S05 | Fehlermeldungen lokalisieren (Deutsch) | 1 | ⬜ |
| M013-S06 | Loading-States für alle Aktionen | 1 | ⬜ |
| M013-S07 | Skeleton-Loader für Listen | 2 | ⬜ |

**Gesamt: 12 SP**

---

### Epic M014: App-Lifecycle & Qualität

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M014-S01 | App-Version prüfen / Force-Update Dialog | 2 | ⬜ |
| M014-S02 | Deep-Link Handling (Einladungen, etc.) | 3 | ⬜ |
| M014-S03 | App-Rating Prompt (nach X Nutzungen) | 1 | ⬜ |
| M014-S04 | Crash-Reporting Integration (Sentry/AppCenter) | 2 | ⬜ |
| M014-S05 | Analytics Integration (anonymisiert) | 2 | ⬜ |
| M014-S06 | App-Icon und Splash-Screen | 1 | ⬜ |
| M014-S07 | Store-Listing vorbereiten (Screenshots, Beschreibung) | 2 | ⬜ |

**Gesamt: 13 SP**

---

## Gesamtübersicht

| Epic | Beschreibung | Story Points | Status |
|------|--------------|--------------|--------|
| M001 | Projekt-Setup & Infrastruktur | 21 | ✅ |
| M002 | Authentifizierung | 35 | 🔶 (27/35) |
| M003 | Dashboard & Navigation | 23 | 🔶 (18/23) |
| M004 | Kontoverwaltung (Kind) | 16 | 🔶 (13/16) |
| M005 | Kontoverwaltung (Eltern) | 22 | ⬜ |
| M006 | Familienverwaltung | 27 | ⬜ |
| M007 | Geldanfragen | 16 | ⬜ |
| M008 | Automatische Zahlungen | 13 | ⬜ |
| M009 | Geschenke | 12 | ⬜ |
| M010 | Offline-Funktionalität | 19 | ⬜ |
| M011 | Push-Benachrichtigungen | 14 | ⬜ |
| M012 | Profil & Account | 16 | ⬜ |
| M013 | Error Handling & Feedback | 12 | ⬜ |
| M014 | App-Lifecycle & Qualität | 13 | ⬜ |
| **Gesamt** | | **259 SP** | |

**Fortschritt:** 79 SP abgeschlossen (31%), 180 SP offen (69%)

---

## Abhängigkeiten zwischen Epics

```
M001 ──► M002 ──► M003 ──┬──► M004 (Kind-Features)
                        ├──► M005 (Eltern-Features)
                        └──► M009 (Verwandten-Features)
                              │
M006 (Familie) ◄──────────────┘
     │
     ├──► M007 (Geldanfragen)
     └──► M008 (Automatische Zahlungen)

M010 (Offline) ◄── Abhängig von M001-M005
M011 (Push) ◄── Abhängig von M002, benötigt für M007, M008
M012 (Profil) ◄── Abhängig von M002
M013 (Error Handling) ◄── Parallel zu allen Features
M014 (App-Lifecycle) ◄── Am Ende, vor Release
```

---

## Implementierungsreihenfolge

### Phase 1: Foundation ✅
1. ✅ Projekt-Setup (M001)
2. ✅ Basis-Auth (M002-S01 bis S09)
3. ✅ Navigation & Dashboards (M003-S01 bis S07)

### Phase 2: Core Features (Aktuell)
1. ✅ Kind-Kontofunktionen (M004-S01 bis S06)
2. ⬜ Eltern-Kontofunktionen (M005)
3. ⬜ Error Handling Basics (M013-S01, S02, S06)

### Phase 3: Familie & Anfragen
1. ⬜ Familienverwaltung (M006)
2. ⬜ Geldanfragen-System (M007)
3. ⬜ Push-Benachrichtigungen Setup (M011-S01, S02)

### Phase 4: Erweiterte Features
1. ⬜ Automatische Zahlungen (M008)
2. ⬜ Geschenke-System (M009)
3. ⬜ Profil & Account (M012)
4. ⬜ Verbleibende Push-Benachrichtigungen (M011-S03 bis S08)

### Phase 5: Auth-Erweiterungen
1. ⬜ Passwort-vergessen (M002-S10)
2. ⬜ Session-Verwaltung (M002-S11)
3. ⬜ Verwandten-Login (M002-S12)
4. ⬜ Email-Verifizierung (M002-S13)

### Phase 6: Offline & Polish
1. ⬜ Offline-Funktionalität (M010)
2. ⬜ Onboarding (M003-S08, S09)
3. ⬜ Verbleibende Kind-Features (M004-S07, S08)
4. ⬜ Error Handling komplett (M013)

### Phase 7: Release-Vorbereitung
1. ⬜ App-Lifecycle (M014)
2. ⬜ Performance-Optimierung
3. ⬜ UX-Feinschliff
4. ⬜ Store-Vorbereitung

---

## Rollen-Matrix

| Feature | Parent | Child | Relative |
|---------|--------|-------|----------|
| Login (Email/PW) | ✅ | - | ✅ |
| Login (Code/PIN) | - | ✅ | - |
| Biometrie | ✅ | ✅ | ✅ |
| Dashboard | ✅ | ✅ | ✅ |
| Kontostand sehen | Alle Kinder | Eigenes | - |
| Transaktionen sehen | Alle Kinder | Eigene | Eigene Geschenke |
| Ausgabe erfassen | Für Kind | Eigene | - |
| Einzahlung | ✅ | - | Als Geschenk |
| Geldanfrage erstellen | - | ✅ | - |
| Geldanfrage bearbeiten | ✅ | - | - |
| Familie verwalten | ✅ | - | - |
| Einladungen | ✅ | - | Annehmen |
| Wiederk. Zahlungen | ✅ | - | - |
| Push-Benachrichtigungen | ✅ | ✅ | ✅ |
| Profil bearbeiten | ✅ | - | ✅ |

---

## Technische Details

### NuGet Pakete
```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="CommunityToolkit.Maui" Version="9.*" />
<PackageReference Include="Refit.HttpClientFactory" Version="7.*" />
<PackageReference Include="sqlite-net-pcl" Version="1.*" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.*" />
<PackageReference Include="Microsoft.Maui.Essentials" Version="10.*" />
<PackageReference Include="Plugin.Fingerprint" Version="2.*" />
<PackageReference Include="Plugin.Firebase.CloudMessaging" Version="2.*" />
<PackageReference Include="Sentry.Maui" Version="4.*" />
```

### API-Integration
- Basis-URL aus `appsettings.json` oder Environment
- JWT Bearer Authentication mit Auto-Refresh
- Retry-Policy für transiente Fehler (Polly)
- Timeout: 30 Sekunden

### Biometrie (Android)
- BiometricPrompt API
- Fingerprint als primäre Methode
- Secure Storage für Biometric Token
- 14 Tage Token-Gültigkeit

### Push-Benachrichtigungen
- Firebase Cloud Messaging (FCM)
- Background/Foreground Handling
- Deep-Link Navigation aus Notification

---

## Akzeptanzkriterien (Definition of Done)

Jede Story gilt als abgeschlossen, wenn:
- [ ] Code implementiert und kompiliert fehlerfrei
- [ ] Unit-Tests geschrieben (wo sinnvoll)
- [ ] UI auf Deutsch lokalisiert
- [ ] Loading-States implementiert
- [ ] Fehlerbehandlung implementiert
- [ ] Offline-Verhalten berücksichtigt (wo relevant)
- [ ] Code Review durchgeführt
- [ ] Auf Emulator getestet

---

## Verifizierung

1. **Build:** `dotnet build src/TaschengeldManager.Mobile`
2. **Tests:** `dotnet test tests/TaschengeldManager.Mobile.Tests`
3. **Emulator:** Android 14 (API 34) Emulator
4. **API-Test:** Login/Logout mit laufendem Backend (Aspire)
5. **Offline-Test:** Flugmodus aktivieren, App-Verhalten prüfen

---

## Changelog

| Datum | Änderung |
|-------|----------|
| Initial | Plan erstellt mit 163 SP |
| Review 1 | +6 SP in M002 (Verwandten-Login, Email-Verifizierung, Remember-Me) |
| Review 1 | +5 SP in M003 (Onboarding, Empty States) |
| Review 1 | +3 SP in M004 (Geschenke-Anzeige, Dankeschön) |
| Review 1 | +6 SP in M005 (Export, Statistiken) |
| Review 1 | +9 SP in M006 (Profil, zweiter Elternteil, Berechtigungen) |
| Review 1 | +3 SP in M007 (Bearbeiten, Schnell-Anfragen) |
| Review 1 | +3 SP in M008 (Historie, Benachrichtigung) |
| Review 1 | +6 SP in M009 (Geburtstag, Dankeschön, Wiederkehrend) |
| Review 1 | +14 SP NEU M011 Push-Benachrichtigungen |
| Review 1 | +16 SP NEU M012 Profil & Account |
| Review 1 | +12 SP NEU M013 Error Handling |
| Review 1 | +13 SP NEU M014 App-Lifecycle |
| **Gesamt** | **259 SP** (+96 SP) |
