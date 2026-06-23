using Opus.Mobile.Shared.Authentication;

namespace Opus.Mobile.API.Services.Authentication;

public interface IAuthenticationService
{
    Task<LoginResponse> Login(LoginDTO login);
}
