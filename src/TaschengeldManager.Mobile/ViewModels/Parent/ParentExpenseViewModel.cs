using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

/// <summary>
/// ViewModel for recording an expense on a child's account (M005-S04)
/// </summary>
[QueryProperty(nameof(ChildId), "childId")]
[QueryProperty(nameof(AccountId), "accountId")]
public partial class ParentExpenseViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;

    private Guid _childId;
    private Guid _accountId;

    [ObservableProperty]
    private string _childName = string.Empty;

    [ObservableProperty]
    private decimal _currentBalance;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSubmit))]
    [NotifyPropertyChangedFor(nameof(FormattedAmount))]
    private decimal _amount;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Sonstiges";

    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private bool _isSuccess;

    public bool CanSubmit => Amount > 0 && Amount <= CurrentBalance && !IsBusy;

    public string FormattedAmount => Amount.ToString("C", new System.Globalization.CultureInfo("de-DE"));

    public string FormattedCurrentBalance => CurrentBalance.ToString("C", new System.Globalization.CultureInfo("de-DE"));

    public List<string> Categories { get; } = ["Spielzeug", "Essen & Trinken", "Kleidung", "Unterhaltung", "Schule", "Sonstiges"];

    public List<decimal> QuickAmounts { get; } = [2m, 5m, 10m, 20m];

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

    public ParentExpenseViewModel(
        ITaschengeldApi api,
        INavigationService navigationService)
    {
        _api = api;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        if (AccountId != Guid.Empty)
        {
            try
            {
                var account = await _api.GetAccountAsync(AccountId);
                ChildName = account.OwnerName;
                CurrentBalance = account.Balance;
            }
            catch
            {
                ChildName = "Kind";
                CurrentBalance = 0;
            }
        }
    }

    [RelayCommand]
    private void SetQuickAmount(decimal amount)
    {
        Amount = amount;
        ClearValidation();
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (!ValidateInput())
            return;

        try
        {
            IsBusy = true;
            ClearError();
            ClearValidation();

            var request = new WithdrawRequest
            {
                Amount = Amount,
                Description = string.IsNullOrWhiteSpace(Description)
                    ? SelectedCategory
                    : $"{SelectedCategory}: {Description}",
                Category = SelectedCategory
            };

            var transaction = await _api.WithdrawAsync(request);

            IsSuccess = true;

            // Show success feedback
            await Shell.Current.DisplayAlert(
                "Ausgabe erfasst",
                $"{FormattedAmount} wurden vom Konto von {ChildName} abgebucht.",
                "OK");

            // Navigate back
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            SetError($"Ausgabe fehlgeschlagen: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.GoBackAsync();
    }

    private bool ValidateInput()
    {
        if (Amount <= 0)
        {
            ValidationError = "Bitte gib einen Betrag ein.";
            return false;
        }

        if (Amount > CurrentBalance)
        {
            ValidationError = $"Der Betrag ({FormattedAmount}) ist größer als das verfügbare Guthaben ({FormattedCurrentBalance}).";
            return false;
        }

        // Round to 2 decimal places
        Amount = Math.Round(Amount, 2);

        return true;
    }

    private void ClearValidation()
    {
        ValidationError = null;
    }

    partial void OnAmountChanged(decimal value)
    {
        OnPropertyChanged(nameof(CanSubmit));
    }

    partial void OnCurrentBalanceChanged(decimal value)
    {
        OnPropertyChanged(nameof(CanSubmit));
        OnPropertyChanged(nameof(FormattedCurrentBalance));
    }
}
