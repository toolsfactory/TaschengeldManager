# TaschengeldManager - Kontextspeicher

## Projekt-Kurzübersicht
Taschengeld-Management-App für Familien mit drei Benutzerrollen:
- **Eltern**: Vollzugriff, Konten verwalten, Taschengeld einzahlen
- **Kinder**: Eigenes Konto, Ausgaben tätigen
- **Verwandte**: Geldgeschenke an Kinder

## Tech-Stack
| Komponente | Technologie |
|------------|-------------|
| Framework | .NET 10, C# 12 |
| Backend | ASP.NET Core Web API |
| Web-Frontend (Blazor) | Blazor WebAssembly + Tailwind CSS |
| Web-Frontend (React) | React 19 + TypeScript + Vite + Tailwind CSS v4 |
| Mobile | .NET MAUI (geplant, niedrige Prio) |
| Orchestrierung | Aspire 9.1 |
| Datenbank | PostgreSQL 16+ / EF Core 10 |
| Cache | Valkey (Redis-kompatibel) |
| Auth | JWT Bearer + Argon2 + TOTP/Biometrie |
| API-Doku | Scalar/OpenAPI (unter `/scalar/v1`) |

## Projektstruktur
```
src/
├── TaschengeldManager.Api/          # Web API (Controllers, Program.cs)
├── TaschengeldManager.Core/         # Entities, DTOs, Interfaces, Enums
├── TaschengeldManager.Infrastructure/ # DbContext, Repositories, Services
├── TaschengeldManager.Web/          # Blazor WASM Frontend
├── TaschengeldManager.React/        # React TypeScript Frontend (NEU)
├── TaschengeldManager.AppHost/      # Aspire Orchestrator
└── TaschengeldManager.ServiceDefaults/
tests/
├── TaschengeldManager.Api.Tests/    # Integration Tests (38 Tests)
├── TaschengeldManager.Core.Tests/   # (Platzhalter)
├── TaschengeldManager.Infrastructure.Tests/  # 74 Unit Tests
└── TaschengeldManager.E2E.Tests/    # 13 Playwright E2E Tests
docs/
├── architecture/                    # ADRs, technical-requirements.md
└── backlog/                         # epics/, stories/, roadmap.md
.claude/agents/                      # Agenten-Konfigurationen
```

## Agenten-Rollen
| Agent | Datei | Verantwortung |
|-------|-------|---------------|
| **TPO** | `.claude/agents/tpo.md` | Epics, Stories, Backlog, Priorisierung |
| **Architect** | `.claude/agents/architect.md` | ADRs, Tech-Entscheidungen, Standards |
| **Backend-Dev** | `.claude/agents/backend-developer.md` | API, Services, EF Core, Tests |
| **Mobile-Dev** | `.claude/agents/mobile-developer.md` | MAUI App, MVVM, Offline |
| **QA** | `.claude/agents/qa.md` | Teststrategie, Coverage (>80% Core) |

## Kern-Entitäten
Alle Entities verwenden **Strongly Typed IDs** (`UserId`, `FamilyId`, `AccountId`, etc.):
- **User**: `UserId`, Email, PasswordHash, Role, MFA-Settings, FamilyId
- **Family**: `FamilyId`, Name, FamilyCode (6-stellig für Kinder-Login)
- **Account**: `AccountId`, Balance, InterestRate, InterestInterval, InterestEnabled, UserId (Kind)
- **Transaction**: `TransactionId`, Amount, Type (Deposit/Withdrawal/Gift/Interest/Allowance/Correction), AccountId
- **RecurringPayment**: `RecurringPaymentId`, AccountId, Amount, Interval (Weekly/Biweekly/Monthly), DayOfWeek/Month, IsActive
- **MoneyRequest**: `MoneyRequestId`, ChildUserId, AccountId, Amount, Status (Pending/Approved/Rejected/Withdrawn), RequestReason
- Weitere IDs: `SessionId`, `PasskeyId`, `BiometricTokenId`, `TotpBackupCodeId`, `LoginAttemptId`, `FamilyInvitationId`, `ParentApprovalRequestId`

## API-Endpunkte (Hauptgruppen)
- `POST /api/auth/*` - Register, Login (Parent/Child/Biometric), MFA, Refresh, Logout
- `GET/POST /api/accounts/*` - Konten, Einzahlungen, Abhebungen, Geschenke, Zinsen
- `GET/POST /api/families/*` - Familie erstellen, Einladungen, Mitglieder verwalten
- `GET/DELETE /api/session/*` - Sessions verwalten
- `GET/POST /api/recurring-payments/*` - Automatische Zahlungen verwalten (E004)
- `GET/POST /api/money-requests/*` - Geld-Anfragen (Kind→Eltern) (E005)
- **Scalar API Reference**: `/scalar/v1` (nur Development)

## Blazor Web UI - Seiten
| Route | Seite | Status | Beschreibung |
|-------|-------|--------|--------------|
| `/login` | Login | ✅ | Parent/Child Login, MFA-Flow |
| `/register` | Register | ✅ | Neue Registrierung |
| `/dashboard` | Dashboard | ✅ | Rollenbasiert (Child/Parent/Relative) |
| `/settings/mfa` | MfaSetup | ✅ | TOTP + Backup Codes |
| `/family/create` | FamilyCreate | ✅ | Familie erstellen |
| `/accounts` | Accounts | ✅ | Kontenübersicht (Eltern) |
| `/accounts/{id}` | AccountDetails | ✅ | Kontodetails + Einzahlung + Zinsen |
| `/accounts/{id}/history` | TransactionHistory | ✅ | Transaktionshistorie mit Filter |
| `/account/history` | TransactionHistory | ✅ | Eigene Transaktionen (Kinder) |
| `/family` | FamilyManage | ✅ | Familienverwaltung |
| `/family/children/add` | AddChild | ✅ | Kind hinzufügen |
| `/family/invite` | InviteMember | ✅ | Mitglied einladen |
| `/recurring-payments` | RecurringPayments | ✅ | Automatische Zahlungen (Eltern) |
| `/money-requests` | MoneyRequests | ✅ | Geld-Anfragen verwalten (Eltern) |
| `/my-requests` | MyRequests | ✅ | Eigene Anfragen (Kinder) |

## React Web UI - Seiten
| Route | Komponente | Status | Beschreibung |
|-------|------------|--------|--------------|
| `/login` | Login | ✅ | Parent/Child Login, MFA-Flow |
| `/register` | Register | ✅ | Neue Registrierung |
| `/dashboard` | Dashboard | ✅ | Rollenbasiert (Child/Parent/Relative) |
| `/settings/mfa` | MfaSetup | ✅ | TOTP + Backup Codes |
| `/family/create` | FamilyCreate | ✅ | Familie erstellen |
| `/accounts` | Accounts | ✅ | Kontenübersicht (Eltern) |
| `/accounts/:id` | AccountDetails | ✅ | Kontodetails + Einzahlung + Zinsen |
| `/accounts/:id/history` | TransactionHistory | ✅ | Transaktionshistorie mit Filter |
| `/account/history` | TransactionHistory | ✅ | Eigene Transaktionen (Kinder) |
| `/family` | FamilyManage | ✅ | Familienverwaltung |
| `/family/children/add` | AddChild | ✅ | Kind hinzufügen |
| `/family/invite` | InviteMember | ✅ | Mitglied einladen |
| `/recurring-payments` | RecurringPayments | ✅ | Automatische Zahlungen (Eltern) |
| `/money-requests` | MoneyRequests | ✅ | Geld-Anfragen verwalten (Eltern) |
| `/my-requests` | MyRequests | ✅ | Eigene Anfragen (Kinder) |

**React Tech-Stack:**
- React 19 + TypeScript
- Vite 7.x (Build Tool)
- Tailwind CSS v4
- React Router DOM (Client-side Routing)
- TanStack React Query (Server State)
- Axios (HTTP Client mit JWT Interceptors)

## Tests
| Projekt | Anzahl | Beschreibung |
|---------|--------|--------------|
| Infrastructure.Tests | 74 | AccountService, PasswordHasher, FamilyService, AuthService |
| E2E.Tests | 13 | Playwright E2E Tests (Login, Navigation, FormValidation) |
| Api.Tests | 38 | Integration Tests (AuthController, AccountController, FamilyController) |

**Test-Befehle:**
```bash
dotnet test tests/TaschengeldManager.Infrastructure.Tests
dotnet test tests/TaschengeldManager.E2E.Tests
dotnet test tests/TaschengeldManager.Api.Tests
dotnet test --collect:"XPlat Code Coverage"
```

## Implementierungsstatus
### ✅ Fertig
- Projektstruktur mit 7 Projekten (inkl. React Frontend)
- Alle Domain-Entities und DTOs
- EF Core DbContext + Initial Migration
- Repository + Unit of Work Pattern
- Services: Auth, Account, Family, MFA, Token, Session, RecurringPayment, MoneyRequest, Interest
- API-Controller (Auth, Account, Family, Session, Dev, RecurringPayment, MoneyRequest)
- **Scalar/OpenAPI** Dokumentation (`/scalar/v1`) - Swashbuckle ersetzt
- **Blazor WASM komplett** (15 Seiten implementiert) - Build erfolgreich
- API-Clients (Auth, Account, Family, RecurringPayment, MoneyRequest)
- **Unit Tests** (74 Tests für AccountService, PasswordHasher, FamilyService, AuthService)
- **E2E Tests** (13 Playwright Tests für Login, Navigation, FormValidation)
- **Integration Tests** (38 Tests für Auth, Account, Family Controller) - ✅ Alle Tests bestanden
- Dokumentation (Epics E001-E009, 54+ Stories)
- **E004: Automatische Taschengeld-Zahlungen** (RecurringPaymentService + BackgroundService)
- **E005: Genehmigungs-System** (MoneyRequestService: Kind→Eltern Anfragen)
- **E006: Zinsberechnung** (InterestService + BackgroundService, monatlich/jährlich)
- **EF Migration**: `AddRecurringPaymentsMoneyRequestsInterest` (erstellt, anwenden mit Aspire)
- **UI: RecurringPayments** (`/recurring-payments`) - Eltern können automatische Zahlungen verwalten
- **UI: MoneyRequests** (`/money-requests`, `/my-requests`) - Geld-Anfragen für Eltern/Kinder
- **Web Build-Fehler behoben** (Authorize-Attribut, IdentityModel, MfaSetup @code)
- **React Frontend komplett** (14 Seiten implementiert) - in Aspire integriert
- **Strongly Typed IDs** (StronglyTypedId v1.0.0-beta08) - 13 ID-Typen für alle Entities, manuelle EF Core ValueConverter

### ⏳ Offen
- Mobile App (MAUI) - niedrige Priorität
- Push-Benachrichtigungen (E007)
- Sparziele (E008)
- Statistiken/Charts (E009)

## Wichtige Befehle
```bash
# Build & Run (mit Aspire - startet API, Blazor, React, PostgreSQL, Valkey)
dotnet build
dotnet run --project src/TaschengeldManager.AppHost

# React Frontend separat starten (ohne Aspire)
cd src/TaschengeldManager.React && npm run dev

# Migration
dotnet ef migrations add NAME -p src/TaschengeldManager.Infrastructure -s src/TaschengeldManager.Api

# Tests
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

## Architektur-Muster
- **Clean Architecture**: Core → Infrastructure → API
- **Repository + Unit of Work**: Abstraktion von EF Core
- **JWT Bearer Auth**: Access Token (15min) + Refresh Token (7d)
- **Async/Await**: Durchgängig mit CancellationToken

## Nächste Schritte (Empfohlen)
1. Aspire starten und EF Migration anwenden:
   ```bash
   dotnet run --project src/TaschengeldManager.AppHost
   # In separatem Terminal:
   dotnet ef database update -p src/TaschengeldManager.Infrastructure -s src/TaschengeldManager.Api
   ```
2. Features aus Backlog implementieren (E007-E009)
