namespace Opus.Mobile.Shared.API;

public class APIResponse
{
    public bool Success { get; set; } = true;

    public string Message { get; set; } = "Success";
}

public class APIResponse<T>(T data) : APIResponse
{
    public T? Data { get; set; } = data;
}
