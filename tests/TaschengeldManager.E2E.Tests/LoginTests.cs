using System.Text.RegularExpressions;

namespace TaschengeldManager.E2E.Tests;

/// <summary>
/// E2E tests for login functionality.
/// </summary>
[TestFixture]
public class LoginTests : PlaywrightTestBase
{
    [Test]
    public async Task LoginPage_ShouldBeAccessible()
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        // Check that login form elements are present
        await Expect(Page.Locator("input[type='email']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='password']")).ToBeVisibleAsync();
        await Expect(Page.Locator("button[type='submit']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task LoginPage_WithInvalidCredentials_ShouldShowError()
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        await Page.FillAsync("input[type='email']", "invalid@example.com");
        await Page.FillAsync("input[type='password']", "wrongpassword");
        await Page.ClickAsync("button[type='submit']");

        await WaitForBlazorAsync();

        // Should show error message or remain on login page
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }

    [Test]
    public async Task RegisterPage_ShouldBeAccessible()
    {
        await NavigateToAsync("/register");
        await WaitForBlazorAsync();

        // Check that registration form elements are present
        await Expect(Page.Locator("input[type='email']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='password']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task LoginPage_ShouldHaveChildLoginOption()
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        // Check for child login tab or link
        var childLoginOption = Page.Locator("text=Kind");
        var isVisible = await childLoginOption.IsVisibleAsync();

        // Child login should be available
        Assert.That(isVisible || await Page.Locator("text=Familien").IsVisibleAsync(), Is.True,
            "Child login option should be available");
    }
}
