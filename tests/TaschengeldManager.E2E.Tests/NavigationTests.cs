using System.Text.RegularExpressions;

namespace TaschengeldManager.E2E.Tests;

/// <summary>
/// E2E tests for navigation and page accessibility.
/// </summary>
[TestFixture]
public class NavigationTests : PlaywrightTestBase
{
    [Test]
    public async Task HomePage_ShouldRedirectToLogin_WhenNotAuthenticated()
    {
        await NavigateToAsync("/");
        await WaitForBlazorAsync();

        // Should redirect to login page when not authenticated
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }

    [Test]
    public async Task DashboardPage_ShouldRequireAuthentication()
    {
        await NavigateToAsync("/dashboard");
        await WaitForBlazorAsync();

        // Should redirect to login page when not authenticated
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }

    [Test]
    public async Task AccountsPage_ShouldRequireAuthentication()
    {
        await NavigateToAsync("/accounts");
        await WaitForBlazorAsync();

        // Should redirect to login page when not authenticated
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }

    [Test]
    public async Task FamilyPage_ShouldRequireAuthentication()
    {
        await NavigateToAsync("/family");
        await WaitForBlazorAsync();

        // Should redirect to login page when not authenticated
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }

    [Test]
    public async Task SettingsPage_ShouldRequireAuthentication()
    {
        await NavigateToAsync("/settings/mfa");
        await WaitForBlazorAsync();

        // Should redirect to login page when not authenticated
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }
}
