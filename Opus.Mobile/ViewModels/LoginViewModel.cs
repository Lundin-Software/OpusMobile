using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Opus.Mobile.Models;
using Opus.Mobile.Services.Modules.Authentication;
using Opus.Mobile.Services.NavigationService;
using Opus.Mobile.Shared.Authentication;
using Opus.Mobile.Shared.Models;

namespace Opus.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService authenticationService;
    private readonly ILoginPreferencesStore preferencesStore;

    public Func<Task>? AfterLoginAsync { get; set; }

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool rememberMe;

    [ObservableProperty]
    private bool showSettings;

    [ObservableProperty]
    private string apiBaseUrl = string.Empty;

    [ObservableProperty]
    private string tokenUsername = string.Empty;

    [ObservableProperty]
    private string tokenPassword = string.Empty;

    [ObservableProperty]
    private string loginTrace = string.Empty;

    public string Version => AppInfo.Current.VersionString;

    public LoginViewModel(
        IAuthenticationService authenticationService,
        ILoginPreferencesStore preferencesStore)
    {
        this.authenticationService = authenticationService;
        this.preferencesStore = preferencesStore;

        Title = "Login";

        LoadSavedSettings();
    }

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy)
        {
            return;
        }

        if (!IsValid(out var validationMessage))
        {
            await PopupNavigationService.ShowWarning(validationMessage, waitForDismissal: true);
            return;
        }

        try
        {
            IsBusy = true;
            LoginTrace = string.Empty;

            ApplyConnectionSettings();
            SaveSettings();

            await PopupNavigationService.ShowLoading("Logging in...");

            var loginResponse = await authenticationService.Login(new LoginRequest
            {
                Username = UserName.Trim(),
                Password = Password,
                TokenUsername = TokenUsername.Trim(),
                TokenPassword = TokenPassword
            });

            if (loginResponse.Failed || loginResponse.Data is null)
            {
                await ShowLoginFailure(loginResponse.Message, "Login failed.");
                return;
            }

            var userResponse = await authenticationService.GetUser();

            if (userResponse.Failed || userResponse.Data is null)
            {
                await ShowLoginFailure(userResponse.Message, "Could not load user details.");
                return;
            }

            ApplicationSettings.User = userResponse.Data;

            await PopupNavigationService.Pop();

            if (AfterLoginAsync is not null)
                await AfterLoginAsync();
        }
        catch (Exception ex)
        {
            await CaptureAndShowException("Something went wrong while trying to login.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleSettings()
    {
        ShowSettings = !ShowSettings;
    }

    [RelayCommand]
    private void ClearSavedLogin()
    {
        preferencesStore.Remove(Settings.UserName);
        preferencesStore.Remove(Settings.Password);
        preferencesStore.Remove(Settings.RememberMe);

        UserName = string.Empty;
        Password = string.Empty;
        RememberMe = false;
    }

    private void SaveSettings()
    {
        preferencesStore.Set(Settings.ApiBaseUrl, ApplicationSettings.ApiBaseUrl);
        preferencesStore.Set(Settings.TokenUserName, ApplicationSettings.TokenUsername);
        preferencesStore.Set(Settings.TokenPassword, ApplicationSettings.TokenPassword);
        preferencesStore.Set(Settings.RememberMe, RememberMe);

        if (RememberMe)
        {
            preferencesStore.Set(Settings.UserName, UserName.Trim());
            preferencesStore.Set(Settings.Password, Password);
            return;
        }

        preferencesStore.Remove(Settings.UserName);
        preferencesStore.Remove(Settings.Password);
    }

    private bool IsValid(out string message)
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            message = "Username is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            message = "Password is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(ApiBaseUrl))
        {
            message = "API URL is required.";
            return false;
        }

        if (!Uri.TryCreate(ApiBaseUrl, UriKind.Absolute, out var uri) ||
            uri.Scheme is not ("http" or "https"))
        {
            message = "API URL must be a valid HTTP or HTTPS address.";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private void LoadSavedSettings()
    {
        RememberMe = preferencesStore.Get(Settings.RememberMe, false);

        ApiBaseUrl = preferencesStore.Get(
            Settings.ApiBaseUrl,
            ApplicationSettings.ApiBaseUrl);

        TokenUsername = preferencesStore.Get(
            Settings.TokenUserName,
            ApplicationSettings.TokenUsername);

        TokenPassword = preferencesStore.Get(
            Settings.TokenPassword,
            ApplicationSettings.TokenPassword);

        if (!RememberMe)
            return;

        UserName = preferencesStore.Get(Settings.UserName, string.Empty);
        Password = preferencesStore.Get(Settings.Password, string.Empty);
    }

    private void ApplyConnectionSettings()
    {
        ApplicationSettings.ApiBaseUrl = ApiBaseUrl;
        ApplicationSettings.TokenUsername = TokenUsername.Trim();
        ApplicationSettings.TokenPassword = TokenPassword;
    }

    private async Task ShowLoginFailure(string responseMessage, string fallbackMessage)
    {
        await PopupNavigationService.Pop();

        var message = string.IsNullOrWhiteSpace(responseMessage)
            ? fallbackMessage
            : responseMessage;

        LoginTrace = message;

        await PopupNavigationService.ShowError(message, waitForDismissal: true);
    }
}
