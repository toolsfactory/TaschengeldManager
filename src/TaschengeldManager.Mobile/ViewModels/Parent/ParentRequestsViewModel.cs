using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.MoneyRequest;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Feedback;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

public partial class ParentRequestsViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    [ObservableProperty]
    private ObservableCollection<ParentMoneyRequestItem> _requests = [];

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private string _filterStatus = "all";

    public ParentRequestsViewModel(
        ITaschengeldApi api,
        INavigationService navigationService,
        IToastService toastService)
    {
        _api = api;
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

    [RelayCommand]
    private async Task FilterChangedAsync(string status)
    {
        FilterStatus = status;
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();

            string? statusFilter = FilterStatus == "all" ? null : FilterStatus;
            var requests = await _api.GetFamilyRequestsAsync(statusFilter);
            Requests.Clear();

            foreach (var r in requests.OrderByDescending(x => x.CreatedAt))
            {
                Requests.Add(new ParentMoneyRequestItem
                {
                    Id = r.Id,
                    ChildName = r.ChildName,
                    ChildUserId = r.ChildUserId,
                    Amount = r.Amount,
                    Reason = r.Reason,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status,
                    ResponseNote = r.ResponseNote,
                    RespondedAt = r.RespondedAt,
                    RespondedByName = r.RespondedByName
                });
            }

            PendingCount = requests.Count(r => r.Status == RequestStatus.Pending);
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
    private async Task ApproveRequestAsync(ParentMoneyRequestItem? request)
    {
        if (request == null || !request.IsPending) return;

        var note = await Shell.Current.DisplayPromptAsync(
            "Anfrage genehmigen",
            $"Moechtest du {request.FormattedAmount} fuer {request.ChildName} genehmigen?",
            "Genehmigen",
            "Abbrechen",
            "Optionale Nachricht...",
            maxLength: 500);

        if (note == null) return; // Cancelled

        try
        {
            IsBusy = true;

            var response = new RespondToRequestRequest
            {
                Approve = true,
                Note = string.IsNullOrWhiteSpace(note) ? null : note
            };

            await _api.RespondToRequestAsync(request.Id, response);
            await _toastService.ShowSuccessAsync($"Anfrage genehmigt! {request.FormattedAmount} wurde {request.ChildName} gutgeschrieben.");
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
    private async Task RejectRequestAsync(ParentMoneyRequestItem? request)
    {
        if (request == null || !request.IsPending) return;

        var note = await Shell.Current.DisplayPromptAsync(
            "Anfrage ablehnen",
            $"Moechtest du die Anfrage von {request.ChildName} ueber {request.FormattedAmount} ablehnen?",
            "Ablehnen",
            "Abbrechen",
            "Grund fuer Ablehnung...",
            maxLength: 500);

        if (note == null) return; // Cancelled

        try
        {
            IsBusy = true;

            var response = new RespondToRequestRequest
            {
                Approve = false,
                Note = string.IsNullOrWhiteSpace(note) ? null : note
            };

            await _api.RespondToRequestAsync(request.Id, response);
            await _toastService.ShowSuccessAsync("Anfrage abgelehnt.");
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

public class ParentMoneyRequestItem
{
    public Guid Id { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public Guid ChildUserId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public RequestStatus Status { get; set; }
    public string? ResponseNote { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? RespondedByName { get; set; }

    public bool IsPending => Status == RequestStatus.Pending;
    public bool HasResponse => !string.IsNullOrEmpty(ResponseNote);

    public string FormattedAmount => Amount.ToString("C", new System.Globalization.CultureInfo("de-DE"));
    public string FormattedDate => CreatedAt.ToString("dd.MM.yyyy HH:mm");
    public string FormattedRespondedDate => RespondedAt?.ToString("dd.MM.yyyy HH:mm") ?? string.Empty;

    public string Initials => string.IsNullOrEmpty(ChildName) ? "?" : ChildName[0].ToString().ToUpper();

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
