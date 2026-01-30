# Story M014-S06: App-Icon und Splash-Screen

## Epic

M014 - App-Lifecycle & Qualitaet

## User Story

Als **Benutzer** moechte ich **ein ansprechendes App-Icon und einen Splash-Screen sehen**, damit **die App professionell wirkt und ich sie leicht wiederfinde**.

## Akzeptanzkriterien

- [ ] Gegeben die installierte App, wenn der Benutzer den Home-Screen sieht, dann ist das App-Icon sichtbar und erkennbar
- [ ] Gegeben ein App-Start, wenn die App laedt, dann wird ein Splash-Screen angezeigt
- [ ] Gegeben verschiedene Android-Versionen, wenn die App installiert ist, dann wird das Icon korrekt angezeigt (Adaptive Icons)
- [ ] Gegeben Dark Mode, wenn der Benutzer ihn aktiviert hat, dann passt sich das Icon entsprechend an (optional)

## Design-Vorgaben

### App-Icon
```
Konzept: Sparschwein mit Muenze

Farben:
- Primaer: #4CAF50 (Gruen)
- Sekundaer: #FFC107 (Gold)
- Hintergrund: Weiss

Groessen (Android):
- mdpi: 48x48
- hdpi: 72x72
- xhdpi: 96x96
- xxhdpi: 144x144
- xxxhdpi: 192x192

Adaptive Icon (Android 8+):
- Foreground: 108x108 (mit Safe Zone)
- Background: Einfarbig oder Gradient
```

### Splash-Screen
```
+------------------------------------+
|                                    |
|                                    |
|                                    |
|         [App-Icon gross]           |
|                                    |
|       TaschengeldManager           |
|                                    |
|                                    |
|                                    |
+------------------------------------+

Farben:
- Hintergrund: #4CAF50 (Primaer)
- Icon/Text: Weiss
```

## Technische Implementierung

### Projektstruktur fuer Icons

```
/Resources
  /AppIcon
    appicon.svg            <- Vektordatei fuer automatische Generierung
    appiconfg.svg          <- Foreground fuer Adaptive Icon
  /Splash
    splash.svg             <- Splash-Screen Grafik
```

### .csproj Konfiguration

```xml
<ItemGroup>
    <!-- App Icon -->
    <MauiIcon Include="Resources\AppIcon\appicon.svg"
              ForegroundFile="Resources\AppIcon\appiconfg.svg"
              Color="#FFFFFF" />

    <!-- Splash Screen -->
    <MauiSplashScreen Include="Resources\Splash\splash.svg"
                      Color="#4CAF50"
                      BaseSize="128,128" />
</ItemGroup>
```

### Android: Adaptive Icon (manuell wenn noetig)

```xml
<!-- /Platforms/Android/Resources/mipmap-anydpi-v26/ic_launcher.xml -->
<?xml version="1.0" encoding="utf-8"?>
<adaptive-icon xmlns:android="http://schemas.android.com/apk/res/android">
    <background android:drawable="@color/ic_launcher_background"/>
    <foreground android:drawable="@mipmap/ic_launcher_foreground"/>
    <monochrome android:drawable="@mipmap/ic_launcher_monochrome"/>
</adaptive-icon>

<!-- /Platforms/Android/Resources/values/ic_launcher_background.xml -->
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <color name="ic_launcher_background">#FFFFFF</color>
</resources>
```

### Splash-Screen Anpassung

```csharp
// MauiProgram.cs - Splash-Screen Dauer anpassen
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder
        .UseMauiApp<App>()
        .ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android => android
                .OnCreate((activity, bundle) =>
                {
                    // Splash Theme setzen
                    activity.Window?.SetBackgroundDrawableResource(Resource.Drawable.splash_screen);
                }));
#endif
        });

    return builder.Build();
}
```

### Android: Splash-Theme

```xml
<!-- /Platforms/Android/Resources/values/styles.xml -->
<?xml version="1.0" encoding="utf-8" ?>
<resources>
    <style name="Maui.SplashTheme" parent="Theme.SplashScreen">
        <item name="windowSplashScreenBackground">#4CAF50</item>
        <item name="windowSplashScreenAnimatedIcon">@mipmap/ic_launcher_foreground</item>
        <item name="windowSplashScreenIconBackgroundColor">#FFFFFF</item>
        <item name="postSplashScreenTheme">@style/Maui.MainTheme</item>
    </style>
</resources>
```

### Icon-Generierung (Tool)

```bash
# Mit .NET MAUI werden Icons automatisch generiert aus SVG
# Alternativ: Android Asset Studio verwenden

# Icon-Groessen generieren (manuell)
# https://romannurik.github.io/AndroidAssetStudio/icons-launcher.html
```

## Icon-Varianten

| Kontext | Groesse | Datei |
|---------|---------|-------|
| Launcher (mdpi) | 48x48 | ic_launcher.png |
| Launcher (hdpi) | 72x72 | ic_launcher.png |
| Launcher (xhdpi) | 96x96 | ic_launcher.png |
| Launcher (xxhdpi) | 144x144 | ic_launcher.png |
| Launcher (xxxhdpi) | 192x192 | ic_launcher.png |
| Play Store | 512x512 | ic_launcher-playstore.png |
| Adaptive Foreground | 108x108 | ic_launcher_foreground.png |
| Monochrome (Android 13+) | 108x108 | ic_launcher_monochrome.png |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | App installiert | Icon auf Home-Screen sichtbar |
| TC-002 | App-Start | Splash-Screen wird angezeigt |
| TC-003 | Android 8+ | Adaptive Icon korrekt |
| TC-004 | Verschiedene DPIs | Icon scharf und korrekt |
| TC-005 | Play Store Listing | 512x512 Icon korrekt |
| TC-006 | Splash-Screen Dauer | Angemessen (nicht zu lang) |

## Story Points

1

## Prioritaet

Mittel
