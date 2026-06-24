namespace Opus.Mobile.Shared.Articles;

public class SaveConsumingArticleRequest
{
    public int? ConsumingArticleId { get; set; }

    public int? ArticleId { get; set; }

    public int? ShelveId { get; set; }

    public double? Qty { get; set; }
}
