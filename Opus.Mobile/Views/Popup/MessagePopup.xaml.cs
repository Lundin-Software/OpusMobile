using Mopups.Pages;
using Opus.Mobile.Services.NavigationService;

namespace Opus.Mobile.Views.Popup;

public partial class MessagePopup : PopupPage
{
    private readonly TaskCompletionSource dismissedSource = new();
    private readonly TaskCompletionSource openedSource = new();

    public Task Dismissed => dismissedSource.Task;

    public Task Opened => openedSource.Task;

    public MessagePopup(string title, string message)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        MessageLabel.Text = message;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        openedSource.TrySetResult();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        dismissedSource.TrySetResult();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await PopupNavigationService.Pop();
    }
}