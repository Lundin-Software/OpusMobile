namespace Opus.Mobile.Shared.API;

public class APIStatusResponse
{
    public string MinimumAppVersion { get; set; } = "";

    public string ConnectionName { get; set; } = "";

    public bool DatabaseConnection { get; set; } = false;

    public bool BackgroundService { get; set; } = false;
}

