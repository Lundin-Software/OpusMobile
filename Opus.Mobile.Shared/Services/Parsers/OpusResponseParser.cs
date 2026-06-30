using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Models.Exceptions;
using Opus.Mobile.Shared.Models.Services;
using System.Net;
using System.Text.Json;

namespace Opus.Mobile.Shared.Services.Parsers;

public static class OpusResponseParser
{
    public static async Task<ServiceResponse<T>> Parse<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.Conflict:
            case HttpStatusCode.Accepted:
            case HttpStatusCode.OK:
                if (string.IsNullOrWhiteSpace(body))
                    return EmptyBodyResponse<T>(response);

                var apiResponse = Deserialize<APIResponse<T>>(body, response);
                return HandleResponse(apiResponse, response.StatusCode);

            default:
                throw new UnknownRequestException();
        }
    }

    public static async Task<ServiceResponse> Parse(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.Conflict:
            case HttpStatusCode.Accepted:
            case HttpStatusCode.OK:
                if (string.IsNullOrWhiteSpace(body))
                    return EmptyBodyResponse(response);

                var apiResponse = Deserialize<APIResponse>(body, response);
                return HandleResponse(apiResponse, response.StatusCode);

            default:
                throw new UnknownRequestException();
        }
    }

    #region Helpers

    private static T? Deserialize<T>(string body, HttpResponseMessage response)
    {
        try
        {
            return JSONSerializer.Deserialize<T>(body);
        }
        catch (JsonException ex)
        {
            throw new DeserializationException($"Invalid API JSON response ({FormatStatus(response)}): {ex.Message}");
        }
    }

    private static ServiceResponse EmptyBodyResponse(HttpResponseMessage response) => new()
    {
        Success = false,
        Message = $"Empty API response ({FormatStatus(response)})",
        StatusCode = response.StatusCode
    };

    private static ServiceResponse<T> EmptyBodyResponse<T>(HttpResponseMessage response) => new()
    {
        Success = false,
        Message = $"Empty API response ({FormatStatus(response)})",
        StatusCode = response.StatusCode
    };

    private static string FormatStatus(HttpResponseMessage response) =>
        $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim();

    private static ServiceResponse HandleResponse(APIResponse? response, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if (response is null)
            throw new DeserializationException();

        return new ServiceResponse
        {
            Success = response.Success,
            Message = response.Message,
            StatusCode = statusCode
        };
    }

    private static ServiceResponse<T> HandleResponse<T>(APIResponse<T>? response, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        if (response is null)
            throw new DeserializationException();

        return new ServiceResponse<T>
        {
            Success = response.Success,
            Message = response.Message,
            Data = response.Data,
            StatusCode = statusCode
        };
    }

    #endregion
}
