using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.Views.Pages.Relative;

public partial class RelativeDashboardPage : ContentPage
{
    private readonly INavigationService _navigationService;

    public RelativeDashboardPage(INavigationService navigationService)
    {
        InitializeComponent();
        _navigationService = navigationService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // TODO: Load data with ViewModel
    }

    private void OnRefreshing(object sender, EventArgs e)
    {
        // TODO: Refresh data with ViewModel
        RefreshViewControl.IsRefreshing = false;
    }

    private async void OnSendGiftClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Routes.RelativeSendGift);
    }

    private async void OnShowAllGiftsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Routes.RelativeGifts);
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert(
            "Abmelden",
            "Möchtest du dich wirklich abmelden?",
            "Ja, abmelden",
            "Abbrechen");

        if (confirm)
        {
            await _navigationService.LogoutAsync();
        }
    }
}
