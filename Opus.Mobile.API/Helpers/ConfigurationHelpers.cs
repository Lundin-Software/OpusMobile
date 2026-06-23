using Opus.Mobile.API.Models.Exceptions;

namespace Opus.Mobile.API.Helpers;

public static class ConfigurationHelpers
{
    private static IConfiguration? Configuration;

    public static void Initialize(IConfiguration configuration) =>
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public static string GetValue(string key)
    {
        if (Configuration is null)
            throw new GeneralException("Configuration Helpers have not been initalized");

        return Configuration[key] ?? "";
    }

    public static string GetMandatoryValue(string key)
    {
        var value = GetValue(key);
        if (string.IsNullOrEmpty(value))
            throw new GeneralException($"Value for key [{key}] has not been set");

        return value;
    }
}
