using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Child;

[QueryProperty(nameof(TransactionId), "TransactionId")]
public partial class ChildTransactionDetailViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Guid _transactionId;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _amountFormatted = string.Empty;

    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private string _dateFormatted = string.Empty;

    [ObservableProperty]
    private string? _category;

    [ObservableProperty]
    private string _categoryIcon = "📦";

    [ObservableProperty]
    private bool _hasCategory;

    [ObservableProperty]
    private bool _isIncome;

    [ObservableProperty]
    private Color _typeColor = Colors.Gray;

    [ObservableProperty]
    private string _typeName = string.Empty;

    [ObservableProperty]
    private string? _note;

    [ObservableProperty]
    private bool _hasNote;

    [ObservableProperty]
    private string? _createdBy;

    [ObservableProperty]
    private string _createdAt = string.Empty;

    [ObservableProperty]
    private bool _isInterestTransaction;

    [ObservableProperty]
    private decimal? _interestRate;

    public ChildTransactionDetailViewModel(
        ITaschengeldApi api,
        INavigationService navigationService)
    {
        _api = api;
        _navigationService = navigationService;
    }

    partial void OnTransactionIdChanged(Guid value)
    {
        if (value != Guid.Empty)
        {
            _ = LoadTransactionAsync();
        }
    }

    private async Task LoadTransactionAsync()
    {
        try
        {
            IsBusy = true;

            // TODO: Load actual data from API
            // var transaction = await _api.GetTransactionAsync(TransactionId);
            // MapTransactionToViewModel(transaction);

            // Demo data for now
            await Task.Delay(300);

            Description = "Beispiel-Transaktion";
            Amount = -5.50m;
            Date = DateTime.Today;
            Category = "Süßigkeiten";
            CategoryIcon = "🍬";
            Note = "Beispiel-Notiz";

            UpdateComputedProperties();
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Laden: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateComputedProperties()
    {
        IsIncome = Amount >= 0;
        TypeColor = IsIncome ? Colors.Green : Colors.Red;
        TypeName = IsIncome ? "Einnahme" : "Ausgabe";
        AmountFormatted = Amount >= 0 ? $"+{Amount:C}" : Amount.ToString("C");
        DateFormatted = Date.ToString("dddd, dd. MMMM yyyy");
        HasCategory = !string.IsNullOrEmpty(Category);
        HasNote = !string.IsNullOrEmpty(Note);
        CreatedAt = Date.ToString("dd.MM.yyyy HH:mm");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }
}
