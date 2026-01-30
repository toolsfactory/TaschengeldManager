using TaschengeldManager.Mobile.ViewModels.Parent;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentPaymentsPage : ContentPage
{
    public ParentPaymentsPage(ParentPaymentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ParentPaymentsViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
