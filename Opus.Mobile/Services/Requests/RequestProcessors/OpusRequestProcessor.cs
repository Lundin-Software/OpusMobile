using Opus.Mobile.Services.Requests.RequestProvider;
using Opus.Mobile.Shared.Models.Services;
using Opus.Mobile.Shared.Services.Parsers;

namespace Opus.Mobile.Services.Requests.RequestProcessors;

public class OpusRequestProcessor(IRequestProvider requestProvider) : BaseRequestProcessor, IRequestProcessor
{
    #region GET

    public Task<ServiceResponse<T>> Get<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Get(endpoint, cancellationToken);
            return await OpusResponseParser.Parse<T>(response);
        });
    }

    public Task<ServiceResponse> Get(string endpoint, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Get(endpoint, cancellationToken);
            return await OpusResponseParser.Parse(response);
        });
    }

    #endregion

    #region POST

    public Task<ServiceResponse<T>> Post<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Post(endpoint, data, cancellationToken);
            return await OpusResponseParser.Parse<T>(response);
        });
    }

    public Task<ServiceResponse> Post(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Post(endpoint, data, cancellationToken);
            return await OpusResponseParser.Parse(response);
        });
    }

    #endregion

    #region PUT

    public Task<ServiceResponse<T>> Put<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Put(endpoint, data, cancellationToken);
            return await OpusResponseParser.Parse<T>(response);
        });
    }

    public Task<ServiceResponse> Put(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Put(endpoint, data, cancellationToken);
            return await OpusResponseParser.Parse(response);
        });
    }

    #endregion

    #region DELETE

    public Task<ServiceResponse<T>> Delete<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Delete(endpoint, cancellationToken);
            return await OpusResponseParser.Parse<T>(response);
        });
    }

    public Task<ServiceResponse> Delete(string endpoint, CancellationToken cancellationToken = default)
    {
        return SendRequest(async () =>
        {
            var response = await requestProvider.Delete(endpoint, cancellationToken);
            return await OpusResponseParser.Parse(response);
        });
    }

    #endregion
}
