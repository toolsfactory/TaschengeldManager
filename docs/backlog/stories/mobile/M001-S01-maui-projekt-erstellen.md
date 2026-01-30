# Story M001-S01: MAUI-Projekt erstellen

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **ein .NET MAUI-Projekt mit korrekter Projektstruktur erstellen**, damit **ich die mobile App für iOS und Android entwickeln kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein neues MAUI-Projekt, wenn es erstellt wird, dann verwendet es .NET 8+
- [ ] Gegeben die Projektstruktur, wenn sie erstellt wird, dann folgt sie dem geplanten Aufbau (TaschengeldManager.Mobile)
- [ ] Gegeben das Projekt, wenn es kompiliert wird, dann ist es für iOS und Android konfiguriert
- [ ] Gegeben die Solution, wenn das Mobile-Projekt hinzugefügt wird, dann ist es korrekt referenziert

## Technische Hinweise

### Projektstruktur
```
/src/TaschengeldManager.Mobile
  /Views
  /ViewModels
  /Services
  /Models
  /Resources
    /Styles
    /Images
    /Fonts
  /Platforms
    /Android
    /iOS
  MauiProgram.cs
  App.xaml
  App.xaml.cs
  AppShell.xaml
  AppShell.xaml.cs
```

### XAML/ViewModel Hinweise
- `MauiProgram.cs`: Entry Point mit Builder-Pattern
- `App.xaml`: Application-Ressourcen und Styles
- `AppShell.xaml`: Shell-Navigation Konfiguration

### Befehle
```bash
dotnet new maui -n TaschengeldManager.Mobile -o src/TaschengeldManager.Mobile
dotnet sln add src/TaschengeldManager.Mobile
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-01 | Projekt kompilieren (Android) | Build erfolgreich |
| TC-M001-02 | Projekt kompilieren (iOS) | Build erfolgreich |
| TC-M001-03 | App starten im Emulator | App startet ohne Fehler |

## Story Points
2

## Priorität
Hoch
