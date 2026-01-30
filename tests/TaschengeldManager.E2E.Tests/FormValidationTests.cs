using System.Text.RegularExpressions;

namespace TaschengeldManager.E2E.Tests;

/// <summary>
/// E2E tests for form validation.
/// </summary>
[TestFixture]
public class FormValidationTests : PlaywrightTestBase
{
    [Test]
    public async Task RegistrationForm_ShouldValidateEmail()
    {
        await NavigateToAsync("/register");
        await WaitForBlazorAsync();

        // Enter invalid email
        await Page.FillAsync("input[type='email']", "notanemail");
        await Page.FillAsync("input[type='password']", "ValidPassword123!");

        // Try to submit
        await Page.ClickAsync("button[type='submit']");
        await WaitForBlazorAsync();

        // Should show validation error or stay on page
        var emailInput = Page.Locator("input[type='email']");
        var isInvalid = await emailInput.EvaluateAsync<bool>("el => !el.validity.valid");

        Assert.That(isInvalid, Is.True, "Email field should be invalid");
    }

    [Test]
    public async Task RegistrationForm_ShouldRequireAllFields()
    {
        await NavigateToAsync("/register");
        await WaitForBlazorAsync();

        // Try to submit without filling any fields
        await Page.ClickAsync("button[type='submit']");
        await WaitForBlazorAsync();

        // Should stay on registration page
        await Expect(Page).ToHaveURLAsync(new Regex(".*/register.*"));
    }

    [Test]
    public async Task LoginForm_ShouldRequireEmail()
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        // Only fill password
        await Page.FillAsync("input[type='password']", "somepassword");
        await Page.ClickAsync("button[type='submit']");
        await WaitForBlazorAsync();

        // Should stay on login page
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }

    [Test]
    public async Task LoginForm_ShouldRequirePassword()
    {
        await NavigateToAsync("/login");
        await WaitForBlazorAsync();

        // Only fill email
        await Page.FillAsync("input[type='email']", "test@example.com");
        await Page.ClickAsync("button[type='submit']");
        await WaitForBlazorAsync();

        // Should stay on login page
        await Expect(Page).ToHaveURLAsync(new Regex(".*/login.*"));
    }
}
