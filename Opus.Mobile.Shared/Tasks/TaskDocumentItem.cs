namespace Opus.Mobile.Shared.Tasks;

public class TaskDocumentItem
{
    public int TaskId { get; set; }

    public int TaskDocumentId { get; set; }

    public int DocumentId { get; set; }

    public string? DocBase64Image { get; set; }
}
