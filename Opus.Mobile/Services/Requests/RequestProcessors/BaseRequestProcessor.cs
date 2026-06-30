using Opus.Mobile.Shared.Models.Exceptions;
using Opus.Mobile.Shared.Models.Services;

namespace Opus.Mobile.Services.Requests.RequestProcessors;

public class BaseRequestProcessor
{
    #region Send Request

    //Generic
    internal static async Task<ServiceResponse<T>> SendRequest<T>(Func<Task<ServiceResponse<T>>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (IsKnownException(ex))
        {
            return CaptureFailedResponse<T>(ex.Message);
        }
    }

    //Non-generic
    internal static async Task<ServiceResponse> SendRequest(Func<Task<ServiceResponse>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (IsKnownException(ex))
        {
            return CaptureFailedResponse(ex.Message);
        }
    }

    #endregion

    #region Capture Failed Response

    //Generic
    private static ServiceResponse<T> CaptureFailedResponse<T>(string errorMessage)
    {
        return new ServiceResponse<T>
        {
            Success = false,
            Message = errorMessage
        };
    }

    //Non-generic
    private static ServiceResponse CaptureFailedResponse(string errorMessage)
    {
        return new ServiceResponse
        {
            Success = false,
            Message = errorMessage
        };
    }

    private static bool IsKnownException(Exception ex) => ex is
        HttpRequestException or
        TimeoutException or
        OperationCanceledException or
        TaskCanceledException or
        DeserializationException or
        UnknownRequestException or
        NoInternetException;

    #endregion
}
