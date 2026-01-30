# Story M001-S04: Basis-Styles und Theme (Light/Dark)

## Epic
M001 - Projekt-Setup

## Status
Abgeschlossen

## User Story

Als **Benutzer** möchte ich **die App im Light- oder Dark-Mode verwenden können**, damit **die App meinen Systemeinstellungen folgt und angenehm zu bedienen ist**.

## Akzeptanzkriterien

- [ ] Gegeben die App, wenn das System auf Dark-Mode steht, dann wechselt die App automatisch
- [ ] Gegeben die App, wenn das System auf Light-Mode steht, dann wird Light-Mode angezeigt
- [ ] Gegeben die Styles, wenn sie definiert sind, dann sind Farben, Fonts und Abstände konsistent
- [ ] Gegeben die Buttons, wenn sie gedrückt werden, dann haben sie ein visuelles Feedback

## Technische Hinweise

### Resources/Styles/Colors.xaml
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Primary Colors -->
    <Color x:Key="Primary">#4CAF50</Color>
    <Color x:Key="PrimaryDark">#388E3C</Color>
    <Color x:Key="PrimaryLight">#C8E6C9</Color>
    <Color x:Key="Accent">#FF9800</Color>

    <!-- Light Theme -->
    <Color x:Key="BackgroundLight">#FFFFFF</Color>
    <Color x:Key="SurfaceLight">#F5F5F5</Color>
    <Color x:Key="TextPrimaryLight">#212121</Color>
    <Color x:Key="TextSecondaryLight">#757575</Color>

    <!-- Dark Theme -->
    <Color x:Key="BackgroundDark">#121212</Color>
    <Color x:Key="SurfaceDark">#1E1E1E</Color>
    <Color x:Key="TextPrimaryDark">#FFFFFF</Color>
    <Color x:Key="TextSecondaryDark">#B3B3B3</Color>

</ResourceDictionary>
```

### Resources/Styles/Styles.xaml
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">

    <!-- Button Style -->
    <Style TargetType="Button" x:Key="PrimaryButton">
        <Setter Property="BackgroundColor" Value="{StaticResource Primary}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontSize" Value="16" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="16,12" />
        <Setter Property="VisualStateManager.VisualStateGroups">
            <VisualStateGroupList>
                <VisualStateGroup x:Name="CommonStates">
                    <VisualState x:Name="Normal" />
                    <VisualState x:Name="Pressed">
                        <VisualState.Setters>
                            <Setter Property="BackgroundColor"
                                    Value="{StaticResource PrimaryDark}" />
                        </VisualState.Setters>
                    </VisualState>
                </VisualStateGroup>
            </VisualStateGroupList>
        </Setter>
    </Style>

    <!-- Entry Style -->
    <Style TargetType="Entry" x:Key="DefaultEntry">
        <Setter Property="FontSize" Value="16" />
        <Setter Property="PlaceholderColor" Value="{AppThemeBinding
            Light={StaticResource TextSecondaryLight},
            Dark={StaticResource TextSecondaryDark}}" />
        <Setter Property="TextColor" Value="{AppThemeBinding
            Light={StaticResource TextPrimaryLight},
            Dark={StaticResource TextPrimaryDark}}" />
    </Style>

    <!-- Label Styles -->
    <Style TargetType="Label" x:Key="HeadlineLabel">
        <Setter Property="FontSize" Value="24" />
        <Setter Property="FontAttributes" Value="Bold" />
        <Setter Property="TextColor" Value="{AppThemeBinding
            Light={StaticResource TextPrimaryLight},
            Dark={StaticResource TextPrimaryDark}}" />
    </Style>

</ResourceDictionary>
```

### App.xaml Theme-Konfiguration
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
            <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M001-10 | System Dark Mode aktiv | App zeigt Dark Theme |
| TC-M001-11 | System Light Mode aktiv | App zeigt Light Theme |
| TC-M001-12 | Theme zur Laufzeit wechseln | App passt sich an |
| TC-M001-13 | Button drücken | Visuelles Feedback sichtbar |

## Story Points
3

## Priorität
Hoch
