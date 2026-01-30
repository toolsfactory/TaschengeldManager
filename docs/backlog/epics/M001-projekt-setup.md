# Epic M001: Projekt-Setup & Infrastruktur

**Status:** ✅ Abgeschlossen

## Beschreibung

Grundlegende Projektstruktur für die .NET MAUI Android-App erstellen. Beinhaltet alle Basis-Konfigurationen, DI-Setup, Navigation und API-Anbindung.

## Business Value

Technische Grundlage für alle weiteren Features. Ohne diese Infrastruktur kann keine weitere Entwicklung stattfinden.

## Stories

| Story | Beschreibung | SP | Status |
|-------|--------------|-----|--------|
| M001-S01 | MAUI-Projekt erstellen und Solution einbinden | 2 | ✅ |
| M001-S02 | NuGet-Pakete konfigurieren (MVVM, Refit, SQLite) | 1 | ✅ |
| M001-S03 | DI-Container und Service-Registrierung (MauiProgram.cs) | 2 | ✅ |
| M001-S04 | Basis-Styles und Theme erstellen (Light/Dark Mode) | 3 | ✅ |
| M001-S05 | Navigation Service mit Shell implementieren | 3 | ✅ |
| M001-S06 | API-Client mit Refit generieren | 3 | ✅ |
| M001-S07 | Lokale SQLite-Datenbank für Offline-Cache | 3 | ✅ |
| M001-S08 | Connectivity-Service für Online/Offline-Erkennung | 2 | ✅ |
| M001-S09 | Test-Projekt aufsetzen mit Mocks | 2 | ✅ |

**Gesamt: 21 SP**

## Abhängigkeiten

- Keine (erstes Epic)

## Akzeptanzkriterien (Epic-Level)

- [x] MAUI-Projekt kompiliert erfolgreich für Android
- [x] Solution enthält Mobile-Projekt und Test-Projekt
- [x] DI-Container ist konfiguriert
- [x] Refit API-Client kann Backend erreichen
- [x] SQLite-Datenbank wird erstellt
- [x] Navigation zwischen Seiten funktioniert
- [x] Light/Dark Mode wechselt korrekt

## Technische Details

### NuGet Pakete
```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="CommunityToolkit.Maui" Version="9.*" />
<PackageReference Include="Refit.HttpClientFactory" Version="7.*" />
<PackageReference Include="sqlite-net-pcl" Version="1.*" />
<PackageReference Include="SQLitePCLRaw.bundle_green" Version="2.*" />
<PackageReference Include="Plugin.Fingerprint" Version="2.*" />
```

### Projektstruktur
```
src/TaschengeldManager.Mobile/
├── Platforms/Android/
├── Views/Pages/
├── ViewModels/
├── Services/
│   ├── Api/
│   ├── Auth/
│   ├── Navigation/
│   └── Storage/
├── Models/
├── Converters/
├── Resources/Styles/
├── App.xaml
├── AppShell.xaml
└── MauiProgram.cs
```

## Priorität

**Hoch** - Blockiert alle anderen Epics

## Story Points

21 SP (abgeschlossen)
