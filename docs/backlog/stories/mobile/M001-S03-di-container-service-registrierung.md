# Story M001-S03: DI-Container und Service-Registrierung

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Entwickler** möchte ich **einen Dependency Injection Container mit Service-Registrierung einrichten**, damit **ich Services und ViewModels lose gekoppelt verwenden kann**.

## Akzeptanzkriterien

- [ ] Gegeben das DI-Setup, wenn Services registriert werden, dann können sie über Constructor Injection verwendet werden
- [ ] Gegeben ViewModels, wenn sie registriert werden, dann sind sie als Transient konfiguriert
- [ ] Gegeben Views, wenn sie registriert werden, dann sind sie mit ihren ViewModels verknüpft
- [ ] Gegeben Services, wenn sie Singletons sein sollen, dann sind sie entsprechend registriert

## Technische Hinweise

### MauiProgram.cs
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Services registrieren
        builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

        // ViewModels registrieren
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();

        // Views registrieren
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();

        return builder.Build();
    }
}
```

### Service-Auflösung in Views
```csharp
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
```

### Extension-Methode für saubere Registrierung
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        // weitere ViewModels...
        return services;
    }

    public static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddTransient<LoginPage>();
        services.AddTransient<DashboardPage>();
        // weitere Views...
        return services;
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-07 | Service auflösen | Service wird korrekt injiziert |
| TC-M001-08 | ViewModel in View | BindingContext gesetzt |
| TC-M001-09 | Singleton-Verhalten | Gleiche Instanz bei mehreren Aufrufen |

## Story Points
2

## Priorität
Hoch
