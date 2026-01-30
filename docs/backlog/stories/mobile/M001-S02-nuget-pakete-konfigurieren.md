# Story M001-S02: NuGet-Pakete konfigurieren

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **alle benötigten NuGet-Pakete konfigurieren**, damit **ich die erforderlichen Bibliotheken für die App-Entwicklung nutzen kann**.

## Akzeptanzkriterien

- [ ] Gegeben das Projekt, wenn die Pakete installiert werden, dann sind alle Versionen kompatibel
- [ ] Gegeben die Paketliste, wenn sie konfiguriert ist, dann enthält sie MVVM, HTTP, SQLite und Testing-Pakete
- [ ] Gegeben die Pakete, wenn sie installiert sind, dann können sie im Code verwendet werden

## Technische Hinweise

### Erforderliche NuGet-Pakete
```xml
<!-- MVVM -->
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="CommunityToolkit.Maui" Version="7.*" />

<!-- HTTP/API -->
<PackageReference Include="Refit" Version="7.*" />
<PackageReference Include="Refit.HttpClientFactory" Version="7.*" />

<!-- Datenbank -->
<PackageReference Include="sqlite-net-pcl" Version="1.*" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.*" />

<!-- Authentifizierung -->
<PackageReference Include="Plugin.Fingerprint" Version="3.*" />

<!-- Sonstiges -->
<PackageReference Include="Polly" Version="8.*" />
<PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.*" />
```

### MauiProgram.cs Anpassung
```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

    return builder.Build();
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-04 | NuGet Restore | Alle Pakete wiederhergestellt |
| TC-M001-05 | Build nach Paketinstallation | Keine Konflikte |
| TC-M001-06 | CommunityToolkit.Mvvm verwenden | ObservableObject funktioniert |

## Story Points
1

## Priorität
Hoch
