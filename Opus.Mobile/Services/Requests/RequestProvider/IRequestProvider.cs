namespace Opus.Mobile.Services.Requests.RequestProvider;

public interface IRequestProvider
{
    Task<HttpResponseMessage> Get(string endpoint, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> Post(string endpoint, object data, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> Put(string endpoint, object data, CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> Delete(string endpoint, CancellationToken cancellationToken = default);

    void SetAuthorizationHeader(string token);
}
