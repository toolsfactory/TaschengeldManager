# Epic M002: Authentifizierung

**Status:** 🔶 Teilweise abgeschlossen (27/35 SP)

## Beschreibung

Vollständige Authentifizierungslösung für alle Benutzerrollen (Eltern, Kinder, Verwandte) mit MFA, Biometrie und Token-Management.

## Business Value

Sicherheit der Benutzerdaten und Zugriffskontrolle. Benutzerfreundlicher Login für alle Altersgruppen.

## Stories

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

## Abhängigkeiten

- M001 (Projekt-Setup)

## Akzeptanzkriterien (Epic-Level)

- [x] Eltern können sich registrieren und einloggen
- [x] Kinder können sich mit Familiencode + PIN einloggen
- [x] MFA/TOTP wird unterstützt
- [x] Biometrie-Login funktioniert
- [x] Token werden automatisch refreshed
- [x] Logout löscht alle lokalen Daten
- [ ] Passwort kann zurückgesetzt werden
- [ ] Sessions können verwaltet werden
- [ ] Verwandte können via Einladung beitreten

## Implementierte Services

- `IAuthenticationService` - Login, Logout, Token-Management
- `IBiometricService` - Fingerprint/Face Authentication
- `ISecureStorageService` - Sichere Token-Speicherung
- `AuthenticatedHttpClientHandler` - Auto Token-Refresh

## Implementierte Pages

- `LoginPage` - Eltern-Login mit Email/Passwort
- `RegisterPage` - Eltern-Registrierung
- `ChildLoginPage` - Kind-Login mit Code/PIN
- `MfaVerifyPage` - TOTP-Eingabe
- `BiometricSetupPage` - Biometrie aktivieren

## Technische Details

### Token Storage
- Access Token: Memory (15 Min)
- Refresh Token: SecureStorage (7 Tage)
- Biometric Token: SecureStorage (14 Tage)

### Biometrie
- Plugin.Fingerprint (Version 2.*)
- Android BiometricPrompt API
- Fallback zu Passwort/PIN

## Priorität

**Hoch** - Blockiert alle authentifizierten Features

## Story Points

35 SP (27 SP abgeschlossen, 8 SP offen)
