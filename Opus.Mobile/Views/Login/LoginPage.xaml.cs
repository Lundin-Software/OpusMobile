using Mopups.Services;
using Opus.Mobile.Services.NavigationService;
using Opus.Mobile.ViewModels;
using Opus.Mobile.Views.Popup;
using Opus.Mobile.Views.UserFlows;

namespace Opus.Mobile.Views.Login;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel viewModel;

    public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = this.viewModel = viewModel;
    }

    protected override bool OnBackButtonPressed() => true;

    private async void ImportSettings_Clicked(object sender, EventArgs e)
    {
        var status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            await PopupNavigationService.ShowWarning("Camera permission is required.");
            return;
        }

        var page = new ScanPage();

        await Navigation.PushModalAsync(page);

        var result = await page.Dismissed;

        if (string.IsNullOrWhiteSpace(result))
            return;

        if (!viewModel.ImportSettings(result, out var error))
        {
            await PopupNavigationService.ShowWarning(error);
            return;
        }

        await PopupNavigationService.ShowAlert("Settings imported.");
    }

    private async void ExportSettings_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PushAsync(new SettingQrPopup(viewModel));
    }
}