using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.Views.Pages.Auth;
using TaschengeldManager.Mobile.Views.Pages.Parent;
using TaschengeldManager.Mobile.Views.Pages.Child;
using TaschengeldManager.Mobile.Views.Pages.Relative;

namespace TaschengeldManager.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        // Auth routes
        Routing.RegisterRoute(Routes.Register, typeof(RegisterPage));
        Routing.RegisterRoute(Routes.ChildLogin, typeof(ChildLoginPage));
        Routing.RegisterRoute(Routes.MfaVerify, typeof(MfaVerifyPage));
        Routing.RegisterRoute(Routes.BiometricSetup, typeof(BiometricSetupPage));

        // Parent routes (detail pages, not in TabBar)
        Routing.RegisterRoute(Routes.ParentChildAccounts, typeof(ParentChildAccountsPage));
        Routing.RegisterRoute(Routes.ParentAccountDetail, typeof(ParentAccountDetailPage));
        Routing.RegisterRoute(Routes.ParentDeposit, typeof(ParentDepositPage));
        Routing.RegisterRoute(Routes.ParentExpense, typeof(ParentExpensePage));
        Routing.RegisterRoute(Routes.ParentAddChild, typeof(ParentAddChildPage));
        Routing.RegisterRoute(Routes.ParentInvite, typeof(ParentInvitePage));
        Routing.RegisterRoute(Routes.ParentRequests, typeof(ParentRequestsPage));
        // Routing.RegisterRoute(Routes.ParentInterest, typeof(ParentInterestPage));

        // Child routes (detail pages, not in TabBar)
        Routing.RegisterRoute(Routes.ChildAddExpense, typeof(ChildAddExpensePage));
        Routing.RegisterRoute(Routes.ChildTransactionDetail, typeof(ChildTransactionDetailPage));
        // Routing.RegisterRoute(Routes.ChildRequest, typeof(ChildRequestPage));

        // Relative routes (detail pages, not in TabBar)
        // Routing.RegisterRoute(Routes.RelativeSendGift, typeof(RelativeSendGiftPage));
    }
}
