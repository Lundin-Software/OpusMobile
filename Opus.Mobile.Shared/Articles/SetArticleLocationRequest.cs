namespace Opus.Mobile.Shared.Articles;

public class SetArticleLocationRequest
{
    public int OldShelfId { get; set; }

    public int NewShelfId { get; set; }

    public int OldQuantity { get; set; }

    public int NewQuantity { get; set; }
}
