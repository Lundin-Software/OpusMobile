using Opus.Mobile.ViewModels;

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
}