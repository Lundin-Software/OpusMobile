namespace Opus.Mobile.Shared.Notifications;

public class NotificationDocumentItem
{
    public int NotificationId { get; set; }

    public int NotificationDocumentId { get; set; }

    public int DocumentId { get; set; }

    public byte[]? DocImage { get; set; }

    public string? DocBase64Image { get; set; }
}
