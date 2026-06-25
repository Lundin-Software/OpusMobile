using Opus.Mobile.Shared.Authentication;

namespace Opus.Mobile.API.Services.Authentication;

public interface IAuthenticationService
{
    Task<LoginItem> Login(LoginRequest login);

    Task<UserDetailsItem?> GetUserDetails(int userId);
}
