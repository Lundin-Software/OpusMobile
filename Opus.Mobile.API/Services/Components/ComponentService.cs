using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Components;
using Opus.Mobile.Shared.Documents;

namespace Opus.Mobile.API.Services.Components;

public class ComponentService(OpusDBContext ctx) : IComponentService
{
    private sealed class ComponentNode
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string? Name { get; set; }

        public int? Position { get; set; }
    }

    public async Task<IEnumerable<ComponentTreeItem>> GetComponentTree()
    {
        var components = await ctx.Components
            .AsNoTracking()
            .Where(component => !(component.Inactive ?? false))
            .Select(component => new ComponentNode
            {
                Id = component.Id,
                ParentId = component.ParentId ?? 0,
                Name = component.Name1,
                Position = component.Position
            })
            .ToListAsync();

        return BuildTree(components, 0);
    }

    public async Task<ComponentLookupDetails> LoadLookupComponent(int employeeId, int componentId)
    {
        var resolvedComponentId = await ResolveComponentId(componentId);

        return new ComponentLookupDetails
        {
            ComponentDetails = await GetComponentDetails(employeeId, resolvedComponentId),
            ComponentClassFields = [.. await GetComponentClassFields(employeeId, resolvedComponentId)],
            ComponentTasks = [.. await GetComponentTasks(employeeId, resolvedComponentId)]
        };
    }

    public async Task<IEnumerable<ComponentDocumentItem>> GetComponentDocuments(int componentId)
    {
        var documents = await ctx.ComponentDocuments
            .AsNoTracking()
            .Include(componentDocument => componentDocument.Document)
            .Include(componentDocument => componentDocument.Component)
            .Where(componentDocument =>
                componentDocument.ComponentId == componentId &&
                componentDocument.Document != null &&
                componentDocument.Document.Extension == ".jpg")
            .OrderByDescending(componentDocument => componentDocument.Id)
            .Select(componentDocument => new ComponentDocumentItem
            {
                ComponentId = componentDocument.ComponentId,
                ComponentDocId = componentDocument.Id,
                DocImage = componentDocument.Document!.Doc,
                DocBase64Image = componentDocument.Document.Doc == null
                    ? null
                    : Convert.ToBase64String(componentDocument.Document.Doc),
                IsMainImage =
                    componentDocument.Component != null &&
                    componentDocument.Document.Doc != null &&
                    componentDocument.Component.Image != null &&
                    componentDocument.Document.Doc.SequenceEqual(componentDocument.Component.Image)
            })
            .ToListAsync();

        return documents;
    }

    public async Task<ComponentDocumentItem> SaveComponentDocument(
        int userId,
        int componentId,
        SaveComponentDocumentRequest request)
    {
        var docBytes = string.IsNullOrWhiteSpace(request.DocBase64Image)
            ? null
            : Convert.FromBase64String(request.DocBase64Image);

        if (docBytes is null || docBytes.Length == 0)
            throw new ArgumentException("Document image is required.");

        var componentDocument = await ctx.ComponentDocuments
            .Include(document => document.Document)
            .Include(document => document.Component)
            .FirstOrDefaultAsync(document => document.Id == request.ComponentDocId);

        if (componentDocument is null)
        {
            var document = new Data.Models.Documents
            {
                Doc = docBytes,
                Extension = ".jpg",
                CreateUserId = userId,
                CreateDate = DateTime.Now
            };

            componentDocument = new ComponentDocuments
            {
                ComponentId = componentId,
                Document = document
            };

            await ctx.ComponentDocuments.AddAsync(componentDocument);
        }
        else if (componentDocument.Document is not null)
        {
            componentDocument.Document.Doc = docBytes;
            componentDocument.Document.ChangedByUserId = userId;
            componentDocument.Document.ChangedDate = DateTime.Now;
        }

        var component = await ctx.Components
            .FirstOrDefaultAsync(component => component.Id == componentId);

        if (component is not null && componentDocument.Document is not null)
        {
            if (request.IsMainImage == true)
                component.Image = componentDocument.Document.Doc;
            else
                component.Image ??= componentDocument.Document.Doc;
        }

        await ctx.SaveChangesAsync();

        return new ComponentDocumentItem
        {
            ComponentId = componentDocument.ComponentId,
            ComponentDocId = componentDocument.Id,
            DocImage = componentDocument.Document?.Doc,
            DocBase64Image = componentDocument.Document?.Doc is null
                ? null
                : Convert.ToBase64String(componentDocument.Document.Doc),
            IsMainImage =
                component?.Image != null &&
                componentDocument.Document?.Doc != null &&
                component.Image.SequenceEqual(componentDocument.Document.Doc)
        };
    }

    public async Task DeleteComponentDocument(int componentId, int componentDocumentId)
    {
        var componentDocument = await ctx.ComponentDocuments
            .Include(document => document.Document)
            .Include(document => document.Component)
            .FirstOrDefaultAsync(document =>
                document.Id == componentDocumentId &&
                document.ComponentId == componentId);

        if (componentDocument is null)
            return;

        if (componentDocument.Document is not null)
        {
            var component = componentDocument.Component;

            if (component is not null &&
                component.Image is not null &&
                componentDocument.Document.Doc is not null &&
                component.Image.SequenceEqual(componentDocument.Document.Doc))
            {
                var replacementDocument = await ctx.ComponentDocuments
                    .Include(document => document.Document)
                    .Where(document =>
                        document.ComponentId == componentId &&
                        document.Id != componentDocumentId &&
                        document.Document != null)
                    .OrderByDescending(document => document.Id)
                    .FirstOrDefaultAsync();

                component.Image = replacementDocument?.Document?.Doc;
            }

            ctx.Documents.Remove(componentDocument.Document);
        }

        ctx.ComponentDocuments.Remove(componentDocument);

        await ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<DocumentItem>> GetComponentFileDocuments(int componentId)
    {
        return await ctx.ComponentDocuments
            .AsNoTracking()
            .Where(componentDocument =>
                componentDocument.ComponentId == componentId &&
                componentDocument.Document != null &&
                componentDocument.Document.Extension != null &&
                documentExtensions.Contains(componentDocument.Document.Extension.ToLower()))
            .Select(componentDocument => new DocumentItem
            {
                Id = componentDocument.Document!.Id,
                Description = componentDocument.Document.Description,
                Extension = componentDocument.Document.Extension
            })
            .ToListAsync();
    }

    public async Task<ComponentDetailsItem?> GetComponentDetails(int employeeId, int componentId)
    {
        var resolvedComponentId = await ResolveComponentId(componentId);

        var details = (await ctx.Procedures.SpXama_ComponentDetailsAsync(employeeId, resolvedComponentId))
            .FirstOrDefault();

        if (details is null)
            return null;

        var component = await ctx.Components
            .AsNoTracking()
            .FirstOrDefaultAsync(component => component.Id == details.ID);

        var componentClass = component?.ClassId is null
            ? null
            : await ctx.ComponentClasses
                .AsNoTracking()
                .FirstOrDefaultAsync(componentClass => componentClass.Id == component.ClassId);

        return new ComponentDetailsItem
        {
            ComponentId = details.ID,
            Name1 = details.Name1,
            Name2 = details.Name2,
            TagNr = details.TagNr,
            SFI2 = details.SFI2,
            TypeNr = details.TypeNr,
            SerialNr = details.SerialNr,
            IconImage = details.Image,
            Image = component?.Image,
            ComponentTree = details.ComponentTree,
            ShowSFI = details.ShowSFI,
            ParentId = details.ParentID,
            ComponentClassId = componentClass?.Id,
            ComponentClassName = componentClass?.Name
        };
    }

    private async Task<int> ResolveComponentId(int componentId)
    {
        var exists = await ctx.Components
            .AsNoTracking()
            .AnyAsync(component => component.Id == componentId);

        if (exists)
            return componentId;

        var qrComponentId = await ctx.QrCodes
            .AsNoTracking()
            .Where(qr => qr.TableId == 7 && qr.QrCodeText == componentId.ToString())
            .Select(qr => qr.RowId)
            .FirstOrDefaultAsync();

        return qrComponentId ?? componentId;
    }

    private static List<ComponentTreeItem> BuildTree(
        IEnumerable<ComponentNode> components,
        int parentId)
    {
        return components
            .Where(component => component.ParentId == parentId)
            .OrderBy(component => component.Position)
            .Select(component => new ComponentTreeItem
            {
                Id = component.Id,
                Name = component.Name ?? "(empty)",
                ParentId = component.ParentId,
                Children = BuildTree(components, component.Id)
            })
            .ToList();
    }

    public async Task<IEnumerable<ComponentClassFieldItem>> GetComponentClassFields(int employeeId, int componentId)
    {
        var fields = await ctx.Procedures.SpXama_ComponentClassFieldsAsync(employeeId, componentId);

        return fields.Select(field => new ComponentClassFieldItem
        {
            ColumnNr = field.ColumnNr,
            FieldNr = field.FieldNr,
            FieldPrompt = field.FieldPrompt,
            FieldData = field.FieldData,
            FieldUnit = field.FieldUnit
        }).ToList();
    }

    public async Task<IEnumerable<ComponentTaskItem>> GetComponentTasks(int employeeId, int componentId)
    {
        var tasks = await ctx.Procedures.SpXama_ComponentTasksAsync(employeeId, componentId);

        return tasks.Select(task => new ComponentTaskItem
        {
            TaskId = task.ID,
            Name = task.Name,
            Description = task.Description,
            LeftTime = task.LeftTime,
            NextDate = task.NextDate,
            IntervalChar = task.IntervalChar,
            TaskClassId = task.TaskClassID,
            TaskClassName = task.TaskClassName,
            IntervalTypeId = task.IntervalTypeID,
            TaskTypeId = task.TaskTypeID,
            TaskIntervalId = task.TaskIntervalId
        }).ToList();
    }

    private static string[] documentExtensions =
    [
        ".pdf",
        ".xlsx",
        ".xls",
        ".doc",
        ".docx"
    ];

}
