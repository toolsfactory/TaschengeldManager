using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.ViewModels.Auth;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

public partial class ParentPaymentsViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<RecurringPaymentItem> _recurringPayments = [];

    [ObservableProperty]
    private ObservableCollection<PendingRequestItem> _pendingRequests = [];

    [ObservableProperty]
    private ObservableCollection<TransactionItem> _recentTransactions = [];

    [ObservableProperty]
    private int _pendingRequestsCount;

    [ObservableProperty]
    private bool _hasPendingRequests;

    [ObservableProperty]
    private bool _isRefreshing;

    public ParentPaymentsViewModel(
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
            // var payments = await _api.GetRecurringPaymentsAsync();
            // var requests = await _api.GetPendingRequestsAsync();
            // var transactions = await _api.GetRecentTransactionsAsync();

            PendingRequestsCount = PendingRequests.Count;
            HasPendingRequests = PendingRequestsCount > 0;
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
    private async Task DepositAsync()
    {
        // TODO: Show deposit dialog
        await Shell.Current.DisplayAlert("Info", "Einzahlung wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task WithdrawAsync()
    {
        // TODO: Show withdraw dialog
        await Shell.Current.DisplayAlert("Info", "Auszahlung wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task AddRecurringPaymentAsync()
    {
        // TODO: Navigate to add recurring payment page
        await Shell.Current.DisplayAlert("Info", "Neue Zahlung wird implementiert", "OK");
    }

    [RelayCommand]
    private async Task PaymentSelectedAsync(RecurringPaymentItem? payment)
    {
        if (payment == null) return;
        // TODO: Navigate to payment detail/edit page
    }

    [RelayCommand]
    private async Task ApproveRequestAsync(PendingRequestItem? request)
    {
        if (request == null) return;

        try
        {
            IsBusy = true;
            // TODO: Call API to approve request
            // await _api.ApproveRequestAsync(request.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            SetError($"Fehler: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RejectRequestAsync(PendingRequestItem? request)
    {
        if (request == null) return;

        var reason = await Shell.Current.DisplayPromptAsync(
            "Anfrage ablehnen",
            "Möchtest du einen Grund angeben?",
            "Ablehnen",
            "Abbrechen");

        if (reason != null)
        {
            try
            {
                IsBusy = true;
                // TODO: Call API to reject request
                // await _api.RejectRequestAsync(request.Id, reason);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                SetError($"Fehler: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

public class RecurringPaymentItem
{
    public Guid Id { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ScheduleDescription { get; set; } = string.Empty;
    public DateTime NextPaymentDate { get; set; }
    public bool IsActive { get; set; }
}

public class PendingRequestItem
{
    public Guid Id { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TransactionItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ChildName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public bool IsIncome => Amount > 0;

    public string AmountFormatted => Amount >= 0 ? $"+{Amount:C}" : Amount.ToString("C");
    public Color AmountColor => IsIncome ? Colors.Green : Colors.Red;
}
