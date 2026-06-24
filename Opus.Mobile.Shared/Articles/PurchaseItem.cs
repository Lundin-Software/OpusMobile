namespace Opus.Mobile.Shared.Articles;

public class PurchaseItem
{
    public int? PurchaseId { get; set; }

    public int? ComponentId { get; set; }

    public int? UnschedTaskId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? IsProcessed { get; set; }

    public List<ConsumingArticleItem> ConsumingArticles { get; set; } = [];
}
