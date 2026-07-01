using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Opus.Mobile.Models;
using Opus.Mobile.Services.Modules.Authentication;
using Opus.Mobile.Services.NavigationService;
using Opus.Mobile.Shared.Authentication;
using Opus.Mobile.Shared.Models;
using System.Text;
using System.Text.Json;
using System.Web;

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

    [ObservableProperty]
    private string settingsJson = string.Empty;

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

    public void GenerateSettings()
    {
        var settings = new LoginSettingsModel
        {
            Username = UserName,
            Password = Password,
            ApiUrl = ApiBaseUrl,
            TokenUsername = TokenUsername,
            TokenPassword = TokenPassword
        };

        SettingsJson = JsonSerializer.Serialize(settings);
    }

    public bool ImportSettings(string value, out string error)
    {
        var settings = TryParseSettings(value);

        if (settings is null)
        {
            error = "Invalid settings QR code.";
            return false;
        }

        UserName = settings.Username?.Trim() ?? string.Empty;
        Password = settings.Password ?? string.Empty;
        ApiBaseUrl = settings.ApiUrl ?? string.Empty;
        TokenUsername = settings.TokenUsername?.Trim() ?? string.Empty;
        TokenPassword = settings.TokenPassword ?? string.Empty;

        ApplyConnectionSettings();
        SaveSettings();

        error = string.Empty;
        return true;
    }

    private static LoginSettingsModel? TryParseSettings(string barcode)
    {
        foreach (var candidate in GetPayloadCandidates(barcode))
        {
            try
            {
                var trimmed = candidate.Trim();

                if (trimmed.StartsWith('"'))
                {
                    var inner = JsonSerializer.Deserialize<string>(trimmed);
                    if (!string.IsNullOrWhiteSpace(inner))
                        return TryParseSettings(inner);
                }

                if (trimmed.StartsWith('{'))
                    return JsonSerializer.Deserialize<LoginSettingsModel>(trimmed);
            }
            catch
            {
            }
        }

        return null;
    }

    private static IEnumerable<string> GetPayloadCandidates(string barcode)
    {
        var normalized = barcode
            .Replace("\0", string.Empty)
            .Trim()
            .Trim('\uFEFF', '\u200B')
            .Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            yield break;

        yield return normalized;

        var htmlDecoded = HttpUtility.HtmlDecode(normalized);
        if (!string.IsNullOrWhiteSpace(htmlDecoded) && htmlDecoded != normalized)
            yield return htmlDecoded;

        var urlDecoded = TryUrlDecode(normalized);
        if (!string.IsNullOrWhiteSpace(urlDecoded) && urlDecoded != normalized)
            yield return urlDecoded;

        var base64Decoded = TryBase64Decode(normalized);
        if (!string.IsNullOrWhiteSpace(base64Decoded))
            yield return base64Decoded;
    }

    private static string? TryUrlDecode(string value)
    {
        try
        {
            return Uri.UnescapeDataString(value);
        }
        catch
        {
            return null;
        }
    }

    private static string? TryBase64Decode(string value)
    {
        try
        {
            var base64 = value.Trim();

            if (base64.Length % 4 != 0)
                base64 = base64.PadRight(base64.Length + 4 - base64.Length % 4, '=');

            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }
        catch
        {
            return null;
        }
    }
}
