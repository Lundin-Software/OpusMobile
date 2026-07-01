using Mopups.Pages;
using Mopups.Services;
using Opus.Mobile.ViewModels;

namespace Opus.Mobile.Views.Popup;

public partial class SettingQrPopup : PopupPage
{
	public SettingQrPopup(LoginViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
        viewModel.GenerateSettings();
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
}