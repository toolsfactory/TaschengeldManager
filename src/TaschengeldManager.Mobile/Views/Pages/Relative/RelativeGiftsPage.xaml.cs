using TaschengeldManager.Mobile.ViewModels.Relative;

namespace TaschengeldManager.Mobile.Views.Pages.Relative;

public partial class RelativeGiftsPage : ContentPage
{
    public RelativeGiftsPage(RelativeGiftsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RelativeGiftsViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
