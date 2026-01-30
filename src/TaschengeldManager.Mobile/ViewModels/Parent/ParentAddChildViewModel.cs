using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Feedback;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

[QueryProperty(nameof(FamilyId), "familyId")]
public partial class ParentAddChildViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    [ObservableProperty]
    private Guid _familyId;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string _pin = string.Empty;

    [ObservableProperty]
    private string _pinConfirmation = string.Empty;

    [ObservableProperty]
    private decimal _initialBalance;

    [ObservableProperty]
    private string _nicknameError = string.Empty;

    [ObservableProperty]
    private string _pinError = string.Empty;

    [ObservableProperty]
    private string _pinConfirmationError = string.Empty;

    [ObservableProperty]
    private bool _canSave;

    public ParentAddChildViewModel(
        ITaschengeldApi api,
        INavigationService navigationService,
        IToastService toastService)
    {
        _api = api;
        _navigationService = navigationService;
        _toastService = toastService;
    }

    partial void OnNicknameChanged(string value) => ValidateForm();
    partial void OnPinChanged(string value) => ValidateForm();
    partial void OnPinConfirmationChanged(string value) => ValidateForm();

    private void ValidateForm()
    {
        // Reset errors
        NicknameError = string.Empty;
        PinError = string.Empty;
        PinConfirmationError = string.Empty;

        bool isValid = true;

        // Validate nickname
        if (string.IsNullOrWhiteSpace(Nickname))
        {
            NicknameError = "Bitte gib einen Spitznamen ein.";
            isValid = false;
        }
        else if (Nickname.Length < 2)
        {
            NicknameError = "Der Spitzname muss mindestens 2 Zeichen haben.";
            isValid = false;
        }
        else if (Nickname.Length > 50)
        {
            NicknameError = "Der Spitzname darf maximal 50 Zeichen haben.";
            isValid = false;
        }

        // Validate PIN
        if (string.IsNullOrWhiteSpace(Pin))
        {
            PinError = "Bitte gib eine PIN ein.";
            isValid = false;
        }
        else if (Pin.Length != 4 || !Pin.All(char.IsDigit))
        {
            PinError = "Die PIN muss genau 4 Ziffern haben.";
            isValid = false;
        }

        // Validate PIN confirmation
        if (string.IsNullOrWhiteSpace(PinConfirmation))
        {
            PinConfirmationError = "Bitte bestaetige die PIN.";
            isValid = false;
        }
        else if (Pin != PinConfirmation)
        {
            PinConfirmationError = "Die PINs stimmen nicht ueberein.";
            isValid = false;
        }

        CanSave = isValid;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!CanSave) return;

        try
        {
            IsBusy = true;
            ClearError();

            var request = new AddChildRequest
            {
                Nickname = Nickname.Trim(),
                Pin = Pin,
                InitialBalance = InitialBalance
            };

            var child = await _api.AddChildAsync(FamilyId, request);
            await _toastService.ShowSuccessAsync($"Kind '{child.Nickname}' wurde hinzugefuegt!");
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            await _toastService.ShowErrorAsync($"Fehler: {ex.Message}");
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
}
