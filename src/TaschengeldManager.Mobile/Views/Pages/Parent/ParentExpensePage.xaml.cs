using TaschengeldManager.Mobile.ViewModels.Parent;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentExpensePage : ContentPage
{
    private readonly ParentExpenseViewModel _viewModel;

    public ParentExpensePage(ParentExpenseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
