# Story M013-S06: Loading-States

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **Benutzer** moechte ich **sehen, wenn die App gerade Daten laedt oder eine Aktion verarbeitet**, damit **ich weiss, dass die App arbeitet und nicht haengt**.

## Akzeptanzkriterien

- [ ] Gegeben ein API-Aufruf, wenn er gestartet wird, dann wird ein Loading-Indikator angezeigt
- [ ] Gegeben ein laufender Loading-Zustand, wenn er angezeigt wird, dann ist die UI nicht blockiert
- [ ] Gegeben ein Button mit laufender Aktion, wenn die Aktion laeuft, dann ist der Button deaktiviert
- [ ] Gegeben ein Loading-Indikator, wenn die Aktion abgeschlossen ist, dann verschwindet er sofort

## UI-Entwurf

### Vollbild-Loading
```
+------------------------------------+
|                                    |
|                                    |
|                                    |
|           [Spinner]                |
|        Wird geladen...             |
|                                    |
|                                    |
|                                    |
+------------------------------------+
```

### Button-Loading
```
Normal:
+--------------------------------+
|       [Save] Speichern         |
+--------------------------------+

Loading:
+--------------------------------+
|    [Spinner] Speichern...      | <- Deaktiviert
+--------------------------------+
```

### Inline-Loading (Pull-to-Refresh)
```
+------------------------------------+
|                                    |
|     [Spinner] Aktualisiere...     |
+------------------------------------+
|                                    |
|  Transaktion 1                     |
|  Transaktion 2                     |
|  ...                               |
|                                    |
+------------------------------------+
```

## Technische Implementierung

### BaseViewModel mit IsBusy

```csharp
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private string _busyMessage = "Wird geladen...";

    protected async Task ExecuteAsync(Func<Task> action, string? loadingMessage = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            BusyMessage = loadingMessage ?? "Wird geladen...";
            await action();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

### Loading-Overlay Control

```csharp
public class LoadingOverlay : ContentView
{
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingOverlay), false);

    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(LoadingOverlay), "Wird geladen...");

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public LoadingOverlay()
    {
        var grid = new Grid
        {
            BackgroundColor = Color.FromArgb("#80000000"),
            IsVisible = false
        };

        var activityIndicator = new ActivityIndicator
        {
            IsRunning = true,
            Color = Colors.White,
            WidthRequest = 50,
            HeightRequest = 50,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        var label = new Label
        {
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 60, 0, 0)
        };
        label.SetBinding(Label.TextProperty, new Binding(nameof(Message), source: this));

        var stack = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children = { activityIndicator, label }
        };

        grid.Add(stack);
        Content = grid;

        SetBinding(IsVisibleProperty, new Binding(nameof(IsLoading), source: this));
    }
}
```

### XAML: Seite mit Loading-Overlay

```xml
<ContentPage xmlns:controls="clr-namespace:TaschengeldManager.Controls">
    <Grid>
        <!-- Hauptinhalt -->
        <ScrollView>
            <VerticalStackLayout>
                <!-- Content -->
            </VerticalStackLayout>
        </ScrollView>

        <!-- Loading Overlay -->
        <controls:LoadingOverlay
            IsLoading="{Binding IsBusy}"
            Message="{Binding BusyMessage}"/>
    </Grid>
</ContentPage>
```

### Loading-Button

```csharp
public class LoadingButton : Button
{
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingButton), false,
            propertyChanged: OnIsLoadingChanged);

    public static readonly BindableProperty LoadingTextProperty =
        BindableProperty.Create(nameof(LoadingText), typeof(string), typeof(LoadingButton), "Wird geladen...");

    private string? _originalText;

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var button = (LoadingButton)bindable;
        var isLoading = (bool)newValue;

        if (isLoading)
        {
            button._originalText = button.Text;
            button.Text = button.LoadingText;
            button.IsEnabled = false;
        }
        else
        {
            button.Text = button._originalText ?? button.Text;
            button.IsEnabled = true;
        }
    }
}
```

### XAML: Loading-Button Verwendung

```xml
<controls:LoadingButton
    Text="Speichern"
    LoadingText="Speichern..."
    IsLoading="{Binding IsBusy}"
    Command="{Binding SaveCommand}"/>
```

### Pull-to-Refresh

```xml
<RefreshView IsRefreshing="{Binding IsRefreshing}"
             Command="{Binding RefreshCommand}">
    <CollectionView ItemsSource="{Binding Transactions}">
        <!-- Items -->
    </CollectionView>
</RefreshView>
```

```csharp
public partial class TransactionsViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _isRefreshing;

    [RelayCommand]
    private async Task RefreshAsync()
    {
        try
        {
            await LoadTransactionsAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
```

## Loading-States nach Kontext

| Kontext | Loading-Typ | Dauer |
|---------|-------------|-------|
| Seiten-Laden | Vollbild-Overlay | Bis Daten da |
| Button-Aktion | Button deaktiviert + Text | Bis fertig |
| Pull-to-Refresh | Inline-Spinner | Bis aktualisiert |
| Seitenwechsel | Navigation-Indikator | Kurz |

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | API-Aufruf startet | Loading-Overlay erscheint |
| TC-002 | API-Aufruf endet | Loading-Overlay verschwindet |
| TC-003 | Button waehrend Aktion | Deaktiviert mit Spinner |
| TC-004 | Pull-to-Refresh | Refresh-Indikator oben |
| TC-005 | Doppelklick auf Button | Zweiter Klick wird ignoriert |

## Story Points

1

## Prioritaet

Hoch
