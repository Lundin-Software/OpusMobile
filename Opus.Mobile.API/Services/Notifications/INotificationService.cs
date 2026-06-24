using Opus.Mobile.Shared.Notifications;

namespace Opus.Mobile.API.Services.Notifications;

public interface INotificationService
{
    Task<NotificationItem> CreateNotification(int userId, CreateNotificationRequest request);

    Task<NotificationDocumentItem?> UploadNotificationDocument(int userId, int notificationId, UploadNotificationDocumentRequest request);
}
