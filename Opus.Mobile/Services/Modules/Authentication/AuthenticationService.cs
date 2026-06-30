using Opus.Mobile.Models;
using Opus.Mobile.Services.Requests.RequestProcessors;
using Opus.Mobile.Services.Requests.RequestProvider;
using Opus.Mobile.Shared.Authentication;
using Opus.Mobile.Shared.Models.Services;

namespace Opus.Mobile.Services.Modules.Authentication;

public class AuthenticationService(IRequestProcessor requestProcessor, IRequestProvider requestProvider) : IAuthenticationService
{
    public async Task<ServiceResponse<LoginItem>> Login(LoginRequest request)
    {
        var response = await requestProcessor.Post<LoginItem>(AuthenticationAPIEndpoints.Login, request);

        if (response.Success && response.Data is not null)
        {
            ApplicationSettings.Token = response.Data.Token;
            requestProvider.SetAuthorizationHeader(response.Data.Token);
        }

        return response;
    }


    public Task<ServiceResponse<UserDetailsItem>> GetUser()
    {
        return requestProcessor.Get<UserDetailsItem>(
            AuthenticationAPIEndpoints.User);
    }
}
