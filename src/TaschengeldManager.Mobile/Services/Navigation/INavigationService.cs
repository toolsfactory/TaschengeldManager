namespace TaschengeldManager.Mobile.Services.Navigation;

/// <summary>
/// Service for handling navigation within the app
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to a page by route
    /// </summary>
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);

    /// <summary>
    /// Navigate back
    /// </summary>
    Task GoBackAsync();

    /// <summary>
    /// Navigate to root
    /// </summary>
    Task GoToRootAsync();

    /// <summary>
    /// Navigate to login page and clear navigation stack
    /// </summary>
    Task NavigateToLoginAsync();

    /// <summary>
    /// Navigate to main page after login based on user role
    /// </summary>
    Task NavigateToMainAsync();

    /// <summary>
    /// Navigate to biometric setup page
    /// </summary>
    Task NavigateToBiometricSetupAsync();

    /// <summary>
    /// Logout the user and navigate to login page
    /// </summary>
    Task LogoutAsync();
}

/// <summary>
/// Route constants for navigation
/// </summary>
public static class Routes
{
    // Auth routes
    public const string Login = "login";
    public const string Register = "register";
    public const string ChildLogin = "childlogin";
    public const string MfaVerify = "mfaverify";
    public const string BiometricSetup = "biometricsetup";

    // Parent routes
    public const string ParentDashboard = "parent/dashboard";
    public const string ParentFamily = "parent/family";
    public const string ParentPayments = "parent/payments";
    public const string ParentSettings = "parent/settings";
    public const string ParentChildAccounts = "parent/childaccounts";
    public const string ParentAccountDetail = "parent/account";
    public const string ParentAddChild = "parent/addchild";
    public const string ParentInvite = "parent/invite";
    public const string ParentInterest = "parent/interest";
    public const string ParentDeposit = "parent/deposit";
    public const string ParentExpense = "parent/expense";
    public const string ParentRequests = "parent/requests";

    // Child routes
    public const string ChildDashboard = "child/dashboard";
    public const string ChildHistory = "child/history";
    public const string ChildRequests = "child/requests";
    public const string ChildRequest = "child/request";
    public const string ChildSettings = "child/settings";
    public const string ChildAddExpense = "child/addexpense";
    public const string ChildTransactionDetail = "child/transaction";

    // Relative routes
    public const string RelativeDashboard = "relative/dashboard";
    public const string RelativeGifts = "relative/gifts";
    public const string RelativeSettings = "relative/settings";
    public const string RelativeSendGift = "relative/sendgift";
}
