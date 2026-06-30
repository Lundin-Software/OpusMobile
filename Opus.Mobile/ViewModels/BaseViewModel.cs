using CommunityToolkit.Mvvm.ComponentModel;
using Opus.Mobile.Services.NavigationService;

namespace Opus.Mobile.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    public async Task CaptureAndShowException(string message, Exception exception)
    {
        await PopupNavigationService.Pop();
        await PopupNavigationService.ShowError(message);

        IsBusy = false;
    }

    public async Task ShowError(string message)
    {
        await PopupNavigationService.Pop();
        await PopupNavigationService.ShowError(message);

        IsBusy = false;
    }
}
