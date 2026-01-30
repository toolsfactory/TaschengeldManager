# Story M013-S07: Skeleton-Loader

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **Benutzer** moechte ich **beim Laden von Listen einen Platzhalter sehen, der die Struktur der kommenden Daten zeigt**, damit **ich weiss, dass die App arbeitet und wie das Ergebnis aussehen wird**.

## Akzeptanzkriterien

- [ ] Gegeben eine Liste wird geladen, wenn sie noch leer ist, dann werden Skeleton-Eintraege angezeigt
- [ ] Gegeben Skeleton-Eintraege, wenn sie angezeigt werden, dann haben sie eine animierte Schimmer-Effekt
- [ ] Gegeben Daten werden geladen, wenn sie ankommen, dann werden die Skeletons durch echte Daten ersetzt
- [ ] Gegeben verschiedene Listen-Typen, wenn sie geladen werden, dann zeigen sie typspezifische Skeleton-Layouts

## UI-Entwurf

### Transaktions-Skeleton
```
+------------------------------------+
|  Transaktionen                     |
|  +--------------------------------+|
|  | ████████████   █████████████  ||
|  | ████████████████████           || <- Animierter Schimmer
|  +--------------------------------+|
|  +--------------------------------+|
|  | ████████████   █████████████  ||
|  | ████████████████████           ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | ████████████   █████████████  ||
|  | ████████████████████           ||
|  +--------------------------------+|
+------------------------------------+
```

### Dashboard-Skeleton
```
+------------------------------------+
|  Hallo, ████████████!              |
|                                    |
|  Kontostand                        |
|  +--------------------------------+|
|  |        ████████████            ||
|  +--------------------------------+|
|                                    |
|  Letzte Transaktionen              |
|  +--------------------------------+|
|  | ████████   ████████████       ||
|  | ████████████████               ||
|  +--------------------------------+|
|  +--------------------------------+|
|  | ████████   ████████████       ||
|  | ████████████████               ||
|  +--------------------------------+|
+------------------------------------+
```

## Technische Implementierung

### Skeleton-View Control

```csharp
public class SkeletonView : ContentView
{
    private readonly Animation _shimmerAnimation;
    private readonly BoxView _shimmer;

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(double), typeof(SkeletonView), 4.0);

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public SkeletonView()
    {
        BackgroundColor = Color.FromArgb("#E0E0E0");

        var frame = new Frame
        {
            Padding = 0,
            HasShadow = false,
            CornerRadius = (float)CornerRadius,
            IsClippedToBounds = true
        };

        _shimmer = new BoxView
        {
            Color = Color.FromArgb("#F5F5F5"),
            WidthRequest = 100,
            Opacity = 0.7
        };

        var grid = new Grid { BackgroundColor = Color.FromArgb("#E0E0E0") };
        grid.Add(_shimmer);
        frame.Content = grid;
        Content = frame;

        // Schimmer-Animation
        _shimmerAnimation = new Animation(v =>
        {
            _shimmer.TranslationX = v * (Width + 100) - 100;
        }, 0, 1);
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent != null)
        {
            _shimmerAnimation.Commit(this, "ShimmerAnimation",
                length: 1500,
                easing: Easing.Linear,
                repeat: () => true);
        }
        else
        {
            this.AbortAnimation("ShimmerAnimation");
        }
    }
}
```

### Skeleton-Template fuer Transaktionen

```xml
<!-- TransactionSkeletonTemplate.xaml -->
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:controls="clr-namespace:TaschengeldManager.Controls">

    <Border StrokeThickness="0"
            BackgroundColor="White"
            Padding="16"
            Margin="0,0,0,8">
        <Grid ColumnDefinitions="Auto,*,Auto" RowDefinitions="Auto,Auto">
            <!-- Icon Placeholder -->
            <controls:SkeletonView
                Grid.RowSpan="2"
                WidthRequest="40"
                HeightRequest="40"
                CornerRadius="20"/>

            <!-- Title Placeholder -->
            <controls:SkeletonView
                Grid.Column="1"
                WidthRequest="120"
                HeightRequest="16"
                Margin="12,0,0,4"
                HorizontalOptions="Start"/>

            <!-- Amount Placeholder -->
            <controls:SkeletonView
                Grid.Column="2"
                WidthRequest="60"
                HeightRequest="16"/>

            <!-- Subtitle Placeholder -->
            <controls:SkeletonView
                Grid.Row="1"
                Grid.Column="1"
                WidthRequest="80"
                HeightRequest="12"
                Margin="12,4,0,0"
                HorizontalOptions="Start"/>
        </Grid>
    </Border>
</ContentView>
```

### Skeleton-Liste

```csharp
public class SkeletonList : ContentView
{
    public static readonly BindableProperty ItemCountProperty =
        BindableProperty.Create(nameof(ItemCount), typeof(int), typeof(SkeletonList), 5);

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(SkeletonList));

    public int ItemCount
    {
        get => (int)GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(ItemCount) || propertyName == nameof(ItemTemplate))
        {
            UpdateContent();
        }
    }

    private void UpdateContent()
    {
        var stack = new VerticalStackLayout { Spacing = 8 };

        for (int i = 0; i < ItemCount; i++)
        {
            var item = ItemTemplate?.CreateContent() as View
                ?? new TransactionSkeletonTemplate();
            stack.Children.Add(item);
        }

        Content = stack;
    }
}
```

### XAML: Liste mit Skeleton

```xml
<ContentPage>
    <Grid>
        <!-- Skeleton waehrend Laden -->
        <controls:SkeletonList
            ItemCount="5"
            IsVisible="{Binding IsBusy}">
            <controls:SkeletonList.ItemTemplate>
                <DataTemplate>
                    <controls:TransactionSkeletonTemplate/>
                </DataTemplate>
            </controls:SkeletonList.ItemTemplate>
        </controls:SkeletonList>

        <!-- Echte Daten -->
        <CollectionView ItemsSource="{Binding Transactions}"
                        IsVisible="{Binding IsNotBusy}">
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:Transaction">
                    <controls:TransactionListItem/>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </Grid>
</ContentPage>
```

### Dashboard mit Skeleton

```xml
<ContentPage>
    <ScrollView>
        <VerticalStackLayout Padding="16" Spacing="16">

            <!-- Begruessung -->
            <Label Text="{Binding Greeting}"
                   IsVisible="{Binding IsNotBusy}"/>
            <controls:SkeletonView
                WidthRequest="200"
                HeightRequest="24"
                HorizontalOptions="Start"
                IsVisible="{Binding IsBusy}"/>

            <!-- Kontostand -->
            <Frame IsVisible="{Binding IsNotBusy}">
                <Label Text="{Binding Balance, StringFormat='{0:C}'}"/>
            </Frame>
            <Frame IsVisible="{Binding IsBusy}">
                <controls:SkeletonView
                    WidthRequest="150"
                    HeightRequest="40"
                    HorizontalOptions="Center"/>
            </Frame>

            <!-- Transaktionen -->
            <Label Text="Letzte Transaktionen"/>

            <controls:SkeletonList
                ItemCount="3"
                IsVisible="{Binding IsBusy}"/>

            <CollectionView ItemsSource="{Binding RecentTransactions}"
                            IsVisible="{Binding IsNotBusy}">
                <!-- ... -->
            </CollectionView>

        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

## Skeleton-Typen

| Liste | Skeleton-Elemente |
|-------|-------------------|
| Transaktionen | Icon (rund) + 2 Textzeilen + Betrag |
| Familienmitglieder | Avatar + Name + Rolle |
| Anfragen | Status-Badge + Betrag + Grund |
| Benachrichtigungen | Icon + Titel + Zeit |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Liste laedt | Skeletons werden angezeigt |
| TC-002 | Daten ankommen | Skeletons durch Daten ersetzt |
| TC-003 | Schimmer-Animation | Laeuft kontinuierlich |
| TC-004 | Leere Liste nach Laden | Empty-State, keine Skeletons |
| TC-005 | Pull-to-Refresh | Keine Skeletons (Daten schon da) |

## Story Points

2

## Prioritaet

Mittel
