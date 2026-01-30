using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace TaschengeldManager.Mobile.Services.Feedback;

/// <summary>
/// Toast/Snackbar service implementation using CommunityToolkit.Maui (M013-S02)
/// </summary>
public class ToastService : IToastService
{
    public async Task ShowSuccessAsync(string message, int durationMs = 3000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Success,
            DurationMs = durationMs
        });
    }

    public async Task ShowErrorAsync(string message, int durationMs = 5000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Error,
            DurationMs = durationMs
        });
    }

    public async Task ShowWarningAsync(string message, int durationMs = 4000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Warning,
            DurationMs = durationMs
        });
    }

    public async Task ShowInfoAsync(string message, int durationMs = 3000)
    {
        await ShowAsync(new ToastOptions
        {
            Message = message,
            Type = ToastType.Info,
            DurationMs = durationMs
        });
    }

    public async Task ShowAsync(ToastOptions options)
    {
        var (backgroundColor, textColor, icon) = GetStyleForType(options.Type);

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            ActionButtonTextColor = textColor,
            CornerRadius = new CornerRadius(8),
            Font = Font.SystemFontOfSize(14),
            CharacterSpacing = 0.5
        };

        var displayMessage = $"{icon} {options.Message}";

        if (options.ActionCallback != null && !string.IsNullOrEmpty(options.ActionText))
        {
            var snackbar = Snackbar.Make(
                displayMessage,
                action: options.ActionCallback,
                actionButtonText: options.ActionText,
                duration: TimeSpan.FromMilliseconds(options.DurationMs),
                visualOptions: snackbarOptions);

            await snackbar.Show();
        }
        else
        {
            // Use simple toast for messages without action
            var toast = Toast.Make(
                displayMessage,
                options.DurationMs <= 3000 ? ToastDuration.Short : ToastDuration.Long,
                14);

            await toast.Show();
        }
    }

    private static (Color Background, Color Text, string Icon) GetStyleForType(ToastType type)
    {
        return type switch
        {
            ToastType.Success => (Color.FromArgb("#22C55E"), Colors.White, "\u2714"), // Check mark
            ToastType.Error => (Color.FromArgb("#EF4444"), Colors.White, "\u2716"),   // X mark
            ToastType.Warning => (Color.FromArgb("#F59E0B"), Colors.White, "\u26A0"), // Warning sign
            ToastType.Info => (Color.FromArgb("#3B82F6"), Colors.White, "\u2139"),    // Info symbol
            _ => (Colors.Gray, Colors.White, "")
        };
    }
}
