# Story M013-S04: Validierungs-Feedback

## Epic

M013 - Error Handling & User Feedback

## User Story

Als **Benutzer** moechte ich **bei Formulareingaben sofort sehen, wenn etwas nicht stimmt**, damit **ich Fehler schnell korrigieren kann**.

## Akzeptanzkriterien

- [ ] Gegeben ein Pflichtfeld, wenn es leer gelassen wird, dann wird ein Hinweis angezeigt
- [ ] Gegeben ein Eingabefeld, wenn die Eingabe ungueltig ist, dann wird das Feld rot markiert
- [ ] Gegeben ein Formular mit Fehlern, wenn der Benutzer absenden will, dann wird zum ersten Fehler gescrollt
- [ ] Gegeben eine korrigierte Eingabe, wenn sie gueltig wird, dann verschwindet die Fehlermeldung sofort
- [ ] Gegeben ein Formular, wenn alle Eingaben gueltig sind, dann ist der Absenden-Button aktiv

## UI-Entwurf

### Eingabefeld-Zustaende
```
Normal:
+--------------------------------+
| Betrag                         |
| +----------------------------+ |
| |                            | |
| +----------------------------+ |
+--------------------------------+

Fokussiert:
+--------------------------------+
| Betrag                         |
| +----------------------------+ | <- Blaue Border
| | 10                         | |
| +----------------------------+ |
+--------------------------------+

Fehler:
+--------------------------------+
| Betrag                         |
| +----------------------------+ | <- Rote Border
| |                            | |
| +----------------------------+ |
| [X] Bitte gib einen Betrag ein |  <- Roter Text
+--------------------------------+

Erfolgreich:
+--------------------------------+
| Betrag                         |
| +----------------------------+ | <- Gruene Border
| | 10,00                      | |
| +----------------------------+ |
| [Check] Gueltig                |  <- Gruener Text
+--------------------------------+
```

### Formular mit Fehlern
```
+------------------------------------+
|  <- Zurueck      Ausgabe erfassen  |
+------------------------------------+
|                                    |
|  Betrag *                          |
|  +--------------------------------+|
|  |                                || <- Rot
|  +--------------------------------+|
|  [X] Bitte gib einen Betrag ein   |
|                                    |
|  Kategorie *                       |
|  +--------------------------------+|
|  | Bitte auswaehlen           [v] || <- Rot
|  +--------------------------------+|
|  [X] Bitte waehle eine Kategorie  |
|                                    |
|  Beschreibung                      |
|  +--------------------------------+|
|  | Eis am Strand                  ||
|  +--------------------------------+|
|                                    |
|  +--------------------------------+|
|  |       [Save] Speichern         || <- Deaktiviert
|  +--------------------------------+|
|                                    |
+------------------------------------+
```

## Technische Implementierung

### Validierungs-Attribute

```csharp
public class CreateExpenseRequest
{
    [Required(ErrorMessage = "Bitte gib einen Betrag ein")]
    [Range(0.01, 10000, ErrorMessage = "Betrag muss zwischen 0,01 und 10.000 EUR liegen")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Bitte waehle eine Kategorie")]
    public string Category { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Beschreibung darf maximal 200 Zeichen haben")]
    public string? Description { get; set; }
}
```

### ValidatableObject

```csharp
public class ValidatableObject<T> : ObservableObject
{
    private T? _value;
    private bool _isValid = true;
    private string _errorMessage = string.Empty;

    public T? Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                Validate();
            }
        }
    }

    public bool IsValid
    {
        get => _isValid;
        private set => SetProperty(ref _isValid, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public List<Func<T?, ValidationResult>> Validations { get; } = new();

    public bool Validate()
    {
        ErrorMessage = string.Empty;
        IsValid = true;

        foreach (var validation in Validations)
        {
            var result = validation(Value);
            if (!result.IsValid)
            {
                ErrorMessage = result.ErrorMessage;
                IsValid = false;
                return false;
            }
        }

        return true;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Error(string message) => new() { IsValid = false, ErrorMessage = message };
}
```

### ViewModel mit Validierung

```csharp
public partial class CreateExpenseViewModel : BaseViewModel
{
    public ValidatableObject<decimal?> Amount { get; } = new();
    public ValidatableObject<string> Category { get; } = new();
    public ValidatableObject<string> Description { get; } = new();

    public CreateExpenseViewModel()
    {
        // Validierungsregeln definieren
        Amount.Validations.Add(value =>
            value.HasValue && value.Value > 0
                ? ValidationResult.Success()
                : ValidationResult.Error("Bitte gib einen gueltigen Betrag ein"));

        Amount.Validations.Add(value =>
            !value.HasValue || value.Value <= 10000
                ? ValidationResult.Success()
                : ValidationResult.Error("Betrag darf maximal 10.000 EUR sein"));

        Category.Validations.Add(value =>
            !string.IsNullOrWhiteSpace(value)
                ? ValidationResult.Success()
                : ValidationResult.Error("Bitte waehle eine Kategorie"));

        Description.Validations.Add(value =>
            string.IsNullOrEmpty(value) || value.Length <= 200
                ? ValidationResult.Success()
                : ValidationResult.Error("Maximal 200 Zeichen erlaubt"));
    }

    public bool IsFormValid =>
        Amount.IsValid &&
        Category.IsValid &&
        Description.IsValid;

    [RelayCommand(CanExecute = nameof(IsFormValid))]
    private async Task SaveAsync()
    {
        // Alle Felder validieren
        if (!ValidateAll())
        {
            return;
        }

        // Speichern
        await _expenseService.CreateAsync(new CreateExpenseRequest
        {
            Amount = Amount.Value!.Value,
            Category = Category.Value!,
            Description = Description.Value
        });

        await _toastService.ShowSuccessAsync("Ausgabe gespeichert");
        await Shell.Current.GoToAsync("..");
    }

    private bool ValidateAll()
    {
        var isAmountValid = Amount.Validate();
        var isCategoryValid = Category.Validate();
        var isDescriptionValid = Description.Validate();

        return isAmountValid && isCategoryValid && isDescriptionValid;
    }
}
```

### XAML: Validiertes Eingabefeld

```xml
<ContentPage xmlns:controls="clr-namespace:TaschengeldManager.Controls">

    <VerticalStackLayout Spacing="16">

        <!-- Validiertes Betrag-Feld -->
        <controls:ValidatedEntry
            Label="Betrag *"
            Placeholder="0,00"
            Value="{Binding Amount.Value}"
            IsValid="{Binding Amount.IsValid}"
            ErrorMessage="{Binding Amount.ErrorMessage}"
            Keyboard="Numeric"/>

        <!-- Validiertes Picker-Feld -->
        <controls:ValidatedPicker
            Label="Kategorie *"
            ItemsSource="{Binding Categories}"
            SelectedItem="{Binding Category.Value}"
            IsValid="{Binding Category.IsValid}"
            ErrorMessage="{Binding Category.ErrorMessage}"/>

        <!-- Optionales Textfeld -->
        <controls:ValidatedEntry
            Label="Beschreibung"
            Placeholder="Optional"
            Value="{Binding Description.Value}"
            IsValid="{Binding Description.IsValid}"
            ErrorMessage="{Binding Description.ErrorMessage}"
            MaxLength="200"/>

        <Button Text="Speichern"
                Command="{Binding SaveCommand}"
                IsEnabled="{Binding IsFormValid}"/>

    </VerticalStackLayout>
</ContentPage>
```

### Custom Control: ValidatedEntry

```csharp
public class ValidatedEntry : ContentView
{
    public static readonly BindableProperty LabelProperty = ...;
    public static readonly BindableProperty ValueProperty = ...;
    public static readonly BindableProperty IsValidProperty = ...;
    public static readonly BindableProperty ErrorMessageProperty = ...;

    public ValidatedEntry()
    {
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };

        var label = new Label();
        label.SetBinding(Label.TextProperty, new Binding(nameof(Label), source: this));

        var entry = new Entry();
        entry.SetBinding(Entry.TextProperty, new Binding(nameof(Value), source: this, mode: BindingMode.TwoWay));

        // Border-Farbe basierend auf Validierung
        var trigger = new DataTrigger(typeof(Entry))
        {
            Binding = new Binding(nameof(IsValid), source: this),
            Value = false
        };
        trigger.Setters.Add(new Setter { Property = Entry.BackgroundColorProperty, Value = Colors.MistyRose });

        entry.Triggers.Add(trigger);

        var errorLabel = new Label { TextColor = Colors.Red, FontSize = 12 };
        errorLabel.SetBinding(Label.TextProperty, new Binding(nameof(ErrorMessage), source: this));
        errorLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(IsValid), source: this, converter: new InverseBoolConverter()));

        grid.Add(label, 0, 0);
        grid.Add(entry, 0, 1);
        grid.Add(errorLabel, 0, 2);

        Content = grid;
    }
}
```

## Testfaelle

| ID | Szenario | Erwartung |
|----|----------|-----------|
| TC-001 | Pflichtfeld leer | Fehlermeldung wird angezeigt |
| TC-002 | Betrag negativ | Validierungsfehler |
| TC-003 | Beschreibung > 200 Zeichen | Validierungsfehler |
| TC-004 | Korrektur der Eingabe | Fehler verschwindet |
| TC-005 | Alle Felder gueltig | Button wird aktiv |
| TC-006 | Submit mit Fehlern | Scroll zum ersten Fehler |

## Story Points

2

## Prioritaet

Hoch
