using System.Net;

namespace Opus.Mobile.Shared.Models.Services;

public class ServiceResponse
{
    public bool Success { get; set; } = true;
    public bool Failed => !Success;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public string Message { get; set; } = string.Empty;
}

public class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; set; }
}
