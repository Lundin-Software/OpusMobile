using Opus.Mobile.Shared.Models.Services;

namespace Opus.Mobile.Services.Requests.RequestProcessors;

public interface IRequestProcessor
{
    Task<ServiceResponse> Get(string endpoint, CancellationToken cancellationToken = default);

    Task<ServiceResponse<T>> Get<T>(string endpoint, CancellationToken cancellationToken = default);

    Task<ServiceResponse> Post(string endpoint, object data, CancellationToken cancellationToken = default);

    Task<ServiceResponse<T>> Post<T>(string endpoint, object data, CancellationToken cancellationToken = default);

    Task<ServiceResponse> Put(string endpoint, object data, CancellationToken cancellationToken = default);

    Task<ServiceResponse<T>> Put<T>(string endpoint, object data, CancellationToken cancellationToken = default);

    Task<ServiceResponse> Delete(string endpoint, CancellationToken cancellationToken = default);

    Task<ServiceResponse<T>> Delete<T>(string endpoint, CancellationToken cancellationToken = default);
}
