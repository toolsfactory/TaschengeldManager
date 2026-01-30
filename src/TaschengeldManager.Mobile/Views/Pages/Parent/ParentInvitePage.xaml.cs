using TaschengeldManager.Mobile.ViewModels.Parent;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentInvitePage : ContentPage
{
    private readonly ParentInviteViewModel _viewModel;

    public ParentInvitePage(ParentInviteViewModel viewModel)
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
