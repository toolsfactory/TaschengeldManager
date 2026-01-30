using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaschengeldManager.Core.DTOs.Family;
using TaschengeldManager.Core.Enums;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Feedback;
using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.ViewModels.Parent;

[QueryProperty(nameof(FamilyId), "familyId")]
public partial class ParentInviteViewModel : BaseViewModel
{
    private readonly ITaschengeldApi _api;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    [ObservableProperty]
    private Guid _familyId;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _relationshipDescription = string.Empty;

    [ObservableProperty]
    private string _emailError = string.Empty;

    [ObservableProperty]
    private bool _canInvite;

    [ObservableProperty]
    private ObservableCollection<InvitationItem> _pendingInvitations = [];

    [ObservableProperty]
    private bool _isRefreshing;

    public ParentInviteViewModel(
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
        await LoadInvitationsAsync();
    }

    partial void OnEmailChanged(string value) => ValidateForm();

    private void ValidateForm()
    {
        EmailError = string.Empty;
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(Email))
        {
            EmailError = "Bitte gib eine E-Mail-Adresse ein.";
            isValid = false;
        }
        else if (!IsValidEmail(Email))
        {
            EmailError = "Bitte gib eine gueltige E-Mail-Adresse ein.";
            isValid = false;
        }

        CanInvite = isValid;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    [RelayCommand]
    private async Task InviteAsync()
    {
        if (!CanInvite) return;

        try
        {
            IsBusy = true;
            ClearError();

            var request = new InviteRequest
            {
                Email = Email.Trim(),
                Role = UserRole.Relative,
                RelationshipDescription = string.IsNullOrWhiteSpace(RelationshipDescription)
                    ? null
                    : RelationshipDescription.Trim()
            };

            var invitation = await _api.InviteAsync(FamilyId, request);
            await _toastService.ShowSuccessAsync($"Einladung an {invitation.InvitedEmail} gesendet!");

            // Reset form
            Email = string.Empty;
            RelationshipDescription = string.Empty;

            // Reload invitations
            await LoadInvitationsAsync();
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
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadInvitationsAsync();
        IsRefreshing = false;
    }

    private async Task LoadInvitationsAsync()
    {
        try
        {
            var invitations = await _api.GetInvitationsAsync(FamilyId);
            PendingInvitations.Clear();

            foreach (var inv in invitations.Where(i => i.Status == InvitationStatus.Pending))
            {
                PendingInvitations.Add(new InvitationItem
                {
                    Id = inv.Id,
                    Email = inv.InvitedEmail,
                    RelationshipDescription = inv.RelationshipDescription ?? string.Empty,
                    CreatedAt = inv.CreatedAt,
                    ExpiresAt = inv.ExpiresAt
                });
            }
        }
        catch (Exception ex)
        {
            SetError($"Fehler beim Laden der Einladungen: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task WithdrawInvitationAsync(InvitationItem? invitation)
    {
        if (invitation == null) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Einladung zurueckziehen",
            $"Moechtest du die Einladung an {invitation.Email} wirklich zurueckziehen?",
            "Ja, zurueckziehen",
            "Abbrechen");

        if (!confirmed) return;

        try
        {
            IsBusy = true;
            await _api.WithdrawInvitationAsync(FamilyId, invitation.Id);
            await _toastService.ShowSuccessAsync("Einladung zurueckgezogen.");
            PendingInvitations.Remove(invitation);
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
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }
}

public class InvitationItem
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string RelationshipDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string FormattedCreatedAt => CreatedAt.ToString("dd.MM.yyyy");
    public string FormattedExpiresAt => ExpiresAt.ToString("dd.MM.yyyy");
    public bool IsExpiringSoon => ExpiresAt <= DateTime.UtcNow.AddDays(2);
}
