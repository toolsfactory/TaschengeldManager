using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Account;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

/// <summary>
/// ViewModel for depositing money into a child's account (M005-S03)
/// </summary>
[QueryProperty(nameof(ChildId), "childId")]
[QueryProperty(nameof(AccountId), "accountId")]
public partial class ParentDepositViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;

    private Guid _childId;
    private Guid _accountId;

    [ObservableProperty]
    private string _childName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeposit))]
    [NotifyPropertyChangedFor(nameof(FormattedAmount))]
    private decimal _amount;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Taschengeld";

    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private bool _isSuccess;

    public bool CanDeposit => Amount > 0 && !IsBusy;

    public string FormattedAmount => Amount.ToString("C", new System.Globalization.CultureInfo("de-DE"));

    public List<string> Categories { get; } = ["Taschengeld", "Geschenk", "Belohnung", "Sonstiges"];

    public List<decimal> QuickAmounts { get; } = [5m, 10m, 20m, 50m];

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

    public ParentDepositViewModel(
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
            }
            catch
            {
                ChildName = "Kind";
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
    private async Task DepositAsync()
    {
        if (!ValidateInput())
            return;

        try
        {
            IsBusy = true;
            ClearError();
            ClearValidation();

            var request = new DepositRequest
            {
                Amount = Amount,
                Description = string.IsNullOrWhiteSpace(Description)
                    ? SelectedCategory
                    : $"{SelectedCategory}: {Description}"
            };

            var transaction = await _api.DepositAsync(AccountId, request);

            IsSuccess = true;

            // Show success feedback
            await Shell.Current.DisplayAlert(
                "Einzahlung erfolgreich",
                $"{FormattedAmount} wurden auf das Konto von {ChildName} eingezahlt.",
                "OK");

            // Navigate back
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            SetError($"Einzahlung fehlgeschlagen: {ex.Message}");
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

        if (Amount > 10000)
        {
            ValidationError = "Der Betrag darf maximal 10.000 € betragen.";
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
        OnPropertyChanged(nameof(CanDeposit));
    }
}
