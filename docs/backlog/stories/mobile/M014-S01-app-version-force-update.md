# Story M014-S01: App-Version pruefen / Force-Update

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **Entwickler** moechte ich **Benutzer zwingen koennen, auf eine neue App-Version zu aktualisieren**, damit **kritische Fehler oder Sicherheitsluecken behoben werden koennen**.

## Akzeptanzkriterien

- [ ] Gegeben ein App-Start, wenn die App die Mindestversion nicht erfuellt, dann wird ein Force-Update Dialog angezeigt
- [ ] Gegeben ein Force-Update Dialog, wenn der Benutzer ihn sieht, dann kann er die App nicht anders nutzen
- [ ] Gegeben ein Force-Update, wenn der Benutzer auf "Aktualisieren" tippt, dann wird er zum App Store geleitet
- [ ] Gegeben eine optionale Update-Empfehlung, wenn sie angezeigt wird, dann kann der Benutzer sie ueberspringen
- [ ] Gegeben ein Version-Check, wenn er fehlschlaegt, dann wird die App trotzdem gestartet (graceful degradation)

## UI-Entwurf

### Force-Update Dialog
```
+------------------------------------+
|                                    |
|           [Update-Icon]            |
|                                    |
|      Update erforderlich           |
|                                    |
|   Eine neue Version ist            |
|   verfuegbar. Bitte aktualisiere   |
|   die App, um fortzufahren.        |
|                                    |
|   Aktuelle Version: 1.0.0          |
|   Mindestversion: 1.2.0            |
|                                    |
|  +--------------------------------+|
|  |     Jetzt aktualisieren        ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```
(Kein Schliessen-Button, kein Zurueck)

### Optionales Update
```
+------------------------------------+
|        Update verfuegbar       [X] |
+------------------------------------+
|                                    |
|   Eine neue Version (1.3.0) ist    |
|   verfuegbar mit:                  |
|                                    |
|   - Neue Funktionen                |
|   - Fehlerbehebungen               |
|                                    |
|  +--------------------------------+|
|  |     Jetzt aktualisieren        ||
|  +--------------------------------+|
|  +--------------------------------+|
|  |      Spaeter erinnern          ||
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## API-Endpunkt

```
GET /api/app/version
Authorization: Bearer {token} (optional)

Response 200:
{
  "platform": "android",
  "minVersion": "1.2.0",
  "currentVersion": "1.3.0",
  "forceUpdate": true,
  "updateUrl": "market://details?id=com.taschengeld.app",
  "releaseNotes": [
    "Neue Funktionen",
    "Fehlerbehebungen"
  ]
}
```

## Technische Implementierung

### Version-Check Service

```csharp
public interface IVersionCheckService
{
    Task<VersionCheckResult> CheckVersionAsync();
}

public class VersionCheckResult
{
    public bool IsUpdateRequired { get; set; }
    public bool IsUpdateAvailable { get; set; }
    public string CurrentVersion { get; set; } = string.Empty;
    public string MinVersion { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public string UpdateUrl { get; set; } = string.Empty;
    public List<string> ReleaseNotes { get; set; } = new();
}

public class VersionCheckService : IVersionCheckService
{
    private readonly IApiClient _apiClient;

    public async Task<VersionCheckResult> CheckVersionAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync<VersionResponse>("/api/app/version");

            var currentVersion = new Version(AppInfo.VersionString);
            var minVersion = new Version(response.MinVersion);
            var latestVersion = new Version(response.CurrentVersion);

            return new VersionCheckResult
            {
                IsUpdateRequired = currentVersion < minVersion,
                IsUpdateAvailable = currentVersion < latestVersion,
                CurrentVersion = AppInfo.VersionString,
                MinVersion = response.MinVersion,
                LatestVersion = response.CurrentVersion,
                UpdateUrl = response.UpdateUrl,
                ReleaseNotes = response.ReleaseNotes
            };
        }
        catch (Exception)
        {
            // Bei Fehlern App starten lassen
            return new VersionCheckResult
            {
                IsUpdateRequired = false,
                IsUpdateAvailable = false,
                CurrentVersion = AppInfo.VersionString
            };
        }
    }
}
```

### App.xaml.cs Integration

```csharp
public partial class App : Application
{
    private readonly IVersionCheckService _versionCheck;

    public App(IVersionCheckService versionCheck)
    {
        InitializeComponent();
        _versionCheck = versionCheck;
    }

    protected override async void OnStart()
    {
        base.OnStart();

        var result = await _versionCheck.CheckVersionAsync();

        if (result.IsUpdateRequired)
        {
            await ShowForceUpdateDialogAsync(result);
        }
        else if (result.IsUpdateAvailable)
        {
            await ShowOptionalUpdateDialogAsync(result);
        }
    }

    private async Task ShowForceUpdateDialogAsync(VersionCheckResult result)
    {
        var page = new ForceUpdatePage(result);
        MainPage = page;
    }

    private async Task ShowOptionalUpdateDialogAsync(VersionCheckResult result)
    {
        // Pruefen ob schon gesehen (fuer diese Version)
        var lastSkippedVersion = Preferences.Get("SkippedUpdateVersion", string.Empty);
        if (lastSkippedVersion == result.LatestVersion)
        {
            return;
        }

        var update = await MainPage!.DisplayAlert(
            "Update verfuegbar",
            $"Version {result.LatestVersion} ist verfuegbar.\n\n" +
            string.Join("\n", result.ReleaseNotes.Select(n => $"- {n}")),
            "Jetzt aktualisieren",
            "Spaeter erinnern");

        if (update)
        {
            await OpenStoreAsync(result.UpdateUrl);
        }
        else
        {
            Preferences.Set("SkippedUpdateVersion", result.LatestVersion);
        }
    }

    private async Task OpenStoreAsync(string url)
    {
        await Browser.OpenAsync(url, BrowserLaunchMode.External);
    }
}
```

### Force-Update Page

```csharp
public partial class ForceUpdatePage : ContentPage
{
    private readonly VersionCheckResult _result;

    public ForceUpdatePage(VersionCheckResult result)
    {
        _result = result;
        InitializeComponent();
        BindingContext = this;
    }

    public string CurrentVersion => _result.CurrentVersion;
    public string MinVersion => _result.MinVersion;

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        await Browser.OpenAsync(_result.UpdateUrl, BrowserLaunchMode.External);
    }

    // Back-Button deaktivieren
    protected override bool OnBackButtonPressed()
    {
        return true; // Event konsumieren, keine Navigation
    }
}
```

### XAML: ForceUpdatePage

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TaschengeldManager.Pages.ForceUpdatePage"
             Shell.NavBarIsVisible="False"
             Shell.TabBarIsVisible="False">

    <VerticalStackLayout VerticalOptions="Center"
                         HorizontalOptions="Center"
                         Spacing="20"
                         Padding="32">

        <Image Source="update_icon.png"
               WidthRequest="80"
               HeightRequest="80"/>

        <Label Text="Update erforderlich"
               FontSize="24"
               FontAttributes="Bold"
               HorizontalTextAlignment="Center"/>

        <Label Text="Eine neue Version ist verfuegbar. Bitte aktualisiere die App, um fortzufahren."
               HorizontalTextAlignment="Center"/>

        <Label Text="{Binding CurrentVersion, StringFormat='Aktuelle Version: {0}'}"
               FontSize="12"
               TextColor="Gray"
               HorizontalTextAlignment="Center"/>

        <Label Text="{Binding MinVersion, StringFormat='Mindestversion: {0}'}"
               FontSize="12"
               TextColor="Gray"
               HorizontalTextAlignment="Center"/>

        <Button Text="Jetzt aktualisieren"
                Clicked="OnUpdateClicked"/>

    </VerticalStackLayout>
</ContentPage>
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Version < MinVersion | Force-Update Dialog |
| TC-002 | Version < LatestVersion | Optionaler Update-Hinweis |
| TC-003 | Version aktuell | Kein Dialog |
| TC-004 | API nicht erreichbar | App startet normal |
| TC-005 | Force-Update: Zurueck-Button | Keine Navigation moeglich |
| TC-006 | Update-Link | Store wird geoeffnet |
| TC-007 | "Spaeter" bei optionalem Update | Wird fuer Version nicht mehr gezeigt |

## Story Points

2

## Prioritaet

Hoch
