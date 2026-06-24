namespace Opus.Mobile.Shared.Articles;

public class ArticleItem
{
    public int? ArticleId { get; set; }

    public string? Barcode { get; set; }

    public string? Nr { get; set; }

    public string? Desc { get; set; }

    public string? Details { get; set; }

    public string? UnitName { get; set; }

    public byte[]? DocImage { get; set; }

    public string? DocBase64Image { get; set; }
}
