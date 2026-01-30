# Story M004-S04: Kategorie-Auswahl mit Icons

## Epic
M004 - Kontoverwaltung Kind

## Status
Abgeschlossen

## User Story

Als **Kind** möchte ich **aus kindgerechten Kategorien mit Emojis auswählen können**, damit **ich meine Ausgaben leicht zuordnen kann und die Übersicht Spaß macht**.

## Akzeptanzkriterien

- [ ] Gegeben die Kategorien, wenn sie angezeigt werden, dann haben alle ein passendes Emoji
- [ ] Gegeben die Kategorie-Auswahl, wenn eine Kategorie ausgewählt wird, dann ist sie visuell markiert
- [ ] Gegeben die Kategorien, wenn sie für Kinder sind, dann sind sie altersgerecht benannt
- [ ] Gegeben eine ausgewählte Kategorie, wenn sie erneut getippt wird, dann wird die Auswahl aufgehoben

## UI-Entwurf

```
┌─────────────────────────────┐
│  Kategorie auswählen        │
├─────────────────────────────┤
│                             │
│  ┌─────┐ ┌─────┐ ┌─────┐   │
│  │ 🍭  │ │ 🎮  │ │ 📚  │   │
│  │Süßes│ │Spiel│ │Schule│  │
│  └─────┘ └─────┘ └─────┘   │
│                             │
│  ┌─────┐ ┌─────┐ ┌─────┐   │
│  │ 🎬  │ │ 🚌  │ │ 🎁  │   │
│  │Kino │ │Fahrt│ │Gesch│   │
│  └─────┘ └─────┘ └─────┘   │
│                             │
│  ┌─────┐ ┌─────┐ ┌─────┐   │
│  │ 🍔  │ │ 👕  │ │ 📱  │   │
│  │Essen│ │Kleid│ │Apps │   │
│  └─────┘ └─────┘ └─────┘   │
│                             │
│  ┌─────┐ ┌─────┐ ┌─────┐   │
│  │ 🎵  │ │ 💄  │ │ ⚽  │   │
│  │Musik│ │Style│ │Sport│   │
│  └─────┘ └─────┘ └─────┘   │
│                             │
│         ┌─────┐             │
│         │ ...  │             │
│         │Sonst│             │
│         └─────┘             │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### CategorySelectView Component
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.Components.CategorySelectView"
             x:Name="CategoryView">

    <VerticalStackLayout Spacing="8">
        <Label Text="Kategorie"
               FontSize="16"
               FontAttributes="Bold" />

        <FlexLayout Wrap="Wrap"
                    JustifyContent="Start"
                    AlignItems="Start"
                    BindableLayout.ItemsSource="{Binding Source={x:Reference CategoryView}, Path=Categories}">
            <BindableLayout.ItemTemplate>
                <DataTemplate x:DataType="vm:CategoryViewModel">
                    <Frame Padding="8"
                           Margin="4"
                           CornerRadius="12"
                           BorderColor="{Binding SelectionBorderColor}"
                           BackgroundColor="{Binding DisplayBackgroundColor}"
                           WidthRequest="72"
                           HeightRequest="72"
                           HasShadow="{Binding IsSelected}">

                        <Frame.GestureRecognizers>
                            <TapGestureRecognizer
                                Command="{Binding Source={x:Reference CategoryView}, Path=SelectCategoryCommand}"
                                CommandParameter="{Binding}" />
                        </Frame.GestureRecognizers>

                        <!-- Animation bei Auswahl -->
                        <Frame.Behaviors>
                            <toolkit:AnimationBehavior
                                AnimationType="{Binding SelectionAnimation}"
                                Animating="{Binding JustSelected}" />
                        </Frame.Behaviors>

                        <VerticalStackLayout HorizontalOptions="Center"
                                             VerticalOptions="Center"
                                             Spacing="2">
                            <Label Text="{Binding Emoji}"
                                   FontSize="28"
                                   HorizontalTextAlignment="Center" />
                            <Label Text="{Binding Name}"
                                   FontSize="10"
                                   HorizontalTextAlignment="Center"
                                   LineBreakMode="TailTruncation"
                                   MaxLines="1" />
                        </VerticalStackLayout>
                    </Frame>
                </DataTemplate>
            </BindableLayout.ItemTemplate>
        </FlexLayout>
    </VerticalStackLayout>

</ContentView>
```

### CategorySelectView.xaml.cs
```csharp
public partial class CategorySelectView : ContentView
{
    public static readonly BindableProperty CategoriesProperty =
        BindableProperty.Create(nameof(Categories), typeof(IEnumerable<CategoryViewModel>),
            typeof(CategorySelectView));

    public static readonly BindableProperty SelectedCategoryProperty =
        BindableProperty.Create(nameof(SelectedCategory), typeof(CategoryViewModel),
            typeof(CategorySelectView), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnSelectedCategoryChanged);

    public static readonly BindableProperty SelectCategoryCommandProperty =
        BindableProperty.Create(nameof(SelectCategoryCommand), typeof(ICommand),
            typeof(CategorySelectView));

    public IEnumerable<CategoryViewModel>? Categories
    {
        get => (IEnumerable<CategoryViewModel>?)GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public CategoryViewModel? SelectedCategory
    {
        get => (CategoryViewModel?)GetValue(SelectedCategoryProperty);
        set => SetValue(SelectedCategoryProperty, value);
    }

    public ICommand SelectCategoryCommand => new Command<CategoryViewModel>(OnCategorySelected);

    private void OnCategorySelected(CategoryViewModel category)
    {
        if (Categories == null) return;

        foreach (var cat in Categories)
        {
            cat.IsSelected = cat == category && !cat.IsSelected;
            cat.JustSelected = cat == category && cat.IsSelected;
        }

        SelectedCategory = category.IsSelected ? category : null;
    }

    private static void OnSelectedCategoryChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CategorySelectView)bindable;
        if (view.Categories == null) return;

        foreach (var cat in view.Categories)
        {
            cat.IsSelected = cat == newValue;
        }
    }
}
```

### Vordefinierte Kategorien
```csharp
public static class DefaultCategories
{
    public static List<CategoryResponse> ExpenseCategories => new()
    {
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Süßigkeiten", Emoji = "🍭", Color = "#FFE0B2" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Spielzeug", Emoji = "🎮", Color = "#E1BEE7" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Schulsachen", Emoji = "📚", Color = "#BBDEFB" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Kino/Film", Emoji = "🎬", Color = "#B2DFDB" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Bus/Bahn", Emoji = "🚌", Color = "#C8E6C9" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Geschenke", Emoji = "🎁", Color = "#F8BBD0" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Essen", Emoji = "🍔", Color = "#FFE082" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Kleidung", Emoji = "👕", Color = "#B3E5FC" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Apps/Spiele", Emoji = "📱", Color = "#D1C4E9" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Musik", Emoji = "🎵", Color = "#FFCCBC" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Beauty", Emoji = "💄", Color = "#F48FB1" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Sport", Emoji = "⚽", Color = "#A5D6A7" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Sonstiges", Emoji = "📦", Color = "#E0E0E0" }
    };

    public static List<CategoryResponse> IncomeCategories => new()
    {
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Taschengeld", Emoji = "💰", Color = "#C8E6C9" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Geschenk", Emoji = "🎁", Color = "#F8BBD0" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Zinsen", Emoji = "📈", Color = "#B2DFDB" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Verdient", Emoji = "⭐", Color = "#FFE082" },
        new CategoryResponse { Id = Guid.Parse("..."), Name = "Sonstiges", Emoji = "📦", Color = "#E0E0E0" }
    };
}
```

### CategoryViewModel mit visuellen Properties
```csharp
public partial class CategoryViewModel : ObservableObject
{
    public Guid Id { get; }
    public string Name { get; }
    public string Emoji { get; }
    public string ColorHex { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectionBorderColor))]
    [NotifyPropertyChangedFor(nameof(DisplayBackgroundColor))]
    private bool _isSelected;

    [ObservableProperty]
    private bool _justSelected;

    public Color SelectionBorderColor => IsSelected
        ? Color.FromArgb("#4CAF50")
        : Colors.Transparent;

    public Color DisplayBackgroundColor => IsSelected
        ? Color.FromArgb("#E8F5E9")
        : Color.FromArgb(ColorHex);

    public AnimationType SelectionAnimation => new ScaleAnimation
    {
        Length = 150,
        Scale = 1.1
    };
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M004-15 | Kategorien anzeigen | Alle mit Emoji sichtbar |
| TC-M004-16 | Kategorie auswählen | Visuell markiert |
| TC-M004-17 | Auswahl aufheben | Markierung entfernt |
| TC-M004-18 | Andere Kategorie wählen | Alte Auswahl aufgehoben |

## Story Points
2

## Priorität
Mittel
