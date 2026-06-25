namespace Opus.Mobile.Shared.Authentication;

public class LoginRequest
{
    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public string TokenUsername { get; set; } = "";

    public string TokenPassword { get; set; } = "";
}
