namespace Opus.Mobile.Shared.Components;

public class SaveComponentDocumentRequest
{
    public int? ComponentDocId { get; set; }

    public string? DocBase64Image { get; set; }

    public bool? IsMainImage { get; set; }
}
