using Mopups.Pages;
using Mopups.Services;
using Opus.Mobile.Views.Popup;

namespace Opus.Mobile.Services.NavigationService;

public static class PopupNavigationService
{
    public static IReadOnlyList<PopupPage> PopupStack => MopupService.Instance.PopupStack;

    public static Task Push(PopupPage popup, bool animated = true)
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
            await MopupService.Instance.PushAsync(popup, animated));
    }

    public static Task Pop(bool animated = true)
    {
        if (PopupStack.Count == 0)
            return Task.CompletedTask;

        return MainThread.InvokeOnMainThreadAsync(async () =>
            await MopupService.Instance.PopAsync(animated));
    }

    public static Task ShowLoading(string message = "Loading...", bool animated = true)
    {
        return Push(new LoadingPopup(message), animated);
    }

    public static async Task ShowMessage(
        string title,
        string message,
        bool waitForDismissal = false,
        bool animated = true)
    {
        var popup = new MessagePopup(title, message);

        await Push(popup, animated);

        if (waitForDismissal)
            await popup.Dismissed;
        else
            await popup.Opened;
    }

    public static Task ShowAlert(string message, bool waitForDismissal = false, bool animated = true)
    {
        return ShowMessage("Alert", message, waitForDismissal, animated);
    }

    public static Task ShowWarning(string message, bool waitForDismissal = false, bool animated = true)
    {
        return ShowMessage("Warning", message, waitForDismissal, animated);
    }

    public static Task ShowError(string message, bool waitForDismissal = false, bool animated = true)
    {
        return ShowMessage("Error", message, waitForDismissal, animated);
    }
}
