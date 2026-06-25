using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Documents;
using Opus.Mobile.Shared.TaskLogs;
using Opus.Mobile.Shared.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.XPath;

namespace Opus.Mobile.API.Services.Tasks;

public class TaskService(OpusDBContext ctx) : ITaskService
{
    public async Task<IEnumerable<DocumentItem>> GetTaskDocuments(int taskId)
    {
        return await ctx.TaskDocuments
            .AsNoTracking()
            .Where(taskDocument =>
                taskDocument.TaskId == taskId &&
                taskDocument.Document.Extension != null &&
                DocumentExtensions.Contains(taskDocument.Document.Extension.ToLower()))
            .Select(taskDocument => new DocumentItem
            {
                Id = taskDocument.Document.Id,
                Description = taskDocument.Document.Description,
                Extension = taskDocument.Document.Extension
            })
            .ToListAsync();
    }

    public async Task<CreatedTaskItem> CreateTask(int userId, CreateTaskRequest request)
    {
        var nr = 1;

        if (await ctx.UnscheduledTasks.AnyAsync())
        {
            var lastNr = await ctx.UnscheduledTasks
                .Where(task => task.Nr != null)
                .OrderBy(task => task.Nr)
                .Select(task => task.Nr)
                .LastOrDefaultAsync();

            if (lastNr.HasValue)
                nr = lastNr.Value + 1;
        }

        var result = await ctx.Procedures.SpXama_CreateUnscheduledTaskAsync(
            nr,
            request.ComponentId,
            request.DeadlineDate,
            request.Title,
            request.Desc,
            request.ResponsibleRoleId is > 0 ? request.ResponsibleRoleId : null,
            "",
            request.Priority,
            request.OnHold,
            request.Warranty,
            request.TaskTypeId is > 0 ? request.TaskTypeId : null,
            "",
            userId,
            request.TaskId);

        var newTaskIdText = result.FirstOrDefault()?.Result;

        if (!int.TryParse(newTaskIdText, out var newTaskId))
        {
            return new CreatedTaskItem
            {
                TaskId = null
            };
        }

        var taskExists = await ctx.UnscheduledTasks
            .AsNoTracking()
            .AnyAsync(task => task.Id == newTaskId);

        return new CreatedTaskItem
        {
            TaskId = taskExists ? newTaskId : null
        };
    }

    public async Task<string> SaveTaskPhoto(
    int userId,
    int taskId,
    SaveTaskPhotoRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ImageBase64))
                return "No image";

            var imageBytes = Convert.FromBase64String(request.ImageBase64);
            var thumbnail = CreateThumbnail(imageBytes);

            var document = new Data.Models.Documents
            {
                Doc = imageBytes,
                Extension = ".jpg",
                CreateUserId = userId,
                CreateDate = DateTime.Now,
                SmallJpg = thumbnail,
                DocSize = Math.Round(imageBytes.Length / (1024.0 * 1024), 2),
                Description = "Photo from phone"
            };

            if (!request.Scheduled)
            {
                var task = await ctx.UnscheduledTasks
                    .FirstOrDefaultAsync(task => task.Id == taskId);

                if (task is null)
                    return "Task not found";

                ctx.UnscheduledTaskDocuments.Add(new UnscheduledTaskDocuments
                {
                    Document = document,
                    UnscheduledTask = task
                });
            }
            else
            {
                var taskLog = await ctx.TaskLogs
                    .FirstOrDefaultAsync(taskLog => taskLog.Id == taskId);

                if (taskLog is null)
                    return "Task not found";

                ctx.TaskLogDocuments.Add(new TaskLogDocuments
                {
                    Document = document,
                    TaskLog = taskLog
                });
            }

            await ctx.SaveChangesAsync();

            return "OK";
        }
        catch (Exception ex)
        {
            return ex.InnerException?.Message ?? ex.Message;
        }
    }

    public async Task<IEnumerable<TaskArticleItem>> GetTaskArticles(int taskId)
    {
        var articles = await ctx.Procedures.spXama_ArticlesAsync(taskId);

        return articles.Select(article => new TaskArticleItem
        {
            ArticleId = article.ArticleId,
            TaskId = article.TaskId,
            Nr = article.Nr,
            Name = article.Name,
            Description = article.Description
        });
    }

    public async Task<TaskPhotoItem?> TakePhoto(
    int userId,
    int taskId,
    TakeTaskPhotoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DocBase64Image))
            return null;

        var task = await ctx.UnscheduledTasks
            .FirstOrDefaultAsync(task => task.Id == taskId);

        if (task is null)
            return null;

        var documentBytes = Convert.FromBase64String(request.DocBase64Image);

        var document = new Data.Models.Documents
        {
            Doc = documentBytes,
            Extension = ".png",
            CreateUserId = userId,
            CreateDate = DateTime.Now
        };

        await ctx.Documents.AddAsync(document);

        var taskDocument = new UnscheduledTaskDocuments
        {
            Document = document,
            UnscheduledTask = task
        };

        await ctx.UnscheduledTaskDocuments.AddAsync(taskDocument);

        await ctx.SaveChangesAsync();

        return new TaskPhotoItem
        {
            TaskId = taskId,
            TaskDocId = taskDocument.Id,
            DocBase64Image = Convert.ToBase64String(document.Doc)
        };
    }

    public async Task<TaskDocumentItem?> UploadTaskDocument(
    int userId,
    int taskId,
    UploadTaskDocumentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DocBase64Image))
            return null;

        var photoBytes = Convert.FromBase64String(request.DocBase64Image);

        var task = await ctx.UnscheduledTasks
            .FirstOrDefaultAsync(task => task.Id == taskId);

        if (task is null)
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

        var taskDocument = new UnscheduledTaskDocuments
        {
            Document = document,
            UnscheduledTask = task
        };

        await ctx.UnscheduledTaskDocuments.AddAsync(taskDocument);
        await ctx.SaveChangesAsync();

        return new TaskDocumentItem
        {
            TaskId = taskId,
            TaskDocumentId = taskDocument.Id,
            DocumentId = document.Id,
            DocBase64Image = Convert.ToBase64String(document.Doc)
        };
    }

    public async Task<string> EditPhoto(int taskId, EditTaskPhotoRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PhotoBase64))
                return "No photo";

            var photo = Convert.FromBase64String(request.PhotoBase64);

            var taskDocument = await ctx.UnscheduledTaskDocuments
                .Include(taskDocument => taskDocument.Document)
                .FirstOrDefaultAsync(taskDocument => taskDocument.UnscheduledTaskId == taskId);

            if (taskDocument?.Document is null)
                return "Task document not found";

            taskDocument.Document.Doc = photo;

            await ctx.SaveChangesAsync();

            return "ok";
        }
        catch (Exception ex)
        {
            return ex.InnerException?.Message ?? ex.Message;
        }
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

    private static readonly string[] DocumentExtensions =
    [
        ".pdf",
        ".xlsx",
        ".xls",
        ".doc",
        ".docx"
    ];
}
