# Epic E001: Benutzerverwaltung, Authentifizierung & Sicherheit

## Beschreibung

Benutzer (Eltern, Verwandte und Kinder) können sich registrieren und sicher anmelden. **Multi-Faktor-Authentifizierung (MFA) ist verpflichtend** - entweder via TOTP (Authenticator-App) oder Passkeys. Auf mobilen Geräten wird zusätzlich Biometrie unterstützt. Das System sensibilisiert Kinder von Anfang an für das Thema IT-Sicherheit.

## Business Value

- **Sicherheit**: Schutz sensibler Finanzdaten durch MFA
- **Pädagogik**: Kinder lernen früh den Umgang mit sicherer Authentifizierung
- **Vertrauen**: Eltern können sicher sein, dass nur berechtigte Personen Zugriff haben
- **Zukunftssicher**: Passkeys als moderne, phishing-resistente Authentifizierung

## Sicherheitskonzept

### Authentifizierungs-Optionen nach Rolle

| Rolle | Primär-Auth | MFA (verpflichtend) | Mobile-Option |
|-------|-------------|---------------------|---------------|
| **Parent** | E-Mail + Passwort | TOTP oder Passkey | + Biometrie |
| **Relative** | E-Mail + Passwort | TOTP oder Passkey | + Biometrie |
| **Child** | Nickname + PIN | Vereinfachter TOTP* | + Biometrie |

*Kinder: Vereinfachter 4-stelliger TOTP-Code oder Eltern-Bestätigung als zweiter Faktor

### Warum MFA erzwingen?

1. **Schutz**: Finanzdaten sind sensibel, auch bei virtuellem Taschengeld
2. **Bildung**: Kinder lernen, dass Sicherheit wichtig ist
3. **Gewöhnung**: MFA wird zur Normalität, nicht zur Hürde
4. **Vorbild**: Eltern leben sichere Praktiken vor

## Stories

### Registrierung & Basis-Auth
- [x] S001 - Registrierung als Elternteil (E-Mail + Passwort) ✅
- [x] S002 - Login mit E-Mail + Passwort ✅
- [x] S003 - Logout (alle Geräte) ✅
- [x] S004 - Passwort zurücksetzen ✅
- [x] S005 - Profil bearbeiten ✅
- [x] S006 - Kind-Account anlegen (durch Eltern) ✅
- [x] S007 - Verwandten-Account anlegen (durch Eltern) ✅

### MFA - TOTP (Authenticator)
- [x] S030 - TOTP einrichten (QR-Code scannen) ✅
- [x] S031 - TOTP bei Login verifizieren ✅
- [x] S032 - TOTP-Gerät wechseln ✅
- [x] S033 - Backup-Codes generieren ✅
- [x] S034 - Backup-Code verwenden ✅

### MFA - Passkeys
- [ ] S035 - Passkey registrieren (WebAuthn) 🔜 Phase 2
- [ ] S036 - Login mit Passkey 🔜 Phase 2
- [ ] S037 - Passkey entfernen 🔜 Phase 2
- [ ] S038 - Mehrere Passkeys verwalten 🔜 Phase 2

### Biometrie (Mobile)
- [x] S040 - Biometrie aktivieren (Face ID / Fingerprint) ✅
- [x] S041 - Login mit Biometrie ✅
- [x] S042 - Biometrie deaktivieren ✅
- [x] S043 - Fallback auf PIN/Passwort bei Biometrie-Fehler ✅

### Kind-spezifische Sicherheit
- [x] S050 - Kind: Vereinfachte TOTP-Einrichtung (mit Eltern-Hilfe) ✅
- [x] S051 - Kind: 4-stelliger TOTP-Code (kindgerecht) ✅
- [x] S052 - Kind: Biometrie als primärer zweiter Faktor ✅
- [x] S053 - Eltern-Bestätigung als MFA-Alternative für Kinder ✅
- [ ] S054 - Sicherheits-Tutorial für Kinder (interaktiv) 🔜 Frontend

### Session & Geräteverwaltung
- [x] S060 - Aktive Sessions anzeigen ✅
- [x] S061 - Einzelne Session beenden ✅
- [x] S062 - Alle anderen Sessions beenden ✅
- [ ] S063 - Vertrauenswürdige Geräte verwalten 🔜 Phase 2

### Account-Sicherheit
- [x] S070 - Account sperren (durch Eltern) ✅
- [x] S071 - Account entsperren ✅
- [x] S072 - Login-Verlauf anzeigen ✅
- [ ] S073 - Verdächtige Login-Versuche melden 🔜 Phase 2

## Abhängigkeiten

- Keine (Basis-Epic)

## Akzeptanzkriterien (Epic-Level)

### Allgemein
- [ ] **MFA ist verpflichtend** - kein Login ohne zweiten Faktor möglich
- [ ] Mindestens eine MFA-Methode muss bei Registrierung eingerichtet werden
- [ ] JWT-basierte Authentifizierung mit Refresh Tokens
- [ ] Passwörter werden mit Argon2id gehashed
- [ ] Alle Auth-Endpunkte sind rate-limited

### Eltern & Verwandte
- [ ] Können zwischen TOTP und Passkey wählen
- [ ] Können beide Methoden parallel nutzen
- [ ] Können auf Mobile zusätzlich Biometrie aktivieren
- [ ] Haben Zugriff auf Login-Historie

### Kinder
- [ ] Erhalten kindgerechte MFA-Erklärung
- [ ] Können vereinfachten TOTP (4-stellig) nutzen
- [ ] Können Biometrie als zweiten Faktor nutzen
- [ ] Alternativ: Eltern-Bestätigung als MFA
- [ ] Sehen interaktives Sicherheits-Tutorial bei Ersteinrichtung

### Mobile
- [ ] Biometrie-Login nach einmaliger vollständiger Authentifizierung
- [ ] Biometrie-Token läuft nach 14 Tagen ab
- [ ] Bei Biometrie-Änderung am Gerät: Re-Authentifizierung nötig
- [ ] Fallback auf vollständigen Login immer möglich

## Datenmodell (Entwurf)

```
User
├── Id
├── Email (nullable für Kinder)
├── PasswordHash
├── Role (Parent/Child/Relative)
├── MfaEnabled (always true, enforced)
├── TotpSecret (encrypted)
├── TotpBackupCodes[] (hashed)
└── CreatedAt

Passkey
├── Id
├── UserId → User
├── CredentialId (WebAuthn)
├── PublicKey
├── SignCount
├── DeviceName
├── CreatedAt
└── LastUsedAt

BiometricToken
├── Id
├── UserId → User
├── DeviceId
├── TokenHash
├── ExpiresAt
├── CreatedAt
└── LastUsedAt

LoginAttempt
├── Id
├── UserId → User (nullable)
├── Email
├── Success (bool)
├── FailureReason
├── IpAddress
├── UserAgent
├── Timestamp

Session
├── Id
├── UserId → User
├── RefreshTokenHash
├── DeviceInfo
├── IpAddress
├── CreatedAt
├── LastActivityAt
├── ExpiresAt
└── IsRevoked
```

## Authentifizierungs-Flows

### Flow 1: Eltern-Login (Web/Mobile)

```
┌─────────────────────────────────────────────────────────────────┐
│                    ELTERN LOGIN FLOW                            │
└─────────────────────────────────────────────────────────────────┘

        E-Mail + Passwort
              │
              ▼
       ┌──────────────┐
       │   Validieren  │
       └──────┬───────┘
              │
              ▼
       MFA-Methode wählen
       ┌──────┴──────┐
       │             │
       ▼             ▼
   ┌────────┐   ┌────────┐
   │  TOTP  │   │Passkey │
   │  Code  │   │        │
   └────┬───┘   └────┬───┘
        │            │
        ▼            ▼
   ┌──────────────────────┐
   │   MFA verifiziert    │
   └──────────┬───────────┘
              │
              ▼
   ┌──────────────────────┐
   │   JWT + Refresh      │
   │   Token ausstellen   │
   └──────────────────────┘
```

### Flow 2: Kind-Login (Mobile mit Biometrie)

```
┌─────────────────────────────────────────────────────────────────┐
│                    KIND LOGIN FLOW (MOBILE)                     │
└─────────────────────────────────────────────────────────────────┘

                    Erst-Login
                        │
        ┌───────────────┴───────────────┐
        │                               │
        ▼                               ▼
   Family-Code                     Nickname
   + Nickname                      + PIN
   + PIN                              │
        │                             │
        ▼                             ▼
   ┌──────────────────────────────────────┐
   │         MFA (eines von):             │
   │  • 4-stelliger TOTP                  │
   │  • Eltern-Bestätigung (Push)         │
   │  • Biometrie (wenn bereits aktiv)    │
   └──────────────────┬───────────────────┘
                      │
                      ▼
   ┌──────────────────────────────────────┐
   │    Biometrie für Folge-Logins        │
   │    aktivieren? (empfohlen)           │
   └──────────────────┬───────────────────┘
                      │
                      ▼
   ┌──────────────────────────────────────┐
   │         Eingeloggt                   │
   └──────────────────────────────────────┘

                 Folge-Login
                     │
                     ▼
            ┌─────────────────┐
            │   Biometrie     │
            │  (Face/Touch)   │
            └────────┬────────┘
                     │
                     ▼
            ┌─────────────────┐
            │   Eingeloggt    │
            └─────────────────┘
```

### Flow 3: MFA-Einrichtung bei Registrierung

```
┌─────────────────────────────────────────────────────────────────┐
│              MFA-EINRICHTUNG (VERPFLICHTEND)                    │
└─────────────────────────────────────────────────────────────────┘

   Registrierung abgeschlossen
              │
              ▼
   ┌──────────────────────────────────────┐
   │  "Jetzt deinen Account sichern!"    │
   │                                      │
   │  Wähle eine Methode:                │
   │  ┌────────────┐  ┌────────────┐     │
   │  │    TOTP    │  │  Passkey   │     │
   │  │ (App-Code) │  │ (Sicher!)  │     │
   │  └────────────┘  └────────────┘     │
   └──────────────────────────────────────┘
              │
   ┌──────────┴──────────┐
   │                     │
   ▼                     ▼
TOTP Setup           Passkey Setup
   │                     │
   ▼                     ▼
┌────────────┐     ┌────────────────┐
│ QR-Code    │     │ Browser/Gerät  │
│ scannen    │     │ Passkey-Dialog │
└─────┬──────┘     └───────┬────────┘
      │                    │
      ▼                    ▼
┌────────────┐     ┌────────────────┐
│ Code       │     │ Passkey        │
│ eingeben   │     │ bestätigen     │
└─────┬──────┘     └───────┬────────┘
      │                    │
      └─────────┬──────────┘
                │
                ▼
   ┌──────────────────────────────────────┐
   │      Backup-Codes anzeigen          │
   │      (speichern empfohlen!)         │
   └──────────────────────────────────────┘
                │
                ▼
   ┌──────────────────────────────────────┐
   │      Account vollständig            │
   │      eingerichtet! 🎉               │
   └──────────────────────────────────────┘
```

## Kind-Sicherheits-Tutorial

### Interaktives Tutorial bei Ersteinrichtung

```
┌─────────────────────────────────────────┐
│  🔐 Dein Geheimcode!                   │
├─────────────────────────────────────────┤
│                                         │
│  Stell dir vor, dein Konto ist          │
│  wie eine Schatztruhe...               │
│                                         │
│       🏴‍☠️ → 🔒 → 💰                     │
│                                         │
│  Dein Passwort ist der Schlüssel.      │
│  Aber was, wenn jemand den             │
│  Schlüssel findet?                     │
│                                         │
│  Deshalb hast du einen ZWEITEN         │
│  Geheimcode - den kennt nur du!        │
│                                         │
│  ┌─────────────────────────────────┐   │
│  │      Weiter →                   │   │
│  └─────────────────────────────────┘   │
│                                         │
└─────────────────────────────────────────┘
```

## API-Endpunkte (Übersicht)

```
# Registrierung & Login
POST /api/auth/register
POST /api/auth/login
POST /api/auth/logout
POST /api/auth/refresh
POST /api/auth/child-login

# MFA - TOTP
POST /api/auth/mfa/totp/setup
POST /api/auth/mfa/totp/verify
POST /api/auth/mfa/totp/disable
POST /api/auth/mfa/backup-codes
POST /api/auth/mfa/backup-codes/verify

# MFA - Passkeys
POST /api/auth/mfa/passkey/register/begin
POST /api/auth/mfa/passkey/register/complete
POST /api/auth/mfa/passkey/authenticate/begin
POST /api/auth/mfa/passkey/authenticate/complete
DELETE /api/auth/mfa/passkey/{id}

# Biometrie
POST /api/auth/biometric/enable
POST /api/auth/biometric/verify
DELETE /api/auth/biometric

# Sessions
GET /api/auth/sessions
DELETE /api/auth/sessions/{id}
DELETE /api/auth/sessions/others

# Account
GET /api/auth/login-history
POST /api/auth/lock-child/{childId}
POST /api/auth/unlock-child/{childId}
```

## Priorität

**Hoch** - MVP-Blocker (Security ist nicht optional)

## Story Points (geschätzt)

55 (Summe aller Stories)

| Bereich | Stories | SP |
|---------|---------|-----|
| Registrierung & Basis-Auth | 7 | 13 |
| MFA - TOTP | 5 | 10 |
| MFA - Passkeys | 4 | 13 |
| Biometrie | 4 | 8 |
| Kind-spezifisch | 5 | 8 |
| Session-Management | 4 | 5 |
| Account-Sicherheit | 4 | 5 |
