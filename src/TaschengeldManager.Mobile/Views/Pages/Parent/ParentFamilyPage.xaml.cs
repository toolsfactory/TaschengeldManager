using TaschengeldManager.Mobile.ViewModels.Parent;

namespace TaschengeldManager.Mobile.Views.Pages.Parent;

public partial class ParentFamilyPage : ContentPage
{
    public ParentFamilyPage(ParentFamilyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ParentFamilyViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
}
