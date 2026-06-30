namespace Opus.Mobile.Services.NavigationService;

public static class PageNavigationService
{
    private static INavigation Navigation => Shell.Current is not null ?
        Shell.Current.Navigation :
        Application.Current.Windows[0].Navigation;

    //Push a new page to the stack
    public static async Task Push(Page page, bool animated = true) =>
        await MainThread.InvokeOnMainThreadAsync(async () => Navigation.PushAsync(page, animated));

    //Removes the last page from the stack
    public static async Task Pop(bool animated = true)
    {
        if (Navigation.NavigationStack.Count > 0)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => await Navigation.PopAsync(animated));
        }
    }

    //Removes all pages from the stack except the root page
    public static async Task PopToRoot(bool animated = true)
    {
        if (Navigation.NavigationStack.Count > 0)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => Navigation.PopToRootAsync(animated));
        }
    }

    //Navigates to the given route
    public static async Task GoTo(string uri, Dictionary<string, object>? queryParameters = null, bool animated = true)
    {
        if (Shell.Current is not null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await Shell.Current.GoToAsync(uri, animated, [.. queryParameters ?? []]));
        }
    }
}
