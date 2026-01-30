using TaschengeldManager.Mobile.Services.Navigation;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentDashboardPage : ContentPage
{
    private readonly INavigationService _navigationService;

    public ParentDashboardPage(INavigationService navigationService)
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

    private async void OnCreateFamilyClicked(object sender, EventArgs e)
    {
        // TODO: Navigate to create family page
        await DisplayAlert("Info", "Familie erstellen wird implementiert", "OK");
    }

    private async void OnAddChildClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(Routes.ParentAddChild);
    }

    private async void OnChildSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not null)
        {
            // TODO: Navigate to child account detail
            await Shell.Current.GoToAsync(Routes.ParentAccountDetail);
            ((CollectionView)sender).SelectedItem = null;
        }
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
