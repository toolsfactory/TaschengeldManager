using TaschengeldManager.Mobile.ViewModels.Child;

namespace TaschengeldManager.Mobile.Views.Pages.Child;

public partial class ChildHistoryPage : ContentPage
{
    public ChildHistoryPage(ChildHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ChildHistoryViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
