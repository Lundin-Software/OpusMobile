namespace Opus.Mobile.Shared.Documents;

public class DocumentSearchRequest
{
    public int ComponentId { get; set; }

    public string SearchString { get; set; } = string.Empty;

    public int LastId { get; set; }
}
