# TaschengeldManager

Eine Anwendung zur Verwaltung von Taschengeld für Familien.

## Tech-Stack

- **Backend**: ASP.NET Core Web API (.NET 10)
- **Web**: Blazor WebAssembly + React 19 (TypeScript)
- **Mobile**: .NET MAUI Android
- **Orchestrierung**: Aspire 9.1
- **Datenbank**: PostgreSQL 16+ / EF Core 10
- **Cache**: Valkey (Redis-kompatibel)
- **Auth**: JWT Bearer + Argon2 + TOTP/Biometrie

## Funktionen

- Taschengeld-Konten für Kinder verwalten
- Einnahmen und Ausgaben erfassen
- Regelmäßige Taschengeld-Zahlungen automatisieren
- Geldanfragen (Kind -> Eltern)
- Zinsberechnung auf Sparkonten
- Sparziele definieren und verfolgen (geplant)
- Push-Benachrichtigungen (geplant)

## Projektstruktur

```
src/
├── TaschengeldManager.Api/              # Web API (Minimal API Endpoints)
├── TaschengeldManager.Core/             # Entities, DTOs, Interfaces, Enums
├── TaschengeldManager.Infrastructure/   # DbContext, Repositories, Services
├── TaschengeldManager.Web/              # Blazor WASM Frontend
├── TaschengeldManager.React/            # React TypeScript Frontend
├── TaschengeldManager.Mobile/           # .NET MAUI Android Client
├── TaschengeldManager.AppHost/          # Aspire Orchestrator
└── TaschengeldManager.ServiceDefaults/  # Shared service configuration
tests/
├── TaschengeldManager.Api.Tests/        # Integration Tests (38 Tests)
├── TaschengeldManager.Core.Tests/       # Unit Tests
├── TaschengeldManager.Infrastructure.Tests/  # Unit Tests (74 Tests)
└── TaschengeldManager.E2E.Tests/        # Playwright E2E Tests (13 Tests)
docs/
├── architecture/                        # ADRs, technical-requirements.md
└── backlog/                             # Epics, Stories, Roadmap
```

## Voraussetzungen

- .NET 10 SDK
- Docker Desktop (für PostgreSQL und Valkey via Aspire)
- Node.js 20+ (für React Frontend)
- Visual Studio 2022 / JetBrains Rider / VS Code

## Entwicklungsumgebung starten

```bash
# Mit Aspire (empfohlen - startet API, Web, PostgreSQL, Valkey)
dotnet run --project src/TaschengeldManager.AppHost

# Aspire Dashboard: https://localhost:17071
# API: http://localhost:5041
# Scalar API Docs: http://localhost:5041/scalar/v1
```

## Build & Test

```bash
# Solution bauen
dotnet build

# Alle Tests ausführen
dotnet test

# Einzelne Test-Projekte
dotnet test tests/TaschengeldManager.Api.Tests
dotnet test tests/TaschengeldManager.Infrastructure.Tests
```

## Datenbank-Migrationen

```bash
# Neue Migration erstellen
dotnet ef migrations add <Name> -p src/TaschengeldManager.Infrastructure -s src/TaschengeldManager.Api

# Migrationen anwenden
dotnet ef database update -p src/TaschengeldManager.Infrastructure -s src/TaschengeldManager.Api
```

## Test-Accounts (Development)

Die folgenden Accounts werden automatisch beim ersten Start erstellt (DevSeeder):

### Familie Müller (Familiencode: `MUEL01`)

| Rolle | Login | Passwort/PIN | Kontostand |
|-------|-------|--------------|------------|
| Parent (primär) | max.mueller@example.com | Test1234! | - |
| Parent | anna.mueller@example.com | Test1234! | - |
| Relative (Oma) | oma.mueller@example.com | Test1234! | - |
| Kind (Tim) | Nickname: `Tim` | PIN: `1234` | 42,50 € |
| Kind (Lisa) | Nickname: `Lisa` | PIN: `5678` | 15,00 € |

### Familie Schmidt (Familiencode: `SCHM01`)

| Rolle | Login | Passwort/PIN |
|-------|-------|--------------|
| Parent | peter.schmidt@example.com | Test1234! |

### Kinder-Login

Kinder melden sich mit **Familiencode + Nickname + PIN** an:
1. Familiencode: `MUEL01`
2. Nickname: `Tim` oder `Lisa`
3. PIN: `1234` bzw. `5678`

### Datenbank zurücksetzen

```bash
# Via API (Development only)
curl -X POST http://localhost:5041/api/dev/seed?force=true
```

## API-Dokumentation

Die API-Dokumentation ist im Development-Modus unter `/scalar/v1` verfügbar:
- **Scalar UI**: http://localhost:5041/scalar/v1

## Lizenz

Noch nicht festgelegt.
