namespace Opus.Mobile.Shared.Lookup;

public class ComponentLookupItem
{
    public int? ComponentId { get; set; }

    public string? Name1 { get; set; }

    public int? ParentId { get; set; }

    public bool? IsHeader { get; set; }

    public byte[]? IconImage { get; set; }

    public bool HasChildren { get; set; }
}
