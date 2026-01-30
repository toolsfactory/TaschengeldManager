using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Child;

public partial class ChildHistoryViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<HistoryTransactionItem> _transactions = [];

    [ObservableProperty]
    private ObservableCollection<HistoryTransactionItem> _allTransactions = [];

    [ObservableProperty]
    private string _currentFilter = "all";

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddMonths(-1);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today;

    [ObservableProperty]
    private DateTime _minDate = DateTime.Today.AddYears(-2);

    [ObservableProperty]
    private DateTime _maxDate = DateTime.Today;

    [ObservableProperty]
    private int _transactionCount;

    [ObservableProperty]
    private decimal _totalAmount;

    [ObservableProperty]
    private Color _totalAmountColor = Colors.Gray;

    [ObservableProperty]
    private string _emptyMessage = "Deine Kontobewegungen erscheinen hier";

    // Filter button styles
    public Style AllFilterStyle => CurrentFilter == "all" ? GetSelectedStyle() : GetUnselectedStyle();
    public Style IncomeFilterStyle => CurrentFilter == "income" ? GetSelectedStyle() : GetUnselectedStyle();
    public Style ExpenseFilterStyle => CurrentFilter == "expense" ? GetSelectedStyle() : GetUnselectedStyle();

    public ChildHistoryViewModel(
        ITaschengeldApi api,
        IAuthenticationService authService,
        INavigationService navigationService)
    {
        _api = api;
        _authService = authService;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadDataAsync();
        IsRefreshing = false;
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;

            // TODO: Load actual data from API
            // var transactions = await _api.GetMyTransactionsAsync();
            // AllTransactions = new ObservableCollection<HistoryTransactionItem>(
            //     transactions.Select(t => new HistoryTransactionItem { ... }));

            ApplyFilter();
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

    [RelayCommand]
    private void Search()
    {
        ApplyFilter();
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        CurrentFilter = filter;
        OnPropertyChanged(nameof(AllFilterStyle));
        OnPropertyChanged(nameof(IncomeFilterStyle));
        OnPropertyChanged(nameof(ExpenseFilterStyle));
        ApplyFilter();
    }

    [RelayCommand]
    private async Task TransactionSelectedAsync(HistoryTransactionItem? transaction)
    {
        if (transaction is null)
            return;

        await _navigationService.NavigateToAsync(Routes.ChildTransactionDetail, new Dictionary<string, object>
        {
            { "TransactionId", transaction.Id }
        });
    }

    partial void OnStartDateChanged(DateTime value)
    {
        ApplyFilter();
    }

    partial void OnEndDateChanged(DateTime value)
    {
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = AllTransactions.AsEnumerable();

        // Apply type filter
        filtered = CurrentFilter switch
        {
            "income" => filtered.Where(t => t.Amount > 0),
            "expense" => filtered.Where(t => t.Amount < 0),
            _ => filtered
        };

        // Apply date filter
        filtered = filtered.Where(t => t.Date.Date >= StartDate.Date && t.Date.Date <= EndDate.Date);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(t =>
                t.Description.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                (t.Category?.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ?? false));
        }

        var filteredList = filtered.OrderByDescending(t => t.Date).ToList();
        Transactions = new ObservableCollection<HistoryTransactionItem>(filteredList);

        // Update summary
        TransactionCount = filteredList.Count;
        TotalAmount = filteredList.Sum(t => t.Amount);
        TotalAmountColor = TotalAmount >= 0 ? Colors.Green : Colors.Red;

        // Update empty message
        EmptyMessage = AllTransactions.Count == 0
            ? "Deine Kontobewegungen erscheinen hier"
            : "Keine Transaktionen für diesen Filter gefunden";
    }

    private static Style GetSelectedStyle()
    {
        return Application.Current?.Resources["PrimaryButton"] as Style ?? new Style(typeof(Button));
    }

    private static Style GetUnselectedStyle()
    {
        return Application.Current?.Resources["OutlineButton"] as Style ?? new Style(typeof(Button));
    }
}

public class HistoryTransactionItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Category { get; set; }
    public bool HasCategory => !string.IsNullOrEmpty(Category);
    public bool IsIncome => Amount > 0;
    public bool IsInterestTransaction { get; set; }
    public decimal? InterestRate { get; set; }

    public string AmountFormatted => Amount >= 0 ? $"+{Amount:C}" : Amount.ToString("C");
    public Color TypeColor => IsInterestTransaction ? Colors.Teal : (IsIncome ? Colors.Green : Colors.Red);
    public string TypeIcon => IsInterestTransaction ? "📈" : (IsIncome ? "💰" : "💸");
}
