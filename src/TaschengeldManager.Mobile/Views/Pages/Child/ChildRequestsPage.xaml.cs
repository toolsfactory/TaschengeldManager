using TaschengeldManager.Mobile.ViewModels.Child;

namespace TaschengeldManager.Mobile.Views.Pages.Child;

public partial class ChildRequestsPage : ContentPage
{
    public ChildRequestsPage(ChildRequestsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ChildRequestsViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
