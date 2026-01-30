using TaschengeldManager.Mobile.ViewModels.Parent;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentRequestsPage : ContentPage
{
    private readonly ParentRequestsViewModel _viewModel;

    public ParentRequestsPage(ParentRequestsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
