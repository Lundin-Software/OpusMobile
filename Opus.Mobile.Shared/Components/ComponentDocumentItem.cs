namespace Opus.Mobile.Shared.Components;

public class ComponentDocumentItem
{
    public int? ComponentId { get; set; }

    public int? ComponentDocId { get; set; }

    public byte[]? DocImage { get; set; }

    public string? DocBase64Image { get; set; }

    public bool? IsMainImage { get; set; }
}
