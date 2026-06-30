using Opus.Mobile.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Opus.Mobile.Services.Requests.RequestProvider;

public class RequestProvider : IRequestProvider
{
    private readonly Lazy<HttpClient> httpClient = new(() =>
    {
        var client = new HttpClient();

        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        return client;
    });

    public Task<HttpResponseMessage> Get(string endpoint, CancellationToken cancellationToken = default)
    {
        return Send(HttpMethod.Get, endpoint, null, cancellationToken);
    }

    public Task<HttpResponseMessage> Post(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        return Send(HttpMethod.Post, endpoint, data, cancellationToken);
    }

    public Task<HttpResponseMessage> Put(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        return Send(HttpMethod.Put, endpoint, data, cancellationToken);
    }

    public Task<HttpResponseMessage> Delete(string endpoint, CancellationToken cancellationToken = default)
    {
        return Send(HttpMethod.Delete, endpoint, null, cancellationToken);
    }

    public void SetAuthorizationHeader(string token)
    {
        httpClient.Value.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<HttpResponseMessage> Send(HttpMethod method, string endpoint, object? data, CancellationToken cancellationToken)
    {
        var url = ApplicationSettings.ApiBaseUrl + endpoint.TrimStart('/');

        using var request = new HttpRequestMessage(method, url);

        if (data is not null)
        {
            var json = JsonSerializer.Serialize(data);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }


        return await httpClient.Value.SendAsync(request, cancellationToken);
    }
}

