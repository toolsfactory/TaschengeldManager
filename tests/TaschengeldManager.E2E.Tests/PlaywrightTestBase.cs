using System.Text.RegularExpressions;

namespace TaschengeldManager.E2E.Tests;

/// <summary>
/// Base class for Playwright E2E tests.
/// </summary>
public class PlaywrightTestBase : PageTest
{
    /// <summary>
    /// Base URL for the Blazor WebAssembly application.
    /// Set via environment variable or default to localhost.
    /// </summary>
    protected string BaseUrl => Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "https://localhost:5001";

    /// <summary>
    /// Navigate to a page in the application.
    /// </summary>
    protected async Task NavigateToAsync(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");
    }

    /// <summary>
    /// Wait for the Blazor app to be fully loaded.
    /// </summary>
    protected async Task WaitForBlazorAsync()
    {
        // Wait for any loading indicators to disappear
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Additional wait for Blazor WASM to hydrate
        await Page.WaitForTimeoutAsync(500);
    }

    /// <summary>
    /// Login as a parent user.
    /// </summary>
    protected async Task LoginAsParentAsync(string email, string password)
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        await Page.FillAsync("input[type='email']", email);
        await Page.FillAsync("input[type='password']", password);
        await Page.ClickAsync("button[type='submit']");

        await WaitForBlazorAsync();
    }

    /// <summary>
    /// Login as a child user.
    /// </summary>
    protected async Task LoginAsChildAsync(string familyCode, string nickname, string pin)
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        // Switch to child login tab if present
        var childTab = Page.Locator("text=Kind");
        if (await childTab.IsVisibleAsync())
        {
            await childTab.ClickAsync();
        }

        await Page.FillAsync("input[placeholder*='Familien']", familyCode);
        await Page.FillAsync("input[placeholder*='Spitzname']", nickname);
        await Page.FillAsync("input[type='password']", pin);
        await Page.ClickAsync("button[type='submit']");

        await WaitForBlazorAsync();
    }

    /// <summary>
    /// Logout the current user.
    /// </summary>
    protected async Task LogoutAsync()
    {
        var logoutButton = Page.Locator("button:has-text('Abmelden'), a:has-text('Abmelden')");
        if (await logoutButton.IsVisibleAsync())
        {
            await logoutButton.ClickAsync();
            await WaitForBlazorAsync();
        }
    }

    /// <summary>
    /// Assert that user is on the dashboard.
    /// </summary>
    protected async Task AssertOnDashboardAsync()
    {
        await Expect(Page).ToHaveURLAsync(new Regex(".*/dashboard.*"));
    }

    /// <summary>
    /// Assert that user is on the login page.
    /// </summary>
    protected async Task AssertOnLoginPageAsync()
    {
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }
}
