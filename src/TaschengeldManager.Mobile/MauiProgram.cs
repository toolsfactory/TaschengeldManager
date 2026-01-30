using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using TaschengeldManager.Mobile.Data;
using TaschengeldManager.Mobile.Services.Api;
using TaschengeldManager.Mobile.Services.Auth;
using TaschengeldManager.Mobile.Services.Feedback;
using TaschengeldManager.Mobile.Services.Navigation;
using TaschengeldManager.Mobile.Services.Storage;
using TaschengeldManager.Mobile.Services.Sync;
using TaschengeldManager.Mobile.ViewModels.Auth;
using TaschengeldManager.Mobile.ViewModels.Parent;
using TaschengeldManager.Mobile.ViewModels.Child;
using TaschengeldManager.Mobile.ViewModels.Relative;
using TaschengeldManager.Mobile.ViewModels.Shared;
using TaschengeldManager.Mobile.Views.Pages.Auth;
using TaschengeldManager.Mobile.Views.Pages.Parent;
using TaschengeldManager.Mobile.Views.Pages.Child;
using TaschengeldManager.Mobile.Views.Pages.Relative;
using TaschengeldManager.Mobile.Views.Pages.Shared;

namespace TaschengeldManager.Mobile;

public static class MauiProgram
{
    // Platform-specific API URL configuration
    // Android Emulator uses 10.0.2.2 as alias to host's localhost
    // Using HTTP to avoid SSL certificate issues during development
    private static string ApiBaseUrl
    {
        get
        {
#if DEBUG
            // Development URLs
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "http://10.0.2.2:5041"; // Android Emulator -> Host
            else
                return "http://localhost:5041"; // Windows/iOS/Mac
#else
            // Production URL - configure for your deployed API
            return "https://api.taschengeldmanager.de";
#endif
        }
    }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        ConfigureServices(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Storage services
        services.AddSingleton<ISecureStorageService, SecureStorageService>();

        // Local database for offline caching
        services.AddSingleton<ILocalDatabase, LocalDatabase>();

        // Auth services - register as singleton to maintain state
        services.AddSingleton<IAuthenticationService, AuthenticationService>();

        // Biometric services
        services.AddSingleton<IBiometricService, BiometricService>();

        // Navigation services
        services.AddSingleton<INavigationService, NavigationService>();

        // Connectivity services
        services.AddSingleton<IConnectivityService, ConnectivityService>();

        // Feedback services
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();

        // HTTP client with authentication handler
        services.AddTransient<AuthenticatedHttpClientHandler>();

        // Configure Refit API client
        services.AddRefitClient<ITaschengeldApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

        // Register ViewModels
        RegisterViewModels(services);

        // Register Pages
        RegisterPages(services);
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        // Auth ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<ChildLoginViewModel>();
        services.AddTransient<MfaVerifyViewModel>();
        services.AddTransient<BiometricSetupViewModel>();

        // Parent ViewModels
        services.AddTransient<ParentDashboardViewModel>();
        services.AddTransient<ParentFamilyViewModel>();
        services.AddTransient<ParentPaymentsViewModel>();
        services.AddTransient<ParentChildAccountsViewModel>();
        services.AddTransient<ParentAccountDetailViewModel>();
        services.AddTransient<ParentDepositViewModel>();
        services.AddTransient<ParentExpenseViewModel>();
        services.AddTransient<ParentAddChildViewModel>();
        services.AddTransient<ParentInviteViewModel>();
        services.AddTransient<ParentRequestsViewModel>();

        // Child ViewModels
        services.AddTransient<ChildDashboardViewModel>();
        services.AddTransient<ChildHistoryViewModel>();
        services.AddTransient<ChildRequestsViewModel>();
        services.AddTransient<ChildAddExpenseViewModel>();
        services.AddTransient<ChildTransactionDetailViewModel>();

        // Relative ViewModels
        services.AddTransient<RelativeDashboardViewModel>();
        services.AddTransient<RelativeGiftsViewModel>();

        // Shared ViewModels
        services.AddTransient<SettingsViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        // Auth Pages
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<ChildLoginPage>();
        services.AddTransient<MfaVerifyPage>();
        services.AddTransient<BiometricSetupPage>();

        // Parent Pages
        services.AddTransient<ParentDashboardPage>();
        services.AddTransient<ParentFamilyPage>();
        services.AddTransient<ParentPaymentsPage>();
        services.AddTransient<ParentChildAccountsPage>();
        services.AddTransient<ParentAccountDetailPage>();
        services.AddTransient<ParentDepositPage>();
        services.AddTransient<ParentExpensePage>();
        services.AddTransient<ParentAddChildPage>();
        services.AddTransient<ParentInvitePage>();
        services.AddTransient<ParentRequestsPage>();

        // Child Pages
        services.AddTransient<ChildDashboardPage>();
        services.AddTransient<ChildHistoryPage>();
        services.AddTransient<ChildRequestsPage>();
        services.AddTransient<ChildAddExpensePage>();
        services.AddTransient<ChildTransactionDetailPage>();

        // Relative Pages
        services.AddTransient<RelativeDashboardPage>();
        services.AddTransient<RelativeGiftsPage>();

        // Shared Pages
        services.AddTransient<SettingsPage>();
    }
}
