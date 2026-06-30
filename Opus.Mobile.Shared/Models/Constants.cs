namespace Opus.Mobile.Shared.Models;

public struct ErrorMessages
{
    public const string UnknownRequestError = "API, Network or Request Error, please contact support";
    public const string DeserializationError = "Could not deserialize, please contact support";
    public const string NoInternet = "No internet connection";
    public const string Timeout = "Operation timed out";
    public const string Canceled = "Canceled";
    public const string NoAPIEndpoint = "API endpoint not configured";
}

public struct Settings
{
    public const string GeneralSettings = "settings_key";
    public const string SettingsApi = "settings_api";

    public const string UserName = "username_key";
    public const string Password = "pswd_key";
    public const string RememberMe = "rememberme_key";

    public const string ApiBaseUrl = "backurl_key";
    public const string TokenUserName = "tokenusername_key";
    public const string TokenPassword = "tokenpswd_key";
}
