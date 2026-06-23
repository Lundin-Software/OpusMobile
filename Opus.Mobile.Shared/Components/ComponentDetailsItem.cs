namespace Opus.Mobile.Shared.Components;

public class ComponentDetailsItem
{
    public int? ComponentId { get; set; }

    public string? Name1 { get; set; }

    public string? Name2 { get; set; }

    public string? TagNr { get; set; }

    public string? SFI2 { get; set; }

    public string? TypeNr { get; set; }

    public string? SerialNr { get; set; }

    public byte[]? IconImage { get; set; }

    public byte[]? Image { get; set; }

    public string? ComponentTree { get; set; }

    public bool? ShowSFI { get; set; }

    public int? ParentId { get; set; }

    public int? ComponentClassId { get; set; }

    public string? ComponentClassName { get; set; }
}
