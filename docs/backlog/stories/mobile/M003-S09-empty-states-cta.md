# Story M003-S09: Empty States mit CTA

## Epic
M003 - Dashboard & Navigation

## Status
Offen

## User Story

Als **Benutzer** möchte ich **bei leeren Listen oder fehlenden Daten einen hilfreichen Hinweis sehen**, damit **ich weiß, wie ich die App nutzen kann und was als nächstes zu tun ist**.

## Akzeptanzkriterien

- [ ] Gegeben eine leere Liste, wenn sie angezeigt wird, dann wird ein Empty State mit Illustration gezeigt
- [ ] Gegeben ein Empty State, wenn er angezeigt wird, dann enthält er einen Call-to-Action
- [ ] Gegeben verschiedene Kontexte, wenn Empty States angezeigt werden, dann sind sie kontextspezifisch
- [ ] Gegeben der CTA, wenn er geklickt wird, dann führt er zur entsprechenden Aktion

## UI-Entwurf

```
Keine Kinder:
┌─────────────────────────────┐
│                             │
│       [Illustration]        │
│          👨‍👩‍👧‍👦              │
│                             │
│   Noch keine Kinder         │
│                             │
│   Füge dein erstes Kind     │
│   hinzu, um loszulegen.     │
│                             │
│  ┌───────────────────────┐  │
│  │   + Kind hinzufügen   │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘

Keine Transaktionen:
┌─────────────────────────────┐
│                             │
│       [Illustration]        │
│           📭                │
│                             │
│   Noch keine Transaktionen  │
│                             │
│   Hier siehst du bald       │
│   alle Ein- und Ausgaben.   │
│                             │
└─────────────────────────────┘

Keine Internetverbindung:
┌─────────────────────────────┐
│                             │
│       [Illustration]        │
│           📡                │
│                             │
│   Keine Verbindung          │
│                             │
│   Bitte prüfe deine         │
│   Internetverbindung.       │
│                             │
│  ┌───────────────────────┐  │
│  │   Erneut versuchen    │  │
│  └───────────────────────┘  │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### EmptyStateView Component
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TaschengeldManager.Mobile.Views.Components.EmptyStateView"
             x:Name="EmptyState">

    <VerticalStackLayout VerticalOptions="Center"
                         HorizontalOptions="Center"
                         Padding="32"
                         Spacing="16">

        <!-- Illustration/Emoji -->
        <Label Text="{Binding Source={x:Reference EmptyState}, Path=Emoji}"
               FontSize="64"
               HorizontalTextAlignment="Center"
               IsVisible="{Binding Source={x:Reference EmptyState}, Path=HasEmoji}" />

        <Image Source="{Binding Source={x:Reference EmptyState}, Path=ImageSource}"
               HeightRequest="150"
               Aspect="AspectFit"
               IsVisible="{Binding Source={x:Reference EmptyState}, Path=HasImage}" />

        <!-- Titel -->
        <Label Text="{Binding Source={x:Reference EmptyState}, Path=Title}"
               FontSize="20"
               FontAttributes="Bold"
               HorizontalTextAlignment="Center" />

        <!-- Beschreibung -->
        <Label Text="{Binding Source={x:Reference EmptyState}, Path=Description}"
               FontSize="14"
               TextColor="{StaticResource TextSecondaryLight}"
               HorizontalTextAlignment="Center"
               LineHeight="1.4" />

        <!-- Call-to-Action Button -->
        <Button Text="{Binding Source={x:Reference EmptyState}, Path=ActionText}"
                Command="{Binding Source={x:Reference EmptyState}, Path=ActionCommand}"
                Style="{StaticResource PrimaryButton}"
                IsVisible="{Binding Source={x:Reference EmptyState}, Path=HasAction}"
                Margin="0,8,0,0" />

        <!-- Sekundärer Link -->
        <Label Text="{Binding Source={x:Reference EmptyState}, Path=SecondaryActionText}"
               TextColor="{StaticResource Primary}"
               TextDecorations="Underline"
               HorizontalTextAlignment="Center"
               IsVisible="{Binding Source={x:Reference EmptyState}, Path=HasSecondaryAction}">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding Source={x:Reference EmptyState}, Path=SecondaryActionCommand}" />
            </Label.GestureRecognizers>
        </Label>

    </VerticalStackLayout>

</ContentView>
```

### EmptyStateView.xaml.cs
```csharp
public partial class EmptyStateView : ContentView
{
    public static readonly BindableProperty EmojiProperty =
        BindableProperty.Create(nameof(Emoji), typeof(string), typeof(EmptyStateView), "📭");

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(EmptyStateView));

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(EmptyStateView));

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description), typeof(string), typeof(EmptyStateView));

    public static readonly BindableProperty ActionTextProperty =
        BindableProperty.Create(nameof(ActionText), typeof(string), typeof(EmptyStateView));

    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(EmptyStateView));

    public static readonly BindableProperty SecondaryActionTextProperty =
        BindableProperty.Create(nameof(SecondaryActionText), typeof(string), typeof(EmptyStateView));

    public static readonly BindableProperty SecondaryActionCommandProperty =
        BindableProperty.Create(nameof(SecondaryActionCommand), typeof(ICommand), typeof(EmptyStateView));

    public string Emoji
    {
        get => (string)GetValue(EmojiProperty);
        set => SetValue(EmojiProperty, value);
    }

    public string? ImageSource
    {
        get => (string?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string? ActionText
    {
        get => (string?)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => (ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public bool HasEmoji => !string.IsNullOrEmpty(Emoji) && string.IsNullOrEmpty(ImageSource);
    public bool HasImage => !string.IsNullOrEmpty(ImageSource);
    public bool HasAction => !string.IsNullOrEmpty(ActionText) && ActionCommand != null;
    public bool HasSecondaryAction => !string.IsNullOrEmpty(SecondaryActionText);
}
```

### Verwendung in CollectionView
```xml
<CollectionView ItemsSource="{Binding Children}">
    <CollectionView.EmptyView>
        <views:EmptyStateView Emoji="👨‍👩‍👧‍👦"
                               Title="Noch keine Kinder"
                               Description="Füge dein erstes Kind hinzu, um loszulegen."
                               ActionText="+ Kind hinzufügen"
                               ActionCommand="{Binding AddChildCommand}" />
    </CollectionView.EmptyView>

    <CollectionView.ItemTemplate>
        <!-- Template -->
    </CollectionView.ItemTemplate>
</CollectionView>
```

### Vordefinierte Empty States
```csharp
public static class EmptyStates
{
    public static EmptyStateViewModel NoChildren => new()
    {
        Emoji = "👨‍👩‍👧‍👦",
        Title = "Noch keine Kinder",
        Description = "Füge dein erstes Kind hinzu, um loszulegen.",
        ActionText = "+ Kind hinzufügen"
    };

    public static EmptyStateViewModel NoTransactions => new()
    {
        Emoji = "📭",
        Title = "Noch keine Transaktionen",
        Description = "Hier siehst du bald alle Ein- und Ausgaben."
    };

    public static EmptyStateViewModel NoConnection => new()
    {
        Emoji = "📡",
        Title = "Keine Verbindung",
        Description = "Bitte prüfe deine Internetverbindung.",
        ActionText = "Erneut versuchen"
    };

    public static EmptyStateViewModel NoSearchResults => new()
    {
        Emoji = "🔍",
        Title = "Keine Ergebnisse",
        Description = "Versuche es mit anderen Suchbegriffen.",
        ActionText = "Filter zurücksetzen"
    };

    public static EmptyStateViewModel NoGifts => new()
    {
        Emoji = "🎁",
        Title = "Noch keine Geschenke",
        Description = "Hier siehst du alle Geschenke, die du gesendet hast.",
        ActionText = "Geschenk senden"
    };

    public static EmptyStateViewModel Error => new()
    {
        Emoji = "😕",
        Title = "Etwas ist schiefgelaufen",
        Description = "Ein unerwarteter Fehler ist aufgetreten.",
        ActionText = "Erneut versuchen"
    };
}
```

### ViewModel-Integration
```csharp
public partial class ChildrenListViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ChildViewModel> _children = new();

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isOffline;

    // Empty State abhängig vom Zustand
    public EmptyStateViewModel CurrentEmptyState
    {
        get
        {
            if (IsOffline)
                return EmptyStates.NoConnection;
            if (HasError)
                return EmptyStates.Error;
            return EmptyStates.NoChildren;
        }
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-34 | Leere Kinderliste | Empty State "Keine Kinder" |
| TC-M003-35 | Leere Transaktionsliste | Empty State "Keine Transaktionen" |
| TC-M003-36 | Keine Internetverbindung | Empty State "Keine Verbindung" |
| TC-M003-37 | CTA klicken | Entsprechende Aktion wird ausgeführt |

## Story Points
2

## Priorität
Mittel
