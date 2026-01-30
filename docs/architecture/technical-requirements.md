# Technische Anforderungen

Dieses Dokument definiert die technischen Rahmenbedingungen für den TaschengeldManager.

## Übersicht

| Bereich | Entscheidung |
|---------|--------------|
| **Hosting** | Flexibel (Cloud oder Self-Hosted) |
| **Orchestrierung** | Aspire 9.1 |
| **Datenbank** | PostgreSQL (via EF Core) |
| **Caching** | Valkey (Redis-kompatibel) |
| **Mobile Plattformen** | Android + iOS |
| **Skalierung** | 1000+ Benutzer |
| **.NET Version** | .NET 10 |
| **Web-Client** | Blazor WebAssembly (siehe ADR-001) |

---

## 1. Hosting

### Anforderung
Das System soll flexibel deploybar sein - sowohl in der Cloud als auch Self-Hosted.

### Implikationen
- Docker-Container für einfaches Deployment
- Keine harten Abhängigkeiten zu Cloud-spezifischen Services
- Konfiguration über Environment Variables
- Health-Checks für Container-Orchestrierung

---

## 2. Aspire 9.1

### Anforderung
Aspire 9.1 als Orchestrierungsframework für die Backend-Services.

### Was ist Aspire?
Aspire ist ein opinionated Stack für Cloud-native .NET Anwendungen, der folgendes bietet:
- **Orchestrierung**: Lokale Entwicklungsumgebung mit Service Discovery
- **Components**: Vorkonfigurierte Integrationen (PostgreSQL, Redis, etc.)
- **Tooling**: Dashboard für Logs, Traces, Metrics
- **Deployment**: Manifest-Generierung für Container-Orchestratoren

### Implikationen
- AppHost-Projekt für lokale Orchestrierung
- ServiceDefaults-Projekt für gemeinsame Konfiguration
- Integrierte Telemetrie (OpenTelemetry)
- Automatische Service Discovery
- Health Checks out-of-the-box
- Aspire Dashboard für Entwicklung

### Projektstruktur (Aspire)
```
/src
  /TaschengeldManager.AppHost          # Aspire Orchestrator
  /TaschengeldManager.ServiceDefaults  # Shared Service Config
  /TaschengeldManager.Api              # Web API (Backend)
  /TaschengeldManager.Web              # Blazor WebAssembly (Web-Frontend)
  /TaschengeldManager.Mobile           # .NET MAUI (Mobile App)
  /TaschengeldManager.Core             # Shared Business Logic & DTOs
  /TaschengeldManager.Infrastructure   # Data Access (EF Core)
```

### Aspire Components
| Component | Zweck |
|-----------|-------|
| `Aspire.Npgsql.EntityFrameworkCore` | PostgreSQL via EF Core |
| `Aspire.StackExchange.Redis` | Valkey Cache (Redis-kompatibel) |
| `Aspire.StackExchange.Redis.OutputCaching` | Output Caching (Valkey) |

**Hinweis**: Valkey ist vollständig API-kompatibel mit Redis. Die StackExchange.Redis-Bibliotheken funktionieren ohne Änderungen.

---

## 3. Caching (Valkey)

### Anforderung
Valkey als verteilter Cache für Performance und Skalierbarkeit.

### Was ist Valkey?
Valkey ist ein Open-Source-Fork von Redis, entstanden nach der Redis-Lizenzänderung 2024. Es ist vollständig API-kompatibel mit Redis und wird von der Linux Foundation unterstützt.

### Implikationen
- Distributed Cache für Session/Token-Daten
- Output Caching für häufige API-Responses
- Pub/Sub für Real-time Features (optional)
- Cache-Invalidierung bei Datenänderungen
- Nutzt Redis-Client-Libraries (StackExchange.Redis)

### Cache-Strategien
| Daten | Strategie | TTL |
|-------|-----------|-----|
| User Sessions | Cache-Aside | 30 min |
| Account Balance | Cache-Aside, Invalidate on Change | 5 min |
| Static Data (Kategorien) | Cache-Aside | 1 Stunde |
| API Responses | Output Caching | 1 min |

### Versionen
- Development: Valkey 8+ (via Aspire/Docker)
- Production: Valkey 8+ (self-hosted oder Cloud)

---

## 4. Datenbank (PostgreSQL + EF Core)

### Anforderung
PostgreSQL als primäre Datenbank, angebunden über Entity Framework Core.

### Warum EF Core?
- Bewährtes ORM für .NET
- Code-First Migrations
- LINQ-Support für typsichere Queries
- Aspire-Integration vorhanden
- Gute PostgreSQL-Unterstützung via Npgsql

### Implikationen
- `Aspire.Npgsql.EntityFrameworkCore` Component nutzen
- Code-First Approach mit Migrations
- PostgreSQL-spezifische Features nutzbar (JSONB, Arrays)
- Connection Pooling via Aspire/Npgsql
- Repository Pattern für Abstraktion

### EF Core Konfiguration
```csharp
// Via Aspire ServiceDefaults
builder.AddNpgsqlDbContext<AppDbContext>("taschengelddb");
```

### Versionen
- Development: PostgreSQL 16+ (via Aspire/Docker)
- Production: PostgreSQL 16+ (managed oder self-hosted)
- EF Core: 10.x (passend zu .NET 10)

---

## 5. Mobile Plattformen

### Anforderung
Unterstützung für Android und iOS.

### Implikationen
- .NET MAUI als Cross-Platform Framework
- Platform-spezifische Anpassungen wo nötig
- Push Notifications für beide Plattformen (FCM + APNs)
- Testing auf beiden Plattformen erforderlich

### Mindestversionen
- Android: API 24 (Android 7.0) oder höher
- iOS: iOS 14 oder höher

---

## 6. Skalierung

### Anforderung
System muss 1000+ Benutzer unterstützen können.

### Implikationen
- Stateless API-Design für horizontale Skalierung
- Caching-Strategie (Redis oder In-Memory)
- Async Processing für Background Jobs
- Connection Pooling und Query-Optimierung
- Rate Limiting zum Schutz der API
- Monitoring und Logging

### Performance-Ziele
| Metrik | Ziel |
|--------|------|
| API Response Time (p95) | < 200ms |
| API Response Time (p99) | < 500ms |
| Concurrent Users | 100+ |
| Uptime | 99.5% |

---

## 7. .NET Version

### Anforderung
.NET 10 (LTS - Long Term Support).

### Implikationen
- Zugriff auf neueste Sprachfeatures (C# 14)
- Native AOT als Option für Performance
- Neueste MAUI-Features
- Langzeit-Support (3 Jahre)

### Vorteile LTS
- Stabiler für Produktions-Deployments
- Längerer Support-Zeitraum
- Sicherheitsupdates garantiert

---

## 8. Web-Client

### Anforderung
API soll später auch Web-Clients unterstützen.

### Implikationen
- CORS-Konfiguration von Anfang an
- OpenAPI/Swagger Dokumentation
- Token-basierte Auth (keine Cookies)
- API-Versionierung einplanen
- Responsive DTOs (keine Mobile-spezifischen Felder)

### Potenzielle Web-Technologien
- Blazor WebAssembly (WASM)
- Blazor Server
- Beliebiges SPA-Framework (React, Vue, Angular)

---

## 9. Sicherheit

### Anforderungen (implizit)
- HTTPS überall (TLS 1.2+)
- JWT-basierte Authentifizierung
- Passwort-Hashing (Argon2 oder BCrypt)
- Input-Validierung
- SQL Injection Prevention (EF Core Parameterized Queries)
- Rate Limiting
- OWASP Top 10 beachten

---

## 10. Nicht-funktionale Anforderungen

| Bereich | Anforderung |
|---------|-------------|
| **Verfügbarkeit** | 99.5% Uptime |
| **Wartbarkeit** | Clean Architecture, Tests |
| **Testbarkeit** | >70% Code Coverage Backend |
| **Dokumentation** | OpenAPI Spec, ADRs |
| **Logging** | Structured Logging (via Aspire/OpenTelemetry) |
| **Monitoring** | Aspire Dashboard, Health Endpoints, Metrics |
| **Tracing** | Distributed Tracing (OpenTelemetry) |
