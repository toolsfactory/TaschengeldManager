# TaschengeldManager - Architekturdokumentation

> Version: 1.0
> Stand: 2026-01-19
> Architekt: Claude (AI-Assisted)

---

## 1. Übersicht

TaschengeldManager ist eine moderne Pocket-Money-Management-Anwendung bestehend aus:
- ASP.NET Core 10 Web API (Minimal APIs)
- .NET MAUI Mobile Clients (geplant)
- PostgreSQL Datenbank
- Redis/Valkey Cache

### Technologie-Stack

| Komponente | Technologie | Version |
|------------|-------------|---------|
| Runtime | .NET | 10.0 |
| Web Framework | ASP.NET Core Minimal APIs | 10.0 |
| ORM | Entity Framework Core | 10.0 |
| Datenbank | PostgreSQL | via Aspire |
| Cache | Redis/Valkey | via Aspire |
| Orchestrierung | .NET Aspire | 13.1.0 |
| Authentifizierung | JWT Bearer + MFA | Custom |
| Validierung | FluentValidation | 11.11.0 |
| Password Hashing | Argon2id | Konscious 1.3.1 |

---

## 2. Projektstruktur

```
TaschengeldManager/
├── src/
│   ├── TaschengeldManager.Api/          # Web API Layer
│   │   ├── Endpoints/                   # Minimal API Endpoints
│   │   ├── Filters/                     # Validation Filters
│   │   ├── Validators/                  # FluentValidation
│   │   ├── Extensions/                  # Helper Extensions
│   │   └── Program.cs                   # Application Entry
│   │
│   ├── TaschengeldManager.Core/         # Domain Layer
│   │   ├── Entities/                    # Domain Entities
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Enums/                       # Enumerations
│   │   ├── Interfaces/                  # Contracts
│   │   └── Configuration/               # Settings Classes
│   │
│   ├── TaschengeldManager.Infrastructure/  # Data Access Layer
│   │   ├── Data/                        # DbContext & Configurations
│   │   ├── Repositories/                # Repository Implementations
│   │   ├── Services/                    # Service Implementations
│   │   └── Utilities/                   # Helpers (TOTP, etc.)
│   │
│   ├── TaschengeldManager.ServiceDefaults/  # Aspire Defaults
│   └── TaschengeldManager.AppHost/          # Aspire Orchestrator
│
├── tests/
│   ├── TaschengeldManager.Api.Tests/
│   ├── TaschengeldManager.Core.Tests/
│   ├── TaschengeldManager.Infrastructure.Tests/
│   └── TaschengeldManager.E2E.Tests/
│
└── docs/
    ├── ARCHITECTURE.md                  # Diese Datei
    ├── requirements/                    # Anforderungen
    ├── backlog/                         # User Stories
    └── conversation/                    # Session Logs
```

---

## 3. Schichtenarchitektur

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              TaschengeldManager.Api                  │    │
│  │  ┌─────────┐ ┌──────────┐ ┌────────────┐           │    │
│  │  │Endpoints│ │Validators│ │  Filters   │           │    │
│  │  └────┬────┘ └────┬─────┘ └─────┬──────┘           │    │
│  └───────┼───────────┼─────────────┼──────────────────┘    │
└──────────┼───────────┼─────────────┼────────────────────────┘
           │           │             │
           ▼           ▼             ▼
┌─────────────────────────────────────────────────────────────┐
│                    APPLICATION LAYER                         │
│  ┌─────────────────────────────────────────────────────┐    │
│  │           TaschengeldManager.Infrastructure          │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │                 SERVICES                     │    │    │
│  │  │  AuthService, FamilyService, AccountService  │    │    │
│  │  │  MfaService, SessionService, CacheService    │    │    │
│  │  └──────────────────────┬──────────────────────┘    │    │
│  └─────────────────────────┼────────────────────────────┘    │
└────────────────────────────┼────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                      DOMAIN LAYER                            │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              TaschengeldManager.Core                 │    │
│  │  ┌──────────┐ ┌──────┐ ┌────────────┐ ┌─────────┐  │    │
│  │  │ Entities │ │ DTOs │ │ Interfaces │ │  Enums  │  │    │
│  │  └──────────┘ └──────┘ └────────────┘ └─────────┘  │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                   PERSISTENCE LAYER                          │
│  ┌─────────────────────────────────────────────────────┐    │
│  │           TaschengeldManager.Infrastructure          │    │
│  │  ┌────────────────┐  ┌─────────────────────────┐    │    │
│  │  │  Repositories  │  │   ApplicationDbContext   │    │    │
│  │  │   (IUnitOfWork)│  │    (EF Core Config)      │    │    │
│  │  └────────┬───────┘  └───────────┬─────────────┘    │    │
│  └───────────┼──────────────────────┼──────────────────┘    │
└──────────────┼──────────────────────┼───────────────────────┘
               │                      │
               ▼                      ▼
        ┌──────────────┐       ┌──────────────┐
        │  PostgreSQL  │       │ Redis/Valkey │
        └──────────────┘       └──────────────┘
```

---

## 4. Domain Model

### 4.1 Entity-Beziehungen

```
                              ┌─────────────┐
                              │    User     │
                              │─────────────│
                              │ Id          │
                              │ Email       │
                              │ Role        │
                              │ MfaEnabled  │
                              └──────┬──────┘
                                     │
         ┌───────────────────────────┼───────────────────────────┐
         │                           │                           │
         ▼                           ▼                           ▼
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│    Session      │         │    Account      │         │    Family       │
│─────────────────│         │─────────────────│         │─────────────────│
│ RefreshTokenHash│         │ Balance         │         │ FamilyCode      │
│ DeviceInfo      │         │ InterestEnabled │         │ Name            │
│ ExpiresAt       │         │ InterestRate    │         │ CreatedByUserId │
└─────────────────┘         └────────┬────────┘         └────────┬────────┘
                                     │                           │
                                     ▼                  ┌────────┴────────┐
                            ┌─────────────────┐         │                 │
                            │  Transaction    │         ▼                 ▼
                            │─────────────────│  ┌────────────┐    ┌────────────┐
                            │ Amount          │  │FamilyParent│    │FamilyChild │
                            │ Type            │  │ (M:N Join) │    │ (1:N)      │
                            │ BalanceAfter    │  └────────────┘    └────────────┘
                            └─────────────────┘
```

### 4.2 Aggregate Roots

| Aggregate | Root Entity | Enthaltene Entities |
|-----------|-------------|---------------------|
| User | `User` | Session, Passkey, BiometricToken, LoginAttempt |
| Account | `Account` | Transaction |
| Family | `Family` | FamilyParent, FamilyRelative, FamilyInvitation |
| RecurringPayment | `RecurringPayment` | - |
| MoneyRequest | `MoneyRequest` | - |

---

## 5. Design Patterns

### 5.1 Repository Pattern

```csharp
// Generic Repository Interface (Core)
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}

// Specialized Repository (Core)
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByNicknameInFamilyAsync(Guid familyId, string nickname, CancellationToken ct = default);
}
```

### 5.2 Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IFamilyRepository Families { get; }
    IAccountRepository Accounts { get; }
    ITransactionRepository Transactions { get; }
    ISessionRepository Sessions { get; }
    // ... weitere Repositories

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

### 5.3 Options Pattern

```csharp
public class AuthSettings
{
    public const string SectionName = "Auth";

    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public int MaxFailedLoginAttempts { get; set; } = 5;
    public int[] LockoutDurationsMinutes { get; set; } = [5, 15, 60, 1440];
}

// Registrierung
services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));

// Verwendung
public class AuthService(IOptions<AuthSettings> options)
{
    private readonly AuthSettings _settings = options.Value;
}
```

---

## 6. API Design

### 6.1 Endpoint-Struktur

| Gruppe | Basis-Route | Auth | Beschreibung |
|--------|-------------|------|--------------|
| Auth | `/api/auth` | Teilweise | Registrierung, Login, MFA, Logout |
| Account | `/api/account` | Ja | Konten, Transaktionen, Einzahlungen |
| Family | `/api/family` | Ja | Familien, Kinder, Einladungen |
| MoneyRequest | `/api/money-request` | Ja | Geldanträge der Kinder |
| RecurringPayment | `/api/recurring-payment` | Ja | Wiederkehrende Zahlungen |
| Session | `/api/session` | Ja | Session-Management |
| Dev | `/api/dev` | Nein (nur Dev) | Test-Endpoints |

### 6.2 Minimal API Pattern

```csharp
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/register", Register)
            .WithSummary("Register a new parent user")
            .WithValidation<RegisterRequest>()
            .RequireRateLimiting("auth")
            .Produces<RegisterResponse>(200)
            .Produces(400);

        // ... weitere Endpoints

        return app;
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);
        return Results.Ok(result);
    }
}
```

---

## 7. Sicherheitsarchitektur

### 7.1 Authentifizierung

```
┌─────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION FLOW                       │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐              │
│  │  Login   │───▶│ Password │───▶│   MFA    │              │
│  │ Request  │    │  Verify  │    │  Check   │              │
│  └──────────┘    └──────────┘    └────┬─────┘              │
│                                       │                     │
│                    ┌──────────────────┼──────────────────┐  │
│                    │                  │                  │  │
│                    ▼                  ▼                  ▼  │
│              ┌──────────┐      ┌──────────┐      ┌──────────┐
│              │   TOTP   │      │ Passkey  │      │Biometric │
│              │  Verify  │      │  Verify  │      │  Verify  │
│              └────┬─────┘      └────┬─────┘      └────┬─────┘
│                   │                 │                 │     │
│                   └─────────────────┼─────────────────┘     │
│                                     ▼                       │
│                              ┌──────────┐                   │
│                              │  Create  │                   │
│                              │ Session  │                   │
│                              └────┬─────┘                   │
│                                   │                         │
│                                   ▼                         │
│                    ┌──────────────────────────┐             │
│                    │  JWT Access Token (15m)  │             │
│                    │  Refresh Token (7d)      │             │
│                    └──────────────────────────┘             │
└─────────────────────────────────────────────────────────────┘
```

### 7.2 Sicherheitsmaßnahmen

| Maßnahme | Implementierung | Status |
|----------|-----------------|--------|
| Password Hashing | Argon2id (64MB, 3 Iterations) | ✅ |
| JWT Authentication | HS256, 15min Expiry | ✅ |
| MFA Support | TOTP, Passkeys, Biometric | ✅ |
| Rate Limiting | 5/min Auth, 100/min Standard | ✅ |
| Account Lockout | Progressive (5-15-60-1440 min) | ✅ |
| CORS | Localhost in Dev, Configured in Prod | ✅ |
| Refresh Token | Hashed in DB, Revocable | ✅ |
| Session Management | Device Tracking, Logout-All | ✅ |

---

## 8. Datenbank

### 8.1 Schema-Übersicht

```sql
-- Haupt-Entities
users (id, email, normalized_email, password_hash, nickname, role, ...)
families (id, name, family_code, created_by_user_id, ...)
accounts (id, user_id, balance, interest_enabled, interest_rate, ...)
transactions (id, account_id, amount, type, category, balance_after, ...)

-- Auth & Security
sessions (id, user_id, refresh_token_hash, device_info, ip_address, ...)
passkeys (id, user_id, credential_id, public_key, ...)
biometric_tokens (id, user_id, device_id, token_hash, ...)
totp_backup_codes (id, user_id, code_hash, is_used, ...)
login_attempts (id, user_id, identifier, success, failure_reason, ...)

-- Business Logic
recurring_payments (id, family_id, child_id, amount, frequency, ...)
money_requests (id, child_id, amount, status, parent_response, ...)

-- Junction Tables
family_parents (family_id, user_id, is_primary, joined_at)
family_relatives (family_id, user_id, relationship_description, ...)
family_invitations (id, family_id, invited_email, token_hash, ...)
```

### 8.2 Indexing-Strategie

| Tabelle | Index | Typ | Zweck |
|---------|-------|-----|-------|
| users | IX_users_NormalizedEmail | Unique, Filtered | Email-Lookup |
| accounts | IX_accounts_UserId | Unique | User-Account Relation |
| accounts | IX_accounts_InterestEnabled | Filtered | Interest Queries |
| transactions | IX_transactions_AccountId_CreatedAt | Composite | Transaction History |
| sessions | IX_sessions_RefreshTokenHash | Unique | Token Lookup |
| families | IX_families_FamilyCode | Unique | Code Lookup |

---

## 9. Caching-Strategie

### 9.1 Cache-Architektur

```
┌────────────────────────────────────────────────────────┐
│                    APPLICATION                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │                  ICacheService                    │  │
│  │  GetAsync<T> / SetAsync<T> / RemoveAsync         │  │
│  └───────────────────────┬──────────────────────────┘  │
└──────────────────────────┼─────────────────────────────┘
                           │
         ┌─────────────────┴─────────────────┐
         │                                   │
         ▼                                   ▼
┌─────────────────┐                 ┌─────────────────┐
│ RedisCacheService│                 │ NullCacheService │
│   (Production)   │                 │   (Dev/Test)     │
└────────┬────────┘                 └─────────────────┘
         │
         ▼
┌─────────────────┐
│  Redis/Valkey   │
└─────────────────┘
```

### 9.2 Cache Keys

```csharp
public static class CacheKeys
{
    // User
    public static string User(Guid id) => $"taschengeld:user:{id}";
    public static string UserByEmail(string email) => $"taschengeld:user:email:{email}";

    // Family
    public static string Family(Guid id) => $"taschengeld:family:{id}";
    public static string FamiliesForUser(Guid userId) => $"taschengeld:families:user:{userId}";
}
```

### 9.3 Cache-Invalidierung

- **Family-Änderungen**: Invalidiert Family + alle Member-Caches
- **User-Änderungen**: Invalidiert User + zugehörige Family-Caches
- **Default TTL**: 5 Minuten

---

## 10. Aspire Integration

### 10.1 AppHost-Konfiguration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Datenbank
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("taschengelddb");

// Cache
var cache = builder.AddValkey("cache");

// API
builder.AddProject<Projects.TaschengeldManager_Api>("api")
    .WithReference(postgres)
    .WithReference(cache);

builder.Build().Run();
```

### 10.2 Service Defaults

- **OpenTelemetry**: Traces, Metrics, Logs
- **Health Checks**: `/health`, `/alive`
- **HTTP Resilience**: Retry, Circuit Breaker
- **Service Discovery**: DNS-basiert

---

## 11. Deployment

### 11.1 Environments

| Environment | Datenbank | Cache | Rate Limiting | Dev Endpoints |
|-------------|-----------|-------|---------------|---------------|
| Development | PostgreSQL (Aspire) | Valkey (Aspire) | Deaktiviert | Aktiv |
| Testing | In-Memory / PostgreSQL | NullCache | Deaktiviert | Aktiv |
| Production | PostgreSQL (Managed) | Redis (Managed) | Aktiv | Deaktiviert |

### 11.2 Konfiguration (Production)

```json
{
  "Jwt": {
    "Key": "FROM_ENVIRONMENT_OR_SECRETS_MANAGER",
    "Issuer": "TaschengeldManager",
    "Audience": "TaschengeldManager"
  },
  "Cors": {
    "AllowedOrigins": ["https://your-frontend.com"]
  },
  "ConnectionStrings": {
    "taschengelddb": "FROM_ENVIRONMENT"
  }
}
```

---

## 12. Anhang

### 12.1 Glossar

| Begriff | Beschreibung |
|---------|--------------|
| Parent | Elternteil mit Vollzugriff auf Familienkonten |
| Child | Kind mit eigenem Konto und eingeschränktem Zugriff |
| Relative | Verwandter mit Geschenk-Rechten |
| Family Code | 6-stelliger Code zur Familienidentifikation |
| TOTP | Time-based One-Time Password (MFA) |
| Passkey | WebAuthn/FIDO2 Credential |

### 12.2 Referenzen

- [.NET Aspire Documentation](https://learn.microsoft.com/aspire)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [FluentValidation](https://docs.fluentvalidation.net)
- [OWASP Top 10](https://owasp.org/Top10)
