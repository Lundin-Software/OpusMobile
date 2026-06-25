namespace Opus.Mobile.Shared.TaskLogs;

public class SaveTaskPhotoRequest
{
    public string? ImageBase64 { get; set; }

    public bool Scheduled { get; set; }
}
