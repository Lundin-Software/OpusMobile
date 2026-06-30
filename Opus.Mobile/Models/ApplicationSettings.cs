using Opus.Mobile.Shared.Authentication;

namespace Opus.Mobile.Models;

public static class ApplicationSettings
{
    public static string ApiBaseUrl { get; set; } = string.Empty;

    public static string Token { get; set; } = string.Empty;

    public static string TokenUsername { get; set; } = string.Empty;

    public static string TokenPassword { get; set; } = string.Empty;

    public static UserDetailsItem? User { get; set; }

}
