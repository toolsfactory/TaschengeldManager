# ADR-002: FamilyService Refactoring

## Status

**Proposed**

## Kontext

Der `FamilyService` in `TaschengeldManager.Infrastructure` ist mit **795 LOC** der größte Service im Projekt und verletzt das Single Responsibility Principle (SRP).

### Aktuelle Verantwortlichkeiten

| Bereich | Methoden | LOC (geschätzt) |
|---------|----------|-----------------|
| Family CRUD | CreateFamily, GetFamily, GetFamilyMembers, LeaveFamily | ~120 |
| Einladungen | CreateInvitation, GetPendingInvitations, AcceptInvitation, RejectInvitation, RevokeInvitation | ~200 |
| Kinderverwaltung | AddChild, GetChildren, UpdateChild, RemoveChild, ChangeChildPin | ~250 |
| Verwandte | AddRelative, GetRelatives, RemoveRelative | ~100 |
| Hilfsmethoden | GenerateFamilyCode, ValidateParent, Cache-Management | ~125 |

### Probleme

1. **SRP-Verletzung**: Ein Service, 5+ verschiedene Verantwortlichkeiten
2. **Schwer testbar**: 795 LOC mit vielen Abhängigkeiten
3. **Hohe Kopplung**: Änderungen in einem Bereich können andere beeinflussen
4. **Schwer erweiterbar**: Neue Features erhöhen die Komplexität weiter
5. **Code-Review**: Große Dateien erschweren Reviews

### Metriken-Vergleich

| Service | LOC | Abhängigkeiten | Verantwortlichkeiten |
|---------|-----|----------------|---------------------|
| FamilyService | 795 | 7 | 5+ |
| AuthService | 463 | 5 | 2-3 |
| AccountService | 424 | 5 | 2-3 |

## Entscheidung

Wir teilen den `FamilyService` in **vier spezialisierte Services** auf:

### 1. FamilyService (Core)
Grundlegende Familienverwaltung.

```csharp
public interface IFamilyService
{
    Task<FamilyDto> CreateFamilyAsync(UserId userId, CreateFamilyRequest request, CancellationToken ct);
    Task<FamilyDto?> GetFamilyAsync(UserId userId, CancellationToken ct);
    Task<FamilyDto?> GetFamilyByIdAsync(UserId userId, FamilyId familyId, CancellationToken ct);
    Task<IEnumerable<FamilyMemberDto>> GetFamilyMembersAsync(UserId userId, FamilyId familyId, CancellationToken ct);
    Task LeaveFamilyAsync(UserId userId, CancellationToken ct);
}
```

### 2. FamilyInvitationService (NEU)
Einladungsmanagement.

```csharp
public interface IFamilyInvitationService
{
    Task<InvitationDto> CreateInvitationAsync(UserId userId, FamilyId familyId, InviteRequest request, CancellationToken ct);
    Task<IEnumerable<InvitationDto>> GetPendingInvitationsAsync(UserId userId, CancellationToken ct);
    Task<IEnumerable<InvitationDto>> GetFamilyInvitationsAsync(UserId userId, FamilyId familyId, CancellationToken ct);
    Task AcceptInvitationAsync(UserId userId, FamilyInvitationId invitationId, CancellationToken ct);
    Task RejectInvitationAsync(UserId userId, FamilyInvitationId invitationId, CancellationToken ct);
    Task RevokeInvitationAsync(UserId userId, FamilyInvitationId invitationId, CancellationToken ct);
}
```

### 3. ChildManagementService (NEU)
Kinderverwaltung und PIN-Management.

```csharp
public interface IChildManagementService
{
    Task<ChildDto> AddChildAsync(UserId parentId, FamilyId familyId, AddChildRequest request, CancellationToken ct);
    Task<IEnumerable<ChildDto>> GetChildrenAsync(UserId parentId, FamilyId familyId, CancellationToken ct);
    Task<ChildDto> UpdateChildAsync(UserId parentId, FamilyId familyId, UserId childId, UpdateChildRequest request, CancellationToken ct);
    Task RemoveChildAsync(UserId parentId, FamilyId familyId, UserId childId, CancellationToken ct);
    Task ChangeChildPinAsync(UserId parentId, FamilyId familyId, UserId childId, ChangeChildPinRequest request, CancellationToken ct);
}
```

### 4. FamilyMemberService (NEU)
Verwaltung von Verwandten/Relatives.

```csharp
public interface IFamilyMemberService
{
    Task<IEnumerable<RelativeDto>> GetRelativesAsync(UserId userId, FamilyId familyId, CancellationToken ct);
    Task RemoveRelativeAsync(UserId userId, FamilyId familyId, UserId relativeId, CancellationToken ct);
    Task RemoveMemberAsync(UserId userId, FamilyId familyId, UserId memberId, CancellationToken ct);
}
```

### Gemeinsame Hilfsdienste

```csharp
// Interner Helper (kein öffentliches Interface)
internal class FamilyValidationHelper
{
    Task<Family> GetFamilyOrThrowAsync(FamilyId familyId, CancellationToken ct);
    Task ValidateParentAccessAsync(UserId userId, FamilyId familyId, CancellationToken ct);
    string GenerateFamilyCode();
}
```

## Begründung

### Single Responsibility Principle
Jeder Service hat genau eine Verantwortlichkeit:
- **FamilyService**: Familie als Entität
- **FamilyInvitationService**: Einladungs-Workflow
- **ChildManagementService**: Kind-Lifecycle
- **FamilyMemberService**: Mitglieder-Management

### Vorteile

1. **Bessere Testbarkeit**
   - Kleinere Units, fokussierte Tests
   - Weniger Mocking erforderlich
   - Höhere Test-Coverage erreichbar

2. **Klarere Verantwortlichkeiten**
   - Entwickler wissen sofort, wo Code hingehört
   - Code-Reviews werden einfacher
   - Onboarding neuer Entwickler schneller

3. **Unabhängige Entwicklung**
   - Features können parallel entwickelt werden
   - Merge-Konflikte reduziert
   - Deployment-Risiko minimiert

4. **Bessere Erweiterbarkeit**
   - Neue Features in passenden Service
   - Keine "God Class" mehr

### Metriken nach Refactoring (geschätzt)

| Service | LOC | Abhängigkeiten |
|---------|-----|----------------|
| FamilyService | ~150 | 4 |
| FamilyInvitationService | ~200 | 4 |
| ChildManagementService | ~250 | 5 |
| FamilyMemberService | ~100 | 3 |
| FamilyValidationHelper | ~100 | 2 |

## Konsequenzen

### Positiv
- Einhaltung von SOLID-Prinzipien
- Verbesserte Wartbarkeit
- Bessere Testbarkeit
- Klarere Code-Organisation
- Einfachere Code-Reviews

### Negativ
- Mehr Dateien im Projekt
- Initiale Refactoring-Aufwand
- Dependency Injection wird komplexer (mehr Services)
- Bestehende Tests müssen angepasst werden

### Risiken
- Breaking Changes in API-Endpoints (müssen angepasst werden)
- Temporäre Instabilität während Refactoring

### Mitigationen
- Schrittweises Refactoring (ein Service nach dem anderen)
- Feature-Branch mit umfassenden Tests
- Backward-Kompatibilität in Endpoints durch Facade-Pattern (optional)

## Implementierungsplan

### Phase 1: Vorbereitung
1. Neue Interfaces in `Core/Interfaces/Services/` erstellen
2. Neue DTOs falls erforderlich
3. Test-Coverage für bestehenden FamilyService erhöhen

### Phase 2: Extraktion (pro Service)
1. **FamilyInvitationService** extrahieren (am unabhängigsten)
2. **FamilyMemberService** extrahieren
3. **ChildManagementService** extrahieren
4. **FamilyService** auf Core-Funktionen reduzieren

### Phase 3: Integration
1. DependencyInjection.cs aktualisieren
2. Endpoints anpassen (FamilyEndpoints.cs aufteilen)
3. Tests aktualisieren/erweitern

### Phase 4: Cleanup
1. Alten Code entfernen
2. Dokumentation aktualisieren
3. CONTEXT.md aktualisieren

## Dateien betroffen

### Neue Dateien
```
Core/Interfaces/Services/
├── IFamilyInvitationService.cs
├── IChildManagementService.cs
└── IFamilyMemberService.cs

Infrastructure/Services/
├── FamilyInvitationService.cs
├── ChildManagementService.cs
├── FamilyMemberService.cs
└── Helpers/FamilyValidationHelper.cs

Api/Endpoints/
├── FamilyInvitationEndpoints.cs (optional, oder in FamilyEndpoints belassen)
└── ChildManagementEndpoints.cs (optional)
```

### Zu ändernde Dateien
```
Core/Interfaces/Services/IFamilyService.cs (reduzieren)
Infrastructure/Services/FamilyService.cs (reduzieren)
Infrastructure/Services/DependencyInjection.cs (neue Services registrieren)
Api/Endpoints/FamilyEndpoints.cs (Services injizieren)
```

## Verwandte Entscheidungen

- ADR-001: Blazor WebAssembly (bestehend)
- ADR-003: Domain Exceptions (ausstehend, empfohlen)
- ADR-004: Result-Pattern (ausstehend, empfohlen)

## Referenzen

- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Refactoring: Improving the Design of Existing Code](https://martinfowler.com/books/refactoring.html)
- Robert C. Martin: "A class should have only one reason to change"
