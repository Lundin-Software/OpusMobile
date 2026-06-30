using Mopups.Pages;

namespace Opus.Mobile.Views.Popup;

public partial class LoadingPopup : PopupPage
{
    public LoadingPopup(string loadingText, bool retryTextVisible = false, int retryNr = 0)
    {
        InitializeComponent();

        BindingContext = new LoadingPopupModel
        {
            LoadingText = loadingText,
            RetryTextVisible = retryTextVisible,
            RetryNr = retryNr
        };
    }

    protected override bool OnBackButtonPressed() => true;

    private sealed class LoadingPopupModel
    {
        public string LoadingText { get; init; } = "Loading...";

        public bool RetryTextVisible { get; init; }

        public int RetryNr { get; init; }
    }
}