using Opus.Mobile.Shared.Authentication;
using Opus.Mobile.Shared.Models.Services;

namespace Opus.Mobile.Services.Modules.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResponse<LoginItem>> Login(LoginRequest request);

    Task<ServiceResponse<UserDetailsItem>> GetUser();
}
