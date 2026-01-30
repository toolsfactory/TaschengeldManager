# ADR-001: Blazor WebAssembly für Web-Frontend

## Status

**Accepted**

## Kontext

Für den TaschengeldManager ist neben der mobilen MAUI-App auch ein Web-Frontend geplant. Es stellt sich die Frage, welche Technologie für das Web-Frontend verwendet werden soll.

### Rahmenbedingungen
- Backend: .NET 10, ASP.NET Core, Aspire 9.1
- Mobile: .NET MAUI (C#)
- Team-Skills: Primär .NET/C#
- API: RESTful, bereits spezifiziert

### Evaluierte Optionen
1. **Blazor WebAssembly** - C#/.NET im Browser via WebAssembly
2. **Blazor Server** - C# serverseitig mit SignalR-Verbindung
3. **React (TypeScript)** - JavaScript-basiertes SPA-Framework

## Entscheidung

Wir verwenden **Blazor WebAssembly** für das Web-Frontend.

## Begründung

### Hauptargumente

1. **Einheitlicher Tech-Stack**
   - Backend, Mobile und Web nutzen dieselbe Sprache (C#)
   - Ein Tooling (Visual Studio / Rider, MSBuild)
   - Keine separate JavaScript-Toolchain (npm, webpack, vite)

2. **Code-Sharing**
   - DTOs können zwischen allen Projekten geteilt werden
   - Validierungslogik (FluentValidation) wiederverwendbar
   - Gemeinsame Interfaces und Contracts
   - Reduziert Duplikation und Synchronisations-Aufwand

3. **Team-Effizienz**
   - Kein Erlernen einer zweiten Sprache (TypeScript)
   - Alle Entwickler können an allen Projekten arbeiten
   - Schnellere Onboarding neuer Team-Mitglieder

4. **Aspire-Integration**
   - Blazor ist nativ in Aspire integriert
   - Service Discovery, Telemetrie out-of-the-box
   - Einheitliches Deployment

5. **Type-Safety**
   - Durchgängige Typsicherheit von DB bis UI
   - Compile-Time Checks statt Runtime-Errors
   - Bessere Refactoring-Unterstützung

### Warum nicht Blazor Server?
- Erfordert ständige SignalR-Verbindung
- Skalierungsprobleme bei vielen gleichzeitigen Nutzern
- Offline-Nutzung nicht möglich
- Latenz bei jeder Interaktion

### Warum nicht React?
- Zweite Sprache (TypeScript) im Projekt
- DTOs müssen synchron gehalten werden (OpenAPI Generator hilft, aber Overhead)
- Separates Build-Tooling und Deployment
- Team müsste React/TypeScript lernen

## Konsequenzen

### Positiv
- Ein konsistenter Tech-Stack über alle Plattformen
- Maximales Code-Sharing möglich
- Team kann flexibel zwischen Projekten wechseln
- Einfachere Wartung und weniger Abhängigkeiten

### Negativ
- Initiale Ladezeit höher als bei React (~2-5 MB WASM-Bundle)
- Kleineres Ökosystem als React
- Weniger verfügbare Entwickler am Arbeitsmarkt mit Blazor-Erfahrung

### Risiken
- WASM-Performance auf älteren Geräten
- Browser-Kompatibilität (moderne Browser erforderlich)

### Mitigationen
- AOT-Compilation für kleinere Bundle-Größe
- Lazy Loading für Module
- Aggressive Caching-Strategie
- Progressive Loading UI während WASM-Download
- Browser-Mindestanforderungen dokumentieren

## Technische Details

### Projektstruktur
```
/src
  /TaschengeldManager.Web           # Blazor WASM Client
  /TaschengeldManager.Web.Shared    # Shared Components (optional)
```

### Empfohlene UI-Library
**MudBlazor** - ausgereift, Material Design, gute Dokumentation

### Packages
```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" />
<PackageReference Include="MudBlazor" />
<PackageReference Include="Blazored.LocalStorage" />
```

### Aspire-Integration
```csharp
// Im AppHost
var webfrontend = builder.AddProject<Projects.TaschengeldManager_Web>("webfrontend")
    .WithReference(api);
```

## Verwandte Entscheidungen

- ADR-002: UI-Component-Library (ausstehend)
- ADR-003: State Management Blazor (ausstehend)
