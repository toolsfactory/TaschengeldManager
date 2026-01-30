using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

/// <summary>
/// ViewModel for displaying a child's account details with transaction history (M005-S02)
/// </summary>
[QueryProperty(nameof(ChildId), "childId")]
[QueryProperty(nameof(AccountId), "accountId")]
public partial class ParentAccountDetailViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;

    private Guid _childId;
    private Guid _accountId;

    [ObservableProperty]
    private string _childName = string.Empty;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private ObservableCollection<AccountTransactionGroup> _accountTransactionGroups = [];

    [ObservableProperty]
    private bool _hasTransactions;

    [ObservableProperty]
    private bool _isLoadingMore;

    private int _currentPage = 1;
    private const int PageSize = 20;
    private bool _hasMoreItems = true;

    public Guid ChildId
    {
        get => _childId;
        set
        {
            _childId = value;
            OnPropertyChanged();
        }
    }

    public Guid AccountId
    {
        get => _accountId;
        set
        {
            _accountId = value;
            OnPropertyChanged();
        }
    }

    public ParentAccountDetailViewModel(
        ITaschengeldApi api,
        INavigationService navigationService)
    {
        _api = api;
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
        _currentPage = 1;
        _hasMoreItems = true;
        AccountTransactionGroups.Clear();
        await LoadDataAsync();
        IsRefreshing = false;
    }

    private async Task LoadDataAsync()
    {
        if (AccountId == Guid.Empty)
        {
            SetError("Kein Konto ausgewählt");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            // Load account details
            var account = await _api.GetAccountAsync(AccountId);
            ChildName = account.OwnerName;
            Balance = account.Balance;

            // Load transactions
            var transactions = await _api.GetTransactionsAsync(AccountId, PageSize, 0);
            GroupTransactions(transactions);

            HasTransactions = AccountTransactionGroups.Count > 0;
            _hasMoreItems = transactions.Count >= PageSize;
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
    private async Task LoadMoreAsync()
    {
        if (IsLoadingMore || !_hasMoreItems || AccountId == Guid.Empty)
            return;

        try
        {
            IsLoadingMore = true;
            _currentPage++;

            var offset = (_currentPage - 1) * PageSize;
            var transactions = await _api.GetTransactionsAsync(AccountId, PageSize, offset);

            if (transactions.Count < PageSize)
                _hasMoreItems = false;

            GroupTransactions(transactions, append: true);
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Laden weiterer Transaktionen: {ex.Message}");
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    private void GroupTransactions(List<TransactionDto> transactions, bool append = false)
    {
        if (!append)
        {
            AccountTransactionGroups.Clear();
        }

        foreach (var tx in transactions)
        {
            var item = new AccountTransactionItem
            {
                Id = tx.Id,
                Amount = tx.Amount,
                Type = tx.Type,
                Description = tx.Description ?? GetDefaultDescription(tx.Type),
                Category = tx.Category,
                CreatedBy = tx.CreatedByName,
                BalanceAfter = tx.BalanceAfter,
                Date = tx.CreatedAt,
                IsDeposit = tx.Amount > 0
            };

            var dateKey = tx.CreatedAt.Date;
            var existingGroup = AccountTransactionGroups.FirstOrDefault(g => g.Date == dateKey);

            if (existingGroup != null)
            {
                existingGroup.Transactions.Add(item);
            }
            else
            {
                var newGroup = new AccountTransactionGroup
                {
                    Date = dateKey,
                    DateText = GetDateText(dateKey),
                    Transactions = [item]
                };

                // Insert in correct position (sorted by date descending)
                var insertIndex = AccountTransactionGroups.Count;
                for (int i = 0; i < AccountTransactionGroups.Count; i++)
                {
                    if (AccountTransactionGroups[i].Date < dateKey)
                    {
                        insertIndex = i;
                        break;
                    }
                }
                AccountTransactionGroups.Insert(insertIndex, newGroup);
            }
        }
    }

    private static string GetDateText(DateTime date)
    {
        var today = DateTime.Today;
        if (date == today)
            return "Heute";
        if (date == today.AddDays(-1))
            return "Gestern";
        if (date > today.AddDays(-7))
            return date.ToString("dddd", new System.Globalization.CultureInfo("de-DE"));

        return date.ToString("d. MMMM yyyy", new System.Globalization.CultureInfo("de-DE"));
    }

    private static string GetDefaultDescription(TransactionType type)
    {
        return type switch
        {
            TransactionType.Deposit => "Einzahlung",
            TransactionType.Withdrawal => "Ausgabe",
            TransactionType.Allowance => "Taschengeld",
            TransactionType.Gift => "Geschenk",
            TransactionType.Interest => "Zinsen",
            TransactionType.Correction => "Korrektur",
            _ => "Transaktion"
        };
    }

    [RelayCommand]
    private async Task TransactionSelectedAsync(AccountTransactionItem? transaction)
    {
        if (transaction == null) return;

        // Show transaction details in a popup
        var details = $"Betrag: {transaction.FormattedAmount}\n" +
                     $"Typ: {GetDefaultDescription(transaction.Type)}\n" +
                     $"Datum: {transaction.Date:dd.MM.yyyy HH:mm}\n" +
                     $"Erstellt von: {transaction.CreatedBy}\n" +
                     $"Kontostand danach: {transaction.BalanceAfter:C}";

        if (!string.IsNullOrEmpty(transaction.Category))
            details += $"\nKategorie: {transaction.Category}";

        await Shell.Current.DisplayAlert(
            transaction.Description ?? "Transaktion",
            details,
            "OK");
    }

    [RelayCommand]
    private async Task DepositAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ParentDeposit, new Dictionary<string, object>
        {
            { "childId", ChildId },
            { "accountId", AccountId }
        });
    }

    [RelayCommand]
    private async Task ExpenseAsync()
    {
        await _navigationService.NavigateToAsync(Routes.ParentExpense, new Dictionary<string, object>
        {
            { "childId", ChildId },
            { "accountId", AccountId }
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }
}

/// <summary>
/// Group of transactions by date
/// </summary>
public class AccountTransactionGroup
{
    public DateTime Date { get; set; }
    public string DateText { get; set; } = string.Empty;
    public ObservableCollection<AccountTransactionItem> Transactions { get; set; } = [];
}

/// <summary>
/// Display model for a single transaction
/// </summary>
public class AccountTransactionItem
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public decimal BalanceAfter { get; set; }
    public DateTime Date { get; set; }
    public bool IsDeposit { get; set; }

    public string FormattedAmount => Amount >= 0
        ? $"+{Amount:C}"
        : Amount.ToString("C", new System.Globalization.CultureInfo("de-DE"));

    public Color AmountColor => IsDeposit
        ? Color.FromArgb("#22C55E")  // Green
        : Color.FromArgb("#EF4444"); // Red

    public string TypeIcon => IsDeposit ? "▲" : "▼";
}
