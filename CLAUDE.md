# CLAUDE.md

This file provides guidance to Claude Code when working with this repository.

## Context Management

**IMPORTANT**: Always keep `CONTEXT.md` up-to-date after making significant changes:
- Update implementation status when features are completed
- Add new entities, endpoints, or services when created
- Update "Nächste Schritte" based on current priorities
- This file serves as persistent memory for continuing work across sessions

## Project Overview

**TaschengeldManager** is a pocket money management application for families with three user roles:
- **Parents**: Full access, manage accounts, deposit allowances
- **Children**: View own account, make withdrawals
- **Relatives**: Send gifts to children

## Technology Stack

- **Framework**: .NET 10, C# 12
- **Backend**: ASP.NET Core Web API (Minimal API)
- **Web Frontend**: Blazor WebAssembly + React 19 (TypeScript, Vite, Tailwind CSS v4)
- **Mobile**: .NET MAUI Android
- **Orchestration**: Aspire 9.1
- **Database**: PostgreSQL 16+ with EF Core 10
- **Cache**: Valkey (Redis-compatible)
- **Authentication**: JWT Bearer + Argon2 + TOTP/Biometric
- **API Docs**: Scalar/OpenAPI (`/scalar/v1`)

## Project Structure

```
/src
  /TaschengeldManager.Api              # Web API (Minimal API Endpoints)
  /TaschengeldManager.Core             # Entities, DTOs, Interfaces, Enums
  /TaschengeldManager.Infrastructure   # DbContext, Repositories, Services
  /TaschengeldManager.Web              # Blazor WASM Frontend
  /TaschengeldManager.React            # React TypeScript Frontend
  /TaschengeldManager.Mobile           # .NET MAUI Android Client
  /TaschengeldManager.AppHost          # Aspire Orchestrator
  /TaschengeldManager.ServiceDefaults  # Shared service configuration
/tests
  /TaschengeldManager.Api.Tests        # Integration Tests (38 Tests)
  /TaschengeldManager.Core.Tests       # Unit Tests (Platzhalter)
  /TaschengeldManager.Infrastructure.Tests  # Unit Tests (74 Tests)
  /TaschengeldManager.E2E.Tests        # Playwright E2E Tests (13 Tests)
/docs
  /architecture                        # ADRs, technical-requirements.md
  /backlog                             # epics/, stories/, roadmap.md
/.claude/agents                        # Agent role configurations
```

## Build Commands

```bash
# Restore dependencies
dotnet restore

# Build entire solution
dotnet build

# Run with Aspire (recommended)
dotnet run --project src/TaschengeldManager.AppHost

# Run API only
dotnet run --project src/TaschengeldManager.Api

# Run tests
dotnet test
dotnet test --collect:"XPlat Code Coverage"

# Database migrations
dotnet ef migrations add MigrationName -p src/TaschengeldManager.Infrastructure -s src/TaschengeldManager.Api
dotnet ef database update -p src/TaschengeldManager.Infrastructure -s src/TaschengeldManager.Api
```

## Development Guidelines

- Use C# 12 features where appropriate
- Follow Microsoft naming conventions (PascalCase for public members, camelCase for private fields with underscore prefix)
- Use nullable reference types
- Prefer record types for DTOs
- Use dependency injection throughout
- Write unit tests for business logic
- Follow Clean Architecture: Core → Infrastructure → API

## Agent Roles

Activate agents by mentioning their role. See `/.claude/agents/` for details:
- **TPO**: Product backlog, epics, user stories
- **Architect**: ADRs, technical decisions, standards
- **Backend-Dev**: API, services, EF Core, tests
- **Mobile-Dev**: MAUI app, MVVM, offline sync
- **QA**: Test strategy, coverage goals

## Language

- Code: English (comments, variable names, documentation)
- User-facing text: German (UI labels, messages, error texts)
