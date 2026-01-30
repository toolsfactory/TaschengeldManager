using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.MoneyRequest;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Feedback;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Child;

public partial class ChildRequestsViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    [ObservableProperty]
    private ObservableCollection<MoneyRequestItem> _requests = [];

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private int _pendingCount;

    public ChildRequestsViewModel(
        ITaschengeldApi api,
        IAuthenticationService authService,
        INavigationService navigationService,
        IToastService toastService)
    {
        _api = api;
        _authService = authService;
        _navigationService = navigationService;
        _toastService = toastService;
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
            ClearError();

            var requests = await _api.GetMyRequestsAsync();
            Requests.Clear();

            foreach (var r in requests.OrderByDescending(x => x.CreatedAt))
            {
                Requests.Add(new MoneyRequestItem
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    Reason = r.Reason,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status,
                    ResponseNote = r.ResponseNote,
                    RespondedAt = r.RespondedAt,
                    RespondedByName = r.RespondedByName
                });
            }

            PendingCount = Requests.Count(r => r.Status == RequestStatus.Pending);
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
    private async Task CreateRequestAsync()
    {
        var amountStr = await Shell.Current.DisplayPromptAsync(
            "Geld anfragen",
            "Wie viel Geld moechtest du anfragen?",
            "Weiter",
            "Abbrechen",
            "0,00",
            keyboard: Keyboard.Numeric);

        if (string.IsNullOrEmpty(amountStr)) return;

        if (!decimal.TryParse(amountStr.Replace(',', '.'), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var amount) || amount <= 0)
        {
            await _toastService.ShowErrorAsync("Bitte gib einen gueltigen Betrag ein.");
            return;
        }

        var reason = await Shell.Current.DisplayPromptAsync(
            "Begruendung",
            "Warum brauchst du das Geld? (mind. 5 Zeichen)",
            "Anfragen",
            "Abbrechen",
            "z.B. Fuer ein neues Buch");

        if (string.IsNullOrEmpty(reason)) return;

        if (reason.Length < 5)
        {
            await _toastService.ShowErrorAsync("Die Begruendung muss mindestens 5 Zeichen haben.");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new CreateMoneyRequestRequest
            {
                Amount = amount,
                Reason = reason
            };

            await _api.CreateRequestAsync(request);
            await _toastService.ShowSuccessAsync("Deine Anfrage wurde gesendet!");
            await LoadDataAsync();
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
    private async Task WithdrawRequestAsync(MoneyRequestItem? request)
    {
        if (request == null || !request.CanWithdraw) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Anfrage zurueckziehen",
            "Moechtest du diese Anfrage wirklich zurueckziehen?",
            "Ja, zurueckziehen",
            "Abbrechen");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _api.WithdrawRequestAsync(request.Id);
            await _toastService.ShowSuccessAsync("Anfrage zurueckgezogen.");
            await LoadDataAsync();
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
}

public class MoneyRequestItem
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public RequestStatus Status { get; set; }
    public string? ResponseNote { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? RespondedByName { get; set; }

    public bool HasResponse => !string.IsNullOrEmpty(ResponseNote);
    public bool CanWithdraw => Status == RequestStatus.Pending;

    public string FormattedAmount => Amount.ToString("C", new System.Globalization.CultureInfo("de-DE"));
    public string FormattedDate => CreatedAt.ToString("dd.MM.yyyy HH:mm");
    public string FormattedRespondedDate => RespondedAt?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty;

    public string StatusText => Status switch
    {
        RequestStatus.Pending => "Ausstehend",
        RequestStatus.Approved => "Genehmigt",
        RequestStatus.Rejected => "Abgelehnt",
        RequestStatus.Withdrawn => "Zurueckgezogen",
        _ => "Unbekannt"
    };

    public Color StatusColor => Status switch
    {
        RequestStatus.Pending => Colors.Orange,
        RequestStatus.Approved => Colors.Green,
        RequestStatus.Rejected => Colors.Red,
        RequestStatus.Withdrawn => Colors.Gray,
        _ => Colors.Gray
    };
}
