using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Child;

public partial class ChildAddExpenseViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _amountText = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private ExpenseCategory? _selectedCategory;

    [ObservableProperty]
    private ObservableCollection<ExpenseCategory> _categories = [];

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private bool _canSave;

    public ChildAddExpenseViewModel(
        ITaschengeldApi api,
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _api = api;
        _authService = authService;
        _navigationService = navigationService;

        LoadCategories();
    }

    private void LoadCategories()
    {
        Categories =
        [
            new ExpenseCategory { Id = 1, Name = "Süßigkeiten", Icon = "🍬", Color = Colors.Pink },
            new ExpenseCategory { Id = 2, Name = "Spielzeug", Icon = "🎮", Color = Colors.Purple },
            new ExpenseCategory { Id = 3, Name = "Kleidung", Icon = "👕", Color = Colors.Blue },
            new ExpenseCategory { Id = 4, Name = "Bücher", Icon = "📚", Color = Colors.Brown },
            new ExpenseCategory { Id = 5, Name = "Essen", Icon = "🍕", Color = Colors.Orange },
            new ExpenseCategory { Id = 6, Name = "Hobby", Icon = "⚽", Color = Colors.Green },
            new ExpenseCategory { Id = 7, Name = "Geschenke", Icon = "🎁", Color = Colors.Red },
            new ExpenseCategory { Id = 8, Name = "Sonstiges", Icon = "📦", Color = Colors.Gray }
        ];
    }

    partial void OnAmountTextChanged(string value)
    {
        if (decimal.TryParse(value.Replace(",", "."), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var amount))
        {
            Amount = amount;
        }
        else
        {
            Amount = 0;
        }
        UpdateCanSave();
    }

    partial void OnDescriptionChanged(string value)
    {
        UpdateCanSave();
    }

    partial void OnSelectedCategoryChanged(ExpenseCategory? value)
    {
        UpdateCanSave();
    }

    private void UpdateCanSave()
    {
        CanSave = Amount > 0 &&
                  !string.IsNullOrWhiteSpace(Description) &&
                  SelectedCategory != null &&
                  !IsBusy;
    }

    [RelayCommand]
    private void SelectCategory(ExpenseCategory category)
    {
        SelectedCategory = category;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (!CanSave)
            return;

        try
        {
            IsBusy = true;
            UpdateCanSave();

            // TODO: Call API to save expense
            // var request = new CreateExpenseRequest
            // {
            //     Amount = Amount,
            //     Description = Description,
            //     CategoryId = SelectedCategory!.Id,
            //     Date = Date
            // };
            // await _api.CreateExpenseAsync(request);

            // Simulate API call
            await Task.Delay(500);

            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Speichern: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            UpdateCanSave();
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.GoBackAsync();
    }
}

public class ExpenseCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "📦";
    public Color Color { get; set; } = Colors.Gray;

    public string DisplayName => $"{Icon} {Name}";
}
