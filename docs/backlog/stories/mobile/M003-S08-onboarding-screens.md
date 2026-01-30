# Story M003-S08: Onboarding-Screens

## Epic
M003 - Dashboard & Navigation

## Status
Offen

## User Story

Als **neuer Benutzer** möchte ich **beim ersten Start eine Einführung in die App sehen**, damit **ich verstehe, wie die App funktioniert und was ich damit machen kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein erster App-Start, wenn die App geöffnet wird, dann werden Onboarding-Screens angezeigt
- [ ] Gegeben die Onboarding-Screens, wenn durch sie navigiert wird, dann kann zwischen den Screens gewischt werden
- [ ] Gegeben der letzte Screen, wenn "Los geht's" gedrückt wird, dann wird zum Login/Register navigiert
- [ ] Gegeben ein wiederholter App-Start, wenn Onboarding abgeschlossen wurde, dann wird es nicht erneut angezeigt

## UI-Entwurf

```
┌─────────────────────────────┐
│            ○ ● ○            │
├─────────────────────────────┤
│                             │
│         [Illustration]      │
│            💰               │
│                             │
│   Taschengeld verwalten     │
│                             │
│   Behalte den Überblick     │
│   über das Taschengeld      │
│   deiner Kinder - einfach   │
│   und übersichtlich.        │
│                             │
│                             │
│                             │
│  ┌───────────────────────┐  │
│  │      Weiter           │  │
│  └───────────────────────┘  │
│                             │
│       Überspringen          │
│                             │
└─────────────────────────────┘
```

## Technische Hinweise

### OnboardingPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:TaschengeldManager.Mobile.ViewModels"
             x:Class="TaschengeldManager.Mobile.Views.OnboardingPage"
             x:DataType="vm:OnboardingViewModel"
             Shell.NavBarIsVisible="False"
             BackgroundColor="{StaticResource BackgroundLight}">

    <Grid RowDefinitions="Auto,*,Auto,Auto">

        <!-- Page Indicator -->
        <HorizontalStackLayout Grid.Row="0"
                               HorizontalOptions="Center"
                               Spacing="8"
                               Margin="0,48,0,0">
            <Ellipse WidthRequest="8" HeightRequest="8"
                     Fill="{Binding Page1IndicatorColor}" />
            <Ellipse WidthRequest="8" HeightRequest="8"
                     Fill="{Binding Page2IndicatorColor}" />
            <Ellipse WidthRequest="8" HeightRequest="8"
                     Fill="{Binding Page3IndicatorColor}" />
        </HorizontalStackLayout>

        <!-- CarouselView -->
        <CarouselView Grid.Row="1"
                      ItemsSource="{Binding OnboardingItems}"
                      CurrentItemChangedCommand="{Binding PageChangedCommand}"
                      CurrentItemChangedCommandParameter="{Binding Source={RelativeSource Self}, Path=CurrentItem}"
                      Position="{Binding CurrentPosition}"
                      Loop="False"
                      IndicatorView="None">
            <CarouselView.ItemTemplate>
                <DataTemplate x:DataType="vm:OnboardingItemViewModel">
                    <VerticalStackLayout VerticalOptions="Center"
                                         Padding="32"
                                         Spacing="24">
                        <!-- Illustration -->
                        <Image Source="{Binding ImageSource}"
                               HeightRequest="200"
                               Aspect="AspectFit" />

                        <!-- Titel -->
                        <Label Text="{Binding Title}"
                               FontSize="28"
                               FontAttributes="Bold"
                               HorizontalTextAlignment="Center" />

                        <!-- Beschreibung -->
                        <Label Text="{Binding Description}"
                               FontSize="16"
                               HorizontalTextAlignment="Center"
                               TextColor="{StaticResource TextSecondaryLight}"
                               LineHeight="1.4" />
                    </VerticalStackLayout>
                </DataTemplate>
            </CarouselView.ItemTemplate>
        </CarouselView>

        <!-- Weiter/Los geht's Button -->
        <Button Grid.Row="2"
                Text="{Binding ButtonText}"
                Command="{Binding NextCommand}"
                Style="{StaticResource PrimaryButton}"
                Margin="32,0" />

        <!-- Überspringen -->
        <Label Grid.Row="3"
               Text="Überspringen"
               TextDecorations="Underline"
               TextColor="{StaticResource TextSecondaryLight}"
               HorizontalTextAlignment="Center"
               Margin="0,16,0,48"
               IsVisible="{Binding CanSkip}">
            <Label.GestureRecognizers>
                <TapGestureRecognizer Command="{Binding SkipCommand}" />
            </Label.GestureRecognizers>
        </Label>

    </Grid>

</ContentPage>
```

### OnboardingViewModel.cs
```csharp
public partial class OnboardingViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private const string OnboardingCompletedKey = "onboarding_completed";

    [ObservableProperty]
    private int _currentPosition;

    [ObservableProperty]
    private ObservableCollection<OnboardingItemViewModel> _onboardingItems;

    public OnboardingViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        OnboardingItems = new ObservableCollection<OnboardingItemViewModel>
        {
            new OnboardingItemViewModel
            {
                ImageSource = "onboarding_1.png",
                Title = "Taschengeld verwalten",
                Description = "Behalte den Überblick über das Taschengeld deiner Kinder - einfach und übersichtlich."
            },
            new OnboardingItemViewModel
            {
                ImageSource = "onboarding_2.png",
                Title = "Sparziele setzen",
                Description = "Hilf deinen Kindern, Sparziele zu erreichen und den Umgang mit Geld zu lernen."
            },
            new OnboardingItemViewModel
            {
                ImageSource = "onboarding_3.png",
                Title = "Familie verbinden",
                Description = "Lade Großeltern und Verwandte ein, damit sie Geschenke direkt überweisen können."
            }
        };
    }

    public bool CanSkip => CurrentPosition < OnboardingItems.Count - 1;
    public string ButtonText => CurrentPosition == OnboardingItems.Count - 1
        ? "Los geht's"
        : "Weiter";

    public Color Page1IndicatorColor => GetIndicatorColor(0);
    public Color Page2IndicatorColor => GetIndicatorColor(1);
    public Color Page3IndicatorColor => GetIndicatorColor(2);

    private Color GetIndicatorColor(int page) =>
        CurrentPosition == page
            ? Color.FromArgb("#4CAF50") // Primary
            : Color.FromArgb("#E0E0E0"); // Gray

    [RelayCommand]
    private void PageChanged(OnboardingItemViewModel? item)
    {
        OnPropertyChanged(nameof(ButtonText));
        OnPropertyChanged(nameof(CanSkip));
        OnPropertyChanged(nameof(Page1IndicatorColor));
        OnPropertyChanged(nameof(Page2IndicatorColor));
        OnPropertyChanged(nameof(Page3IndicatorColor));
    }

    [RelayCommand]
    private async Task NextAsync()
    {
        if (CurrentPosition < OnboardingItems.Count - 1)
        {
            CurrentPosition++;
        }
        else
        {
            await CompleteOnboardingAsync();
        }
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        await CompleteOnboardingAsync();
    }

    private async Task CompleteOnboardingAsync()
    {
        Preferences.Default.Set(OnboardingCompletedKey, true);
        await _navigationService.NavigateToAsync("//login");
    }

    public static bool IsOnboardingCompleted =>
        Preferences.Default.Get(OnboardingCompletedKey, false);
}

public class OnboardingItemViewModel
{
    public string ImageSource { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

### App.xaml.cs - Onboarding Check
```csharp
public partial class App : Application
{
    protected override void OnStart()
    {
        // Prüfen ob Onboarding bereits abgeschlossen
        if (!OnboardingViewModel.IsOnboardingCompleted)
        {
            MainPage = new OnboardingPage();
        }
        else
        {
            MainPage = new AppShell();
        }
    }
}
```

### Animierte Übergänge
```csharp
// Parallax-Effekt für Images
public class ParallaxCarouselBehavior : Behavior<CarouselView>
{
    protected override void OnAttachedTo(CarouselView carousel)
    {
        base.OnAttachedTo(carousel);
        carousel.Scrolled += OnScrolled;
    }

    private void OnScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        // Parallax-Berechnung basierend auf Scroll-Position
        // Animation der Bilder
    }
}
```

## Testfälle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-M003-29 | Erster App-Start | Onboarding wird angezeigt |
| TC-M003-30 | Zwischen Screens wischen | Navigation funktioniert |
| TC-M003-31 | "Los geht's" drücken | Navigation zum Login |
| TC-M003-32 | Überspringen | Onboarding wird übersprungen |
| TC-M003-33 | Zweiter App-Start | Kein Onboarding |

## Story Points
3

## Priorität
Niedrig
