using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Child;

public partial class ChildDashboardViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private string _balanceFormatted = "0,00 €";

    [ObservableProperty]
    private string _lastUpdated = "Jetzt";

    [ObservableProperty]
    private bool _isBalanceAnimating;

    [ObservableProperty]
    private bool _hasInterestEnabled;

    [ObservableProperty]
    private string _interestInfo = string.Empty;

    [ObservableProperty]
    private decimal _interestRate;

    [ObservableProperty]
    private ObservableCollection<ChildTransactionItem> _recentTransactions = [];

    [ObservableProperty]
    private int _pendingRequestsCount;

    [ObservableProperty]
    private bool _hasPendingRequests;

    [ObservableProperty]
    private bool _isRefreshing;

    public ChildDashboardViewModel(
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
            // var account = await _api.GetMyAccountAsync();
            // Balance = account.Balance;
            // HasInterestEnabled = account.InterestEnabled;
            // InterestRate = account.InterestRate;

            // Trigger balance animation
            IsBalanceAnimating = false;
            await Task.Delay(50);
            IsBalanceAnimating = true;

            BalanceFormatted = Balance.ToString("C");
            LastUpdated = DateTime.Now.ToString("HH:mm");

            // Update interest info if enabled
            if (HasInterestEnabled && InterestRate > 0)
            {
                InterestInfo = $"{InterestRate:P2} p.a.";
            }

            // Reset animation flag after animation completes
            await Task.Delay(600);
            IsBalanceAnimating = false;
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
    private async Task AddExpenseAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ChildAddExpense);
    }

    [RelayCommand]
    private async Task RequestMoneyAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ChildRequest);
    }

    [RelayCommand]
    private async Task ShowAllTransactionsAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ChildHistory);
    }

    [RelayCommand]
    private async Task TransactionSelectedAsync(ChildTransactionItem? transaction)
    {
        if (transaction is null)
            return;

        await _navigationService.NavigateToAsync(Routes.ChildTransactionDetail, new Dictionary<string, object>
        {
            { "TransactionId", transaction.Id }
        });
    }

    [RelayCommand]
    private async Task ShowRequestsAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ChildRequests);
    }
}

public class ChildTransactionItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Category { get; set; }
    public bool IsIncome => Amount > 0;
    public bool HasCategory => !string.IsNullOrEmpty(Category);
    public bool IsInterestTransaction { get; set; }
    public decimal? InterestRate { get; set; }

    public string AmountFormatted => Amount >= 0 ? $"+{Amount:C}" : Amount.ToString("C");
    public Color TypeColor => IsInterestTransaction ? Colors.Teal : (IsIncome ? Colors.Green : Colors.Red);
    public string TypeIcon => IsInterestTransaction ? "📈" : (IsIncome ? "💰" : "💸");
}
