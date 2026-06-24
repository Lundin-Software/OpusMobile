using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Notifications;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace Opus.Mobile.API.Services.Notifications;

public class NotificationService(OpusDBContext ctx) : INotificationService
{
    public async Task<NotificationItem> CreateNotification(
        int userId,
        CreateNotificationRequest request)
    {
        var currentYear = DateTime.Today.Year;
        var baseNumber = currentYear * 10000;

        var maxNr = await ctx.Notifications
            .Where(notification =>
                notification.Nr.HasValue &&
                notification.Nr.Value / 10000 == currentYear)
            .Select(notification => notification.Nr)
            .MaxAsync();

        var nr = maxNr.HasValue
            ? maxNr.Value + 1
            : baseNumber + 1;

        var notification = new Data.Models.Notifications
        {
            Nr = nr,
            ComponentId = request.ComponentId,
            NotificationTypeId = request.NotificationTypeId > 0
                ? request.NotificationTypeId
                : null,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Completed = request.Completed,
            Date = DateTime.Now,
            UserId = userId,
            CreatedRoleId = request.CreatedByRoleId > 0
                ? request.CreatedByRoleId
                : null,
            ResponsibleRoleId = request.ResponsibleRoleId > 0
                ? request.ResponsibleRoleId
                : null
        };

        await ctx.Notifications.AddAsync(notification);
        await ctx.SaveChangesAsync();

        return new NotificationItem
        {
            NotificationId = notification.Id,
            Nr = notification.Nr
        };
    }

    public async Task<NotificationDocumentItem?> UploadNotificationDocument(
        int userId,
        int notificationId,
        UploadNotificationDocumentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DocBase64Image))
            return null;

        var photoBytes = Convert.FromBase64String(request.DocBase64Image);

        if (photoBytes.Length == 0)
            return null;

        var notification = await ctx.Notifications
            .FirstOrDefaultAsync(notification => notification.Id == notificationId);

        if (notification is null)
            return null;

        var thumbnail = CreateThumbnail(photoBytes);

        var document = new Data.Models.Documents
        {
            Doc = photoBytes,
            Extension = ".jpg",
            CreateUserId = userId,
            CreateDate = DateTime.Now,
            SmallJpg = thumbnail,
            DocSize = Math.Round(photoBytes.Length / (1024.0 * 1024), 2),
            Description = "Photo from phone"
        };

        await ctx.Documents.AddAsync(document);

        var notificationDocument = new NotificationDocuments
        {
            Document = document,
            Notification = notification
        };

        await ctx.NotificationDocuments.AddAsync(notificationDocument);
        await ctx.SaveChangesAsync();

        return new NotificationDocumentItem
        {
            NotificationId = notificationId,
            NotificationDocumentId = notificationDocument.Id,
            DocumentId = document.Id,
            DocImage = document.Doc,
            DocBase64Image = Convert.ToBase64String(document.Doc)
        };
    }

    private static byte[] CreateThumbnail(byte[] imageBytes)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var mainImage = Image.FromStream(inputStream);

        var width = 100;
        var height = 100 * mainImage.Height / mainImage.Width;

        using var thumbnail = new Bitmap(mainImage, new Size(width, height));
        using var outputStream = new MemoryStream();

        thumbnail.Save(outputStream, ImageFormat.Jpeg);

        return outputStream.ToArray();
    }
}
